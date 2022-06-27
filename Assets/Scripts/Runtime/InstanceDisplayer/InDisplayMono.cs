using System.Collections.Generic;
using UnityEngine;


namespace Instances
{
    public sealed class InDisplayMono : InstanceDisplayer
    {
        private List<ComputeBuffer> _buffers;
        private List<ComputeBuffer> _argsbuffers;
        private List<Material> _tmpMaterial;

        private MInstanceDisplay _mono;
        private Matrix4x4 cachedMatrix;
        public Bounds bounds
        {
            get
            {
                return CBufferHelper.GetBounds(_mono.transform.position, 1000);
            }
        }

        public InDisplayMono(MInstanceDisplay _mono)
        {
            this._mono = _mono;
            this._buffers = new List<ComputeBuffer>();
            this._argsbuffers = new List<ComputeBuffer>();
            this._tmpMaterial = new List<Material>();

            this._instanceDatas = _mono.instanceDatas;

            for (int i = 0; i < _instanceDatas.Count; i++)
            {
                if (_instanceDatas[i].instances.Count == 0)
                {
                    _buffers.Add(null);
                    _argsbuffers.Add(CBufferHelper.GetArgsBuffer(_instanceDatas[i]));
                    _tmpMaterial.Add(null);
                    continue;
                }
                _buffers.Add(CBufferHelper.GetInfoBuffer(_instanceDatas[i]));
                _argsbuffers.Add(CBufferHelper.GetArgsBuffer(_instanceDatas[i]));

                _tmpMaterial.Add(new Material(_instanceDatas[i].material));
                _tmpMaterial[i].SetBuffer(CBufferHelper.ShaderId, _buffers[i]);
            }
        }

        public override void Display(int index)
        {

            Matrix4x4 local2World = _mono.transform.localToWorldMatrix;

            if (cachedMatrix != local2World)
            {
                cachedMatrix = local2World;
                for (int i = 0; i < _instanceDatas.Count; i++)
                {
                    if (_buffers[i] != null)
                    {
                        _buffers[i].SetData(_instanceDatas[i].GetBufferInfoSetsWithArgs(cachedMatrix));
                    }
                }
            }
            if (index == -1)
            {
                mode = DisplayMode.All;

                for (int i = 0; i < _instanceDatas.Count; i++)
                {
                    if (_tmpMaterial[i] == null)
                    {
                        continue;
                    }
                    Graphics.DrawMeshInstancedIndirect(_instanceDatas[i].mesh, 0, _tmpMaterial[i], bounds, _argsbuffers[i], castShadows: UnityEngine.Rendering.ShadowCastingMode.TwoSided);
                }
            }
            else
            {
                mode = DisplayMode.Isolated;
                if (_tmpMaterial[index] == null)
                {
                    return;
                }
                Graphics.DrawMeshInstancedIndirect(_instanceDatas[index].mesh, 0, _tmpMaterial[index], bounds, _argsbuffers[index], castShadows: UnityEngine.Rendering.ShadowCastingMode.TwoSided);
            }

        }
        public override void OnDisable(bool destroy)
        {
            if (destroy == false) return;

            for (int i = 0; i < _buffers.Count; i++)
            {
                if (_buffers[i] != null)
                {
                    _buffers[i].Dispose();
                }
                _buffers[i] = null;
            }
            for (int i = 0; i < _argsbuffers.Count; i++)
            {
                if (_argsbuffers[i] != null)
                {
                    _argsbuffers[i].Dispose();
                }
                _argsbuffers[i] = null;
            }
        }
    }
}