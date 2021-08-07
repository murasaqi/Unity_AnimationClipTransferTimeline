using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace UMotionGraphicUtilities
{

    [Serializable]
    public class TransformCash
    {
        public Transform OwnTransform;
        public Vector3 LocalPosition;
        public Vector3 LocalEulerAngle;
        public Vector3 LocalScale;
        public double Progress;


        public void ResetTransform()
        {
            if(OwnTransform == null) return;
            
            OwnTransform.localPosition = LocalPosition;
            OwnTransform.localEulerAngles = LocalEulerAngle;
            OwnTransform.localScale = LocalScale;
        }
    }


   
    public enum StaggerType
    {
        AutoIn,
        AutoOut,
        AutoInOut,
        Random,
        RandomPerlin,
        Custom
        
    }

    [Serializable]
    public enum ValueCalcType
    {
        Add,
        OverWrite,
        Multiply,
        Acceleration,
        Subtract,
        None,
    }

    public enum AnimationTargetType
    {
        Own,
        Children
    }

    [Serializable]
    public class AnimationClipTransferClip : PlayableAsset, ITimelineClipAsset
    {

        public AnimationClipTransferBehaviour template = new AnimationClipTransferBehaviour();
        public AnimationClip AnimationClip;
        public ExposedReference<GameObject> TartgetObject;
        [SerializeField] private AnimationTargetType _animationTargetType;
        [SerializeField] private bool _toggleActiveOnClip;
        [SerializeField] private List<StaggerProps> staggerPropsList;
        [SerializeField] private ValueCalcType _positionCalcType;
        [SerializeField] private ValueCalcType _eulerCalcType;
        [SerializeField] private ValueCalcType _scaleCalcScale;
        [SerializeField] private StaggerType _staggerType;
        [SerializeField] private TransformCash _transformCash;
        [SerializeField] private List<TransformCash> _childTransformCash;
       
        public StaggerType StaggerType => _staggerType;

        public AnimationTargetType AnimationTargetType => _animationTargetType;
        public bool ToggleActiveOnClip => _toggleActiveOnClip;
        // public AnimationCurve curve;
        public ValueCalcType PositionCalcType => _positionCalcType;
        public ValueCalcType EulerCalcType => _eulerCalcType;
        public ValueCalcType ScaleCalcScale => _scaleCalcScale;
        public TransformCash TransformCash => _transformCash;

        public List<TransformCash> ChildTransformCash => _childTransformCash;
        private GameObject _targetObject;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Looping | ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {


            _childTransformCash = new List<TransformCash>();
            var playable = ScriptPlayable<AnimationClipTransferBehaviour>.Create(graph, template);
          
            AnimationClipTransferBehaviour clone = playable.GetBehaviour();
            clone.AnimationClip = AnimationClip;
            clone.TartgetObject = TartgetObject.Resolve(graph.GetResolver());
            _targetObject = clone.TartgetObject;
            _transformCash = new TransformCash();
            _transformCash.OwnTransform = _targetObject.transform;
            _transformCash.LocalPosition = clone.TartgetObject.transform.localPosition;
            _transformCash.LocalEulerAngle = clone.TartgetObject.transform.localEulerAngles;
            _transformCash.LocalScale = clone.TartgetObject.transform.localScale;
            _transformCash.Progress = -1f;

            foreach (Transform child in _targetObject.transform)
            {
                var cash = new TransformCash();
                cash.OwnTransform = child;
                cash.LocalPosition = child.localPosition;
                cash.LocalEulerAngle = child.localEulerAngles;
                cash.LocalScale = child.localScale;
                cash.Progress = -1f;
                _childTransformCash.Add(cash);
            }

            return playable;


        }

        public void InitTargetObjectTransform()
        {
            _transformCash.ResetTransform();

            foreach (var transformCash in _childTransformCash)
            {
                transformCash.ResetTransform();
            }

            if (_toggleActiveOnClip && _targetObject.activeSelf != true) _targetObject.SetActive(true);
        }

        private void OnDestroy()
        {
            InitTargetObjectTransform();
            throw new NotImplementedException();
        }
    }
}