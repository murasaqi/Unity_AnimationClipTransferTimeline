using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;

[Serializable]
public class EditorTestBehaviour : PlayableBehaviour
{
    public AimConstraint newExposedReference;
    public AimConstraint newBehaviourVariable;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
