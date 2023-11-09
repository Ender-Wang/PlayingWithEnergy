using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A class replace ES3 save funtion, allowing multiple use in unity.
/// Don't forget to call AddDown() when you finish Dictionary.add(...)
/// The Save function will be run once when all user finish Dictionary.add(...)
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
public class DictWriter <K, V>
{
    private static DictWriter <K, V> Instance;
    private Dictionary<K, V> dict;
    private static int user = 0;

    private DictWriter(){

    }

    private string _filePath;
    public string FilePath {
        get {return _filePath;}
        set { _filePath = value;}
    }
    
    private string _key;
    public string Key {
        get {return _key;}
        set { _key = value;}
    }

    public static DictWriter<K, V> getInstance () {
        if (Instance == null)
            Instance = new DictWriter<K, V>();
        user++;
        return Instance;
    }
    public void Add(K key, V value) {
        if (dict == null)
            dict = new Dictionary<K, V>();
        if (!dict.ContainsKey(key))
            dict.Add(key, value);
    }

    public void Save() {
        if (user == 0) {
            if (ES3.FileExists(_filePath)){
                if (ES3.KeyExists(_key, _filePath)) {
                    Dictionary<K, V> data = ES3.Load<Dictionary<K, V>>(_key, _filePath);
                    Dictionary<K, V> mergedDict = dict.Union(data).ToDictionary(x => x.Key, x => x.Value);
                    ES3.Save<Dictionary<K, V>>(_key, mergedDict, _filePath);
                } else {
                    ES3.Save<Dictionary<K, V>>(_key, dict, _filePath);
                }
            } else {
                ES3.Save<Dictionary<K, V>>(_key, dict, _filePath);
            }
        }
    }

    public void AddDown() {
        user--;
    }
}
