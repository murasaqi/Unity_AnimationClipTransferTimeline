using System;
using UnityEngine;
using UnityEngine.Playables;

// namespace AnimationClipTransfer
// {

    [Serializable]
    public class AnimationClipTransferBehaviour : PlayableBehaviour
    {
        public AnimationClip AnimationClip;
        public GameObject TartgetObject;
        // public double Curve;

        public float a = 1f;

        public override void OnPlayableCreate(Playable playable)
        {
            
        }



    }
// }