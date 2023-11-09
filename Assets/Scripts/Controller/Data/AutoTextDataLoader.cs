using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;

public class AutoTextDataLoader : MonoBehaviour
{
    [Tooltip("The directory of the assets")]
    public string assetDirectory;
    [Tooltip("The list of the asset-paths: key: semantic name, value: file path")]
    private Dictionary<string, string> assetPaths;

    private Vector3 _position { get { return SelectingController.position; } }
    private Vector3 position;   //V3 of collider
    private GameObject buildings;    //building prefab
    private BoxCollider[] buildingsBC;    //colliders of buildings
    private string semanticName;

    Dictionary<Vector3, string> gpsTextData; //gps format source data
    Dictionary<Vector3, string> epsgTextData; //epsg format converted data

    void Awake()
    {
        assetPaths = new Dictionary<string, string>();
        getAllAssetPaths();
        loadTextFiles();
        buildings = GameObject.FindGameObjectWithTag("Selectable");
        buildingsBC = buildings.GetComponentsInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!position.Equals(_position))
        {
            position = _position;
            // Debug.Log("Load text file of:" + position);
        }
    }

    /// <summary>
    /// get all text data file paths, store them in Dictionary<string, List<string>> assetPaths
    /// </summary>
    void getAllAssetPaths()
    {
        string[] directories = Directory.GetFiles("Assets/Resources/" + assetDirectory, "*", SearchOption.AllDirectories);

        assetPaths.Clear();
        foreach (string filePath in directories)
        {
            //check if the file is a text data file (.csv) and not a DataSetting file
            //filter out the files have "_" (different line format, need another parser)
            //TODO: add another parser deal with text data file with "_" in the name (different line format)
            if (filePath.Contains("Text Data") && filePath.EndsWith(".csv") && !filePath.Contains("DataSetting") && !filePath.Contains("_"))
            {
                string semanticName = Path.GetFileNameWithoutExtension(filePath);
                // Debug.Log("File Name: " + semanticName);

                // Debug.Log("File Path: " + filePath);
                char[] splitChar = { '\\', '/' };
                string[] filePathSplit = filePath.Split(splitChar);
                // Debug.Log("semantic Name: " + filePathSplit[3]);
                semanticName = filePathSplit[3];
                assetPaths.Add(semanticName, filePath);
            }
        }
    }

    void loadTextFiles()
    {
        foreach (KeyValuePair<string, string> file in assetPaths)
        {
            loadTextFile(file.Key, file.Value);
        }
    }

    // load the whole file into: Dictionary<V3, string> data
    void loadTextFile(string semanticName, string filePath)
    {
        //data Dictionary init
        gpsTextData = new Dictionary<Vector3, string>();

        StreamReader reader = new StreamReader(filePath);
        bool endOfFile = false;
        reader.ReadLine();  //skip the header (first line)
        while (!endOfFile)
        {
            string line = reader.ReadLine();
            if (line == null)
            {
                endOfFile = true;
                break;
            }
            string[] values = line.Split(',');
            /*
            values[0]: x & y value
            values[1]: suitable or not
            values[2]: rgb (not used in this .cs file)
            */

            //Construct Vector3
            int l = values[0].Length;
            values[0] = values[0].Substring(7, l - 8);
            string[] points = values[0].Split(' ');
            Vector3 v3 = new Vector3(float.Parse(points[0]), float.Parse(points[1]), 0f); //precision loss when parsing to float
            string str = values[1];
            if (!gpsTextData.ContainsKey(v3))
            {
                gpsTextData.Add(v3, str);
            }

            // //placeholder for another Dictionary (v3, rgb)
            // //rgb in Dictionary, construct another Dictionary with (V3, rgb) if needed later
            // string rgb = values[2].ToString();            
            // textData.Add(v3, rgb);
        }
        //convert gps to epsg in textData
        convertTextData(semanticName);
    }

    void convertTextData(string semanticName)
    {
        int count = 0;
        epsgTextData = new Dictionary<Vector3, string>();
        foreach (KeyValuePair<Vector3, string> kvp in gpsTextData)
        {
            float longitude = kvp.Key.x;
            float latitude = kvp.Key.y;
            string home = "https://epsg.io/srs/transform/";
            string tail = ".json?key=default&s_srs=4326&t_srs=25832";
            string url = home + longitude.ToString() + "," + latitude.ToString() + tail;

            getEPSG(url);

            void getEPSG(string url) => StartCoroutine(getData(url));

            IEnumerator getData(string url)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    yield return request.SendWebRequest();
                    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.Log("Network or HTTP Error!");
                    }
                    else
                    {
                        count++;
                        string response = request.downloadHandler.text.ToString();
                        string[] data = response.Substring(53, response.Length - 56).Split(",");
                        data[0] = data[0].Substring(4, data[0].Length - 4);
                        data[1] = data[1].Substring(4, data[1].Length - 4);
                        //precision loss when parsing to float, but should be fine for epsg coordinates
                        float x = float.Parse(data[0], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat) / -100;
                        float y = float.Parse(data[1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat) / 100;
                        float z = 0f;
                        foreach (BoxCollider bc in buildingsBC)
                        {
                            Color red = new Color(1f, 0f, 0f, 0.5f);
                            if ((bc.center.x - bc.size.x / 2.0f < x && x < bc.center.x + bc.size.x / 2.0f))
                            {
                                z = bc.center.z;
                                Vector3 epsg = new Vector3(x, y, z);
                                if (!epsgTextData.ContainsKey(bc.center))
                                {
                                    epsgTextData.Add(bc.center, kvp.Value);
                                }

                                break;
                            }
                        }
                        //wait for all data to be converted, then save to ES3
                        if (count == gpsTextData.Count)
                        {
                            saveTextFile(semanticName);
                        }
                    }
                }
            }
        }
    }

    // save Dictionary data to file using ES3
    void saveTextFile(string semanticName)
    {
        Dictionary<Vector3, Color> es3Data = ES3.Load<Dictionary<Vector3, Color>>(semanticName, "Building/SemanticData/" + semanticName, new Dictionary<Vector3, Color>());
        DataSetting.createAndSave("Assets/Resources/EnergyData", semanticName, DataSetting.DataType.Text);
        DataSetting dataSetting = DataSetting.getDataSetting(semanticName);
        foreach (KeyValuePair<Vector3, string> kvp in epsgTextData)
        {
            if (!es3Data.ContainsKey(kvp.Key))
            {
                Color color = ColorHelper.getColorByDataSetting(kvp.Value, dataSetting);
                es3Data.Add(kvp.Key, color);
            }
        }
        ES3.Save<Dictionary<Vector3, Color>>(semanticName, es3Data, "Building/SemanticData/" + semanticName);
        BuildingLoader.Instance.loadDatas();
    }
}