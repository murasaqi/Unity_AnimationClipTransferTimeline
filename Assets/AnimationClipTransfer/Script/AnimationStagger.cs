using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;


namespace UMotionGraphicUtilities
{

   

    [ExecuteAlways]
    public class AnimationStagger : MonoBehaviour

    {
    [SerializeField] public bool updateInEditor = false;
    [SerializeField, Range(0, 1)] public float progress = 0f;
    [SerializeField, Range(0, 1)] private float debugProgress = 0f;
    public float startTiming  = 0.3f;
    public float endTiming  = 0.7f;
    public float startTimingCustom   = 0.3f;
    public float endTimingCustom  = 0.7f;
    public float lowLimit  = 0f;
    public float highLimit  =1f;
    public float randomSeed  =0f;
    public NumberedAnimationClip assignedSingleAnimationClip = new NumberedAnimationClip();
    public NumberedAnimationClip assignedRandomAnimationClip = new NumberedAnimationClip();
    public NumberedAnimationClip assignedManualAnimationClip = new NumberedAnimationClip();
    public List<NumberedAnimationClip> assignedMultipleAnimationClip  = new List<NumberedAnimationClip>();
    public StaggerType staggerType  = StaggerType.AutoInOut;
    public AnimationClipMode animationClipMode  = AnimationClipMode.Single;
    public ValueCalcType valueCalcType_Position  = ValueCalcType.Add;
    public ValueCalcType valueCalcType_Rotation  = ValueCalcType.Add;
    public ValueCalcType valueCalcType_Scale  = ValueCalcType.Multiply;
    public List<NumberedAnimationClip> numberedAnimationClipCue  = new List<NumberedAnimationClip>();
    public TransformCash transformCash  = new TransformCash();
    // private List<string> targetChildList = new List<string>();
    public NumberedAnimationClip PickSingleAnimationClipByMode()
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

       


        // foreach (var animationClip in assignedMultipleAnimationClip)
        // {
        //     animationClip.targets = targetChildList;
        // }

        numberedAnimationClipCue.Clear();

        if (animationClipMode == AnimationClipMode.Multiple)
        {
            foreach (var v in assignedMultipleAnimationClip)
            {
                if (v != null) numberedAnimationClipCue.Add(v);
            }
        }
        else
        {
            var a = PickSingleAnimationClipByMode();
            if (a != null) numberedAnimationClipCue.Add(a);
        }

    }

    public void Reset()
    {
        progress = 0f;
        debugProgress = 0f;

        transformCash.LocalPosition = transformCash.LocalPosition;
        transformCash.LocalEulerAngle = transformCash.LocalEulerAngle;
        transformCash.LocalScale = transformCash.LocalScale;

        randomSeed = Random.Range(0, 1f);

    }

    public void UpdateSampleAnimation(float progress, List<NumberedAnimationClip> animationClips)
    {
        MultipleSampleAnimation(CalcProgress(progress), animationClips);
    }


    public void InitNumberedAnimationClipIndex()
    {
        
    }

    public void MultipleSampleAnimation(float childProgress, List<NumberedAnimationClip> numberedAnimationClips)
    {
        foreach (var numberedAnimationClip in numberedAnimationClips)
        {
            // Debug.Log(numberedAnimationClip.clip);
            if(numberedAnimationClip == null || numberedAnimationClip.clip == null) return;
            if (numberedAnimationClip.index == 0)
            {
                numberedAnimationClip.clip.SampleAnimation(gameObject, childProgress * numberedAnimationClip.clip.averageDuration);    
            }
            else if (numberedAnimationClip.index > 0)
            {
                numberedAnimationClip.clip.SampleAnimation(gameObject.transform.GetChild(numberedAnimationClip.index-1).gameObject, childProgress * numberedAnimationClip.clip.averageDuration);
            }
            
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
    public void UpdateSampleAnimation(float progress)
    {
        InitAnimationClipCue();
        MultipleSampleAnimation(CalcProgress(progress),numberedAnimationClipCue);
    }

    private float CalcProgress(float progress)
    {
        this.progress = progress;
        var start = staggerType == StaggerType.Custom ? startTimingCustom : startTiming;
        var end = staggerType == StaggerType.Custom ? endTimingCustom : endTiming;
        return Mathf.Clamp(Mathf.InverseLerp( start,end, (float) progress), 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateInEditor) UpdateSampleAnimation(debugProgress);
    }
    }
}