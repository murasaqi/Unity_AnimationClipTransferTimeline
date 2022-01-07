
using System;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
        [HideInInspector] [SerializeField] private AnimationClipMode animationClipMode = AnimationClipMode.Single;
        [SerializeField] private List<AnimationClip> animationClips = new List<AnimationClip>();
        [HideInInspector] [SerializeField] private GameObject targetObject;
        [HideInInspector] [SerializeField] private ValueCalcType positionCalcType = ValueCalcType.Add;
        [HideInInspector] [SerializeField] private ValueCalcType eulerCalcType = ValueCalcType.Add;
        [HideInInspector] [SerializeField] private ValueCalcType scaleCalcType = ValueCalcType.Multiply;
        [HideInInspector] [SerializeField] private StaggerType staggerType = StaggerType.AutoInOut;
        [HideInInspector] [SerializeField] private List<StaggerPropsBehaviour> staggerPropsList =new List<StaggerPropsBehaviour>();
        [HideInInspector] [SerializeField] private float staggerRatio = 0.3f;
        [SerializeField] private TransformCashList transformCashList;
        [SerializeField] private List<TransformCash> childTransformCash = new List<TransformCash>();
        [HideInInspector] [SerializeField] private bool debugMode = false;
        [HideInInspector] [SerializeField] [Range(0, 1)] private float debugProgress;
        [HideInInspector] [SerializeField] [Range(0, 1)] private float progress;
        [HideInInspector] [SerializeField] private AnimationCurve durationCurve;


        public AnimationClipMode AnimationClipMode
        {
            get => animationClipMode;
        }
        public TransformCashList TransformCashList

        {
            set => transformCashList = value;
        }
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

        public void SaveTransformCashList()
        {
            if (childTransformCash.Count > 0)
            {
                if (transformCashList.cashs == null) transformCashList.cashs = new List<TransformCash>();
                transformCashList.cashs.Clear();

                foreach (var childTransformCash in childTransformCash)
                {
                    transformCashList.cashs.Add(childTransformCash);
                }
            }
        }
        public void Init()
        {
            
            Debug.Log("Init:AnimationClipTransfer");
            if (targetObject == null) return;
         
            OnInitHandler?.Invoke();
            var count = 0;
            durationCurve = new AnimationCurve();
            durationCurve.AddKey(0, 1);
            durationCurve.AddKey(1, 1);
            progress = 0f;
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
            ProcessFrame(0);

        }

        public void ResetChildTransform()
        {
            progress = 0;
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
            if (animationClips.Count == 0)
            {
                animationClips = new List<AnimationClip>();
                animationClips.Add(animationClip);
            }
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


                if (animationClipMode == AnimationClipMode.Single)
                {
                    staggerPropsBehaviour.assignedAnimationClip = animationClip;
                }
                if (animationClipMode == AnimationClipMode.Random)
                {
                    staggerPropsBehaviour.assignedAnimationClip = animationClips[Random.Range(0, animationClips.Count)];
                }
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

            if (targetObject != null)
            {
                if (childTransformCash.Count != targetObject.transform.childCount)
                {
                    Init();
                }
            }

            if (debugMode && debugProgress != progress)
            {
                ProcessFrame(debugProgress);
            }
            
            
          
        }

        private void FixedUpdate()
        {
           
        }
        
        public float Process => progress;
        public void ProcessFrame(float time)
        {
            
            progress = time;
            if (animationClip == null || targetObject == null) return;
            if(transform.childCount == 0) return;
          
            if(childTransformCash.Count <= 0) Init();
          
            var childCount = 0;

            foreach (var child in childTransformCash)
            {
          
                var childProgress = Mathf.Clamp(Mathf.InverseLerp( staggerPropsList[childCount].startTiming,  staggerPropsList[childCount].endTiming, (float) progress), 0f, 1f);
                UpdateAnimation(child, childProgress);

                childCount++;

            }
           
        }

        private void UpdateAnimation(TransformCash transformCash, float progress)
        {
            var target = transformCash.OwnTransform.gameObject;
         


              animationClip.SampleAnimation(target, progress * animationClip.averageDuration);
                transformCash.Progress = progress;
                // AnimationClipはなんかGetKeyできないからTransformのどこに差分があるかを初期値と比較してるやつ
                if (target.transform.localPosition != transformCash.LocalPosition)
                {
                    
                    if (positionCalcType == ValueCalcType.None)
                    {
                        // target.transform.localPosition = transformCash.LocalPosition;
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
                        // target.transform.localEulerAngles = transformCash.LocalEulerAngle;
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
                        // target.transform.localScale = transformCash.LocalScale;
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