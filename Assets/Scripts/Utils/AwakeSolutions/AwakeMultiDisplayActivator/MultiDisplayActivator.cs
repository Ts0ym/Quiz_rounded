using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeSolutions
{
    public class MultiDisplayActivator : MonoBehaviour
    {
        void Start()
        {
            for (int i = 0; i < Display.displays.Length; i++)
                Display.displays[i].Activate();
            Debug.Log("Activated displays: "+Display.displays.Length);
        }
    }
}