using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Instances;
namespace Instances.Editor.Brush
{
    sealed
    public class EraserState : BrushState
    {
        private Vector3 _lastPosition;
        private Vector3 _nowPosition;

        public EraserState()
        {
            BrushConstData constData = BrushConstData.LoadBrushDataRaw();

            _myColor = constData.COLOR_Eraser;
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _distance = _brushInfo.distance;
        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);

            DrawHandles(InstanceBrushTool.Instance.DirectHitPosition.position, InstanceBrushTool.Instance.DirectHitPosition.normal);

            if (_event.button == 1)
            {
                return;
            }

            if (_event.type == EventType.KeyUp && _event.keyCode == KeyCode.LeftShift)
            {
                context.SetState(new PaintState());
            }
            
            if (_event.shift)
            {
   
                if (_event.keyCode == KeyCode.F)
                {
                    context.SetState(new ConfigDensityState());
                }
                if (_event.keyCode == KeyCode.D)
                {
                    context.SetState(new ConfigNoiseStrengthState());
                }
                if (_event.keyCode == KEY_scale)
                {
                    context.SetState(new ConfigObjSizeState());
                }
            }

            if (_event.type == EventType.MouseDown && _event.button == 0 && _event.alt == false)
            {
                //clear data on brush
                EditorUtility.SetDirty(context.instanceData);
                Clear(context, InstanceBrushTool.Instance.DirectHitPosition.position);
                //
            }
            if (_event.rawType == EventType.MouseDrag && _event.button == 0 && _event.alt == false)
            {
                if (_lastPosition == null)
                {
                    _lastPosition = InstanceBrushTool.Instance.DirectHitPosition.position;
                }

                _nowPosition = InstanceBrushTool.Instance.DirectHitPosition.position;

                float deltaDistance = Vector3.Distance(_nowPosition, _lastPosition);

                if (deltaDistance > _distance)
                {
                    //clear data on brush
                    EditorUtility.SetDirty(context.instanceData);
                    Clear(context, InstanceBrushTool.Instance.DirectHitPosition.position);
                    //
                }
                _lastPosition = _nowPosition;
            }
        }
        private void DrawHandles(Vector3 position, Vector3 normal)
        {
            float _radius = _brushInfo.radius;
            float _height = _brushInfo.height;
            float _weight = _brushInfo.noiseweight;
            int _density = _brushInfo.density;
            float localscale = _brushInfo.localscale;
            float noiseStrength = _brushInfo.noiseStrength;

            //Handles.DrawGizmos(Camera.current);

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, 0.1f);
            Handles.DrawSolidDisc(position, normal, _radius);

            Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b, .5f);
            Handles.DrawWireDisc(position, normal, _radius, 2);
            Handles.DrawLine(position, position + normal * _height);
            //Handles.DrawLine(position, position + normal * _height);

            Handles.color = Color.white;
            Handles.Label(position, $"EraserMode");
        }
        private void Clear(BrushStateMgr context, Vector3 position)
        {
            float _radius = _brushInfo.radius;

            if (context.instanceData.Instancesinfo.Count <= 0)
            {
                return;
            }
            
            //Profiler.BeginSample("Eraser.Find");
            for (int i = 0; i < context.instanceData.Instancesinfo.Count; i++)
            {

                //Profiler.BeginSample("Eraser.Compare");
                bool foo = Vector3.Distance(position, context.instanceData.Instancesinfo[i].Position) >= _radius;
                //Profiler.EndSample();

                if (foo)
                {
                    continue;
                }

                //Profiler.BeginSample("Eraser.Remove");
                context.instanceData.Instancesinfo.Remove(context.instanceData.Instancesinfo[i]);
                //Undo.RecordObject(context.instanceData, "Cached Erasing");
                //Profiler.EndSample();
            }
            //Profiler.EndSample();

            //InDisplayEditor.SetDatas(InstanceBrushTool.Instance.InstanceDisplay);

            //Debug.Log("clearing");
        }
    }
}
