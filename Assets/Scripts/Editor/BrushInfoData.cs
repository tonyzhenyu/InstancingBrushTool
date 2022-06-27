using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Instances.Editor.Brush
{
    public class BrushInfoData : ScriptableObject
    {
        public float radius = 5f;
        public float height = 5f;
        public float localscale = 1;
        public int density = 6;
        public int areaDensity = 64;
        public float noiseweight = 0;
        public float noiseStrength = 1;
        public Color color;
        public float distance = 0.25f;
        public LayerMask layerMask;
        public static void ResetData()
        {

        }
        public static BrushInfoData LoadBrushDataRaw()
        {
            return EditorScriptableObjectHelper.LoadEditorDataRaw<BrushInfoData>(BrushToolConstInfo.brushInfoData);
        }
    }
    public struct HitInfo
    {
        public Vector3 position;
        public Vector3 normal;

        public HitInfo(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }
    }
}
#endif