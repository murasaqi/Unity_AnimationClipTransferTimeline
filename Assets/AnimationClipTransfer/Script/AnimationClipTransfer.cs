
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMotionGraphicUtilities
{
    [ExecuteAlways]
    public class AnimationClipTransfer : MonoBehaviour
    {

        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private GameObject targetObject;
        [SerializeField] private AnimationTargetType animationTargetType;
        [SerializeField] private bool toggleActiveOnClip;
        [SerializeField] private ValueCalcType positionCalcType;
        [SerializeField] private ValueCalcType eulerCalcType;
        [SerializeField] private ValueCalcType scaleCalcScale;
        [SerializeField] private StaggerType staggerType;
        [SerializeField] private List<StaggerPropsBehaviour> staggerPropsList =new List<StaggerPropsBehaviour>();
        // [SerializeField] private StaggerOption staggerOption;
        [SerializeField] private float staggerRatio = 0.3f;
        [SerializeField] private TransformCash transformCash = null;
        [SerializeField] private List<TransformCash> childTransformCash = null;
        [SerializeField] private bool debugMode = false;
        [SerializeField] [Range(0, 1)] private float debugProgress;
        private float _previousProgress = 0f;

        public GameObject TargetObject
        {
            get => targetObject;
            set
            {
                if (targetObject != value)
                {
                    targetObject = value;
                    Init();
                }
            }
        }

        public StaggerType StaggerType
        {
            get => staggerType;
            set => staggerType = value;
        }

        public List<StaggerPropsBehaviour> StaggerPropsList => staggerPropsList;

        // Start is called before the first frame update
        void Start() { }

        private void OnEnable()
        {
            // Init();
        }

        private void OnValidate()
        {
            if (transformCash != null && transformCash.OwnTransform != targetObject.transform)
            {
                Init();
            }
        }


        public void Init()
        {
            if (targetObject == null) return;

            // targetObject = clone.TartgetObject;
            transformCash = new TransformCash();
            transformCash.OwnTransform = targetObject.transform;
            transformCash.LocalPosition = targetObject.transform.localPosition;
            transformCash.LocalEulerAngle = targetObject.transform.localEulerAngles;
            transformCash.LocalScale = targetObject.transform.localScale;
            transformCash.Progress = -1f;


            if (childTransformCash == null) childTransformCash = new List<TransformCash>();
            childTransformCash.Clear();
            foreach (Transform child in targetObject.transform)
            {
                var cash = new TransformCash();
                cash.OwnTransform = child;
                cash.LocalPosition = child.localPosition;
                cash.LocalEulerAngle = child.localEulerAngles;
                cash.LocalScale = child.localScale;
                cash.Progress = -1f;
                childTransformCash.Add(cash);
            }

            InitStaggerValues();

        }

        public void InitStaggerValues()
        {
            // staggerPropsList.Clear();
            // staggerPropsList = new List<StaggerPropsBehaviour>();
            var childLength = targetObject.transform.childCount;
            var childCount = 0;
            
            var ratio = staggerRatio;
            if (staggerType == StaggerType.AutoInOut) ratio *= 0.5f;
            var ratioStep = ratio / (targetObject.transform.childCount - 1);
            
            foreach (Transform child in targetObject.transform)
            {
                StaggerPropsBehaviour staggerPropsBehaviour;
                if (staggerPropsList.Count <= childCount)
                {
                    staggerPropsBehaviour = new StaggerPropsBehaviour();
                    staggerPropsList.Add(staggerPropsBehaviour);
                }

                staggerPropsBehaviour = staggerPropsList[childCount];

                if (staggerType != StaggerType.Custom)
                { 
                    var isIn = (staggerType == StaggerType.AutoIn || staggerType == StaggerType.AutoInOut);
                    var isOut = staggerType == StaggerType.AutoOut || staggerType == StaggerType.AutoInOut;
                    var childStart = isIn ? ratioStep * childCount : 0;
                    var childEnd = isOut ? 1f - ratioStep * (childLength - 1 - childCount) : 1;
                    
                    staggerPropsBehaviour.lowLimit = 0;
                    staggerPropsBehaviour.highLimit = 1;
                    staggerPropsBehaviour.startTiming = childStart;
                    staggerPropsBehaviour.endTiming = childEnd;
                }
             
                childCount++;

            }

            if (staggerPropsList.Count > childCount)
            {
                staggerPropsList.RemoveRange(childCount-1,staggerPropsList.Count- childCount);
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (targetObject != null && transformCash == null || targetObject != null && childTransformCash == null)
            {
                Init();
            }

            if (debugMode && _previousProgress != debugProgress)
            {
                ProcessFrame(debugProgress);
                _previousProgress = debugProgress;
            }
        }

        public void ProcessFrame(float progress)
        {
            if (animationClip == null || targetObject == null) return;
            
            if(staggerPropsList.Count == 0 || staggerPropsList.Count != targetObject.transform.childCount) InitStaggerValues();

            if (animationTargetType == AnimationTargetType.Own)
            {

                var animation = targetObject.GetComponent<Animation>();
                if (animation == null)
                {
                    animation = targetObject.AddComponent<Animation>();
                }
                animation.clip = animationClip;
                UpdateAnimation(transformCash, progress);

            }

            if (animationTargetType == AnimationTargetType.Children)
            {
                // var ratio = staggerOption.StaggerRatio;
                // if (staggerOption.In && staggerOption.Out) ratio *= 0.5f;
                // var ratioStep = ratio / (targetObject.transform.childCount - 1);

                var childLength = targetObject.transform.childCount;
                var childCount = 0;

                InitStaggerValues();
                foreach (Transform child in targetObject.transform)
                {
                    // var isIn = staggerOption.In;
                    // var isOut = staggerOption.Out;
                    // var childStart = isIn ? ratioStep * childCount : 0;
                    // var childEnd = isOut ? 1f - ratioStep * (childLength - 1 - childCount) : 1;
                    // Debug.Log(child.name);

                    var childProgress = Mathf.Clamp(Mathf.InverseLerp( staggerPropsList[childCount].startTiming,  staggerPropsList[childCount].endTiming, (float) progress), 0f, 1f);
                    // Debug.Log($"{child.name},{childProgress}");

                    // Debug.Log($"{child.name},{childStart},{childEnd},{childProgress}");
                    var animation = child.gameObject.GetComponent<Animation>();
                    if (animation == null)
                    {
                        animation = child.gameObject.AddComponent<Animation>();
                    }
                    animation.clip = animationClip;
                    UpdateAnimation(childTransformCash[childCount], childProgress);

                    childCount++;

                }
            }
        }

        public void UpdateAnimation(TransformCash transformCash, float progress)
        {
            var target = transformCash.OwnTransform.gameObject;
            // var clipAsset = clip.asset as AnimationClipTransferClip;
            if (toggleActiveOnClip) target.SetActive(true);


            if (transformCash.Progress != progress)
            {



                // Debug.Log($"Update motion");
                // animation.clip = input.AnimationClip;
                // animation.enabled = true;
                animationClip.SampleAnimation(target, progress * animationClip.averageDuration);
                transformCash.Progress = progress;
                // AnimationClipはなんかGetKeyできないからTransformのどこに差分があるかを初期値と比較してるやつ
                if (target.transform.localPosition != transformCash.LocalPosition)
                {

                    if (positionCalcType == ValueCalcType.Add)
                    {
                        target.transform.localPosition += transformCash.LocalPosition;
                    }

                    if (positionCalcType == ValueCalcType.Subtract)
                    {
                        target.transform.localPosition -= transformCash.LocalPosition;
                    }

                    if (positionCalcType == ValueCalcType.Multiply)
                    {
                        var offsetPos = target.transform.localPosition;
                        target.transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
                    }

                    if (positionCalcType == ValueCalcType.Acceleration)
                    {
                        var offsetPos = target.transform.localPosition;
                        target.transform.localPosition = Vector3.Scale(offsetPos, transformCash.LocalPosition);
                    }

                }

                if (target.transform.localEulerAngles != transformCash.LocalEulerAngle)
                {

                    if (eulerCalcType == ValueCalcType.Add)
                    {
                        target.transform.localEulerAngles += transformCash.LocalEulerAngle;
                    }

                    if (eulerCalcType == ValueCalcType.Subtract)
                    {
                        target.transform.localEulerAngles -= transformCash.LocalEulerAngle;
                    }

                    if (eulerCalcType == ValueCalcType.Multiply)
                    {
                        var offsetEuler = target.transform.localEulerAngles;
                        target.transform.localEulerAngles = Vector3.Scale(offsetEuler, transformCash.LocalEulerAngle);
                    }
                }

                if (target.transform.localScale != transformCash.LocalScale)
                {

                    if (scaleCalcScale == ValueCalcType.Add)
                    {
                        target.transform.localScale += transformCash.LocalScale;
                    }

                    if (scaleCalcScale == ValueCalcType.Subtract)
                    {
                        target.transform.localScale -= transformCash.LocalScale;
                    }

                    if (scaleCalcScale == ValueCalcType.Multiply)
                    {
                        var offsetScale = target.transform.localScale;
                        target.transform.localScale = Vector3.Scale(offsetScale, transformCash.LocalScale);
                    }

                }
            }

        }
    }
}