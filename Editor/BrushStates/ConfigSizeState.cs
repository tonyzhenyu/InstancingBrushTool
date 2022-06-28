using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    sealed
    public class ConfigSizeState : BrushState
    {
        private HitInfo _lsthitinfo;

        private float _dampingBrushSize = 100;
        private float _tmpRad;
        
        //float tmpHeight;
        public ConfigSizeState()
        {

            BrushConstData constData = BrushConstData.LoadBrushDataRaw();

            _myColor = constData.COLOR_ConfigSize;
            _scColor = constData.COLOR_Paint;
            _lsthitinfo = InstanceBrushTool.Instance.DirectHitPosition;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _lastMousePosition = Event.current.mousePosition;

            _tmpRad = _brushInfo.radius;
            //tmpHeight = brushInfo.height;

            _dampingBrushSize = 100;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);

            DrawHandlesWire(_lsthitinfo.position, _lsthitinfo.normal, _tmpRad, _brushInfo.height, 1);
            DrawHandles(_lsthitinfo.position, _lsthitinfo.normal, _brushInfo.radius, _brushInfo.height, 0);

            Vector2 currentMousePosition = _event.mousePosition;

            float sizeDelta;//
            //float heightDelta;

            sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / 10, 0, 25);
            //heightDelta = Mathf.Clamp(- (currentMousePosition.y - lastMousePosition.y) / 10, 0, 10);

            if (_event.type == EventType.MouseDown &&  _event.button == 0)
            {
                //save data
                _brushInfo.radius = _tmpRad;
                EditorUtility.SetDirty(_brushInfo);
                context.SetState(new PaintState());
                //brushInfo.height = tmpHeight;
            }
            if (_event.type == EventType.MouseDown && _event.button == 1)
            {
                //cancel 
                context.SetState(new PaintState());
            }
            if ( _event.shift)
            {
                sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / _dampingBrushSize, 0, 25);
            }

            _tmpRad = sizeDelta;
            //tmpHeight = heightDelta;
        }
        private void DrawHandles(Vector3 position, Vector3 normal , float _radius ,float _height, float offset)
        {
            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, _radius);

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, _radius, 2);

            Handles.DrawLine(position, position + normal * _height);

            Handles.Label(position + normal * offset, $"Radius:{_radius}");
        }
        private void DrawHandlesWire(Vector3 position, Vector3 normal, float _radius, float _height, float offset)
        {
            Handles.color = new Color(_scColor.r, _scColor.g, _scColor.b, 1f);
            Handles.DrawWireDisc(position, normal, _radius,2);

            Handles.DrawLine(position, position + normal * _height);

            Handles.Label(position + normal * offset, $"Radius:{_radius}");
        }
        
    }
}
