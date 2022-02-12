
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace UMotionGraphicUtilities
{
    [ExecuteAlways]
    public class AnimationClipTransfer : MonoBehaviour
    {
        [SerializeField] private GameObject targetObject;
        [SerializeField] public AnimationClipTransferProfile animationClipTransferProfile;
        [SerializeField] private AnimationClipMode animationClipMode = AnimationClipMode.Single;
        // [SerializeField] private NumberedAnimationClip numberedAnimationClip = new NumberedAnimationClip();
        [SerializeField] private List<NumberedAnimationClip> numberedAnimationClips = new List<NumberedAnimationClip>();
        
        [SerializeField] private ValueCalcType positionCalcType = ValueCalcType.Add;
        [SerializeField] private ValueCalcType eulerCalcType = ValueCalcType.Add;
        [SerializeField] private ValueCalcType scaleCalcType = ValueCalcType.Multiply;
        [SerializeField] private StaggerType staggerType = StaggerType.AutoInOut;
        [SerializeField] private float staggerRatio = 0.3f;
        [SerializeField] public List<AnimationStagger> animationStaggers =new List<AnimationStagger>();

        [SerializeField] private bool debugMode = false;
        [SerializeField] [Range(0, 1)] private float debugProgress;
        [SerializeField] [Range(0, 1)] private float progress;


        private List<string> targetDropdownList = new List<string>();
        public AnimationClipMode AnimationClipMode
        {
            get => animationClipMode;
            set
            {
                animationClipMode = value;
            }
        }
        public AnimationClipTransferProfile AnimationClipTransferProfile

        {
            set => animationClipTransferProfile = value;
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
                    InitAnimationStaggerList();
                }
            }
        }
        
        // public int ChildTransformCashCount => childTransformCash.Count;

        public StaggerType StaggerType
        {
            get => staggerType;
            set => staggerType = value;
        }

        // public List<AnimationStagger> AnimationStaggers => animationStaggers;

        // Start is called before the first frame update
        void Start()
        {
            InitAnimationStaggerList();
        }

        private void OnEnable()
        {
            InitAnimationStaggerList();
            
        }

        private void OnValidate()
        {
            
        }

        public void SaveCash()
        {
            InitAnimationStaggerList();
            if (animationStaggers.Count > 0)
            {
                if (animationClipTransferProfile.cashs == null) animationClipTransferProfile.cashs = new List<AnimationStaggerElementCash>();
                animationClipTransferProfile.cashs.Clear();

                foreach (var animationStagger in animationStaggers)
                {
                    var cash = new AnimationStaggerElementCash();
                    cash.startTiming = animationStagger.startTiming;
                    cash.endTiming = animationStagger.endTiming;
                    cash.startTimingCustom = animationStagger.startTimingCustom;
                    cash.endTimingCustom = animationStagger.endTimingCustom;
                    cash.lowLimit = animationStagger.lowLimit;
                    cash.highLimit = animationStagger.highLimit;
                    cash.assignedSingleAnimationClip = animationStagger.assignedSingleAnimationClip;
                    cash.assignedRandomAnimationClip = animationStagger.assignedRandomAnimationClip;
                    cash.assignedMultipleAnimationClip = animationStagger.assignedMultipleAnimationClip;
                    cash.assignedMultipleAnimationClip = animationStagger.assignedMultipleAnimationClip;
                    cash.animationClipMode = animationStagger.animationClipMode;
                    cash.valueCalcType_Position = animationStagger.valueCalcType_Position;
                    cash.valueCalcType_Rotation = animationStagger.valueCalcType_Rotation;
                    cash.valueCalcType_Scale = animationStagger.valueCalcType_Scale;
                    // cash.animationClipCue = ;
                    animationStagger.SetTransformCash();
                    cash.transformCash = animationStagger.transformCash;
                    animationClipTransferProfile.cashs.Add(cash);
                }
            }
        }

        public void UpdateProfile()
        {
            if(animationClipTransferProfile == null ) return;
            if (animationClipTransferProfile.cashs == null) animationClipTransferProfile.cashs = new List<AnimationStaggerElementCash>();

            var count = 0;
            foreach (var animationStagger in animationStaggers)
            {
                AnimationStaggerElementCash cash;

                if (animationClipTransferProfile.cashs.Count < count)
                {
                    cash = animationClipTransferProfile.cashs[count];
                }
                else
                {
                    cash = new AnimationStaggerElementCash();
                    animationClipTransferProfile.cashs.Add(cash);
                }
                 
                cash.startTiming = animationStagger.startTiming;
                cash.endTiming = animationStagger.endTiming;
                cash.startTimingCustom = animationStagger.startTimingCustom;
                cash.endTimingCustom = animationStagger.endTimingCustom;
                cash.lowLimit = animationStagger.lowLimit;
                cash.highLimit = animationStagger.highLimit;
                cash.assignedSingleAnimationClip = animationStagger.assignedSingleAnimationClip;
                cash.assignedRandomAnimationClip = animationStagger.assignedRandomAnimationClip;
                cash.assignedMultipleAnimationClip = animationStagger.assignedMultipleAnimationClip;
                cash.assignedMultipleAnimationClip = animationStagger.assignedMultipleAnimationClip;
                cash.animationClipMode = animationStagger.animationClipMode;
                cash.valueCalcType_Position = animationStagger.valueCalcType_Position;
                cash.valueCalcType_Rotation = animationStagger.valueCalcType_Rotation;
                cash.valueCalcType_Scale = animationStagger.valueCalcType_Scale;
                // animationStagger.SetTransformCash();
                // cash.transformCash = animationStagger.transformCash;
            }
        }
        
        public int GetAllChildCount(Transform parent)
        {
            var all =parent.GetComponentsInChildren<Transform>();
            return all.Length; // 親をスキップする
        }
        public void InitAnimationStaggerList()
        {
            
            Debug.Log("Init:AnimationClipTransfer");
            if (targetObject == null) return;
            
            
            animationStaggers.Clear();
            var minChildCount = 999999;
            
            foreach (Transform child in targetObject.transform)
            {

                var childCount = GetAllChildCount(child);
                if (minChildCount > childCount)
                    minChildCount = childCount;
                var animationStagger =  child.gameObject.GetComponent<AnimationStagger>();
                if (animationStagger == null) child.gameObject.AddComponent<AnimationStagger>();
                animationStaggers.Add(animationStagger);
            }

            
            targetDropdownList.Clear();
            for (int i = 0; i < minChildCount; i++)
            {
                var name = "";
                name += i.ToString();
                if (i == 0) name += " (root)";
                targetDropdownList.Add(name);
            }

            foreach (var numberedAnimationClip in numberedAnimationClips)
            {
                numberedAnimationClip.targets = targetDropdownList;
            }

        }

        public void ResetChildTransform()
        {
            progress = 0;
            foreach (var cash in animationStaggers)
            {
                cash.Reset();
            }
            OnResetChildTransformHandler?.Invoke();
        }

        public void AssignAnimationClip()
        {
            
            foreach (var staggerProps in animationStaggers)
            {
                if (numberedAnimationClips != null && numberedAnimationClips.Count > 0)
                {
                    staggerProps.assignedManualAnimationClip = numberedAnimationClips.First();
                    staggerProps.assignedSingleAnimationClip = numberedAnimationClips.First();
                    
                    staggerProps.assignedRandomAnimationClip = numberedAnimationClips[Random.Range(0, numberedAnimationClips.Count)];
                    staggerProps.assignedMultipleAnimationClip.Clear();

                    foreach (var a in numberedAnimationClips)
                    {
                        staggerProps.assignedMultipleAnimationClip.Add(a);
                    }
                }

                staggerProps.animationClipMode = animationClipMode;


            }
        
          
        }

        public void AssignRandomAnimationClip()
        {
            if (numberedAnimationClips == null || numberedAnimationClips.Count == 0)
            {
                numberedAnimationClips = new List<NumberedAnimationClip>();
                numberedAnimationClips.Add(numberedAnimationClips.First());
            }
            if (animationClipMode == AnimationClipMode.Random)
            {
                foreach (var staggerProps in animationStaggers)
                {
                    staggerProps.assignedRandomAnimationClip = numberedAnimationClips[Random.Range(0, numberedAnimationClips.Count)];
                }
            }
            
        }
        
        public void AssignMultipleAnimationClip()
        {
           
            if (animationClipMode == AnimationClipMode.Random)
            {
                foreach (var staggerProps in animationStaggers)
                {
                    if (staggerProps.assignedMultipleAnimationClip == null)
                        staggerProps.assignedMultipleAnimationClip = new List<NumberedAnimationClip>();

                    if (numberedAnimationClips != null && numberedAnimationClips.Count > 0)
                    {
                        foreach (var animationClip in numberedAnimationClips)
                        {
                            staggerProps.assignedMultipleAnimationClip.Add(animationClip);
                        }
                    }
                    
                }
            }
            
        }

        public void AssignSingleAnimationClip()
        {
            
            if (animationClipMode == AnimationClipMode.Single)
            {
                if (numberedAnimationClips != null && numberedAnimationClips.Count > 0) 
                foreach (var staggerProps in animationStaggers)
                {
                    staggerProps.assignedSingleAnimationClip= numberedAnimationClips.First();
                }
            }

        }
        
        

        public void InitStaggerValues()
        {
            if(targetObject == null) return;
            // var childLength = targetObject.transform.childCount;
            var childCount = 0;

            
            
            

            if (animationClipMode == AnimationClipMode.Single)
            {
               AssignSingleAnimationClip();
            }
            if (animationClipMode == AnimationClipMode.Random)
            {
                AssignRandomAnimationClip();
            }
            if (animationClipMode == AnimationClipMode.Multiple)
            {
                AssignMultipleAnimationClip();
            }

           
            foreach (var animationStagger in animationStaggers)
            {
               
                var  duration = Mathf.Clamp(staggerRatio,0f, 1f);
                var delay = 1f - duration;
                var delayStep = (1f-duration) / (animationStaggers.Count - 1);
                animationStagger.animationClipMode = animationClipMode;
                animationStagger.staggerType = staggerType;

                animationStagger.valueCalcType_Position = positionCalcType;
                animationStagger.valueCalcType_Rotation = eulerCalcType;
                animationStagger.valueCalcType_Scale = scaleCalcType;
                 if (staggerType == StaggerType.Custom)
                {
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    var pivot = (animationStagger.startTimingCustom + animationStagger.endTimingCustom) / 2f;
                    animationStagger.startTimingCustom = Mathf.Max(pivot-duration/2f,0f);
                    animationStagger.endTimingCustom = Mathf.Min(pivot+duration/2f,1);
                }
                if (staggerType == StaggerType.Random)
                {
                    var childStart = Random.Range(0,delay);
                    var childEnd = childStart + duration;
                    
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    animationStagger.startTiming = childStart;
                    animationStagger.endTiming = childEnd;
                }
                
                if (staggerType == StaggerType.RandomPerlin)
                {
                    var childStart = Mathf.PerlinNoise(childCount*staggerRatio, animationStagger.randomSeed*staggerRatio) *delay;
                    var childEnd = childStart + duration;
                    
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    animationStagger.startTiming = childStart;
                    animationStagger.endTiming = childEnd;
                }
                if(staggerType == StaggerType.AutoInOut)
                {
                    // if (staggerType == StaggerType.AutoInOut) ratioStep *= 0.5f;
                    // var isIn = (staggerType == StaggerType.AutoIn || staggerType == StaggerType.AutoInOut);
                    // var isOut = staggerType == StaggerType.AutoOut || staggerType == StaggerType.AutoInOut;
                    var childStart = delayStep * childCount;
                    var childEnd = childStart + duration;
                    
                    
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    animationStagger.startTiming = childStart;
                    animationStagger.endTiming = childEnd;
                }
                
                if(staggerType == StaggerType.AutoIn)
                {
                    // if (staggerType == StaggerType.AutoInOut) ratioStep *= 0.5f;
                    
                    var childStart = delayStep * childCount;
                    var childEnd =1f;
                    
                    
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    animationStagger.startTiming = childStart;
                    animationStagger.endTiming = childEnd;
                }
                
                if(staggerType == StaggerType.AutoOut)
                {
                    
                    var childStart = 0;
                    var childEnd = 1f-delayStep * childCount;
                    
                    
                    animationStagger.lowLimit = 0;
                    animationStagger.highLimit = 1;
                    animationStagger.startTiming = childStart;
                    animationStagger.endTiming = childEnd;
                }
             
                childCount++;
            
            }


            
            
        }

        // Update is called once per frame
        void Update()
        {

            // if (targetObject != null)
            // {
            //     if (childTransformCash.Count != targetObject.transform.childCount)
            //     {
            //         Init();
            //     }
            // }

            if (debugMode)
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
            if (numberedAnimationClips == null || targetObject == null) return;
            if(transform.childCount == 0) return;
          
            // if(childTransformCash.Count <= 0) Init();
          
            var childCount = 0;
            
            foreach (var child in animationStaggers)
            {

                child.UpdateSampleAnimation(progress);

                // var staggerProp = animationStaggers[childCount];
                //
                // var start = 0f;
                // var end = 0f;
                //
                // if (staggerType == StaggerType.Custom)
                // {
                //     start = staggerProp.startTimingCustom;
                //     end = staggerProp.endTimingCustom;
                // }
                // else
                // {
                //     start = staggerProp.startTiming;
                //     end = staggerProp.endTiming;
                // }
                // var childProgress = Mathf.Clamp(Mathf.InverseLerp( start,end, (float) progress), 0f, 1f);
                // UpdateAnimation(child, childProgress, childCount);

                childCount++;

            }
           
        }

        // private void UpdateAnimation(AnimationStagger animationStagger, float progress)
        // {
        //     animationStagger.progress = progress;
        //     animationStagger.UpdateSampleAnimation(progress);
        //
        // }
        


        private void OnDestroy()
        {
            ResetChildTransform();
        }
    }
}