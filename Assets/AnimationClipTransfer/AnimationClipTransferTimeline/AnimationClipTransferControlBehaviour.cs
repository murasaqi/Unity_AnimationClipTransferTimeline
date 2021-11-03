using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UMotionGraphicUtilities;

[Serializable]
public class AnimationClipTransferControlBehaviour : PlayableBehaviour
{
    public AnimationClipTransfer animationClipTransfer;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
