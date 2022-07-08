using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System;

namespace Instances
{
    public class InstanceShadowPass : ScriptableRenderPass
    {
        const int k_ShadowmapBufferBits = 16;
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        public InstanceShadowPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingShadows;
        }
        public void SetUp(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            
            this.source = source;
            this.destination = destination;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var m_ShadowCasterCascadesCount = renderingData.shadowData.mainLightShadowCascadesCount;
            var renderTargetWidth = renderingData.shadowData.mainLightShadowmapWidth;
            var renderTargetHeight = (m_ShadowCasterCascadesCount == 2) ?
                renderingData.shadowData.mainLightShadowmapHeight >> 1 :
                renderingData.shadowData.mainLightShadowmapHeight;

            RenderTargetBinding rtb = new RenderTargetBinding();
            CommandBuffer cmd = CommandBufferPool.Get();
            ShadowUtils.GetTemporaryShadowTexture(renderTargetWidth, renderTargetHeight, k_ShadowmapBufferBits);
            //cmd.SetRenderTarget()
            //RenderTargetIdentifier shadowmaskRT = ;

            
            //RenderTexture rt = RenderTexture.GetTemporary();


            VisibleLight shadowlight = renderingData.lightData.visibleLights[renderingData.lightData.mainLightIndex];

            //var shadowbias = ShadowUtils.GetShadowBias(ref shadowlight, renderingData.lightData.mainLightIndex, ref renderingData.shadowData,, renderingData.shadowData.resolution);
            //cachedInfos = data.BufferInfoSets;
            //renderingData.

            //ShadowUtils.SetupShadowCasterConstantBuffer(cmd, ref shadowlight, shadowbias);
            shadowlight.light.AddCommandBuffer(LightEvent.AfterShadowMapPass, cmd);

        }
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
        }
    }
}
