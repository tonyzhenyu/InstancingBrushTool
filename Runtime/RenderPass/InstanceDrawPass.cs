using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace Instances
{
    public class InstanceDrawPass : ScriptableRenderPass
    {
        InstanceRenderFeature _instanceRenderFeature;
        InstanceInfo.InstanceBufferInfo[] cachedInfos;
        public List<ComputeBuffer> cbuffer;

        public InstanceDrawPass(InstanceRenderFeature instanceRenderFeature)
        {
            _instanceRenderFeature = instanceRenderFeature;
            
            cachedInfos = new InstanceInfo.InstanceBufferInfo[1];
            //cbuffer = new List<ComputeBuffer>();
            //cbuffer.ForEach((x) => x?.Dispose());


        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            
            //cbuffer.Clear();
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //VisibleLight shadowlight = renderingData.lightData.visibleLights[renderingData.lightData.mainLightIndex];

            //var shadowbias = ShadowUtils.GetShadowBias(ref shadowlight, renderingData.lightData.mainLightIndex, ref renderingData.shadowData,, renderingData.shadowData.resolution);

            for (int i = 0; i < _instanceRenderFeature.instanceData.Length; i++)
            {
                InstanceData data = _instanceRenderFeature.instanceData[i];
                if (data == null)
                {
                    continue;
                }

                CommandBuffer cmd = CommandBufferPool.Get($"DrawBrushInstances.part.{_instanceRenderFeature.instanceData[i].instances.Count}");
                MaterialPropertyBlock block = new MaterialPropertyBlock();

                //if (cbuffer[i].count != data.instances.Count)
                //{

                //}
                if (cachedInfos != data.BufferInfoSets)
                {
                    cachedInfos = data.BufferInfoSets;
                    cmd.SetBufferData(cbuffer[i], data.BufferInfoSets);
                    block.SetBuffer(CBufferHelper.ShaderId, cbuffer[i]);
                }
                //cachedInfos = data.BufferInfoSets;
                //renderingData.

                //ShadowUtils.SetupShadowCasterConstantBuffer(cmd, ref shadowlight, shadowbias);
                //shadowlight.light.AddCommandBuffer(LightEvent.AfterShadowMapPass, cmd);

                cmd.DrawMeshInstancedProcedural(data.mesh, 0, data.material, -1, data.instances.Count, block);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
            
            return;
        }
        
    }
}
