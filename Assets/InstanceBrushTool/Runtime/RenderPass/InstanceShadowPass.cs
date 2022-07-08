using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System;

namespace Instances
{
    public class InstanceShadowPass : ScriptableRenderPass
    {
        public InstanceShadowPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingShadows;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            CommandBuffer cmd = CommandBufferPool.Get();
            


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
