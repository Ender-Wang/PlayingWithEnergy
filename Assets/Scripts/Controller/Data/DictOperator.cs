using System.Collections.Generic;

public class DictOperator <K, V>
{
    private Dictionary<K, V> dict;
    public DictOperator(Dictionary<K, V> _dict) {
        this.dict = _dict;
    }

    public void Add(K key, V value) {
        if (!dict.ContainsKey(key))
            dict.Add(key, value);
    }

    public void Update(K key, V value) {
        if (dict.ContainsKey(key)){
            dict[key] = value;
        }
    }
    public void Remove(K key) {
        if (dict.ContainsKey(key))
            dict.Remove(key);
    }
    public void Save(string key, string filePath) {
        ES3.Save<Dictionary<K, V>>(key, dict, filePath);
    }
}