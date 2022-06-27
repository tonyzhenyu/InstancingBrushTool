
using UnityEditor;
using UnityEngine;

namespace Instances.Editor.Brush
{
    public static class BrushToolConstInfo
    {
        public const string instanceDataInEditor = "Brush.InstanceDisplayerLayerInfo";
        public const string brushConstData = "Brush.Profile";
        public const string brushInfoData = "Brush.Info";
    }
}
namespace Instances.Editor
{
    public static class EditorScriptableObjectHelper
    {
        public static T LoadEditorDataRaw<T>(string name) where T : ScriptableObject
        {
            string path = $"EditorData.{name}";
            string foldername = "EditorResources";

            string parentFolder = "Assets";
            string floderPath = $"{parentFolder}/{foldername}";
            string assetPath = $"{floderPath}/{path}.asset";

            if (AssetDatabase.IsValidFolder(floderPath) == false)
            {
                AssetDatabase.CreateFolder(parentFolder, foldername);
            }

            T data = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance(typeof(T)) as T;
                EditorUtility.SetDirty(data);
                AssetDatabase.CreateAsset(data, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            //Debug.Log("LoadData");
            return data;
        }
        public static T CreateNewData<T>() where T : ScriptableObject
        {
            //int count = 0;
            //string path = $"Instance.New Layer";
            //string foldername = "Resources";

            //string parentFolder = "Assets/Game";
            //string floderPath = $"{parentFolder}/{foldername}";
            //string assetPath = $"{floderPath}/{path}.asset";

            //if (AssetDatabase.IsValidFolder(floderPath) == false)
            //{
            //    AssetDatabase.CreateFolder(parentFolder, foldername);
            //}

            //T data;

            //do
            //{
            //    count++;
            //    path = path + count;
            //    AssetDatabase.ass
            //    data = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            //} while (data == null);

            //if (data == null)
            //{
            //    data = ScriptableObject.CreateInstance(typeof(T)) as T;
            //    EditorUtility.SetDirty(data);
            //    AssetDatabase.CreateAsset(data, assetPath);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();
            //}
            return null;
        }
    }

}
