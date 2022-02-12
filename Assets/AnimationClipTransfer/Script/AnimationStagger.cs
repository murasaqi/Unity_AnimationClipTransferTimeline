using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;


namespace UMotionGraphicUtilities
{

    // public interface  AnimationStaggerElement
    // {
    //     float startTiming { get; set; }
    //     float endTiming { get; set; }
    //     public float startTimingCustom  { get; set; }
    //     public float endTimingCustom  { get; set; }
    //     public float lowLimit { get; set; }
    //     public float highLimit { get; set; }
    //     public float randomSeed { get; set; }
    //     public AnimationClip assignedSingleAnimationClip { get; set; }
    //     public AnimationClip assignedRandomAnimationClip { get; set; }
    //     public AnimationClip assignedManualAnimationClip { get; set; }
    //     public List<AnimationClip> assignedMultipleAnimationClip { get; set; }
    //     public StaggerType staggerType { get; set; }
    //     public AnimationClipMode animationClipMode { get; set; }
    //     public ValueCalcType valueCalcType { get; set; }
    //     public List<AnimationClip> animationClipCue  { get; set; }
    //     public TransformCash transformCash { get; set; }
    // }

    [ExecuteAlways]
    public class AnimationStagger : MonoBehaviour

    {
    [SerializeField] public bool updateInEditor = false;
    [SerializeField, Range(0, 1)] private float debugProgress = 0f;

    public float startTiming  = 0.3f;
    public float endTiming  = 0.7f;
    public float startTimingCustom   = 0.3f;
    public float endTimingCustom  = 0.7f;
    public float lowLimit  = 0f;
    public float highLimit  =1f;
    // public float randomSeed  = 0f;
    public AnimationClip assignedSingleAnimationClip;
    public AnimationClip assignedRandomAnimationClip;
    public AnimationClip assignedManualAnimationClip;
    public List<AnimationClip> assignedMultipleAnimationClip  = new List<AnimationClip>();
    // public StaggerType staggerType  = StaggerType.Custom;
    public AnimationClipMode animationClipMode  = AnimationClipMode.Single;
    public ValueCalcType valueCalcType_Position  = ValueCalcType.Add;
    public ValueCalcType valueCalcType_Rotation  = ValueCalcType.Add;
    public ValueCalcType valueCalcType_Scale  = ValueCalcType.Multiply;
    public List<AnimationClip> animationClipCue  = new List<AnimationClip>();
    public TransformCash transformCash  = new TransformCash();

    public AnimationClip PickSingleAnimationClipByMode()
    {
        if (animationClipMode == AnimationClipMode.Single)
        {
            return assignedSingleAnimationClip;
        }
        else if (animationClipMode == AnimationClipMode.Random)
        {
            return assignedRandomAnimationClip;
        }
        else if (animationClipMode == AnimationClipMode.Multiple)
        {
            return null;
        }
        else
        {
            return assignedManualAnimationClip;
        }
    }

    void Start()
    {

    }

    public void SetTransformCash()
    {
        transformCash = new TransformCash();
        transformCash.OwnTransform = transform;
        transformCash.LocalPosition = transform.localPosition;
        transformCash.LocalEulerAngle = transform.localEulerAngles;
        transformCash.LocalScale = transform.localScale;


    }

    private void InitAnimationClipCue()
    {

        animationClipCue.Clear();

        if (animationClipMode == AnimationClipMode.Multiple)
        {
            foreach (var v in assignedMultipleAnimationClip)
            {
                if (v != null) animationClipCue.Add(v);
            }
        }
        else
        {
            var a = PickSingleAnimationClipByMode();
            if (a != null) animationClipCue.Add(a);
        }

    }

    public void RandomAssignAnimationClip()
    {

    }

    public void UpdateSampleAnimation(float progress)
    {
        InitAnimationClipCue();
        transformCash.Progress = progress;
        foreach (var animationClip in animationClipCue)
        {
            animationClip.SampleAnimation(gameObject, progress * animationClip.averageDuration);
            // AnimationClipはなんかGetKeyできないからTransformのどこに差分があるかを初期値と比較してるやつ
            if (transform.localPosition != transformCash.LocalPosition)
            {

                if (valueCalcType_Position == ValueCalcType.None)
                {
                    transform.localPosition = transformCash.LocalPosition;
                }

                if (valueCalcType_Position == ValueCalcType.Add)
                {
                    transform.localPosition += transformCash.LocalPosition;
                }

                if (valueCalcType_Position == ValueCalcType.Subtract)
                {
                    transform.localPosition -= transformCash.LocalPosition;
                }

                if (valueCalcType_Position == ValueCalcType.Multiply)
                {
                    var offsetPos = transform.localPosition;
                    transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
                }

                if (valueCalcType_Position == ValueCalcType.Acceleration)
                {
                    var offsetPos = transform.localPosition;
                    transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
                }

            }

            if (transform.localEulerAngles != transformCash.LocalEulerAngle)
            {

                if (valueCalcType_Rotation == ValueCalcType.None)
                {
                    transform.localEulerAngles = transformCash.LocalEulerAngle;
                }

                if (valueCalcType_Rotation == ValueCalcType.Add)
                {
                    transform.localEulerAngles += transformCash.LocalEulerAngle;
                }

                if (valueCalcType_Rotation == ValueCalcType.Subtract)
                {
                    transform.localEulerAngles -= transformCash.LocalEulerAngle;
                }

                if (valueCalcType_Rotation == ValueCalcType.Multiply)
                {
                    var offsetEuler = transform.localEulerAngles;
                    transform.localEulerAngles = Vector3.Scale(offsetEuler, transformCash.LocalEulerAngle);
                }
            }

            if (transform.localScale != transformCash.LocalScale)
            {
                if (valueCalcType_Scale == ValueCalcType.None)
                {
                    // target.transform.localScale = transformCash.LocalScale;
                }

                if (valueCalcType_Scale == ValueCalcType.Add)
                {
                    transform.localScale += transformCash.LocalScale;
                }

                if (valueCalcType_Scale == ValueCalcType.Subtract)
                {
                    transform.localScale -= transformCash.LocalScale;
                }

                if (valueCalcType_Scale == ValueCalcType.Multiply)
                {
                    var offsetScale = transform.localScale;
                    transform.localScale = Vector3.Scale(offsetScale, transformCash.LocalScale);
                }

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (updateInEditor) UpdateSampleAnimation(debugProgress);
    }
    }
}