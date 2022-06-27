using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    sealed
    public class ConfigNoiseWeightState : BrushState
    {
        private HitInfo _lsthitinfo;
        private float _tmpWeight;
        //float tmpHeight;
        public ConfigNoiseWeightState()
        {
            _myColor = BrushConstData.LoadBrushDataRaw().COLOR_ConfigSize;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _lsthitinfo = InstanceBrushTool.Instance.DirectHitPosition;
            _lastMousePosition = Event.current.mousePosition;
            _tmpWeight = _brushInfo.noiseweight;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);

            DrawHandles(_lsthitinfo.position, _lsthitinfo.normal,_tmpWeight);

            Vector2 currentMousePosition = _event.mousePosition;

            float sizeDelta;//

            sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / 100, 0, 1);
            if (_event.type == EventType.MouseDown &&  _event.button == 0)
            {
                //save data
                context.SetState(new PaintState());
                _brushInfo.noiseweight = _tmpWeight;
            }
            if (_event.type == EventType.MouseDown && _event.button == 1)
            {
                //cancel 
                context.SetState(new PaintState());
            }
            if ( _event.shift)
            {

            }

            _tmpWeight = sizeDelta;
        }
        private void DrawHandles(Vector3 position, Vector3 normal, float _weight)
        {
            float _radius = _brushInfo.radius;
            float _height = _brushInfo.height;
            int _density = _brushInfo.density;
            float localscale = _brushInfo.localscale;
            float noiseStrength = _brushInfo.noiseStrength;

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, _radius);
            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, _radius, 2);
            Handles.DrawLine(position, position + normal * _height);
            Handles.Label(position + normal, $"NoiseWeight:{_weight}");

            ProceduralDisc proceduralDisc = new ProceduralDisc(normal, position);
            proceduralDisc.noiseStrength = noiseStrength;
            Vector3[] points = proceduralDisc.GetPoints(_density, _radius, _weight);

            // physics direction ray
            List<HitInfo> lsthitinfo = new List<HitInfo>();

            for (int i = 0; i < _density; i++)
            {
                Ray ray = new Ray(normal * _height + position, points[i] - (normal * _height + position));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Vector3.Magnitude(points[i] - (normal * _height + position)) + 0.38f))
                {
                    lsthitinfo.Add(new HitInfo(hit.point, hit.normal));

                }
            }
            foreach (var item in lsthitinfo)
            {

                //Tint From Distance
                float _dis = Vector3.Distance(item.position, position) / _radius;
                Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, Mathf.Clamp01(_dis));

                Handles.DrawWireCube(item.position, Vector3.one * 0.05f * _radius * proceduralDisc.Noise * localscale);

                Handles.DrawLine(item.position, item.position + item.normal);
            }
        }

    }
}
