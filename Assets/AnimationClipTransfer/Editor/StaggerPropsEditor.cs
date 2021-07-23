using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UMotionGraphicUtilities
{
     
    [CustomEditor(typeof(StaggerProps), true)]
    public class StaggerPropsEditor : Editor
    {
      
        public override VisualElement CreateInspectorGUI()
        {
            // Inspector拡張の場合、VisualElementはnewする   
            var root = new VisualElement();
            var target = serializedObject.targetObject as StaggerProps;
            root.Bind(serializedObject);
            

            var visualTree = Resources.Load<VisualTreeAsset>("MinMaxDurationSlider");
            visualTree.CloneTree(root);

            // root.Query<MinMaxSlider>().First().lowLimit = target.lowLimit;
            // root.Query<MinMaxSlider>().First().highLimit = target.highLimit;

            var delayField = root.Query<FloatField>("Start").First();
            delayField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                
                var v = evt.newValue;
                if (target.lowLimit > evt.newValue)
                    v = evt.previousValue;
                
                target.startTiming = evt.newValue;
                root.Query<MinMaxSlider>().First().value = new Vector2(target.startTiming, target.endTiming);
         

            });

            var durationField = root.Query<FloatField>("End").First();
            durationField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                var v = evt.newValue;
                if (target.highLimit < evt.newValue)
                    v = evt.previousValue;
                target.endTiming = v;
                root.Query<MinMaxSlider>().First().value = new Vector2(target.startTiming, target.endTiming);
            });

            root.Query<Label>("Title").First().text = target.gameObject.name;
            
            root.Query<FloatField>("LowLimit").First().RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                
               
                target.lowLimit = evt.newValue;
                root.Query<MinMaxSlider>().First().lowLimit = evt.newValue;


            });

            root.Query<FloatField>("HighLimit").First().RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                target.highLimit = evt.newValue;
                root.Query<MinMaxSlider>().First().highLimit = evt.newValue;
            });


            root.Query<EnumField>("AnimationTargetType").First().RegisterValueChangedCallback((evt) =>
            {
                
            });

           
            var minMaxSlider = root.Query<MinMaxSlider>("MinMaxSlider").First();
            Debug.Log(minMaxSlider.value);
            minMaxSlider.RegisterValueChangedCallback((evt) =>
            {
                // Debug.Log(evt.newValue);
                durationField.SetValueWithoutNotify(evt.newValue.y);
                delayField.SetValueWithoutNotify(evt.newValue.x);
                target.startTiming = evt.newValue.x;
                target.endTiming = evt.newValue.y;
            });



            return root;
        }
    }
}