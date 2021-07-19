using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EditorTest2Clip : PlayableAsset, ITimelineClipAsset
{
    public EditorTest2Behaviour template = new EditorTest2Behaviour ();
    public ExposedReference<GameObject> newExposedReference;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Looping | ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EditorTest2Behaviour>.Create (graph, template);
        EditorTest2Behaviour clone = playable.GetBehaviour ();
        clone.newExposedReference = newExposedReference.Resolve (graph.GetResolver ());
        return playable;
    }
}
