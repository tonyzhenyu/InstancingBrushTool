using System.Collections.Generic;
using UnityEngine;

namespace Instances.Editor.Brush
{
    public class ProceduralDisc
    {
        public Vector3 centerWorldSpace;
        public Vector3 normal;
        public float noiseStrength = 1;
        public float Noise { get => noise; }

        float noise;
        public ProceduralDisc(Vector3 normal, Vector3 position)
        {
            this.normal = normal;
            this.centerWorldSpace = position;
        }
        private Vector3 CaculateDiscPoint(float radius, float theta)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            Vector3 point = new Vector3(x, 0, y);

            point = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, normal)).MultiplyPoint3x4(point);

            return point;
        }
        const float PI = Mathf.PI;
        const float _2PI = Mathf.PI * 2;
        public Vector3[] GetPoints(int nums, float radius, float noiseWeight)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < nums; i++)
            {
                float _theta = i * _2PI / nums;

                Vector2 _noisePos = new Vector2(centerWorldSpace.x + _theta, centerWorldSpace.z + _theta) * noiseStrength;

                float _noise = Mathf.PerlinNoise(_noisePos.x, _noisePos.y);
                _noise = Mathf.Lerp(1, _noise, noiseWeight);

                //ouput noise
                noise = _noise;
                float _pradius = radius * _noise;
                points.Add(CaculateDiscPoint(_pradius, _theta) + centerWorldSpace);
            }
            return points.ToArray();
        }
        public Ray[] GetPointRays(int nums, float radius, float noiseWeight)
        {
            List<Ray> rays = new List<Ray>();
            Vector3[] points = GetPoints(nums, radius, noiseWeight);
            Vector3 origin = normal + centerWorldSpace;
            for (int i = 0; i < nums; i++)
            {
                rays.Add(new Ray(origin, points[i] - origin));
            }
            return rays.ToArray();
        }
    }

}