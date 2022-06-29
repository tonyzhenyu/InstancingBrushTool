
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
namespace Instances.Editor.Brush
{
    public class BrushConstData : ScriptableObject
    {

        public Color COLOR_Paint = new Color(0.22f, 0.52f, 1);
        public Color COLOR_ConfigSize = new Color(1, 0.75f, 0.05f);
        public Color COLOR_ConfigDensity = new Color(0.98f, .33f, .02f);
        public Color COLOR_Eraser = new Color(1, 0, 0.43f);

        public int MaxDensity = 64;
        public int MaxObjNums = 1024;
        public KeyCode KEY_FOCUS = KeyCode.G;
        public KeyCode KEY_SIZE = KeyCode.F;
        public KeyCode KEY_SCALE = KeyCode.S;
        public KeyCode KEY_DENSITY = KeyCode.D;
        public KeyCode KEY_PageUp = KeyCode.PageUp;
        public KeyCode KEY_PageDown = KeyCode.PageDown;
        public static BrushConstData LoadBrushDataRaw()
        {
            return EditorScriptableObjectHelper.LoadEditorDataRaw<BrushConstData>(BrushToolConstInfo.brushConstData);
        }

    }
}
#endif