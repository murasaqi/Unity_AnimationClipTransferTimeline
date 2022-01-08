using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UMotionGraphicUtilities;

public class AnimationClipTransferControlMixerBehaviour : PlayableBehaviour
{
    
    // private AnimationClipTransfer m_TrackBinding;
    private bool m_FirstFrameHappened;
    
    public List<TimelineClip> clips;
    internal PlayableDirector m_PlayableDirector;
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
       
        
        // m_TrackBinding = playerData as AnimationClipTransfer;

        // if (m_TrackBinding == null)
        //     return;

        if (!m_FirstFrameHappened)
        {
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount ();

        // Color blendedColor = Color.clear;
        // float blendedIntensity = 0f;
        // float blendedBounceIntensity = 0f;
        // float blendedRange = 0f;
        // float totalWeight = 0f;
        // float greatestWeight = 0f;
        // int currentInputs = 0;

        
        
        for (int i = 0; i < inputCount; i++)
        {
            var clip = clips[i];
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<AnimationClipTransferControlBehaviour> inputPlayable = (ScriptPlayable<AnimationClipTransferControlBehaviour>)playable.GetInput(i);
            AnimationClipTransferControlBehaviour input = inputPlayable.GetBehaviour ();
            var animationClipTransfer = input.animationClipTransfer;
            float normalisedTime = (float)(clip.ToLocalTime(m_PlayableDirector.time) / clip.duration );


            if (inputWeight > 0)
            {
                input.animationClipTransfer.gameObject.SetActive(true);
                animationClipTransfer.ProcessFrame(normalisedTime);    
            }
            else
            {
                if (clip.preExtrapolationMode == TimelineClip.ClipExtrapolation.None)
                {
                    input.animationClipTransfer.gameObject.SetActive(!input.targetDisableOutOfClip);
                    
                    if(input.animationClipTransfer.Process > 0) input.animationClipTransfer.ProcessFrame(0);
                }

                if (clip.postExtrapolationMode == TimelineClip.ClipExtrapolation.None)
                {
                    if(input.animationClipTransfer.Process < 1) input.animationClipTransfer.ProcessFrame(1);
                }
                
                
                
                
            }

            // if (input.targetDisableOutOfClip)
            // {
            //     
            // }
            // else
            // {
            //     // if (m_PlayableDirector.time < clip.start)
            //     // {
            //     //     if (animationClipTransfer.Process != 0)
            //     //     {
            //     //         if (clip.preExtrapolationMode == TimelineClip.ClipExtrapolation.Hold)
            //     //         {
            //     //             animationClipTransfer.ProcessFrame(0);     
            //     //         }
            //     //
            //     //     }
            //     // }
            // }
            //
            
                
           
            
            //
            // if (m_PlayableDirector.time > clip.end)
            // {
            //     if (clip.postExtrapolationMode == TimelineClip.ClipExtrapolation.PingPong)
            //     {
            //         var isInverse = Mathf.CeilToInt(((float)m_PlayableDirector.time - (float)clip.end) / (float)clip.duration) % 2 != 1;
            //         
            //         var progress = (float)((m_PlayableDirector.time - clip.end) % clip.duration)/(float) clip.duration;
            //
            //         progress = isInverse ? 1f - progress : progress;
            //         
            //         animationClipTransfer.ProcessFrame(progress);
            //     }
            //     
            //     if (animationClipTransfer.Process != 1)
            //     {
            //         if (clip.postExtrapolationMode == TimelineClip.ClipExtrapolation.Hold)
            //         {
            //             animationClipTransfer.ProcessFrame(1);     
            //         }
            //
            //     }
            // }


        }
    }
}
