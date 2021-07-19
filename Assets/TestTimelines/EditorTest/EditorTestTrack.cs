using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(EditorTestClip))]
public class EditorTestTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EditorTestMixerBehaviour>.Create (graph, inputCount);
    }
}
