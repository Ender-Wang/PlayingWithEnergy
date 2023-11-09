using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This class is responsible for storing and retrieving data settings.
/// The data setting defines the color, contribution, and displayed info of each semantic data.
/// </summary>
public class DataSetting
{
    [ES3Serializable]
    public string displayedTitle { get; set; }
    [ES3Serializable]
    public List<Color> colors { get; set; }

    [ES3Serializable]
    public List<string> originInfos { get; set; }

    [ES3Serializable]
    public List<Color> originColors { get; set; }

    [ES3Serializable]
    public List<string> displayedInfos { get; set; }
    [ES3Serializable]
    public List<int> contributions { get; set; }
    [ES3Serializable]
    public string levelFactor { get; set; }
    [ES3Serializable]
    public int totalContributions { get; set; }
    [ES3Serializable]
    public int initBase { get; set; }

    private static string es3Path = "Building/SemanticData/DataSettings";
    private static string es3Key = "DataSettings";

    public enum DataType { Text, Pic };

    private static Dictionary<string, DataSetting> dataSettings = ES3.Load<Dictionary<string, DataSetting>>(es3Key, es3Path, new Dictionary<string, DataSetting>());

    public DataSetting(List<Color> colors, List<string> displayedInfos, List<int> contributions, string levelFactor, string displayedTitle, List<string> originInfos, List<Color> originColors = null)
    {
        this.displayedTitle = displayedTitle;
        this.colors = colors;
        this.displayedInfos = displayedInfos;
        this.contributions = contributions;
        this.levelFactor = levelFactor;
        this.originInfos = originInfos;
        this.totalContributions = 0;
        this.originColors = originColors;
        this.initBase = 0;
    }

    public static void store()
    {
        ES3.Save<Dictionary<string, DataSetting>>(es3Key, dataSettings, es3Path);
    }

    public static Dictionary<string, DataSetting> getDataSettings()
    {
        return dataSettings;
    }

    // get the total contributions of all data
    public static Dictionary<string, int> getTotalContributionsOfAllData()
    {
        Dictionary<string, int> contributions = new Dictionary<string, int>(); // semantic name -> total contribution
        foreach (var dataSetting in dataSettings)
        {
            contributions.Add(dataSetting.Key, dataSetting.Value.totalContributions);
        }
        return contributions;
    }

    // get energy distribution which is caculated by the count of buildings
    public static Dictionary<string, Dictionary<string, float>> getEnergyDistribution()
    {
        Dictionary<string, Dictionary<string, float>> energyDistribution = new Dictionary<string, Dictionary<string, float>>();
        List<LevelFactor> levelFactors = LevelManager.getAllLevelFactors();
        foreach (LevelFactor levelFactor in levelFactors)
        {
            List<DataSetting> dataSettings = DataSetting.getDataSettings(levelFactor);
            Dictionary<string, float> energyDistributionOfOneLevelFactor = new Dictionary<string, float>();
            foreach (DataSetting dataSetting in dataSettings)
            {
                float sum = dataSetting.totalContributions + dataSetting.initBase;
                if (levelFactor.name() == "Energy Consumption")
                {
                    sum = dataSetting.contributions.Count * BuildingManager.Instance.buildings.Count - sum;
                }
                energyDistributionOfOneLevelFactor.Add(dataSetting.displayedTitle, sum);
            }
            energyDistribution.Add(levelFactor.name(), energyDistributionOfOneLevelFactor);
        }
        return energyDistribution;
    }

    public static List<DataSetting> getDataSettings(LevelFactor levelFactor)
    {
        List<DataSetting> dataSettings = new List<DataSetting>();
        foreach (var dataSetting in DataSetting.getDataSettings())
        {
            if (dataSetting.Value.levelFactor == levelFactor.name())
            {
                dataSettings.Add(dataSetting.Value);
            }
        }
        return dataSettings;
    }

    public static DataSetting getDataSetting(string semanticName)
    {
        return dataSettings[semanticName];
    }

    public static List<string> getSemanticNames()
    {
        return new List<string>(dataSettings.Keys);
    }

    public static void createAndSave(string assetDirectory, string semanticName, DataType dataType)
    {
        if (!dataSettings.ContainsKey(semanticName))
        {
            string folderPath = dataType == DataType.Pic ? Path.Join(assetDirectory, semanticName, "Pic Data") : Path.Join(assetDirectory, semanticName, "Text Data");
            dataSettings.Add(semanticName, build(folderPath, semanticName, dataType));
        }
    }

    private static DataSetting build(string folderPath, string semanticName, DataType dataType)
    {
        List<Color> colors = new List<Color>();
        List<string> originInfos = new List<string>();
        List<Color> originColors = new List<Color>();
        List<string> displayedInfos = new List<string>();
        List<int> contributions = new List<int>();
        string levelFactor = "";
        string displayedTitle = semanticName.Split('_')[0];

        var settingPath = Path.Combine(folderPath, "DataSetting");
        TextAsset textAsset = Resources.Load<TextAsset>(settingPath);
        
        using (var reader = new StreamReader(GenerateStreamFromString(textAsset.text)))
        {
            // Read the first line to get the column headers
            var headers = reader.ReadLine()?.Split(',');

            // Find the indices of the Value, contributions, and Info columns
            var valueIndex = Array.IndexOf(headers, "Value");
            var contributionIndex = Array.IndexOf(headers, "Contribution");
            var infoIndex = Array.IndexOf(headers, "Displayed Info");
            var levelFactorIndex = Array.IndexOf(headers, "LevelFactor");
            var displayedTitleIndex = Array.IndexOf(headers, "Displayed Title");

            // Read the rest of the lines and store the corresponding colors in the lists
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var valuesArray = CsvParser.parse(line);
                if (valuesArray == null || valuesArray.Length < 3)
                {
                    continue;
                }

                // load level factor according to the value in the csv es3
                if (valuesArray[levelFactorIndex] != "")
                {
                    levelFactor = valuesArray[levelFactorIndex];
                }

                if (valuesArray[displayedTitleIndex] != "")
                {
                    displayedTitle = valuesArray[displayedTitleIndex];
                }
                originColors.Add(getColor(valuesArray[0], folderPath)); // get color from ColorMap.png
                contributions.Add(int.Parse(valuesArray[contributionIndex]));
                originInfos.Add(valuesArray[valueIndex]);
                displayedInfos.Add(valuesArray[infoIndex]);
            }
        }
        Color[] colorsStandard = produceColors(contributions.Count, levelFactor);
        for (int i = 0; i < contributions.Count; i++)
        {
            colors.Add(colorsStandard[contributions[i]]);
        }

        return new DataSetting(colors, displayedInfos, contributions, levelFactor, displayedTitle, originInfos, originColors);
    }

    private static Color getColor(string position, string folderPath)
    {
        
        // Load image from es3
        // byte[] es3Data = System.IO.File.ReadAllBytes(imagePath);
        // Texture2D texture = new Texture2D(2, 2);
        // texture.LoadImage(es3Data);
        var imagePath = Path.Combine(folderPath, "ColorMap");
        Sprite image = Resources.Load<Sprite>(imagePath);

        // Get color of pixel at x,y
        splitPosition(position, out int x, out int y);

        //For Texture2D: The lower left corner is (0, 0)
        // Sprite image = Sprite.Create(
        //     texture,
        //     new Rect(0, 0, texture.width, texture.height),
        //     Vector2.zero
        // );
        Color pixelColor = image.texture.GetPixel(x, image.texture.height - y - 1);
        return pixelColor;
    }

    // produce color according to the contribution
    private static Color[] produceColors(int count, String levelFactor)
    {
        return ColorHelper.GetColors(count);
    }

    private static void splitPosition(string input, out int x, out int y, char separator = ',')
    {
        string[] parts = input.Split(separator);
        int.TryParse(parts[0].Replace(",", ""), out x);
        int.TryParse(parts[1].Replace(",", ""), out y);
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
