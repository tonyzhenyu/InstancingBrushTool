using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;

namespace Instances.Editor.Brush
{
    sealed
    public class ConfigDensityState : BrushState
    {
        private HitInfo _lsthitinfo;
        private int _tmpDensity;

        public ConfigDensityState()
        {

            BrushConstData constData = BrushConstData.LoadBrushDataRaw();

            _maxDensity = constData.MaxDensity;
            _scColor = constData.COLOR_ConfigDensity;
            _myColor = constData.COLOR_ConfigDensity;

            _lsthitinfo = InstanceBrushTool.Instance.DirectHitPosition;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _lastMousePosition = Event.current.mousePosition;
            _tmpDensity = _brushInfo.density;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);
            
            DrawHandles(_lsthitinfo.position, _lsthitinfo.normal, _brushInfo.radius, 0);

            Vector2 currentMousePosition = _event.mousePosition;

            float sizeDelta = Mathf.Clamp((currentMousePosition.x - _lastMousePosition.x) / 10, 0, 64);//
            if (_event.type == EventType.MouseDown && _event.button == 0)
            {
                //save data
                _brushInfo.density = _tmpDensity;
                EditorUtility.SetDirty(_brushInfo);
                context.SetState(new PaintState());
            }
            if (_event.type == EventType.MouseDown && _event.button == 1)
            {
                //cancel 
                context.SetState(new PaintState());
            }

            _tmpDensity = (int)Mathf.Floor(sizeDelta);
        }

        private void DrawHandles(Vector3 position, Vector3 normal, float _radius, float offset)
        {
            float _height = _brushInfo.height;
            int _density = _tmpDensity;
            float _weight = _brushInfo.noiseweight;
            float localscale = _brushInfo.localscale;
            float noiseStrength = _brushInfo.noiseStrength;

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidArc(position, normal, Vector3.Cross(Vector3.right,normal), ((float)_density / (float)_maxDensity) * 360, _radius);

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireArc(position, normal, Vector3.Cross(Vector3.right, normal), ((float)_density / (float)_maxDensity) * 360, _radius, 2);

            Handles.color = new Color(_scColor.r, _scColor.g, _scColor.b, 0.1f);
            Handles.DrawSolidArc(position, normal, Vector3.Cross(Vector3.right, normal), 360, _radius);

            Handles.color = new Color(_scColor.r, _scColor.g, _scColor.b, .5f);
            Handles.DrawWireArc(position, normal, Vector3.Cross(Vector3.right, normal), 360, _radius, 2);

            Handles.DrawLine(position, position + normal * _height);

            Handles.Label(position + normal * offset, $"Density:{_density}");


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
                Handles.color = new Color(0, 1, 0, Mathf.Clamp01(_dis));

                Handles.DrawWireCube(item.position, Vector3.one * 0.05f * _radius * proceduralDisc.Noise * localscale);
                Handles.DrawLine(item.position, item.position + item.normal);
            }
        }
    }
}

