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
        public List<AnimationClip> assignedMultipleAnimationClip = new List<AnimationClip>();
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
    
    [CreateAssetMenu(menuName = "AnimationClipTransfer/Create AnimationStaggerProps")]
    [Serializable]
    public class AnimationStaggerProps: ScriptableObject
    {
        [Range(0, 1)] public float startTiming = 0.3f;
        [Range(0, 1)] public float endTiming = 0.7f;
        public float startTimingCustom = 0.3f;
        public float endTimingCustom = 0.3f;
        public float lowLimit = 0;
        public float highLimit = 1;
        public float randomSeed = 0;
        public AnimationClip assignedSingleAnimationClip = null;
        public AnimationClip assignedRandomAnimationClip = null;
        public AnimationClip assignedManualAnimationClip = null;
        public List<AnimationClip> assignedMultipleAnimationClip = new List<AnimationClip>();
        public StaggerType staggerType;
        public AnimationClipMode animationClipMode = AnimationClipMode.Single;
        public ValueCalcType valueCalcType;
        public List<AnimationClip> animationClipCue = new List<AnimationClip>();
        public TransformCash transformCash;

    }
    
    public class StaggerProps: MonoBehaviour
    {
        [Range(0,1) ]  public float startTiming = 0.3f;
        [Range(0,1) ]public float endTiming = 0.7f;

        public float lowLimit = 0;
        public float highLimit = 2;


    }
    
    
}

