using System.Collections.Generic;

namespace Instances
{
    public interface IInstanceDisplay
    {
        public void Display(int index);
        public void OnDisable(bool destroy);
    }
    public abstract class InstanceDisplayer : IInstanceDisplay
    {
        public enum DisplayMode { Isolated, All }
        public DisplayMode mode;
        public List<InstanceData> _instanceDatas;
        public abstract void Display(int index);
        public abstract void OnDisable(bool destroy);
    }
}