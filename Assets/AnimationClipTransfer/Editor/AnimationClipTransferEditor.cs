using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UMotionGraphicUtilities
{

    [CustomEditor(typeof(AnimationClipTransfer), true)]
    public class AnimationClipTransferEditor : Editor
    {
        // public override void OnInspectorGUI()
        // {
        //     // DrawDefaultInspector();
        // }

        public override VisualElement CreateInspectorGUI()
        {
            // Inspector拡張の場合、VisualElementはnewする
            var root = new VisualElement();
            var target = serializedObject.targetObject as AnimationClipTransfer;
            root.Bind(serializedObject);
            var button = new Button
            {
                text = "Init"
            };
            button.clickable.clicked += target.Init;
            root.Add(button);

            // デフォルトのInspector表示を追加
            IMGUIContainer defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
            root.Add(defaultInspector);


            var visualTree = Resources.Load<VisualTreeAsset>("StaggerAnimationSettings");
            visualTree.CloneTree(root);

            
            var minMaxSliderElement = Resources.Load<VisualTreeAsset>("MinMaxDurationSlider");
            var staggerPropList = root.Query<VisualElement>("StaggerPropList").First();

            var count = 0;
            foreach (var staggerProps in target.StaggerPropsList)
            {
               var clone = minMaxSliderElement.CloneTree();
             
                SetUpStaggerList(clone, count);
                staggerPropList.Add(clone);
                count++;
            }
            
            
            root.Query<EnumField>("StaggerType").First().RegisterValueChangedCallback((evt) =>
            {

                if (evt.newValue.ToString() == "AutoIn") target.StaggerType = StaggerType.AutoIn;
                if (evt.newValue.ToString() == "AutoOut") target.StaggerType = StaggerType.AutoOut;
                if (evt.newValue.ToString() == "AutoInOut") target.StaggerType = StaggerType.AutoInOut;
                if (evt.newValue.ToString() == "Custom") target.StaggerType = StaggerType.Custom;
                Debug.Log(target.StaggerType);
                // target.StaggerType = 
                target.InitStaggerValues();
                UpdateStaggerUI(target.StaggerPropsList, staggerPropList);
            });

            
            root.Query<Slider>("StaggerRatio").First().RegisterValueChangedCallback((evt) =>
            {
                target.InitStaggerValues();
                UpdateStaggerUI(target.StaggerPropsList, staggerPropList);
            });

            return root;
        }

        private void UpdateStaggerUI(List<StaggerPropsBehaviour> staggerPropsList, VisualElement root)
        {
            var staggerPropList = root.Query<VisualElement>("StaggerPropList").First();
            var count = 0;
            foreach (var child in staggerPropList.Children())
            {
                var values = staggerPropsList[count];
                // Debug.Log(values.startTiming);
                child.Query<FloatField>("Start").First().value = values.startTiming;
                child.Query<FloatField>("End").First().value = values.endTiming;
                child.Query<FloatField>("LowLimit").First().value = values.lowLimit;
                child.Query<FloatField>("HighLimit").First().value = values.highLimit;
                var minMaxSlider = child.Query<MinMaxSlider>("MinMaxSlider").First();
                
                minMaxSlider.highLimit = values.highLimit;
                minMaxSlider.lowLimit = values.lowLimit;
                minMaxSlider.maxValue = values.endTiming;
                minMaxSlider.minValue = values.startTiming;
                count++;
            }
        }

        private void SetUpStaggerList(VisualElement root, int index)
        {
            var target = serializedObject.targetObject as AnimationClipTransfer;
            var staggerProps = target.StaggerPropsList[index];
            var delayField = root.Query<FloatField>("Start").First();

            delayField.value = staggerProps.startTiming;
            delayField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                
                var v = evt.newValue;
                if (staggerProps.lowLimit > evt.newValue)
                    v = evt.previousValue;
                
                staggerProps.startTiming = evt.newValue;
                root.Query<MinMaxSlider>().First().value = new Vector2(staggerProps.startTiming, staggerProps.endTiming);
         

            });

            var durationField = root.Query<FloatField>("End").First();
            durationField.value = staggerProps.endTiming;
            durationField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                var v = evt.newValue;
                if (staggerProps.highLimit < evt.newValue)
                    v = evt.previousValue;
                staggerProps.endTiming = v;
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
            Debug.Log(minMaxSlider.value);
            minMaxSlider.lowLimit = lowLimitField.value;
            minMaxSlider.highLimit = highLimitField.value;
            minMaxSlider.minValue = delayField.value;
            minMaxSlider.maxValue = durationField.value;
            minMaxSlider.RegisterValueChangedCallback((evt) =>
            {
                var target = serializedObject.targetObject as AnimationClipTransfer;
                var staggerProps = target.StaggerPropsList[index];
                // Debug.Log(evt.newValue.x);
                delayField.SetValueWithoutNotify(evt.newValue.x);
                durationField.SetValueWithoutNotify(evt.newValue.y);
                staggerProps.startTiming = evt.newValue.x;
                staggerProps.endTiming = evt.newValue.y;
                serializedObject.ApplyModifiedProperties();
                // Debug.Log(staggerProps.startTiming);
            });
        }
    }
}