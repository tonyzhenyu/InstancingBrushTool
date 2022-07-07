using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Instances
{
    public class InDisplayUniversal : InstanceDisplayer
    {
        protected InstanceData[] instanceData { get; set; }
        public InDisplayUniversal(InstanceData[] instanceDatas)
        {
            instanceData = instanceDatas;
        }
        public override void Display(int index)
        {

        }
        public override void initData(int index,Action<int> action)
        {
            if (index == -1)
            {
                if (mode != DisplayMode.All)
                {
                    mode = DisplayMode.All;
                }
                
                for (int i = 0; i < instanceData.Length; i++)
                {
                    if (instanceData[i].material == null)
                    {
                        continue;
                    }
                    //inject
                    action(i);
                }
            }
            else
            {
                if (mode != DisplayMode.Isolated)
                {
                    mode = DisplayMode.Isolated;
                }
                if (instanceData[index].material == null)
                {
                    return;
                }
                //inject
                action(index);
            }
        }

        public override void OnDisable(bool destroy)
        {
            if (destroy == false) return;

        }
    }
}
