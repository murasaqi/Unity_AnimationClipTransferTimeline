using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UMotionGraphicUtilities
{

    [TrackColor(0.6640359f, 0.5985226f, 0.7075472f)]
    [TrackClipType(typeof(AnimationClipTransferClip))]
    public class AnimationClipTransferTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<AnimationClipTransferMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().Clips = GetClips().ToArray();

            return mixer;
        }
    }
}