using Unity.Mathematics;

using UnityEngine;

namespace Instances
{
    [System.Serializable]
    public struct InstanceInfo
    {
        [SerializeField] private Matrix4x4 matrix;
        [SerializeField] private quaternion localRotation;
        [SerializeField] private Color color;

        public enum AXIS_DIRECTION { FORWARD, UP, RIGHT }
        public Color Color { get => color; set => color = value; }
        public InstanceBufferInfo BufferInfo
        {
            get
            {
                return new InstanceBufferInfo(Position, Rotation, LocalScale, color);
            }
        }
        public InstanceBufferInfo GetBufferInfoWithArgs(Matrix4x4 transpose)
        {
            var t = new InstanceBufferInfo(transpose * Position, Rotation, LocalScale, Color);
            return t;
        }
        public Vector3 Position { get => matrix.GetPosition(); set { this.Matrix = Matrix4x4.TRS(value, this.Rotation, this.LocalScale); } }
        public Quaternion Rotation
        {
            get => matrix.rotation;
            set
            {
                this.matrix = Matrix4x4.TRS(this.Position, value, this.LocalScale);
            }
        }
        public quaternion LocalRotation
        {
            get
            {
                return localRotation;
            }
            set
            {
                localRotation = value;
                Rotation = Rotation * value;
            }
        }
        public Vector3 LocalScale { get => matrix.lossyScale; set { this.Matrix = Matrix4x4.TRS(this.Position, this.Rotation, value); } }
        public Vector3 Up
        {
            get
            {
                return Rotation * Vector3.up;
            }
            set
            {
                Rotation = Quaternion.FromToRotation(Vector3.up, value);
            }
        }
        public Vector3 Right
        {
            get
            {
                return Rotation * Vector3.right;
            }
            set
            {
                Rotation = Quaternion.FromToRotation(Vector3.right, value);
            }
        }
        public Vector3 Forward
        {
            get
            {
                return Rotation * Vector3.forward;
            }
            set
            {
                Rotation = Quaternion.FromToRotation(Vector3.forward, value);
            }
        }
        public Matrix4x4 Matrix
        {
            get => matrix;
            set => matrix = value;
        }
        public InstanceInfo(Vector3 position, Quaternion rotation, Vector3 localScale, Color col)
        {
            this.localRotation = Quaternion.identity;
            this.color = col;
            this.matrix = Matrix4x4.TRS(position, rotation, localScale);
        }

        public InstanceInfo(Vector3 position, Vector3 axisvector, AXIS_DIRECTION aXIS, Vector3 localScale, Color col)
        {
            this.localRotation = Quaternion.identity;
            this.color = col;
            this.matrix = Matrix4x4.TRS(position, Quaternion.identity, localScale);

            switch (aXIS)
            {
                case AXIS_DIRECTION.FORWARD:
                    Forward = axisvector;
                    break;
                case AXIS_DIRECTION.UP:
                    Up = axisvector;
                    break;
                case AXIS_DIRECTION.RIGHT:
                    Right = axisvector;
                    break;
                default:
                    break;
            }
        }
        public void SetPosition(Vector3 value)
        {
            this.Matrix = Matrix4x4.TRS(value, Rotation, LocalScale);
        }
        public struct InstanceBufferInfo
        {
            public float3 position;
            public float4 rotation;
            public float3 localScale;
            public float4 Color;

            public InstanceBufferInfo(Vector3 pos, Quaternion rotation, Vector3 localScale, Color color)
            {
                this.position = pos;
                this.rotation = new float4(rotation.x, rotation.y, rotation.z, rotation.w);
                this.localScale = localScale;
                this.Color = new float4(color.r, color.g, color.b, color.a);
            }

            public static int GetSize()
            {

                int rotationsize = sizeof(float) * 4;
                int positionsize = sizeof(float) * 3;
                int localscalesize = sizeof(float) * 3;
                int colorsize = sizeof(float) * 4;
                int totalsize = rotationsize + positionsize + localscalesize + colorsize;
                return totalsize;
                //return Marshal.SizeOf<InstanceBufferInfo>();
            }
        }
    }

}