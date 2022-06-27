using UnityEngine;
using Instances;

namespace Instances.Editor.Brush 
{
    public sealed class BrushPaintStateModifier
    {
        public static InstanceInfo InfoModifier_AU01(Vector3 position, Vector3 normal, float noise, float localScale ,Color color)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0, Mathf.Rad2Deg * 2 * Mathf.PI * noise, 0));

            InstanceInfo instance = new InstanceInfo(position, normal, InstanceInfo.AXIS_DIRECTION.UP, Vector3.one * localScale * noise, color);
            instance.LocalRotation = rotation;

            float h;
            float s;
            float v;

            Color.RGBToHSV(color, out h, out s, out v);

            v = v * noise;
            s = s * noise;
            h = h * noise;

            //instance.Color = Color.HSVToRGB(h, s, v);

            return instance;
        }
    }
}
