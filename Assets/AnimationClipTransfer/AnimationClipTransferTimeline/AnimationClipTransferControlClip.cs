using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UMotionGraphicUtilities;

[Serializable]
public class AnimationClipTransferControlClip : PlayableAsset, ITimelineClipAsset
{
    public AnimationClipTransferControlBehaviour template = new AnimationClipTransferControlBehaviour ();
    public ExposedReference<AnimationClipTransfer> animationClipTransfer;
    public bool targetDisableOutOfClip = false;

    public bool reverse;
    // public bool disableTargetOutOfClip_post = false;
   
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Looping | ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending; } 
    }
    

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AnimationClipTransferControlBehaviour>.Create (graph, template);
        AnimationClipTransferControlBehaviour clone = playable.GetBehaviour ();
        clone.animationClipTransfer = animationClipTransfer.Resolve (graph.GetResolver ());
        clone.targetDisableOutOfClip = targetDisableOutOfClip;
        clone.reverse = reverse;
        // clone.DisableTargetOutOfClip_post = disableTargetOutOfClip_post;
        return playable;
    }
}
