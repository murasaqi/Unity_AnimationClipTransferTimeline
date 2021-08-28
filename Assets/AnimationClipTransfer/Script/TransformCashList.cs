using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UMotionGraphicUtilities
{
    [CreateAssetMenu(menuName = "AnimationClipTransfer/Create TransformCashObject")]
    public class TransformCashList : ScriptableObject
    {
        public List<TransformCash> transformCashList;
    }
}