
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UMotionGraphicUtilities
{
    [ExecuteAlways]
    public class AnimationClipTransfer : MonoBehaviour
    {

        [HideInInspector] [SerializeField] private AnimationClip animationClip;
        [HideInInspector] [SerializeField] private GameObject targetObject;
        // [SerializeField] private AnimationTargetType animationTargetType;
        // [SerializeField] private bool toggleActiveOnClip;
        [HideInInspector] [SerializeField] private ValueCalcType positionCalcType = ValueCalcType.Add;
        [HideInInspector] [SerializeField] private ValueCalcType eulerCalcType = ValueCalcType.Add;
        [HideInInspector] [SerializeField] private ValueCalcType scaleCalcType = ValueCalcType.Multiply;
        [HideInInspector] [SerializeField] private StaggerType staggerType = StaggerType.AutoInOut;
        [HideInInspector] [SerializeField] private List<StaggerPropsBehaviour> staggerPropsList =new List<StaggerPropsBehaviour>();
        // [SerializeField] private StaggerOption staggerOption;
        [HideInInspector] [SerializeField] private float staggerRatio = 0.3f;
        // [SerializeField] private TransformCash transformCash = null;
        [SerializeField] private List<TransformCash> childTransformCash = new List<TransformCash>();
        [HideInInspector] [SerializeField] private bool debugMode = true;
        [HideInInspector] [SerializeField] [Range(0, 1)] private float debugProgress;
        [HideInInspector] [SerializeField] private AnimationCurve durationCurve;
        // [SerializeField] private float randomSeed = 123;
        [HideInInspector] [SerializeField] private float debugDuration = 1;
        // private Animation debugAnimation;
        // private float _previousProgress = 0;
        // private float _timer = 0f;
        // [HideInInspector] public bool isDebugPlay = false;
        
        
        
        public delegate void OnInitDelegate();
        public event OnInitDelegate OnInitHandler; 
        
        public delegate void OnResetChildTransformDelegate();
        public event OnInitDelegate OnResetChildTransformHandler; 
        
        public bool DebugMode
        {
            get => debugMode;
            set => debugMode = value;
        }
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

        public AnimationClip AnimationClip
        {
            get => animationClip;
            set => animationClip = value;
        }

        public int ChildTransformCashCount => childTransformCash.Count;

        public StaggerType StaggerType
        {
            get => staggerType;
            set => staggerType = value;
        }

        public List<StaggerPropsBehaviour> StaggerPropsList => staggerPropsList;

        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnEnable()
        {
            // Init();
        }

        private void OnValidate()
        {
            
        }
        
        public void Init()
        {
            if (targetObject == null) return;
         
            OnInitHandler?.Invoke();
            var count = 0;
            durationCurve = new AnimationCurve();
            durationCurve.AddKey(0, 1);
            durationCurve.AddKey(1, 1);

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

        public void ResetChildTransform()
        {
            debugProgress = 0;
            foreach (var cash in childTransformCash)
            {
                cash.ResetTransform();
            }
            OnResetChildTransformHandler?.Invoke();
        }

        public void InitStaggerValues()
        {
            if(targetObject == null) return;
            var childLength = targetObject.transform.childCount;
            var childCount = 0;
            
            foreach (Transform child in targetObject.transform)
            {
                
                var duration = staggerRatio * durationCurve.Evaluate((float)childCount/(float)(targetObject.transform.childCount-1));
                duration = Mathf.Clamp(duration,0f, 1f);
                // Debug.Log(ratio);
                // 
                var delay = 1f - duration;
                var delayStep = (1f-duration) / (targetObject.transform.childCount - 1);

                
                StaggerPropsBehaviour staggerPropsBehaviour;
                if (staggerPropsList.Count <= childCount)
                {
                    staggerPropsBehaviour = new StaggerPropsBehaviour();
                    staggerPropsList.Add(staggerPropsBehaviour);
                }

                staggerPropsBehaviour = staggerPropsList[childCount];
                staggerPropsBehaviour.name = $"{childCount}: {child.gameObject.name}";
                
                // staggerPropsBehaviour.RandomSeed = Random.Range()
                if (staggerType != StaggerType.Custom)
                {
                    
                }
                if (staggerType == StaggerType.Random)
                {
                    var childStart = Random.Range(0,delay);
                    var childEnd = childStart + duration;
                    
                    staggerPropsBehaviour.lowLimit = 0;
                    staggerPropsBehaviour.highLimit = 1;
                    staggerPropsBehaviour.startTiming = childStart;
                    staggerPropsBehaviour.endTiming = childEnd;
                }
                
                if (staggerType == StaggerType.RandomPerlin)
                {
                    var childStart = Mathf.PerlinNoise(childCount*staggerRatio, staggerPropsBehaviour.RandomSeed*staggerRatio) *delay;
                    var childEnd = childStart + duration;
                    
                    staggerPropsBehaviour.lowLimit = 0;
                    staggerPropsBehaviour.highLimit = 1;
                    staggerPropsBehaviour.startTiming = childStart;
                    staggerPropsBehaviour.endTiming = childEnd;
                }
                if(staggerType == StaggerType.AutoInOut)
                {
                    // if (staggerType == StaggerType.AutoInOut) ratioStep *= 0.5f;
                    // var isIn = (staggerType == StaggerType.AutoIn || staggerType == StaggerType.AutoInOut);
                    // var isOut = staggerType == StaggerType.AutoOut || staggerType == StaggerType.AutoInOut;
                    var childStart = delayStep * childCount;
                    var childEnd = childStart + duration;
                    
                    
                    staggerPropsBehaviour.lowLimit = 0;
                    staggerPropsBehaviour.highLimit = 1;
                    staggerPropsBehaviour.startTiming = childStart;
                    staggerPropsBehaviour.endTiming = childEnd;
                }
                
                if(staggerType == StaggerType.AutoIn)
                {
                    // if (staggerType == StaggerType.AutoInOut) ratioStep *= 0.5f;
                    
                    var childStart = delayStep * childCount;
                    var childEnd =1f;
                    
                    
                    staggerPropsBehaviour.lowLimit = 0;
                    staggerPropsBehaviour.highLimit = 1;
                    staggerPropsBehaviour.startTiming = childStart;
                    staggerPropsBehaviour.endTiming = childEnd;
                }
                
                if(staggerType == StaggerType.AutoOut)
                {
                    
                    var childStart = 0;
                    var childEnd = 1f-delayStep * childCount;
                    
                    
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

            if (debugMode)
            {
                ProcessFrame(debugProgress);
            }
            // if (targetObject != null && targetObject != null && childTransformCash == null)
            // {
            //     Init();
            // }
            // Debug.Log($"{_isDebugPlay},{debugProgress},{60f / 1000f / debugDuration}");
           
        }

        private void FixedUpdate()
        {
           
        }

        public void ProcessFrame(float progress)
        {
            if (animationClip == null || targetObject == null) return;
            if(transform.childCount == 0) return;
            ;
            // if(staggerPropsList.Count == 0 || staggerPropsList.Count != targetObject.transform.childCount) InitStaggerValues();

            if(childTransformCash.Count <= 0) Init();
            // if (animationTargetType == AnimationTargetType.Own)
            // {
            //
            //     var animation = targetObject.GetComponent<Animation>();
            //     if (animation == null)
            //     {
            //         animation = targetObject.AddComponent<Animation>();
            //     }
            //     animation.clip = animationClip;
            //     UpdateAnimation(transformCash, progress);
            //
            // }
            //
            // if (animationTargetType == AnimationTargetType.Children)
            // {
               
            // var childLength = targetObject.transform.childCount;
            var childCount = 0;

            // InitStaggerValues();
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
            // }
        }

        public void UpdateAnimation(TransformCash transformCash, float progress)
        {
            var target = transformCash.OwnTransform.gameObject;
            // var clipAsset = clip.asset as AnimationClipTransferClip;
            // if (toggleActiveOnClip) target.SetActive(true);


            // if (transformCash.Progress != progress)
            // {



                // Debug.Log($"Update motion");
                // animation.clip = input.AnimationClip;
                // animation.enabled = true;
                animationClip.SampleAnimation(target, progress * animationClip.averageDuration);
                transformCash.Progress = progress;
                // AnimationClipはなんかGetKeyできないからTransformのどこに差分があるかを初期値と比較してるやつ
                if (target.transform.localPosition != transformCash.LocalPosition)
                {
                    
                    if (positionCalcType == ValueCalcType.None)
                    {
                        target.transform.localPosition = transformCash.LocalPosition;
                    }

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
                    
                    if (eulerCalcType == ValueCalcType.None)
                    {
                        target.transform.localEulerAngles = transformCash.LocalEulerAngle;
                    }

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
                    if (scaleCalcType == ValueCalcType.None)
                    {
                        target.transform.localScale = transformCash.LocalScale;
                    }
                    
                    if (scaleCalcType == ValueCalcType.Add)
                    {
                        target.transform.localScale += transformCash.LocalScale;
                    }

                    if (scaleCalcType == ValueCalcType.Subtract)
                    {
                        target.transform.localScale -= transformCash.LocalScale;
                    }

                    if (scaleCalcType == ValueCalcType.Multiply)
                    {
                        var offsetScale = target.transform.localScale;
                        target.transform.localScale = Vector3.Scale(offsetScale, transformCash.LocalScale);
                    }

                }
            // }

        }
        


        private void OnDestroy()
        {
            ResetChildTransform();
        }
    }
}