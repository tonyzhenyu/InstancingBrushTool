using System;
using System.Collections.Generic;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    public class InstanceDataInEditor : ScriptableObject
    {
        public List<InstanceData> instanceDatas;

        [HideInInspector]public Action onvaildate;
        private void OnValidate()
        {
            if (InstanceBrushTool.Instance.IsActivated)
            {
                onvaildate?.Invoke();
            }
        }
        public static InstanceDataInEditor LoadBrushDataRaw()
        {
            return EditorScriptableObjectHelper.LoadEditorDataRaw<InstanceDataInEditor>(BrushToolConstInfo.instanceDataInEditor);
        }
        public static void NewLayer()
        {
            InstanceDataInEditor editorData = InstanceDataInEditor.LoadBrushDataRaw();

            //..todo
            //InstanceData data = EditorScriptableObjectHelper.;
            //editorData.instanceDatas.Add(data);
        }
    }

}
