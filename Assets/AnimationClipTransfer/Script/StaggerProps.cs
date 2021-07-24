using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace UMotionGraphicUtilities
{
    
    [Serializable]
    public class StaggerPropsBehaviour
    {
        public string name;
        [Range(0,1) ]  public float startTiming = 0.3f;
        [Range(0,1) ]public float endTiming = 0.7f;

        public float lowLimit = 0;
        public float highLimit = 2;

        public float RandomSeed = 0;

    }
    
    public class StaggerProps: MonoBehaviour
    {
        [Range(0,1) ]  public float startTiming = 0.3f;
        [Range(0,1) ]public float endTiming = 0.7f;

        public float lowLimit = 0;
        public float highLimit = 2;


    }
    
    
}

