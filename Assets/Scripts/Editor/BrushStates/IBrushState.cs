using Instances;
using UnityEngine;
namespace Instances.Editor.Brush
{
    public interface IBrushState
    {
        public void OnUpdate(BrushStateMgr context);
    }
    public class BrushStateMgr
    {
        public IBrushState Brushstate { get => _brushMode; }
        private IBrushState _brushMode;
        public InstanceData instanceData;
        public BrushStateMgr(InstanceData instanceData)
        {
            this.instanceData = instanceData;
        }
        
        public void SetState(IBrushState brushMode)
        {
            //Debug.Log(brushMode.ToString());
            _brushMode = brushMode;
        }
        public void OnUpdate()
        {
            _brushMode?.OnUpdate(this);
        }
    }

    public abstract class BrushState : IBrushState
    {
        protected Event _event;
        protected BrushInfoData _brushInfo = BrushInfoData.LoadBrushDataRaw();
        protected BrushConstData _brushconstData = BrushConstData.LoadBrushDataRaw();
        protected float _distance;
        protected float _maxDensity;

        protected Color _myColor;
        protected Color _scColor;

        protected Vector2 _lastMousePosition;

        protected KeyCode KEY_density = BrushConstData.LoadBrushDataRaw().KEY_DENSITY;
        protected KeyCode KEY_size = BrushConstData.LoadBrushDataRaw().KEY_SIZE;
        protected KeyCode KEY_scale = BrushConstData.LoadBrushDataRaw().KEY_SCALE;

        protected int _maxOjbectCount = BrushConstData.LoadBrushDataRaw().MaxObjNums;
        public BrushState()
        {

        }
        public virtual void OnUpdate(BrushStateMgr context)
        {
            _event = Event.current;

            if (_event.alt)
            {
                return;
            }
        }

        protected virtual void OnInstanceDraw(Mesh mesh,Material mat,Matrix4x4[] matrix4X4s)
        {
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrix4X4s);
        }
        protected virtual void OnBrushShapeDraw()
        {

        }
    }
}
