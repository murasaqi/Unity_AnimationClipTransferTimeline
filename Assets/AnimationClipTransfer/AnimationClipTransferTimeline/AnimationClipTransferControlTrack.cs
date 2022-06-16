using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UMotionGraphicUtilities;

[TrackColor(0.6959884f, 0.5204105f, 0.9245283f)]
[TrackClipType(typeof(AnimationClipTransferControlClip))]
public class AnimationClipTransferControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<AnimationClipTransferControlMixerBehaviour>.Create (graph, inputCount);

        mixer.GetBehaviour().clips = GetClips().ToList();
			
        mixer.GetBehaviour().m_PlayableDirector = go.GetComponent<PlayableDirector>();
      

        return mixer;
    }
}
