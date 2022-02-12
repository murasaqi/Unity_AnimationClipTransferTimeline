using System;
using System.Collections.Generic;   
using System.Linq;
using System.Timers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UMotionGraphicUtilities
{

#if UNITY_EDITOR
    

    // [CustomEditor(typeof(AnimationClipTransfer), true)]
    
    public class AnimationClipTransferEditor : Editor
    {
        private AnimationClipTransfer _serializedTargetObject;
        private VisualElement staggerPropList;
        private VisualElement root;
        private VisualElement animationProps;
        private Button SetTransformCash;
        private Slider debugProgressSlider;
        private VisualElement DebugPlayer;
        private Timer timer;
        private Foldout animationClipFoldout;
        private ObjectField animationClipField;
        public override VisualElement CreateInspectorGUI()
        {
            
            // Inspector拡張の場合、VisualElementはnewする
            root = new VisualElement();
            _serializedTargetObject = serializedObject.targetObject as AnimationClipTransfer;
            root.Bind(serializedObject);
            root.viewDataKey = "AnimationClipTransfer";
            
            
            var visualTree = Resources.Load<VisualTreeAsset>("StaggerAnimationSettings");
            visualTree.CloneTree(root);
            
           
            var container = new IMGUIContainer(OnInspectorGUI);
            root.Add(container);
            
            staggerPropList = root.Query<VisualElement>("StaggerPropList").First();
            animationProps = root.Query<VisualElement>("AnimationProps");
            SetTransformCash = root.Query<Button>("ApplyChildrenButton").First();

            var targetObjectField = root.Query<ObjectField>("TargetObject").First();
            targetObjectField.objectType = typeof(GameObject);
            
            SetTransformCash.SetEnabled(targetObjectField.value != null);
            targetObjectField.RegisterValueChangedCallback((evt) =>
            {
                SetTransformCash.SetEnabled(evt.newValue != null);
            });
            if(_serializedTargetObject.TargetObject != null)targetObjectField.value = _serializedTargetObject.TargetObject;
            
            
            // ----------------------------- Animation Clips ---------------------------------- //
            animationClipField = root.Query<ObjectField>("AnimationClip").First();
            if(_serializedTargetObject.AnimationClip != null)animationClipField.value = _serializedTargetObject.AnimationClip;
            animationClipField.objectType = typeof(AnimationClip);
            var modeField = root.Query<EnumField>("AnimationClipMode").First();
            
            animationClipFoldout = root.Query<Foldout>("AnimationClipFoldout").First();
            animationClipField.SetEnabled(_serializedTargetObject.AnimationClipMode == AnimationClipMode.Single);
            animationClipFoldout.SetEnabled(_serializedTargetObject.AnimationClipMode == AnimationClipMode.Random);

            modeField.RegisterValueChangedCallback((evt) =>
            {
                // Debug.Log(evt.newValue);
                animationClipField.SetEnabled(_serializedTargetObject.AnimationClipMode == AnimationClipMode.Single);
                animationClipFoldout.SetEnabled(_serializedTargetObject.AnimationClipMode  == AnimationClipMode.Random || 
                                                _serializedTargetObject.AnimationClipMode  == AnimationClipMode.Multiple);
                animationClipFoldout.value = animationClipFoldout.enabledSelf;
                _serializedTargetObject.CheckAssignAnimationClip();
                if(animationClipField.value != null)InitStaggerUIList();
            });
            
            if ( _serializedTargetObject.AnimationClipMode == AnimationClipMode.Single && animationClipField.value != null)
            {
                if (_serializedTargetObject.AnimationClip == null)
                    _serializedTargetObject.AnimationClip = animationClipField.value as AnimationClip;
                _serializedTargetObject.AssignSingleAnimationClip();
            }

            animationClipField.RegisterValueChangedCallback((evt =>
            {
                staggerPropList.SetEnabled(evt.newValue != null);
            }));
            var assignButton = root.Query<Button>("RandomAssignedButton").First();

            assignButton.clicked += () =>
            {
                _serializedTargetObject.AssignRandomAnimationClip();
                InitStaggerUIList();
            };
            
            SetTransformCash.clicked += () =>
            {

                TransformCashList asset = ScriptableObject.CreateInstance<TransformCashList>();

                AssetDatabase.CreateAsset(asset, $"Assets/{_serializedTargetObject.name}_{_serializedTargetObject.GetInstanceID()}.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                _serializedTargetObject.TransformCashList = asset;
                
                _serializedTargetObject.Init();

                if (_serializedTargetObject.ChildTransformCashCount > 0)
                {
                    animationProps.SetEnabled(true);
                    root.Query<Foldout>("StaggerSliderFoldout").First().value = true;
                    
                    root.Query<Foldout>("TransformCalcType").First().value = true;
                }
                
                _serializedTargetObject.SaveTransformCashList();
            };
            
            animationClipField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue != null)
                {
                    _serializedTargetObject.AnimationClip = evt.newValue as AnimationClip;
                    if(evt.newValue != null)_serializedTargetObject.AssignSingleAnimationClip();
                }
            });
         
            
            
            targetObjectField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue != null)
                {
                    // if(animationClipField.value)SetTransformCash.SetEnabled(true);
                    _serializedTargetObject.TargetObject = evt.newValue as GameObject;
                    // _serializedTargetObject.Init();
                    InitStaggerUIList();
                    
                }
            });
            
            
            var staggerRatio = root.Query<Slider>("StaggerRatio").First(); 
            staggerRatio.RegisterValueChangedCallback((evt) =>
            {
                _serializedTargetObject.InitStaggerValues();
                InitStaggerUIList();
            });
           
           
            root.Query<EnumField>("StaggerType").First().RegisterValueChangedCallback((evt) =>
            {

                if (evt.newValue != null)
                { 
                    if (evt.newValue.ToString() == "AutoIn") _serializedTargetObject.StaggerType = StaggerType.AutoIn;
                    if (evt.newValue.ToString() == "AutoOut") _serializedTargetObject.StaggerType = StaggerType.AutoOut;
                    if (evt.newValue.ToString() == "AutoInOut") _serializedTargetObject.StaggerType = StaggerType.AutoInOut;
                    if (evt.newValue.ToString() == "Random") _serializedTargetObject.StaggerType = StaggerType.Random;
                    if (evt.newValue.ToString() == "Custom")
                    {
                        _serializedTargetObject.StaggerType = StaggerType.Custom;
                        staggerRatio.SetEnabled(false);
                    }
                    else
                    {
                        staggerRatio.SetEnabled(true);
                    }
                }
              
                _serializedTargetObject.InitStaggerValues();
                InitStaggerUIList();
            });




            DebugPlayer = root.Query<VisualElement>("DebugPlayer");
            debugProgressSlider = root.Query<Slider>("DebugProgress").First();
            debugProgressSlider.RegisterValueChangedCallback((evt) =>
            {
                if(_serializedTargetObject.DebugMode) _serializedTargetObject.ProcessFrame(evt.newValue);
            });


            // root.Query<Button>("DebugPlayButton").First().clickable.clicked += _serializedTargetObject.DebugPlay;
            root.Query<Toggle>("DebugMode").First().RegisterValueChangedCallback((evt) =>
            {
                if (evt.previousValue != null && evt.newValue == false && evt.previousValue == true)
                {
                    _serializedTargetObject.ResetChildTransform();
                }

                if (evt.newValue)
                {
                    _serializedTargetObject.ProcessFrame(debugProgressSlider.value);

                } 
                
                if(evt.newValue == false)debugProgressSlider.SetValueWithoutNotify(0);
                
                
                DebugPlayer.SetEnabled(evt.newValue);

                _serializedTargetObject.DebugMode = evt.newValue;
            });

            root.Query<CurveField>("DurationCurve").First().RegisterValueChangedCallback((evt) =>
            {
                _serializedTargetObject.InitStaggerValues();
                InitStaggerUIList();
            });



            var enableInit = _serializedTargetObject.AnimationClip && _serializedTargetObject.TargetObject;
            SetTransformCash.SetEnabled(enableInit);
            
            
            animationProps.SetEnabled(enableInit && _serializedTargetObject.ChildTransformCashCount > 0);
            root.Query<Foldout>("StaggerSliderFoldout").First().value = animationProps.enabledSelf;
            root.Query<Foldout>("TransformCalcType").First().value = animationProps.enabledSelf;
            

        return root;
        }

        private void InitStaggerUIList()
        {
            var minMaxSliderElement = Resources.Load<VisualTreeAsset>("MinMaxDurationSlider");
            staggerPropList = root.Query<VisualElement>("StaggerPropList").First();
            var count = 0;
            foreach (var staggerProps in _serializedTargetObject.StaggerPropsList)
            {
                if (staggerPropList.childCount <= count)
                {
                    var clone = minMaxSliderElement.CloneTree();
                    var animationClipField = clone.Query<ObjectField>("AnimationClipField").First();
                    animationClipField.objectType = typeof(AnimationClip);
                    
                    var animationClipListField = clone.Query<ListView>("AnimationClipListField").First();
                    if (_serializedTargetObject.AnimationClipMode == AnimationClipMode.Multiple)
                    {
                        animationClipListField.SetEnabled(true);
                        foreach (var a in staggerProps.assignedMultipleAnimationClip)
                        {
                            var field = new ObjectField();
                            field.objectType = typeof(AnimationClip);
                            field.value = a;
                            animationClipListField.Add(field);
                        }
                        
                    }
                 
                    animationClipField.RegisterValueChangedCallback((evt =>
                    {
                        if (_serializedTargetObject.AnimationClipMode == AnimationClipMode.Manual)
                        {
                            staggerProps.assignedManualAnimationClip = (AnimationClip) animationClipField.value;
                        }
                        if (_serializedTargetObject.AnimationClipMode == AnimationClipMode.Random)
                        {
                            staggerProps.assignedRandomAnimationClip = (AnimationClip) animationClipField.value;
                        }
                        
                        if (_serializedTargetObject.AnimationClipMode == AnimationClipMode.Manual)
                        {
                            staggerProps.assignedSingleAnimationClip = (AnimationClip) animationClipField.value;
                        }

                    }));
                    SetUpStaggerElement(clone, count, staggerProps.name);
                    staggerPropList.Add(clone);     
                }
                else
                {
                    UpdateStaggerElement(staggerPropList.Children().ElementAt(count),staggerProps);
                }
               
                
                count++;
            }
        }

        private void UpdateStaggerListProps(List<StaggerPropsBehaviour> staggerPropsList, VisualElement root)
        {
            // var staggerPropList = root.Query<VisualElement>("StaggerPropList").First();
            var count = 0;
            foreach (var child in staggerPropList.Children())
            {
                var values = staggerPropsList[count];
                // Debug.Log(values.startTiming);
                UpdateStaggerElement(child, values);
                count++;
            }
        }

        // private void OnChangeMode()
        // {
        //     
        // }

        private void UpdateStaggerElement(VisualElement root, StaggerPropsBehaviour staggerProps)
        {
            
            
            // Debug.Log(values.startTiming);
            root.Query<FloatField>("Start").First().value = staggerProps.startTiming;
            root.Query<FloatField>("End").First().value = staggerProps.endTiming;
            root.Query<FloatField>("LowLimit").First().value = staggerProps.lowLimit;
            root.Query<FloatField>("HighLimit").First().value = staggerProps.highLimit;
            var minMaxSlider = root.Query<MinMaxSlider>("MinMaxSlider").First();
            var animationClipField = root.Query<ObjectField>("AnimationClipField").First();
            animationClipField.value = staggerProps.PickAnimationClipByMode(_serializedTargetObject.AnimationClipMode);
            animationClipField.SetEnabled(_serializedTargetObject.AnimationClipMode == AnimationClipMode.Manual);
            minMaxSlider.highLimit = staggerProps.highLimit;
            minMaxSlider.lowLimit = staggerProps.lowLimit;
            minMaxSlider.maxValue = staggerProps.currentStaggerType == StaggerType.Custom ? staggerProps.endTimingCustom : staggerProps.endTiming;
            minMaxSlider.minValue = staggerProps.currentStaggerType == StaggerType.Custom ? staggerProps.startTimingCustom : staggerProps.startTiming;
            
        }
        

        private void SetUpStaggerElement(VisualElement root, int index, string childName)
        {
            var target = serializedObject.targetObject as AnimationClipTransfer;
            var staggerProps = target.StaggerPropsList[index];
            var startField = root.Query<FloatField>("Start").First();


            root.Query<Label>("Name").First().text = childName;
                
                
            startField.value = staggerProps.currentStaggerType == StaggerType.Random ? staggerProps.startTimingCustom : staggerProps.startTiming;
            // delayField.RegisterCallback().;
            startField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                
                var v = evt.newValue;
                if (staggerProps.lowLimit > evt.newValue)
                    v = evt.previousValue;

                if (staggerProps.currentStaggerType == StaggerType.Custom)
                {
                    staggerProps.startTimingCustom = evt.newValue;
                }
                else
                {
                    staggerProps.startTiming = evt.newValue;
                }
                root.Query<MinMaxSlider>().First().value = new Vector2(staggerProps.startTiming, staggerProps.endTiming);
         

            });

            var endField = root.Query<FloatField>("End").First();
            endField.value = staggerProps.currentStaggerType == StaggerType.Random ? staggerProps.endTimingCustom : staggerProps.endTiming;
            endField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                var v = evt.newValue;
                if (staggerProps.highLimit < evt.newValue)
                    v = evt.previousValue;
                if (staggerProps.currentStaggerType == StaggerType.Custom)
                {
                    staggerProps.endTimingCustom = v;
                }
                else
                {
                    staggerProps.endTiming = v;
                }
                root.Query<MinMaxSlider>().First().value = new Vector2(staggerProps.startTiming, staggerProps.endTiming);
            });


            var lowLimitField = root.Query<FloatField>("LowLimit").First();
            lowLimitField.value = staggerProps.lowLimit;
            lowLimitField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                
               
                staggerProps.lowLimit = evt.newValue;
                root.Query<MinMaxSlider>().First().lowLimit = evt.newValue;


            });

            var highLimitField = root.Query<FloatField>("HighLimit").First();
            highLimitField.value = staggerProps.highLimit;
            highLimitField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                staggerProps.highLimit = evt.newValue;
                root.Query<MinMaxSlider>().First().highLimit = evt.newValue;
            });


            
           

           
            var minMaxSlider = root.Query<MinMaxSlider>("MinMaxSlider").First();
            minMaxSlider.lowLimit = lowLimitField.value;
            minMaxSlider.highLimit = highLimitField.value;
            minMaxSlider.minValue = startField.value;
            minMaxSlider.maxValue = endField.value;
            minMaxSlider.RegisterValueChangedCallback((evt) =>
            {
                var target = serializedObject.targetObject as AnimationClipTransfer;
                var staggerProps = target.StaggerPropsList[index];
                // Debug.Log(evt.newValue.x);
                startField.SetValueWithoutNotify(evt.newValue.x);
                endField.SetValueWithoutNotify(evt.newValue.y);
                if (staggerProps.currentStaggerType == StaggerType.Custom)
                {
                    staggerProps.startTimingCustom = evt.newValue.x;
                }
                else
                {
                    staggerProps.startTiming = evt.newValue.x;
                }
                if (staggerProps.currentStaggerType == StaggerType.Custom)
                {
                    staggerProps.endTimingCustom = evt.newValue.y;
                }
                else
                {
                    staggerProps.endTiming = evt.newValue.y;
                }
                serializedObject.ApplyModifiedProperties();
                // Debug.Log(staggerProps.startTiming);
            });
        }
    }
    
#endif
}