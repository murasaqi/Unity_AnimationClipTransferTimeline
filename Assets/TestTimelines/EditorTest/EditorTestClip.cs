using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;

[Serializable]
public class EditorTestClip : PlayableAsset, ITimelineClipAsset
{
    public EditorTestBehaviour template = new EditorTestBehaviour ();
    public ExposedReference<AimConstraint> newExposedReference;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Looping | ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EditorTestBehaviour>.Create (graph, template);
        EditorTestBehaviour clone = playable.GetBehaviour ();
        clone.newExposedReference = newExposedReference.Resolve (graph.GetResolver ());
        return playable;
    }
}
