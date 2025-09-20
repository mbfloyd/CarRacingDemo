using System.IO;
using UnityEngine;

namespace ALIyerEdon
{
    public class JsonFileHelper 
    {
        public static string GetFolderPath => Application.persistentDataPath + "/";
        public static T Load<T>(string fileName)
        {
            string path = GetFolderPath + fileName;
            if (!File.Exists(path))
            {
                Debug.LogWarning($"JSON file not found at path: {path}");
                return default;
            }

            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load JSON from {path}: {ex.Message}");
                return default;
            }
        }

        public static void Save( object obj, string fileName) {
            string path = GetFolderPath + fileName;
            try
            {
                string json = JsonUtility.ToJson(obj);
                string folderPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(folderPath);
                StreamWriter sw = File.CreateText(path);
                sw.Write(json);
                sw.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save JSON to {path}: {ex.Message}");                
            }
        }
    }
}
