using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System;

namespace Instances
{
    public class InstanceDrawPass : ScriptableRenderPass
    {
        InstanceInfo.InstanceBufferInfo[] cachedInfos;
        InstanceData[] instanceDatas;

        public List<ComputeBuffer> cbuffer;
        public Action<InstanceData[]> refreashaction;

        public InstanceDrawPass(InstanceData[] data)
        {
            instanceDatas = data;
            cachedInfos = new InstanceInfo.InstanceBufferInfo[1];

            refreashaction += RefreashCBuffer;
            RefreashCBuffer(data);

        }
        public void OnDistroy()
        {
            refreashaction -= RefreashCBuffer;
        }
        private void RefreashCBuffer(InstanceData[] datas)
        {
            if (cbuffer != null)
            {
                cbuffer?.ForEach(x => x?.Dispose());
                cbuffer.Clear();
            }
            else
            {
                cbuffer = new List<ComputeBuffer>();
            }

            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] == null || instanceDatas[i].Instancesinfo.Count < 1)
                {
                    continue;
                }
                if (instanceDatas[i].Instancesinfo.Count != datas[i].Instancesinfo.Count)
                {
                    cbuffer?.ForEach((x) => x?.Dispose());
                    cbuffer.Clear();
                }
                //Debug.Log("Re Cbuffer");
                cbuffer?.Add(new ComputeBuffer(datas[i].Instancesinfo.Count, InstanceInfo.InstanceBufferInfo.GetSize(), ComputeBufferType.Structured));
            }
            instanceDatas = datas;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            for (int i = 0; i < instanceDatas.Length; i++)
            {
                InstanceData data = instanceDatas[i];
                if (data == null || cbuffer == null || instanceDatas[i].Instancesinfo.Count < 1)
                {
                    continue;
                }

                CommandBuffer cmd = CommandBufferPool.Get($"DrawBrushInstances.part.{instanceDatas[i].Instancesinfo.Count}");
                MaterialPropertyBlock block = new MaterialPropertyBlock();

                if (cachedInfos != data.BufferInfoSets)
                {
                    cachedInfos = data.BufferInfoSets;
                    cmd.SetBufferData(cbuffer[i], data.BufferInfoSets);
                    block.SetBuffer(CBufferHelper.ShaderId, cbuffer[i]);
                }

                int pass = data.material.FindPass("ForwardLit");
                cmd.DrawMeshInstancedProcedural(data.mesh, 0, data.material, pass, data.Instancesinfo.Count, block);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            return;
        }
    }

}
