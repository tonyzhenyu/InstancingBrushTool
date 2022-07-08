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
            _drawInstancePass = new InstanceDrawPass(instanceData);
            _drawInstancePass.renderPassEvent = settings.m_RenderEvent;

            for (int i = 0; i < instanceData?.Length; i++)
            {
                if (instanceData[i] != null)
                {
                    instanceData[i].refreashAction += () =>
                    {
                        _drawInstancePass?.refreashaction?.Invoke(instanceData);
                    };
                }
            }
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
            _drawInstancePass.OnDistroy();
            for (int i = 0; i < instanceData?.Length; i++)
            {
                if (instanceData[i] != null)
                {
                    instanceData[i].refreashAction -= OnValidate;
                }
            }
        }
        private void OnDisable()
        {
            _drawInstancePass.cbuffer.ForEach(x => x?.Dispose());
            _drawInstancePass.OnDistroy();
        }
        private void OnEnable()
        {
            _drawInstancePass?.refreashaction?.Invoke(instanceData);
        }
        private void OnValidate()
        {
            _drawInstancePass?.refreashaction?.Invoke(instanceData);

            if (_drawInstancePass != null)
            {
                _drawInstancePass.renderPassEvent = settings.m_RenderEvent;
            }
            for (int i = 0; i < instanceData?.Length; i++)
            {
                if (instanceData[i] != null)
                {
                    instanceData[i].refreashAction += () =>
                    {
                        _drawInstancePass?.refreashaction?.Invoke(instanceData);
                    };
                }
            }
        }

    }
}
