using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Instances
{
    [CreateAssetMenu(menuName = "Instances/New InstanceData", fileName = "InstanceData")]
    public class InstanceData : ScriptableObject
    {
        public Mesh mesh;
        public Material material;
        public List<InstanceInfo> instances;

        public InstanceInfo.InstanceBufferInfo[] BufferInfoSets
        {
            get
            {
                List<InstanceInfo.InstanceBufferInfo> m = new List<InstanceInfo.InstanceBufferInfo>();
                foreach (var item in instances)
                {
                    m.Add(item.BufferInfo);
                }
                return m.ToArray();
            }
        }
        public Matrix4x4[] matrix4X4s
        {
            get
            {
                List<Matrix4x4> m = new List<Matrix4x4>();
                foreach (var item in instances)
                {
                    m.Add(item.Matrix);
                }
                return m.ToArray();
            }
        }
        public Matrix4x4[] GetMatrixWithTranspose(Matrix4x4 matrix4X4)
        {
            List<Matrix4x4> m = new List<Matrix4x4>();
            foreach (var item in instances)
            {
                Vector3 newPos = matrix4X4 * new Vector4(item.Position.x, item.Position.y, item.Position.z, 1.0f);
                Matrix4x4 tmpMatrix = Matrix4x4.TRS(newPos, item.Rotation, item.LocalScale);
                m.Add(tmpMatrix);
            }
            return m.ToArray();
        }
        public InstanceInfo.InstanceBufferInfo[] GetBufferInfoSetsWithArgs(Matrix4x4 transpose)
        {
            List<InstanceInfo.InstanceBufferInfo> m = new List<InstanceInfo.InstanceBufferInfo>();
            foreach (var item in instances)
            {
                m.Add(item.GetBufferInfoWithArgs(transpose));
            }
            return m.ToArray();
        }
        public Color[] colors
        {
            get
            {
                List<Color> m = new List<Color>();
                foreach (var item in instances)
                {
                    m.Add(item.Color);
                }
                return m.ToArray();
            }
        }
        private void OnValidate()
        {

        }
    }
}