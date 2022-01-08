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
        public float startTimingCustom = 0.3f;
        public float endTimingCustom = 0.3f;
        public AnimationClip assignedSingleAnimationClip;
        public AnimationClip assignedRandomAnimationClip;
        public AnimationClip assignedManualAnimationClip;
        public float lowLimit = 0;
        public float highLimit = 2;

        public float RandomSeed = 0;

        public StaggerType currentStaggerType;
        public AnimationClip PickAnimationClipByMode(AnimationClipMode mode)
        {
            if (mode == AnimationClipMode.Single)
            {
                return assignedSingleAnimationClip;
            }else
            if (mode == AnimationClipMode.Random)
            {
                return assignedRandomAnimationClip;
            }
            else
            {
                return assignedManualAnimationClip;
            }
        }

    }
    
    public class StaggerProps: MonoBehaviour
    {
        [Range(0,1) ]  public float startTiming = 0.3f;
        [Range(0,1) ]public float endTiming = 0.7f;

        public float lowLimit = 0;
        public float highLimit = 2;


    }
    
    
}

