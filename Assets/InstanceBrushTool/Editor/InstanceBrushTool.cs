using System.Collections;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System;
using Instances;
using System.IO;

namespace Instances.Editor.Brush
{
    [EditorTool(displayName: "Instance Brush Tool")]
    public class InstanceBrushTool : EditorTool
    {
        private static InstanceBrushTool _instance;
        public static InstanceBrushTool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = InstanceBrushTool.CreateInstance<InstanceBrushTool>();
                }
                return _instance;
            }
        }
        public Ray MousePosition
        {
            get
            {
                Event @event = Event.current;
                Vector2 mousePos = @event.mousePosition;

                mousePos.y = (float)Camera.current.pixelHeight - mousePos.y;

                return Camera.current.ScreenPointToRay(mousePos);
            }
        }
        public HitInfo DirectHitPosition
        {
            get
            {
                RaycastHit hit;
                if (Physics.Raycast(MousePosition, out hit ,1000, BrushInfoData.LoadBrushDataRaw().layerMask.value))
                {
                    return new HitInfo(hit.point, hit.normal);
                }
                else
                {
                    return default;
                }
            }
        }
        public InDisplayEditor InstanceDisplay
        {
            get => _instanceDisplay as InDisplayEditor;
        }
        
        private BrushConstData _brushConstData;
        private InstanceDisplayer _instanceDisplay;
        private BrushStateMgr _brushState;
        private InstanceDataInEditor _instanceDatas;
        private int _index = 0;
        private Action _onRefreash;
        private bool _isInstanceLstNull = true;
        private bool _isInstanceNull = true;
        private bool _isActivated = true;
        public bool IsActivated { get => _isActivated; set => _isActivated = value; }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
            _isActivated = false;
            if (_instanceDatas != null)
            {
                _instanceDatas.onvaildate -= _onRefreash;
            }


            //Debug.Log("DeActivated Brush Tool");
        }
        public override void OnActivated()
        {
            base.OnActivated();

            _isActivated = true;
            _instanceDatas = InstanceDataInEditor.LoadBrushDataRaw();
            _brushConstData = BrushConstData.LoadBrushDataRaw();

            _index = 0;

            _onRefreash += ReFreashBrushState;
            _instanceDatas.onvaildate += _onRefreash;
            
            _onRefreash?.Invoke();
            
            //Deselection all
            Selection.SetActiveObjectWithContext(null, null);

            _instance = this;
        }
        public override GUIContent toolbarIcon
        {
            get
            {
                GUIContent content = new GUIContent();
                content.tooltip = "Instance Brush";
                
                string path = "Packages/com.zy.instancingbrushtool/icon.png";
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (icon != null)
                {
                    content.image = icon;

                    return content;
                }
            
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/EditorResources/" + BrushToolConstInfo.icon);
                content.image = icon;

                if (icon == null) return base.toolbarIcon;

                return content;
            }
        }
        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);

            Event @event = Event.current;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            _brushState?.OnUpdate();

            // -- Global Input
            if (@event.type == EventType.KeyDown)
            {
                if (@event.keyCode == KeyCode.G)
                {
                    // Focus On point
                    SceneView.currentDrawingSceneView.LookAt(DirectHitPosition.position);
                }
                //on switch pages/Layers
                if (@event.keyCode == _brushConstData.KEY_PageUp)
                {
                    if (_index < _instanceDatas.instanceDatas.Count - 1)
                    {
                        _index++;
                        _onRefreash?.Invoke();
                    }
                }
                if (@event.keyCode == _brushConstData.KEY_PageDown)
                {
                    if (_index > -1)
                    {
                        _index--;
                        _onRefreash?.Invoke();
                    }
                }
            }
            // -- 

            PanelGUI(window);

            _instanceDisplay?.Display(_index);
        }

        private void CheckBool()
        {
            if (_index == -1)
            {
                return;
            }
            else
            {
                bool isLstNull;
                if (_instanceDatas.instanceDatas == null||
                    _instanceDatas.instanceDatas.Count < 1)
                {
                    isLstNull = true;
                }
                else
                {
                    isLstNull = false;
                }
                _isInstanceLstNull = isLstNull;
                if (isLstNull == false)
                {
                    bool isInstanceNull = _instanceDatas.instanceDatas[_index] == null;
                    _isInstanceNull = isInstanceNull;
                }
            }
            
        }
        private void ReFreashBrushState()
        {
            CheckBool();
            if (_isInstanceNull == true || _isInstanceLstNull == true)
            {
                _brushState = new BrushStateMgr(null);
                _brushState.SetState(new NewLayerState());

                _instanceDisplay = null;
                return;
            }
            _instanceDisplay = new InDisplayEditor(_instanceDatas.instanceDatas);
            if (_index >= 0)
            {
                _brushState = new BrushStateMgr(_instanceDatas.instanceDatas[_index]);
                _brushState.SetState(new PaintState());
            }
            
        }
        private void PanelGUI(EditorWindow window)
        {
            GUILayout.BeginArea(new Rect(new Vector2(0, 0), new Vector2(300, 100)), "InstanceBrushTool");

            if (_isInstanceLstNull)
            {
                GUILabel01();
            }
            else
            {
                if (_isInstanceNull)
                {
                    GUILabel01();
                }
                else
                {
                    GUILabel02();
                }
            }
            GUILayout.EndArea();
        }

        private void GUILabel01()
        {
            string keyCode = KeyCode.Mouse0.ToString();
            GUILayout.BeginVertical();
            GUILayout.Label($"Instance Layer Not Set Yet.");
            GUILayout.Label($"Press {keyCode} To New an Instance Layer");
            GUILayout.EndVertical();
        }
        private void GUILabel02()
        {
            GUILayout.BeginVertical();
            
            GUILayout.Label($"{_brushState?.Brushstate?.ToString()}");
            if (_index == -1)
            {
                GUI.Box(new Rect(0, 0, 250, 70), "");
                int count = 0;
                for (int i = 0; i < _instanceDatas.instanceDatas.Count; i++)
                {
                    count += _instanceDatas.instanceDatas[i].instances.Count;
                }
                GUILayout.Label($"InstanceCount£º{count}");
            }
            else
            {
                GUI.Box(new Rect(0, 0, 250, 100), "");
                GUILayout.Label($"InstanceLayer£º{_instanceDatas.instanceDatas[_index].name}");
                GUILayout.Label($"CurrentLayerIndex£º{ _index}");
                GUILayout.Label($"InstanceCount£º{_instanceDatas.instanceDatas[_index].instances.Count}");
            }

            GUILayout.Label($"DisplayMode£º{_instanceDisplay?.mode}");
            GUILayout.EndVertical();
        }

        //shortcut
        [Shortcut(id: "Brush Tool/Active", context: typeof(SceneView), defaultKeyCode: KeyCode.B)]
        static void SetActivated()
        {
            ToolManager.SetActiveTool<InstanceBrushTool>();
        }



    }
}
