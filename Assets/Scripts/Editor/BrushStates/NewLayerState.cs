using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    public sealed class NewLayerState : BrushState
    {
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);
            
            if (_event.type == EventType.MouseDown && _event.button == 0 && _event.alt == false)
            {
                bool dialog = EditorUtility.DisplayDialog("Create New Instance Layer?", "Create New Instance Layer On ../Resouces/Instance/..", "ok","cancel");

                if (dialog)
                {
                    InstanceDataInEditor.NewLayer();
                }
                else
                {

                }
            }
        }

    }
}
