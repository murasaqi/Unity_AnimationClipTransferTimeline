using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

namespace UMotionGraphicUtilities
{
    [Serializable]
    public class NumberedAnimationClip
    {
        public int index = 0;
        public List<string> targets;
        public AnimationClip clip;

        // public NumberedAnimationClip()
        // {
        //     targets =  new List<string>() {"0"};
        //     clip = new AnimationClip();
        // }
    }
}