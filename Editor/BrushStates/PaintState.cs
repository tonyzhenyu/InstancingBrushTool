using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Instances;
namespace Instances.Editor.Brush
{
    public sealed class PaintState : BrushState
    {
        private Vector3 _lastPosition;
        private Vector3 _nowPosition;

        public delegate InstanceInfo InfoModifier(Vector3 position, Vector3 normal, float noise,float localScale , Color color);
        private InfoModifier _infoModifier;

        public PaintState()
        {
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _myColor = BrushConstData.LoadBrushDataRaw().COLOR_Paint;
            _distance = _brushInfo.distance;
            _maxDensity = _brushInfo.areaDensity;
            _infoModifier += BrushPaintStateModifier.InfoModifier_AU01;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);
            
            InstanceInfo[] data = DrawHandles(InstanceBrushTool.Instance.DirectHitPosition.position, InstanceBrushTool.Instance.DirectHitPosition.normal);

            if (_event.type == EventType.MouseDown && _event.button == 0 && _event.alt == false)
            {
                //Paintting
                EditorUtility.SetDirty(context.instanceData);

                foreach (var item in data)
                {
                    Planting(context, item);
                }
                //Undo.RecordObjects(data, "undoPainting");
                
                //editor displayer SetsBufferData
                //InDisplayEditor.SetDatas(InstanceBrushTool.Instance.InstanceDisplay);
                //
            }

            if ( _event.button == 0 && _event.rawType == EventType.MouseDrag && _event.alt == false)
            {
                if (_lastPosition == null)
                {
                    _lastPosition = InstanceBrushTool.Instance.DirectHitPosition.position;
                }

                _nowPosition = InstanceBrushTool.Instance.DirectHitPosition.position;

                float deltaDistance = Vector3.Distance(_nowPosition,_lastPosition);

                if (deltaDistance > _distance)
                {
                    //Paintting
                    EditorUtility.SetDirty(context.instanceData);

                    foreach (var item in data)
                    {
                        Planting(context, item);
                    }
                    //editor displayer SetsBufferData
                    //InDisplayEditor.SetDatas(InstanceBrushTool.Instance.InstanceDisplay);
                    //
                }
                _lastPosition = _nowPosition;
            }
            
            if (_event.type == EventType.KeyDown && _event.button != 1)
            {
                if (_event.keyCode == KeyCode.LeftShift)
                {
                    context.SetState(new EraserState());
                }
                if (_event.keyCode == KEY_size)
                {
                    context.SetState(new ConfigSizeState());
                }
                if (_event.keyCode == KEY_density)
                {
                    context.SetState(new ConfigNoiseWeightState());
                }
                if (_event.keyCode == KEY_scale)
                {
                    context.SetState(new ColorPickState());
                }
            }
        }

        //Execute Function
        private void Planting(BrushStateMgr context, InstanceInfo data)
        {                        
            //check AreaDensity
            float _radius = _brushInfo.radius;
            Vector3 position = InstanceBrushTool.Instance.DirectHitPosition.position;
            int count = 0;
            foreach (var j in context.instanceData.instances)
            {
                if (Vector3.Distance(j.Position, position) < _radius)
                {
                    count += 1;
                }
                if (count > _maxDensity)
                {
                    break;
                }
            }
            if (count > _maxDensity)
            {
                return;
            }
            //Check End

            if (context.instanceData.instances.Count > Mathf.Max(0, _maxOjbectCount - 2))
            {
                return;
            }

            context.instanceData.instances.Add(data);
            //Undo.RecordObject(context.instanceData, "Cached Painting");
            Debug.Log("Planting");
        }
        //Draw Function
        private InstanceInfo[] DrawHandles(Vector3 position, Vector3 normal)
        {
            float _radius = _brushInfo.radius;
            float _height = _brushInfo.height;
            float _weight = _brushInfo.noiseweight;
            int _density = _brushInfo.density;
            float localScale = _brushInfo.localscale;
            float noiseStrength = _brushInfo.noiseStrength;

            List<InstanceInfo> datas = new List<InstanceInfo>();

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, _radius);
            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, _radius,2);
            Handles.DrawLine(position, position + normal * _height);
            ProceduralDisc proceduralDisc = new ProceduralDisc(normal, position);
            proceduralDisc.noiseStrength = noiseStrength;
            Vector3[] points = proceduralDisc.GetPoints(_density, _radius, _weight);
            List<HitInfo> lsthitinfo = new List<HitInfo>();

            for (int i = 0; i < _density; i++)
            {
                //Handles.color = Color.red;
                Ray ray = new Ray(normal * _height + position, points[i] - (normal * _height + position));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Vector3.Magnitude(points[i] - (normal * _height + position)) + 0.38f,_brushInfo.layerMask.value))
                {
                    lsthitinfo.Add(new HitInfo(hit.point, hit.normal));
                    //Handles.color = new Color(0, 0.5f, 1, .5f);
                }
                //Handles.DrawLine(normal * _height + position, points[i]);
            }
            Handles.Label(position, $"Nums:{lsthitinfo.Count}");
            foreach (var item in lsthitinfo)
            {
                //Tint From Distance
                float _dis = Vector3.Distance(item.position, position) / _radius;
                Handles.color = new Color(0, 1, 0, Mathf.Clamp01(_dis));

                //------instance
                datas.Add(_infoModifier.Invoke(item.position, item.normal, proceduralDisc.Noise, localScale,_brushInfo.color));
                //---------
            }
            return datas.ToArray();
        }


        
    }
}
