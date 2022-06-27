using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    sealed
    public class ConfigNoiseStrengthState : BrushState
    {
        private HitInfo _lsthitinfo;

        private float _dampingBrushSize = 100;
        private float _tmpStrength;

        public ConfigNoiseStrengthState()
        {

            _myColor = BrushConstData.LoadBrushDataRaw().COLOR_ConfigSize;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();

            _lsthitinfo = InstanceBrushTool.Instance.DirectHitPosition;
            _lastMousePosition = Event.current.mousePosition;
            _tmpStrength = _brushInfo.noiseStrength;

            _dampingBrushSize = 100;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);

            DrawHandles(_lsthitinfo.position, _lsthitinfo.normal , _tmpStrength);

            Vector2 currentMousePosition = _event.mousePosition;

            float sizeDelta;//
            sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / 10, 0, 5);

            //-----------
            if (_event.type == EventType.MouseDown &&  _event.button == 0)
            {
                //save data
                _brushInfo.noiseStrength = _tmpStrength;
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
                sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / _dampingBrushSize, 0, 5);
            }
            //----------------


            _tmpStrength = sizeDelta;

        }
        private void DrawHandles(Vector3 position, Vector3 normal , float noiseStrength)
        {
            float _radius = _brushInfo.radius;
            float _height = _brushInfo.height;
            float _weight = _brushInfo.noiseweight;
            int _density = _brushInfo.density;
            float localscale = _brushInfo.localscale;

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, _radius);

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, _radius, 2);

            Handles.DrawLine(position, position + normal * _height);

            Handles.Label(position + normal, $"NoiseStrength:{noiseStrength}"); 
            
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
