using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataStorage
{
    private static Dictionary<string, object> dataStorage = new Dictionary<string, object>();

    public static void StoreData(string key, object data, bool replace = true)
    {
        key = key.ToLower().Trim();
        if (replace)
        {
            dataStorage[key] = data;
        }
        else
        {
            if (dataStorage.ContainsKey(key))
            {
                return;
            }
            else
            {
                dataStorage[key] = data;
            }
        }
    }

    public static bool GetStoredData(string key, out object data, bool delete = false)
    {
        key = key.ToLower().Trim();
        if (dataStorage.TryGetValue(key, out data))
        {
            if (delete)
            {
                dataStorage.Remove(key);
            }
            return true;
        }
        return false;
    }
}
