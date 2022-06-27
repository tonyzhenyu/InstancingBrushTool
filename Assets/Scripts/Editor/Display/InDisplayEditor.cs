using System.Collections.Generic;
using UnityEngine;
using Instances;

namespace Instances.Editor
{
    public sealed class InDisplayEditor : InstanceDisplayer
    {
        public List<ComputeBuffer> buffers;
        public List<ComputeBuffer> argsbuffers;
        public List<Material> tmpMaterial;
        private Bounds bounds
        {
            get
            {
                return CBufferHelper.GetBounds(Vector3.zero, 1000);
            }
        }
        public InDisplayEditor(List<InstanceData> datas)
        {
            this._instanceDatas = datas;
            this.buffers = new List<ComputeBuffer>();
            this.argsbuffers = new List<ComputeBuffer>();
            this.tmpMaterial = new List<Material>();

            if (_instanceDatas.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < _instanceDatas.Count; i++)
            {
                if (_instanceDatas[i].instances.Count == 0)
                {
                    buffers.Add(null);
                    argsbuffers.Add(null);
                    tmpMaterial.Add(null);
                    continue;
                }

                buffers.Add(CBufferHelper.GetInfoBuffer(_instanceDatas[i]));
                argsbuffers.Add(CBufferHelper.GetArgsBuffer(_instanceDatas[i]));

                tmpMaterial.Add(new Material(_instanceDatas[i].material));
                tmpMaterial[i].SetBuffer(CBufferHelper.ShaderId, buffers[i]);

            }
        }
        public override void Display(int index)
        {
            if (_instanceDatas.Count <= 0)
            {
                return;
            }
            if (index == -1)
            {
                mode = DisplayMode.All;
                for (int i = 0; i < _instanceDatas.Count; i++)
                {
                    if (buffers[i] == null || argsbuffers[i] == null || tmpMaterial[i] == null)
                    {
                        continue;
                    }
                    Graphics.DrawMeshInstancedIndirect(_instanceDatas[i].mesh, 0, tmpMaterial[i], bounds, argsbuffers[i]);
                }
            }
            else
            {
                mode = DisplayMode.Isolated;
                if (_instanceDatas[index].instances.Count <= 0)
                {
                    return;
                }
                else
                {
                    if (index > _instanceDatas.Count || tmpMaterial[index] == null)
                    {
                        return;
                    }
                    Graphics.DrawMeshInstancedIndirect(_instanceDatas[index].mesh, 0, tmpMaterial[index], bounds, argsbuffers[index]);
                }
            }
        }
        public override void OnDisable(bool destroy)
        {
            if (destroy == false) return;

            for (int i = 0; i < buffers.Count; i++)
            {
                if (buffers[i] != null)
                {
                    buffers[i].Dispose();
                }
                buffers[i] = null;
            }
            for (int i = 0; i < argsbuffers.Count; i++)
            {
                if (argsbuffers[i] != null)
                {
                    argsbuffers[i].Dispose();
                }
                argsbuffers[i] = null;
            }
            for (int i = 0; i < tmpMaterial.Count; i++)
            {
                tmpMaterial[i] = null;
            }
            buffers.Clear();
            argsbuffers.Clear();
            tmpMaterial.Clear();
        }
        public static void SetDatas(InDisplayEditor inDisplayEditor)
        {
            if (inDisplayEditor._instanceDatas.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < inDisplayEditor._instanceDatas.Count; i++)
            {
                if (inDisplayEditor._instanceDatas[i].instances.Count <= 0)
                {
                    continue;
                }

                inDisplayEditor.argsbuffers[i] = CBufferHelper.GetArgsBuffer(inDisplayEditor._instanceDatas[i]);
                inDisplayEditor.buffers[i] = CBufferHelper.GetInfoBuffer(inDisplayEditor._instanceDatas[i]);

                if (inDisplayEditor.tmpMaterial[i] == null)
                {
                    inDisplayEditor.tmpMaterial[i] = new Material(inDisplayEditor._instanceDatas[i].material);
                }
                inDisplayEditor.tmpMaterial[i].SetBuffer(CBufferHelper.ShaderId, inDisplayEditor.buffers[i]);
            }
        }
    }
}