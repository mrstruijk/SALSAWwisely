using System;
using System.Text;
using UnityEngine;
using WwiseEvent = AK.Wwise.Event;


/// <summary>
///     This allows you to send an Event to Wwise to fire a specific voiceline to be played.
///     At the same time, the Animator can be set to play a specific (and corresponding) Animation.
///     Those Animations can be recorded using AudioClips with RecordSkinnedMeshRendererForDurationOfAudioClips.cs
/// </summary>
public class PostWwisePlayEventWithAnimation : PostWwisePlayEvent
{
    public AnimationAudioSO AnimationSettings;

    [Header("ANIMATOR SETTINGS")]
    public Animator Animator;
    [Tooltip("In the Animator there should be a state with a name, which needs to have a (possibly empty) AnimationClip with this name")]
    public string NameOfClipToSwap = "Talking";
    [Tooltip("This is for setting the Animator speed for this specific state")]
    public string SpeedMultiplierParameter = "SpeedMultiplier";
    [Tooltip("In the Animator that you're controlling, this should be the name of the trigger you want to have set to move to the new AnimationClip")]
    public string TriggerParameter = "Talk";


    private AnimatorOverrideController _animatorOverrideController;


    private void CreateNewAOC()
    {
        _animatorOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController)
        {
            name = new StringBuilder("Override " + NameOfClipToSwap + " with " + AnimationSettings.AnimationClip.name).ToString()
        };

        Animator.runtimeAnimatorController = _animatorOverrideController;
    }


    private void SwapClip()
    {
        _animatorOverrideController[NameOfClipToSwap] = AnimationSettings.AnimationClip;
    }


    [ContextMenu(nameof(SendPlayAnimationAndWwiseEvent))]
    public void SendPlayAnimationAndWwiseEvent()
    {
        CreateNewAOC();
        SwapClip();

        if (AnimationSettings.AnimationOffset > 0) // Animation trigger will happen later than Audio Event
        {
            SendPlayEvent();
            Invoke(nameof(StartAnimation), AnimationSettings.AnimationOffset);
        }
        else if (AnimationSettings.AnimationOffset < 0) // Animation trigger will happen before Audio Event
        {
            StartAnimation();
            Invoke(nameof(SendPlayEvent), Math.Abs(AnimationSettings.AnimationOffset));
        }
        else // Both will be send at the same time
        {
            SendPlayEvent();
            StartAnimation();
        }
    }


    private void StartAnimation()
    {
        Animator.SetTrigger(TriggerParameter);
        Animator.SetFloat(SpeedMultiplierParameter, AnimationSettings.PlayBackSpeedMultiplier);
    }
}