using System.Collections.Generic;
using UnityEngine;

using Instances;
public class MInstanceDisplay : MonoBehaviour
{
    [Min(-1)]public int index = 0;
    public List<InstanceData> instanceDatas;
    public bool isdisplay = false;

    private InstanceDisplayer instanceDisplay;

    private void Start()
    {
        instanceDisplay = new InDisplayMono(this);
    }
    private void Update()
    {
        if (instanceDatas.Count < 1 || isdisplay == false) return;

        instanceDisplay?.Display(index);
    }
    private void OnDisable()
    {
        instanceDisplay?.OnDisable(true);
    }
    private void OnRenderObject()
    {
        
    }
    private void OnDrawGizmos()
    {
        if (isdisplay == false)
        {
            return;
        }
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(transform.position, 1000 * Vector3.one);
    }
}
