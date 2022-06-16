using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UMotionGraphicUtilities
{

    public class AnimationClipTransferMixerBehaviour : PlayableBehaviour
    {
        public TimelineClip[] Clips { get; set; }
        public PlayableDirector Director { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<AnimationClipTransferBehaviour> inputPlayable = (ScriptPlayable<AnimationClipTransferBehaviour>) playable.GetInput(i);
                AnimationClipTransferBehaviour input = inputPlayable.GetBehaviour();
                var clip = Clips[i];
                var clipAsset = clip.asset as AnimationClipTransferClip;

                var progress = (Director.time - clip.start) / (clip.end - clip.start);
                // if (clipAsset.AnimationTargetType == AnimationTargetType.Own)
                // {
                //
                //     var animation = input.TartgetObject.GetComponent<Animation>();
                //     if (animation == null)
                //     {
                //         animation = input.TartgetObject.AddComponent<Animation>();
                //     }
                //     animation.clip = input.AnimationClip;
                //     UpdateAnimation(clip, input.TartgetObject, clipAsset.TransformCash, animation, (float) progress);
                //
                // }
                //
                // if (clipAsset.AnimationTargetType == AnimationTargetType.Children)
                // {
                //     var ratio = clipAsset.StaggerRatio;
                //     if (clipAsset.StaggerType.In && clipAsset.StaggerType.Out) ratio *= 0.5f;
                //     var ratioStep = ratio / (input.TartgetObject.transform.childCount - 1);
                //
                //     var childLength = input.TartgetObject.transform.childCount;
                //     var childCount = 0;
                //
                //
                //     foreach (Transform child in input.TartgetObject.transform)
                //     {
                //         var isIn = clipAsset.StaggerType.In;
                //         var isOut = clipAsset.StaggerType.Out;
                //         var childStart = isIn ? ratioStep * childCount : 0;
                //         var childEnd = isOut ? 1f - ratioStep * (childLength - 1 - childCount) : 1;
                //         // Debug.Log(child.name);
                //
                //         var childProgress = Mathf.Clamp(Mathf.InverseLerp((float) childStart, (float) childEnd, (float) progress), 0f, 1f);
                //         // Debug.Log($"{child.name},{childProgress}");
                //
                //         Debug.Log($"{child.name},{childStart},{childEnd},{childProgress}");
                //         var animation = child.gameObject.GetComponent<Animation>();
                //         if (animation == null)
                //         {
                //             animation = child.gameObject.AddComponent<Animation>();
                //         }
                //         animation.clip = input.AnimationClip;
                //         UpdateAnimation(clip, child.gameObject, clipAsset.ChildTransformCash[childCount], animation, childProgress);
                //
                //         childCount++;
                //
                //     }
                // }


            }
        }

        private void UpdateAnimation(TimelineClip clip, GameObject target, TransformCash transformCash, Animation animation, float progress)
        {
            var clipAsset = clip.asset as AnimationClipTransferClip;
            if (clipAsset.ToggleActiveOnClip) target.SetActive(true);


            // if (transformCash.Progress != progress)
            // {
            //
            //
            //
            //     Debug.Log($"Update motion");
            //     // animation.clip = input.AnimationClip;
            //     // animation.enabled = true;
            //     animation.clip.SampleAnimation(target, (float) progress * animation.clip.averageDuration);
            //     transformCash.Progress = progress;
            //     // AnimationClipはなんかGetKeyできないからTransformのどこに差分があるかを初期値と比較してるやつ
            //     if (target.transform.localPosition != transformCash.LocalPosition)
            //     {
            //
            //         if (clipAsset.PositionCalcType == ValueCalcType.Add)
            //         {
            //             target.transform.localPosition += transformCash.LocalPosition;
            //         }
            //
            //         if (clipAsset.PositionCalcType == ValueCalcType.Subtract)
            //         {
            //             target.transform.localPosition -= transformCash.LocalPosition;
            //         }
            //
            //         if (clipAsset.PositionCalcType == ValueCalcType.Multiply)
            //         {
            //             var offsetPos = target.transform.localPosition;
            //             target.transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
            //         }
            //
            //         if (clipAsset.PositionCalcType == ValueCalcType.Acceleration)
            //         {
            //             var offsetPos = target.transform.localPosition;
            //             target.transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
            //         }
            //
            //     }
            //
            //     if (target.transform.localEulerAngles != transformCash.LocalEulerAngle)
            //     {
            //
            //         if (clipAsset.EulerCalcType == ValueCalcType.Add)
            //         {
            //             target.transform.localEulerAngles += transformCash.LocalEulerAngle;
            //         }
            //
            //         if (clipAsset.EulerCalcType == ValueCalcType.Subtract)
            //         {
            //             target.transform.localEulerAngles -= transformCash.LocalEulerAngle;
            //         }
            //
            //         if (clipAsset.EulerCalcType == ValueCalcType.Multiply)
            //         {
            //             var offsetEuler = target.transform.localEulerAngles;
            //             target.transform.localEulerAngles = Vector3.Scale(offsetEuler, transformCash.LocalEulerAngle);
            //         }
            //     }
            //
            //     if (target.transform.localScale != transformCash.LocalScale)
            //     {
            //
            //         if (clipAsset.ScaleCalcScale == ValueCalcType.Add)
            //         {
            //             target.transform.localScale += transformCash.LocalScale;
            //         }
            //
            //         if (clipAsset.ScaleCalcScale == ValueCalcType.Subtract)
            //         {
            //             target.transform.localScale -= transformCash.LocalScale;
            //         }
            //
            //         if (clipAsset.ScaleCalcScale == ValueCalcType.Multiply)
            //         {
            //             var offsetScale = target.transform.localScale;
            //             target.transform.localScale = Vector3.Scale(offsetScale, transformCash.LocalScale);
            //         }
            //
            //     }
            // }

        }

        private Animation CheckAnimationComponent(GameObject target)
        {
            var animaion = target.GetComponent<Animation>();
            if (animaion == null)
            {
                animaion = target.AddComponent<Animation>();
            }

            return animaion;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);
            foreach (var clip in Clips)
            {
                var asset = clip.asset as AnimationClipTransferClip;
                asset.InitTargetObjectTransform();
            }
        }
    }
}