using UnityEditor;
using UnityEngine;
using Instances;

namespace Instances.Editor.Brush
{
    sealed
    public class ColorPickState : BrushState
    {
        private Color _shotColor;
        private RenderTexture _rt1;
        private Rect _viewRect;
        private Camera _virtualCam;
        private Vector2Int MousePositionFormat
        {
            get => new Vector2Int(Mathf.FloorToInt(_event.mousePosition.x), Mathf.FloorToInt(Screen.height - _event.mousePosition.y));
        }

        public ColorPickState()
        {
            _brushInfo = BrushInfoData.LoadBrushDataRaw();
            _viewRect = Camera.current.pixelRect;
            _rt1 = RenderTexture.GetTemporary((int)_viewRect.width, (int)_viewRect.height);
            _virtualCam = new GameObject("_editorUseVirtualCam").AddComponent<Camera>();

        }
        public override void OnUpdate(BrushStateMgr context)
        {
            base.OnUpdate(context);

            Vector3 offsetPosition = GetPositionFromCamera(Vector3.zero);
            Vector3 originMousePosition = GetPositionFromCamera(Vector3.up * -50);

            DrawLine(offsetPosition, originMousePosition, _shotColor);
            DrawHandles(offsetPosition, Camera.current.transform.forward, _shotColor);

            _shotColor = Pick(MousePositionFormat);

            if (_event.type == EventType.KeyUp && _event.keyCode == KEY_scale)
            {
                _brushInfo.color = _shotColor;

                RenderTexture.ReleaseTemporary(_rt1);

                //_virtualCam.targetTexture?.Release();
                //RenderTexture.active.Release();

                GameObject.DestroyImmediate(_virtualCam.gameObject);
                //_virtualCam = null;
                

                EditorUtility.SetDirty(_brushInfo);
                context.SetState(new PaintState());
            }
        }
        private Vector3 GetPositionFromCamera(Vector3 offset)
        {
            return Camera.current.ScreenToWorldPoint(new Vector3(_event.mousePosition.x, (Screen.height - _event.mousePosition.y), 10) + offset);
        }

        private void DrawHandles(Vector3 position,Vector3 normal , Color col)
        {
            float _radius = 0.25f;

            Handles.color = col;
            Handles.DrawSolidDisc(position, normal, _radius);
            Handles.DrawWireDisc(position, normal, _radius,2);

            Handles.Label(position, $"    {col.ToString()}");
            //Handles.color = Color.white;
        }
        private void DrawLine(Vector3 origin,Vector3 destination,Color col)
        {
            Handles.color = col;
            //Handles.color = new Color(_myColor.r, _myColor.g, _myColor.b);
            Handles.DrawLine(origin, destination,2);
        }
        private Color Pick(Vector2Int position)
        {
            Rect viewRect = _viewRect;
            RenderTexture rt1 = _rt1;
            Camera cam = SceneView.currentDrawingSceneView.camera;

            cam.targetTexture = rt1;
            cam.forceIntoRenderTexture = true;

            _virtualCam.CopyFrom(cam);

            RenderTexture.active = _rt1;
            Texture2D screenshot = new Texture2D((int)viewRect.width, (int)viewRect.height, TextureFormat.ARGB32, false);

            //screenshot.SetPixels()
            //screenshot = _virtualCam.targetTexture
            screenshot.ReadPixels(viewRect,0,0,false);
            screenshot.Apply(false);

            Color color = screenshot.GetPixel(
               Mathf.Clamp(position.x, 0, (int)viewRect.width), 
               Mathf.Clamp(position.y - 50, 0, (int)viewRect.height)
                );

            //Color color = screenshot.GetPixelBilinear(position.x / viewRect.width, position.y / viewRect.height);

            
            return color;
        }
    }
}
