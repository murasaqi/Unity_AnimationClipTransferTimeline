using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UMotionGraphicUtilities
{
    [RequireComponent(typeof(AnimationClipTransfer))]
    [ExecuteAlways]
    public class AnimationClipTransferEventDebugger : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

            GetComponent<AnimationClipTransfer>().OnInitHandler += (() =>
            {
                Debug.Log("Init");
            });
            GetComponent<AnimationClipTransfer>().OnResetChildTransformHandler += (() =>
            {
                Debug.Log("ResetChild");
            });
        }

        private void OnEnable()
        {
            GetComponent<AnimationClipTransfer>().OnInitHandler += (() =>
            {
                Debug.Log("Init");
            });
            
            GetComponent<AnimationClipTransfer>().OnResetChildTransformHandler += (() =>
            {
                Debug.Log("ResetChild");
            });
        }

        // Update is called once per frame
        void Update() { }
    }
}