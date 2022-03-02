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
    

    [CustomEditor(typeof(AnimationClipTransfer), true)]
    
    public class AnimationClipTransferEditor : Editor
    {
        private AnimationClipTransfer _serializedTargetObject;
        private VisualElement staggerList;
        private VisualElement root;
        private VisualElement animationProps;
        private Button createProfileButton;
        private Button saveProfileButton;
        private Button loadProfileButton;
        private Slider debugProgressSlider;
        private Toggle debugMode;
        private Timer timer;
        
        // private ObjectField animationClipField;
        private ListView animationClipListField;
        public override VisualElement CreateInspectorGUI()
        {
            
            // Inspector拡張の場合、VisualElementはnewする
            root = new VisualElement();
            _serializedTargetObject = serializedObject.targetObject as AnimationClipTransfer;
            root.Bind(serializedObject);
            root.viewDataKey = "AnimationClipTransfer";
            
            
            var visualTree = Resources.Load<VisualTreeAsset>("AnimationClipTransferUI");
            visualTree.CloneTree(root);
          
            var targetObjectField = root.Q<ObjectField>("TargetObject");
            targetObjectField.objectType = typeof(GameObject);

            var profileField = root.Q<ObjectField>("ProfileField");
            profileField.objectType = typeof(AnimationClipTransferProfile);

            // -------------------------------- Button Event ------------------------------------ //
            createProfileButton = root.Q<Button>("CreateButton");
            createProfileButton.SetEnabled(_serializedTargetObject.animationClipTransferProfile == null);
            profileField.RegisterValueChangedCallback((evt) =>
            {
                createProfileButton.SetEnabled(evt.newValue == null);
            });
            
            
            createProfileButton.clicked += () =>
            {
            
                _serializedTargetObject.InitAnimationStaggerList();
                AnimationClipTransferProfile asset = ScriptableObject.CreateInstance<AnimationClipTransferProfile>();
            
                AssetDatabase.CreateAsset(asset, $"Assets/{_serializedTargetObject.name}_{_serializedTargetObject.GetInstanceID()}.asset");
                AssetDatabase.SaveAssets();
            
                EditorUtility.FocusProjectWindow();
            
                _serializedTargetObject.AnimationClipTransferProfile = asset;
                if (_serializedTargetObject.animationStaggers != null)
                {
                    animationProps.SetEnabled(true);
                    root.Query<Foldout>("StaggerSliderFoldout").First().value = true;
                    
                    root.Query<Foldout>("TransformCalcType").First().value = true;
                }
                _serializedTargetObject.SaveCash();
            };
            
            saveProfileButton = root.Q<Button>("SaveButton");
            saveProfileButton.SetEnabled(_serializedTargetObject.animationClipTransferProfile != null);
            saveProfileButton.clicked += () =>
            {
            
                _serializedTargetObject.SaveCash();
            };
            
            // ----------------------------- Stagger List ---------------------------------- //

            if (_serializedTargetObject.animationStaggers != null &&
                _serializedTargetObject.animationStaggers.Count > 0)
            {
                _serializedTargetObject.InitStaggerValues();
                InitStaggerUIList();
                _serializedTargetObject.UpdateProfile();
            }
            //
            // // ----------------------------- Animation Clips ---------------------------------- //
            
            var modeField = root.Q<EnumField>("AnimationClipMode");
            animationClipListField = root.Q<ListView>("AnimationClipsField");
            modeField.RegisterValueChangedCallback((evt) =>
            {
                _serializedTargetObject.AssignAnimationClip();
            });

            var assignButton = root.Q<Button>("AssignButton");
            assignButton.clicked += () =>
            {
                _serializedTargetObject.AssignAnimationClip();
                _serializedTargetObject.UpdateProfile();
            };
            //
            // if ( _serializedTargetObject.AnimationClipMode == AnimationClipMode.Single && animationClipField.value != null)
            // {
            //     if (_serializedTargetObject.AnimationClip == null)
            //         _serializedTargetObject.AnimationClip = animationClipField.value as AnimationClip;
            //     _serializedTargetObject.AssignSingleAnimationClip();
            // }
            //
            // animationClipField.RegisterValueChangedCallback((evt =>
            // {
            //     staggerPropList.SetEnabled(evt.newValue != null);
            // }));
            // var assignButton = root.Query<Button>("RandomAssignedButton").First();
            //
            // assignButton.clicked += () =>
            // {
            //     _serializedTargetObject.AssignRandomAnimationClip();
            //     InitStaggerUIList();
            // };
            //
       
            //
            // animationClipField.RegisterValueChangedCallback((evt) =>
            // {
            //     if (evt.newValue != null)
            //     {
            //         _serializedTargetObject.AnimationClip = evt.newValue as AnimationClip;
            //         if(evt.newValue != null)_serializedTargetObject.AssignSingleAnimationClip();
            //     }
            // });
            //
            //
            //
            // targetObjectField.RegisterValueChangedCallback((evt) =>
            // {
            //     if (evt.newValue != null)
            //     {
            //         // if(animationClipField.value)SetTransformCash.SetEnabled(true);
            //         _serializedTargetObject.TargetObject = evt.newValue as GameObject;
            //         // _serializedTargetObject.Init();
            //         InitStaggerUIList();
            //         
            //     }
            // });
            //
            //
            var staggerRatio = root.Q<Slider>("StaggerRatio"); 
            staggerRatio.RegisterValueChangedCallback((evt) =>
            {
                _serializedTargetObject.InitStaggerValues();
                UpdateStaggerList();
                _serializedTargetObject.UpdateProfile();
            });
            //
            //
            root.Q<EnumField>("StaggerType").RegisterValueChangedCallback((evt) =>
            {
                _serializedTargetObject.InitStaggerValues();
                InitStaggerUIList();
                _serializedTargetObject.UpdateProfile();
            });
            //
            //
            //
            //
            debugMode = root.Q<Toggle>("DebugMode");
            debugProgressSlider = root.Q<Slider>("DebugProgress");
            debugProgressSlider.SetEnabled(debugMode.value);
            debugMode.RegisterValueChangedCallback((evt) =>
            {
                // _serializedTargetObject.

                if (debugMode.value == false)
                {
                    _serializedTargetObject.ResetChildTransform();
                    debugProgressSlider.value = 0f;
                }
                debugProgressSlider.SetEnabled(debugMode.value);
            });
            //
            //
            // // root.Query<Button>("DebugPlayButton").First().clickable.clicked += _serializedTargetObject.DebugPlay;
            // root.Query<Toggle>("DebugMode").First().RegisterValueChangedCallback((evt) =>
            // {
            //     if (evt.previousValue != null && evt.newValue == false && evt.previousValue == true)
            //     {
            //         _serializedTargetObject.ResetChildTransform();
            //     }
            //
            //     if (evt.newValue)
            //     {
            //         _serializedTargetObject.ProcessFrame(debugProgressSlider.value);
            //
            //     } 
            //     
            //     if(evt.newValue == false)debugProgressSlider.SetValueWithoutNotify(0);
            //     
            //     
            //     DebugPlayer.SetEnabled(evt.newValue);
            //
            //     _serializedTargetObject.DebugMode = evt.newValue;
            // });
            //
            // root.Query<CurveField>("DurationCurve").First().RegisterValueChangedCallback((evt) =>
            // {
            //     _serializedTargetObject.InitStaggerValues();
            //     InitStaggerUIList();
            // });
            //
            //
            //
            // var enableInit = _serializedTargetObject.AnimationClip && _serializedTargetObject.TargetObject;
            // SetTransformCash.SetEnabled(enableInit);
            //
            //
            // animationProps.SetEnabled(enableInit && _serializedTargetObject.ChildTransformCashCount > 0);
            // root.Query<Foldout>("StaggerSliderFoldout").First().value = animationProps.enabledSelf;
            // root.Query<Foldout>("TransformCalcType").First().value = animationProps.enabledSelf;
            //

             return root;
        }

        private void InitStaggerUIList()
        {
            var animationStaggerUI = Resources.Load<VisualTreeAsset>("BaseStaggerUI");
            staggerList = root.Query<VisualElement>("AnimationStaggerList").First();
            var count = 0;
            staggerList.Clear();
            foreach (var animationStagger in _serializedTargetObject.animationStaggers)
            {
                if (staggerList.childCount <= count)
                {
                    var clone = animationStaggerUI.CloneTree();
                    
                    SetUpStaggerElement(clone, animationStagger);
                    staggerList.Add(clone);     
                }
                count++;
            }

            if (staggerList.childCount > _serializedTargetObject.animationStaggers.Count)
            {
                staggerList.RemoveAt(0);
            }
            
            UpdateStaggerList();
        }

        private void UpdateStaggerList()
        {
            // var staggerPropList = root.Query<VisualElement>("StaggerPropList").First();
            var count = 0;
            foreach (var child in staggerList.Children())
            {
                var visualElement =child;
                UpdateStaggerElement(visualElement, _serializedTargetObject.animationStaggers[count]);
                count++;
            }   
        }

        // private void OnChangeMode()
        // {
        //     
        // }

        private void UpdateStaggerElement(VisualElement element, AnimationStagger animationStagger)
        {
            
            
            var nameField = element.Q<Label>("NameField");
            nameField.text = animationStagger.name;
            element.Q<FloatField>("Start").value = animationStagger.staggerType == StaggerType.Custom ? animationStagger.startTimingCustom: animationStagger.startTiming;
            element.Q<FloatField>("End").value = animationStagger.staggerType == StaggerType.Custom ? animationStagger.endTimingCustom : animationStagger.endTiming;
            element.Q<FloatField>("LowLimit").value = animationStagger.lowLimit;
            element.Q<FloatField>("HighLimit").value = animationStagger.highLimit;
            var minMaxSlider = element.Q<MinMaxSlider>("MinMaxSlider");
            
            var animationClipListField = element.Q<ListView>("AssignedAnimationClipListField");
            animationClipListField.Clear();
            // if (_serializedTargetObject.AnimationClipMode == AnimationClipMode.Multiple)
            // {
            //            
            //     foreach (var a in animationStagger.assignedMultipleAnimationClip)
            //     {
            //         var field = new ObjectField();
            //         field.objectType = typeof(AnimationClip);
            //         field.value = a;
            //         animationClipListField.Add(field);
            //     }
            //             
            // }
            // else
            // {
            //     var field = new ObjectField();
            //     field.objectType = typeof(AnimationClip);
            //     field.value = animationStagger.PickSingleAnimationClipByMode();
            //     // animationClipListField.Add(field);
            // }
            // minMaxSlider.highLimit = animationStagger.highLimit;
            // minMaxSlider.lowLimit = animationStagger.lowLimit;
            // minMaxSlider.maxValue = animationStagger.staggerType == StaggerType.Custom ? animationStagger.startTimingCustom: animationStagger.endTiming;
            // minMaxSlider.minValue = animationStagger.staggerType == StaggerType.Custom ? animationStagger.endTimingCustom : animationStagger.startTiming;
            
        }
        

        private void SetUpStaggerElement(VisualElement staggerElement, AnimationStagger animationStagger)
        {
            if(animationStagger == null) return;
            var nameField = staggerElement.Q<Label>("NameField");
            nameField.text = animationStagger.name;
            var start = staggerElement.Q<FloatField>("Start");
            start.value = animationStagger.staggerType  == StaggerType.Custom ? animationStagger.startTimingCustom : animationStagger.startTiming;
            var end = staggerElement.Q<FloatField>("End");
            end.value = animationStagger.staggerType  == StaggerType.Custom ? animationStagger.endTimingCustom: animationStagger.endTiming;
            var low = staggerElement.Q<FloatField>("LowLimit");
            low.value = animationStagger.lowLimit;
            var high = staggerElement.Q<FloatField>("HighLimit");
            high.value = animationStagger.highLimit;
            var minMaxSlider = staggerElement.Q<MinMaxSlider>("MinMaxSlider");
            minMaxSlider.value = new Vector2(
                start.value,
                end.value
            );

            start.RegisterValueChangedCallback((evt =>
            {
                if (animationStagger.staggerType == StaggerType.Custom)
                {
                    animationStagger.startTimingCustom = evt.newValue;
                    minMaxSlider.value = new Vector2(
                        animationStagger.startTimingCustom,
                        animationStagger.endTimingCustom
                    );
                }
                else
                {
                    animationStagger.startTiming = evt.newValue;
                    minMaxSlider.value = new Vector2(
                        animationStagger.startTiming,
                        animationStagger.endTiming
                    );
                }
               
                
            }));
            
            
            end.RegisterValueChangedCallback((evt =>
            {
                
                if (animationStagger.staggerType == StaggerType.Custom)
                {
                    animationStagger.endTimingCustom = evt.newValue;
                    minMaxSlider.value = new Vector2(
                        animationStagger.startTimingCustom,
                        animationStagger.endTimingCustom
                    );
                }
                else
                {
                    animationStagger.endTiming = evt.newValue;
                    minMaxSlider.value = new Vector2(
                        animationStagger.startTiming,
                        animationStagger.endTiming
                    );
                }
            }));
            
            
            high.RegisterValueChangedCallback((evt =>
            {
                animationStagger.highLimit = evt.newValue;
                minMaxSlider.highLimit = evt.newValue;
            }));
            
            
            low.RegisterValueChangedCallback((evt =>
            {
                animationStagger.lowLimit = evt.newValue;
                minMaxSlider.lowLimit = evt.newValue;
            }));


            minMaxSlider.RegisterValueChangedCallback((evt) =>
            {
               
                start.value = evt.newValue.x;
                end.value = evt.newValue.y;    
                
                
            });
        }
    }
    
#endif
}