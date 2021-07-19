using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EditorTest2Behaviour : PlayableBehaviour
{
    public GameObject newExposedReference;
    public Animation AnimationClip;
    public Vector3 LocalPostion;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
