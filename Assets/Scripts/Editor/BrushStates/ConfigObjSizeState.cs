using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    sealed
    public class ConfigObjSizeState : BrushState
    {
        private HitInfo _lsthitinfo;

        private float _dampingBrushSize = 100;
        private float _tmpLocalScale;

        private Material _previewMat;
        public ConfigObjSizeState()
        {

            BrushConstData constData = BrushConstData.LoadBrushDataRaw();

            _myColor = constData.COLOR_ConfigSize;
            _scColor = constData.COLOR_Paint;
            _lsthitinfo = InstanceBrushTool.Instance.DirectHitPosition;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _lastMousePosition = Event.current.mousePosition;

            _tmpLocalScale = _brushInfo.localscale;

            _dampingBrushSize = 100;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            Event _event = Event.current;

            DrawHandles(_lsthitinfo.position, _lsthitinfo.normal);

            Vector2 currentMousePosition = _event.mousePosition;

            float sizeDelta;
            sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / 10, 0, 25);

            if (_event.type == EventType.MouseDown &&  _event.button == 0)
            {
                //save data and exit
                _brushInfo.localscale = _tmpLocalScale;
                EditorUtility.SetDirty(_brushInfo);
                context.SetState(new PaintState());
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

            _tmpLocalScale = sizeDelta;
        }
        private void DrawHandles(Vector3 position, Vector3 normal)
        {
            float radius = _brushInfo.radius;
            float height = _brushInfo.height;
            int density = _brushInfo.density;
            float weight = _brushInfo.noiseweight;
            float localscale = _tmpLocalScale;
            float noiseStrength = _brushInfo.noiseStrength;

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, radius);
            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, radius, 2);
            Handles.DrawLine(position, position + normal * height);
            Handles.Label(position + normal, $"Obj Scale:{localscale}");

            ProceduralDisc proceduralDisc = new ProceduralDisc(normal, position);
            proceduralDisc.noiseStrength = noiseStrength;
            Vector3[] points = proceduralDisc.GetPoints(density, radius, weight);

            // physics direction ray
            List<HitInfo> lsthitinfo = new List<HitInfo>();

            for (int i = 0; i < density; i++)
            {
                Ray ray = new Ray(normal * height + position, points[i] - (normal * height + position));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Vector3.Magnitude(points[i] - (normal * height + position)) + 0.38f))
                {
                    lsthitinfo.Add(new HitInfo(hit.point, hit.normal));
                }
            }
            foreach (var item in lsthitinfo)
            {
                //Tint From Distance
                float _dis = Vector3.Distance(item.position, position) / radius;
                Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, Mathf.Clamp01(_dis));

                Handles.DrawWireCube(item.position, Vector3.one * proceduralDisc.Noise * localscale);

                //preview obj
                
                Handles.DrawLine(item.position, item.position + item.normal);
            }
        }

    }
}
