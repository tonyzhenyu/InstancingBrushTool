using System.Collections.Generic;
using UnityEngine;

namespace Instances
{
    public class CBufferHelper
    {
        public static string ShaderId = "_Infosbuffer";
        public static ComputeBuffer GetArgsBuffer(InstanceData layerData)
        {
            uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
            int submeshIndex = 0;
            int count = layerData.instances.Count;

            ComputeBuffer argsbuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            if (layerData.mesh == null)
            {
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            args[0] = (uint)layerData.mesh.GetIndexCount(submeshIndex);
            args[1] = (uint)count;
            args[2] = (uint)layerData.mesh.GetIndexStart(submeshIndex);
            args[3] = (uint)layerData.mesh.GetBaseVertex(submeshIndex);

            argsbuffer.SetData(args);

            return argsbuffer;
        }
        public static void SetBufferDatasInOne(ref ComputeBuffer cbuffer, List<InstanceData> instanceDatas)
        {
            List<InstanceInfo.InstanceBufferInfo> lstbuf = new List<InstanceInfo.InstanceBufferInfo>();

            for (int i = 0; i < instanceDatas.Count; i++)
            {
                var buf = instanceDatas[i].BufferInfoSets;
                for (int j = 0; j < buf.Length; j++)
                {
                    lstbuf.Add(buf[j]);
                }
            }

            cbuffer.SetData(lstbuf.ToArray());

            lstbuf.Clear();
        }
        public static void SetBufferDatasInOne(ref ComputeBuffer cbuffer, List<InstanceData> instanceDatas, Matrix4x4 args)
        {
            List<InstanceInfo.InstanceBufferInfo> lstbuf = new List<InstanceInfo.InstanceBufferInfo>();

            for (int i = 0; i < instanceDatas.Count; i++)
            {
                var buf = instanceDatas[i].GetBufferInfoSetsWithArgs(args);
                for (int j = 0; j < buf.Length; j++)
                {
                    lstbuf.Add(buf[j]);
                }
            }

            cbuffer.SetData(lstbuf.ToArray());

            lstbuf.Clear();
        }
        public static ComputeBuffer GetBufferInOne(List<InstanceData> instanceDatas)
        {

            int count = default;
            int _shaderid = Shader.PropertyToID(ShaderId);
            int strideSize = InstanceInfo.InstanceBufferInfo.GetSize();

            if (instanceDatas.Count <= 0)
            {
                return null;
            }

            List<InstanceInfo.InstanceBufferInfo> lstbuf = new List<InstanceInfo.InstanceBufferInfo>();

            for (int i = 0; i < instanceDatas.Count; i++)
            {
                int tmpcount = instanceDatas[i].instances.Count;
                count += tmpcount;

                var buf = instanceDatas[i].BufferInfoSets;
                for (int j = 0; j < buf.Length; j++)
                {
                    lstbuf.Add(buf[j]);
                }
            }

            ComputeBuffer cbuffer = new ComputeBuffer(count, strideSize, ComputeBufferType.Structured);

            cbuffer.SetData(lstbuf.ToArray());

            for (int i = 0; i < instanceDatas.Count; i++)
            {
                instanceDatas[i].material.SetBuffer(_shaderid, cbuffer);
            }

            return cbuffer;
        }
        public static ComputeBuffer GetInfoBuffer(InstanceData layerData)
        {
            string buffername = layerData.name;
            int count = layerData.instances.Count;
            int _shaderid = Shader.PropertyToID(ShaderId);
            int strideSize = InstanceInfo.InstanceBufferInfo.GetSize();

            ComputeBuffer cbuffer = new ComputeBuffer(count, strideSize, ComputeBufferType.Structured);
            cbuffer.name = buffername;
            cbuffer.SetData(layerData.BufferInfoSets);
            //layerData.material.SetBuffer(_shaderid, cbuffer);

            return cbuffer;
        }
        public static ComputeBuffer GetInfoBuffer(InstanceData layerData, Matrix4x4 args)
        {
            string buffername = layerData.name;
            int count = layerData.instances.Count;
            int _shaderid = Shader.PropertyToID(ShaderId);
            int strideSize = InstanceInfo.InstanceBufferInfo.GetSize();

            ComputeBuffer cbuffer = new ComputeBuffer(count, strideSize, ComputeBufferType.Structured);
            cbuffer.name = buffername;
            cbuffer.SetData(layerData.GetBufferInfoSetsWithArgs(args));
            //layerData.material.SetBuffer(_shaderid, cbuffer);

            return cbuffer;
        }
        public static Bounds GetBounds(Vector3 position, float farClipPlane)
        {
            return new Bounds(position, Vector3.one * farClipPlane);
        }
        public static int GetCountOffset(int layerIndex)
        {
            return InstanceInfo.InstanceBufferInfo.GetSize() * layerIndex * 1024;
        }
        public static int GetLayerByteSize(InstanceData layerData)
        {
            int count = layerData.instances.Count;
            int stridebyte = InstanceInfo.InstanceBufferInfo.GetSize();

            return count * stridebyte;
        }
    }
}