using System;
using UnityEngine;
using UnityEngine.Playables;

namespace UMotionGraphicUtilities
{

    [Serializable]
    public class AnimationClipTransferBehaviour : PlayableBehaviour
    {
        public AnimationClip AnimationClip;
        public GameObject TartgetObject;
        // public double Curve;

        private float a = 1f;

        public override void OnPlayableCreate(Playable playable)
        {
            
        }



    }
}