using UnityEngine;
using UnityEngine.Rendering.Universal;
using Instances;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Instances
{
    public class InstanceRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        struct Settings
        {
            public RenderPassEvent m_RenderEvent;
        }
        [SerializeField] Settings settings;

        public InstanceData[] instanceData;
        private InstanceDrawPass _drawInstancePass;


        /// <inheritdoc/>
        public override void Create()
        {
            _drawInstancePass = new InstanceDrawPass(this);
            _drawInstancePass.renderPassEvent = settings.m_RenderEvent;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {

            //_drawInstancePass.cbuffer?.ForEach(x => x?.Dispose());
            if (instanceData != null)
            {
                renderer.EnqueuePass(_drawInstancePass);    

            }
            
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _drawInstancePass.cbuffer.ForEach(x => x?.Dispose());
        }
        private void OnDisable()
        {
            _drawInstancePass.cbuffer.ForEach(x => x?.Dispose());
        }
        private void OnEnable()
        {
            RefreashBufferData();
        }
        private void OnValidate()
        {
            RefreashBufferData();
        }
        void RefreashBufferData()
        {
            if (_drawInstancePass != null)
            {
                _drawInstancePass?.cbuffer?.ForEach(x => x?.Dispose());
                _drawInstancePass.cbuffer = new List<ComputeBuffer>();

                for (int i = 0; i < instanceData.Length; i++)
                {
                    if (instanceData[i] == null)
                    {
                        continue;
                    }
                    _drawInstancePass?.cbuffer?.Add(new ComputeBuffer(instanceData[i].instances.Count, InstanceInfo.InstanceBufferInfo.GetSize(), ComputeBufferType.Structured));
                }
            }
            
        }
    }
}
