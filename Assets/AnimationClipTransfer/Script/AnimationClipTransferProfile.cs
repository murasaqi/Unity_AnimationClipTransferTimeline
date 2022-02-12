using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UMotionGraphicUtilities
{
    [CreateAssetMenu(menuName = "AnimationClipTransfer/Create AnimationClipTransferProfile")]
    public class AnimationClipTransferProfile : ScriptableObject
    {
        public AnimationClipMode animationClipMode = AnimationClipMode.Single;
        public AnimationClip animationClip;
        public List<AnimationClip> animationClips = new List<AnimationClip>();
        public List<AnimationStaggerElementCash> cashs;
    }
}