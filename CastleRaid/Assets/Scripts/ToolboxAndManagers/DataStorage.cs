using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

/// <summary>
/// For saving game data using Json.
/// </summary>
/// <remarks>
/// Read file when first set or get any values.
/// </remarks>
public class DataStorage
{
    private static SaveDataBase savedatabase = null;

    private static SaveDataBase Savedatabase
    {
        get
        {
            if (savedatabase == null)
            {
                string path = Application.persistentDataPath + "/";
                string fileName = Application.companyName + "." + Application.productName + ".savedata.json";
                savedatabase = new SaveDataBase(path, fileName);
            }
            return savedatabase;
        }
    }

    private DataStorage()
    {
    }

    #region Public Static Methods

    public static void SetList<T>(string key, List<T> list)
    {
        Savedatabase.SetList(key, list);
    }

    public static List<T> GetList<T>(string key, List<T> _default)
    {
        return Savedatabase.GetList<T>(key, _default);
    }
    public static bool TryGetList<T>(string key, out List<T> data, List<T> _default = null)
    {
        return Savedatabase.TryGetList<T>(key, out data, _default);
    }

    public static T GetClass<T>(string key, T _default) where T : class, new()
    {
        return Savedatabase.GetClass(key, _default);

    }


    public static void SetClass<T>(string key, T obj) where T : class, new()
    {
        Savedatabase.SetClass<T>(key, obj);
    }

    public static void SetString(string key, string value)
    {
        Savedatabase.SetString(key, value);
    }

    public static string GetString(string key, string _default = "")
    {
        return Savedatabase.GetString(key, _default);
    }

    public static bool TryGetString(string key, out string data, string _default = "")
    {
        return Savedatabase.TryGetString(key, out data, _default);
    }

    public static void SetInt(string key, int value)
    {
        Savedatabase.SetInt(key, value);
    }


    public static int GetInt(string key, int _default = 0)
    {
        return Savedatabase.GetInt(key, _default);
    }

    public static bool TryGetInt(string key, out int data, int _default = 0)
    {
        return Savedatabase.TryGetInt(key, out data);
    }


    public static void SetFloat(string key, float value)
    {
        Savedatabase.SetFloat(key, value);
    }

    public static float GetFloat(string key, float _default = 0.0f)
    {
        return Savedatabase.GetFloat(key, _default);
    }
    public static bool TryGetFloat(string key, out float data)
    {
        return Savedatabase.TryGetFloat(key, out data);
    }
    /// <summary>
    /// Delete ALL key and values
    /// </summary>
    public static void Clear()
    {
        Savedatabase.Clear();
    }

    public static void Remove(string key)
    {
        Savedatabase.Remove(key);
    }

    public static bool ContainsKey(string _key)
    {
        return Savedatabase.ContainsKey(_key);
    }

    /// <summary>
    /// セーブデータに格納されたキーの一覧を取得します。
    /// </summary>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static List<string> Keys()
    {
        return Savedatabase.Keys();
    }

    public static void Save()
    {
        Savedatabase.Save();
    }



    #endregion

    #region SaveDatabase Class

    [Serializable]
    private class SaveDataBase
    {
        #region Fields

        //Saving path.
        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        // Saving file name.
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        //For set the key and the json strings.
        private Dictionary<string, string> saveDictionary;

        #endregion

        #region Constructor&Destructor

        public SaveDataBase(string _path, string _fileName)
        {
            path = _path;
            fileName = _fileName;
            saveDictionary = new Dictionary<string, string>();
            Load();
        }

        /// <summary>
        /// Write data to file when delete this class. 
        /// </summary>
        ~SaveDataBase()
        {
            Save();
        }

        #endregion

        #region Public Methods

        public void SetList<T>(string key, List<T> list)
        {
            keyCheck(key);
            var serializableList = new Serialization<T>(list);
            string json = JsonUtility.ToJson(serializableList);
            saveDictionary[key] = json;
        }

        public List<T> GetList<T>(string key, List<T> _default = null, bool safetyKey = false)
        {
            if (!safetyKey)
            {
                keyCheck(key);
                if (!saveDictionary.ContainsKey(key))
                {
                    return _default;
                }
            }
            string json = saveDictionary[key];
            Serialization<T> deserializeList = JsonUtility.FromJson<Serialization<T>>(json);

            return deserializeList.ToList();
        }
        public bool TryGetList<T>(string key, out List<T> data, List<T> _default = null)
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                data = _default;
                return false;
            }

            data = Savedatabase.GetList<T>(key, _default, true);
            return true;
        }


        public void SetClass<T>(string key, T obj) where T : class, new()
        {
            keyCheck(key);
            string json = JsonUtility.ToJson(obj);
            saveDictionary[key] = json;
        }
        public T GetClass<T>(string key, T _default, bool safetyKey = false) where T : class, new()
        {
            if (!safetyKey)
            {
                keyCheck(key);
                if (!saveDictionary.ContainsKey(key))
                    return _default;
            }
            string json = saveDictionary[key];
            T obj = JsonUtility.FromJson<T>(json);
            return obj;

        }
        public bool TryGetClass<T>(string key, out T data, T _default) where T : class, new()
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                data = _default;
                return false;
            }
            data = Savedatabase.GetClass<T>(key, _default, true);
            return true;
        }

        public void SetString(string key, string value)
        {
            keyCheck(key);
            saveDictionary[key] = value;
        }

        public string GetString(string key, string _default = "", bool safetyKey = false)
        {
            if (!safetyKey)
            {
                keyCheck(key);

                if (!saveDictionary.ContainsKey(key))
                    return _default;
            }
            return saveDictionary[key];
        }

        public bool TryGetString(string key, out string data, string _default = "")
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                data = _default;
                return false;
            }
            data = Savedatabase.GetString(key);
            return true;
        }

        public void SetInt(string key, int value)
        {
            keyCheck(key);
            saveDictionary[key] = value.ToString();
        }

        public int GetInt(string key, int _default = 0, bool safetyKey = false)
        {
            if (!safetyKey)
            {
                keyCheck(key);
                if (!saveDictionary.ContainsKey(key))
                    return _default;
            }
            int ret;
            if (!int.TryParse(saveDictionary[key], out ret))
            {
                ret = 0;
            }
            return ret;
        }

        public bool TryGetInt(string key, out int data, int _default = 0)
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                data = _default;
                return false;
            }
            data = Savedatabase.GetInt(key, _default, true);
            return true;
        }

        public void SetFloat(string key, float value)
        {
            keyCheck(key);
            saveDictionary[key] = value.ToString();
        }

        public float GetFloat(string key, float _default = 0.0f, bool safety = false)
        {
            if (!safety)
            {
                keyCheck(key);
            }
            float ret;
            if (!saveDictionary.ContainsKey(key))
                ret = _default;
            if (!float.TryParse(saveDictionary[key], out ret))
            {
                ret = 0.0f;
            }
            return ret;
        }
        public bool TryGetFloat(string key, out float data, float _default = 0.0f)
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                data = _default;
                return false;
            }
            data = Savedatabase.GetFloat(key, _default, true);
            return true;
        }
        public void Clear()
        {
            saveDictionary.Clear();

        }

        public void Remove(string key)
        {
            keyCheck(key);
            if (saveDictionary.ContainsKey(key))
            {
                saveDictionary.Remove(key);
            }

        }

        public bool ContainsKey(string _key)
        {

            return saveDictionary.ContainsKey(_key);
        }

        public List<string> Keys()
        {
            return saveDictionary.Keys.ToList<string>();
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(path + fileName, false, Encoding.GetEncoding("utf-8")))
            {
                var serialDict = new Serialization<string, string>(saveDictionary);
                serialDict.OnBeforeSerialize();
                string dictJsonString = JsonUtility.ToJson(serialDict);
                writer.WriteLine(dictJsonString);
            }
        }

        public void Load()
        {
            if (File.Exists(path + fileName))
            {
                using (StreamReader sr = new StreamReader(path + fileName, Encoding.GetEncoding("utf-8")))
                {
                    if (saveDictionary != null)
                    {
                        var sDict = JsonUtility.FromJson<Serialization<string, string>>(sr.ReadToEnd());
                        sDict.OnAfterDeserialize();
                        saveDictionary = sDict.ToDictionary();
                    }
                }
            }
            else { saveDictionary = new Dictionary<string, string>(); }
        }

        public string GetJsonString(string key)
        {
            keyCheck(key);
            if (saveDictionary.ContainsKey(key))
            {
                return saveDictionary[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// キーに不正がないかチェックします。
        /// </summary>
        private void keyCheck(string _key)
        {
            if (string.IsNullOrEmpty(_key))
            {
                throw new ArgumentException("invalid key!!");
            }
        }

        #endregion
    }

    #endregion

    #region Serialization Class

    // List<T>
    [Serializable]
    private class Serialization<T>
    {
        public List<T> target;

        public List<T> ToList()
        {
            return target;
        }

        public Serialization()
        {
        }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }
    // Dictionary<TKey, TValue>
    [Serializable]
    private class Serialization<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;
        private Dictionary<TKey, TValue> target;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return target;
        }

        public Serialization() { }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            Enumerable.Range(0, count).ToList().ForEach(i => target.Add(keys[i], values[i]));
        }
    }

    #endregion
}