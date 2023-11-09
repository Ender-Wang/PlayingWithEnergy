using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

class DataAssets
{
    public TextAsset csv { get; set; }
    public Sprite img { get; set; }
}

public class AutoPicDataLoader : MonoBehaviour
{
    [Tooltip("The path of directory where semantic data stored")]
    public string assetDirectory;
    public List<string> assetDirectorys;
    private string energyDataDirectory;
    [Tooltip("Tag of Gameobject")]
    public string Tag;
    // private GameObject buildings;
    private List<GameObject> buildings;

    private Dictionary<string, Dictionary<Vector3, Color>> globalES3Record;
    private Dictionary<string, string> semanticNamesFullPaths = new Dictionary<string, string>();
    // load all unity asset with respect to it's semantic name
    private Dictionary<string, List<DataAssets>> globalAssets = new Dictionary<string, List<DataAssets>>();
    private Dictionary<string, List<PicDataPreprocessor>> globalPicDataPreprocessors = new Dictionary<string, List<PicDataPreprocessor>>();
    private List<string> semanticNeedReload = new List<string>();
    private string es3FilePathPrefix = "Building/SemanticData/";
    void Awake()
    {   
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tag));
        for (int i = buildings.Count - 1; i > 0; i--)
        {
            GameObject obj = buildings[i];
            if (obj.name != "Building")
            {
                buildings.Remove(obj);
            }
        }
        FindAndLoadAssets();
        LoadGlobalES3Record();
        PreprocessingAssets();
        ReloadSemanticData_coroutine();
    }
    
    void OnDestroy()
    {   
        foreach (string semanticName in semanticNeedReload)
        {
            string es3Key = semanticName;
            string filePath = es3FilePathPrefix + semanticName;
            ES3.Save<Dictionary<Vector3, Color>>(es3Key, globalES3Record[semanticName], filePath);
        }
        // distory this script after it's job done
        BuildingLoader.Instance.loadDatas();
    }

    // load all semantic data which stored using ES3, if not exists yet, then create new dict
    private void LoadGlobalES3Record()
    {
        globalES3Record = new Dictionary<string, Dictionary<Vector3, Color>>();
        foreach (string semanticName in assetDirectorys)
        {
            string es3key = semanticName;
            string filePath = es3FilePathPrefix + semanticName;
            globalES3Record.Add(semanticName, ES3.Load<Dictionary<Vector3, Color>>(es3key, filePath, new Dictionary<Vector3, Color>()));
            if (globalES3Record[semanticName].Count == 0) {
                semanticNeedReload.Add(semanticName);
                DataSetting.createAndSave(assetDirectory, semanticName, DataSetting.DataType.Pic); // create and save data setting
            }
        }
    }

    // load pic and corresponding csv assets after finding semanticData(after calling CheckDirectoryWithPicData())
    private void FindAndLoadAssets()
    {
        foreach (string semanticName in assetDirectorys)
        {   
            List<DataAssets> dataAssets = new List<DataAssets>();
            string absolutePath = Path.Join(assetDirectory, semanticName, "Pic Data");
            Sprite[] sprites = Resources.LoadAll<Sprite>(absolutePath);
            TextAsset[] texts = Resources.LoadAll<TextAsset>(absolutePath);
            LoadAssets(sprites, texts, dataAssets);
            globalAssets.Add(semanticName, dataAssets);
        }

        void LoadAssets(Sprite[] sprites, TextAsset[] texts, List<DataAssets> dataAssets)
        {   
            Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();
            Dictionary<string, TextAsset> textsDic = new Dictionary<string, TextAsset>();
            foreach(Sprite s in sprites) {
                spritesDic.Add(s.name, s);
            }
            foreach(TextAsset t in texts) {
                textsDic.Add(t.name, t);
            }

            foreach(KeyValuePair<string, Sprite> kvp in spritesDic) {
                if (textsDic.ContainsKey(kvp.Key)) {
                    DataAssets temp = new DataAssets();
                    temp.csv = textsDic[kvp.Key];
                    temp.img = kvp.Value;
                    dataAssets.Add(temp);
                }
            }
        }
    }

    // read and prepare imgs to get building color
    private void PreprocessingAssets()
    {
        foreach (string semanticName in assetDirectorys)
        {
            List<DataAssets> dataAssets = globalAssets[semanticName];
            List<PicDataPreprocessor> picDataPreprocessors = new List<PicDataPreprocessor>();
            foreach (DataAssets asset in dataAssets)
            {
                picDataPreprocessors.Add(new PicDataPreprocessor(semanticName, asset.csv, asset.img));
            }
            globalPicDataPreprocessors.Add(semanticName, picDataPreprocessors);
        }
    }

    void ReloadSemanticData_coroutine()
    {
        BoxCollider bc;
        Color color;
        foreach (GameObject building in buildings)
        {
            foreach (Transform child in building.transform)
            {
                bc = child.GetComponent<BoxCollider>();
                float x = bc.center.x * -100;
                float y = bc.center.y * 100;
                foreach (string semanticName in semanticNeedReload)
                {   
                    List<PicDataPreprocessor> picDataPreprocessors = globalPicDataPreprocessors[semanticName];
                    Dictionary<Vector3, Color> semanticDataDic = globalES3Record[semanticName];
                    DataSetting dataSetting = DataSetting.getDataSetting(semanticName);
                    foreach (PicDataPreprocessor pdp in picDataPreprocessors)
                    {
                        if (pdp.isInrange(x, y))
                        {
                            color = pdp.getColor(x, y);
                            color = ColorHelper.getColorByDataSetting(color, dataSetting);
                            semanticDataDic.Add(bc.center, color);
                            int index = dataSetting.colors.IndexOf(color);
                            int contribution = dataSetting.contributions[index];
                            dataSetting.initBase += contribution;
                            break;
                        }
                    }
                }
            }
        }
        Destroy(this);
    }
}