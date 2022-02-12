using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UMotionGraphicUtilities
{
#if UNITY_EDITOR

     
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
            
            root.Query<ObjectField>("AnimationClipField").First().objectType = typeof(AnimationClip);
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
    
    
    [CustomEditor(typeof(AnimationStagger), true)]
    public class AnimationStaggerElementEditor : Editor
    {
        private MinMaxSlider _minMaxSlider;
        private AnimationStagger _animationStagger;
        private FloatField _low;
        private FloatField _high;
        private FloatField _start;
        private FloatField _end;
        public override VisualElement CreateInspectorGUI()
        {
            // Inspector拡張の場合、VisualElementはnewする   
            var root = new VisualElement();
            _animationStagger = serializedObject.targetObject as AnimationStagger;
            root.Bind(serializedObject);
            

            var visualTree = Resources.Load<VisualTreeAsset>("AnimationStaggerElementUI");

            visualTree.CloneTree(root);
            
            var progressBar = root.Q<Slider>("DebugProgressField");
            progressBar.SetEnabled(_animationStagger.updateInEditor);

            var updateInEditor = root.Q<Toggle>("UpdateInEditorModeField");
            updateInEditor.RegisterValueChangedCallback(evt => progressBar.SetEnabled(evt.newValue));





            var single = root.Q<ObjectField>("SingleAnimationClipField");
            single.objectType = typeof(AnimationClip);
            
            var random = root.Q<ObjectField>("RandomAnimationClipField");
            random.objectType = typeof(AnimationClip);
            
            var manual = root.Q<ObjectField>("ManualAnimationClipField");
            manual.objectType = typeof(AnimationClip);
            
            _minMaxSlider = root.Q<MinMaxSlider>("MinMaxSlider");
            
            
            _low = root.Q<FloatField>("LowLimit");
            _high = root.Q<FloatField>("HighLimit");
            _start = root.Q<FloatField>("Start");
            _end = root.Q<FloatField>("End");
            
            _low.RegisterValueChangedCallback(evt=>UpdateMinMaxSlider());
            _high.RegisterValueChangedCallback(evt=>UpdateMinMaxSlider());
            _start.RegisterValueChangedCallback(evt=>UpdateMinMaxSlider());
            _end.RegisterValueChangedCallback(evt=>UpdateMinMaxSlider());
            
            
            _minMaxSlider.RegisterValueChangedCallback(evt => UpdateStartEndValue());
            
            
            return root;
        }


        private void UpdateMinMaxSlider()
        {
            if(_minMaxSlider == null || _animationStagger == null) return;
            CheckHighLowValue();
            _minMaxSlider.lowLimit = _animationStagger.lowLimit;
            _minMaxSlider.highLimit = _animationStagger.highLimit;
            _minMaxSlider.minValue = _animationStagger.startTiming;
            _minMaxSlider.maxValue = _animationStagger.endTiming;
        }
        
        private void CheckHighLowValue()
        {
            
            _animationStagger.startTiming = Mathf.Clamp(_animationStagger.startTiming,
                _animationStagger.lowLimit, _animationStagger.endTiming);
            
            _animationStagger.endTiming = Mathf.Clamp(_animationStagger.endTiming,
                _animationStagger.startTiming, _animationStagger.highLimit);
            
        }
        
        private void UpdateStartEndValue()
        {
            // Debug.Log(_animationStaggerElement.startTiming);
            if(_minMaxSlider == null || _animationStagger == null) return;
            _animationStagger.startTiming = _minMaxSlider.value.x;
            _animationStagger.endTiming = _minMaxSlider.value.y;
        }
    }
    
  
    
    
#endif
}