using System;
using System.Text;
using UnityEngine;
using WwiseEvent = AK.Wwise.Event;


/// <summary>
///     This allows you to send an Event to Wwise to fire a specific voiceline to be played.
///     At the same time, the Animator can be set to play a specific (and corresponding) Animation.
///     Those Animations can be recorded using AudioClips with RecordSkinnedMeshRendererForDurationOfAudioClips.cs
/// </summary>
public class PostWwisePlayEventWithAnimation : MonoBehaviour
{
    public Animator Animator;

    [Space(10)]
    [Tooltip("The WwiseEvent and the Clip should match, and trigger the corresponding part of the same thing")]
    public WwiseEvent WwiseEvent_play;
    [Tooltip("The WwiseEvent and the Clip should match, and trigger the corresponding part of the same thing")]
    public AnimationClip CorrespondingAnimationClip;
    [Space(10)]
    [Tooltip("In the Animator there should be a state with a name, which needs to have a (possibly empty) AnimationClip with this name")]
    public string NameOfClipToSwap = "Talking";
    [Tooltip("In the Animator that you're controlling, this should be the name of the trigger you want to have set to move to the new AnimationClip")]
    public string Trigger;
    [Tooltip("If the AnimationClip and the WwiseEvent don't line up perfectly, you can set the relative start time here")]
    [Range(-1f, 1f)] public float AnimationOffset;

    private AnimatorOverrideController _animatorOverrideController;


    private void CreateNewAOC()
    {
        _animatorOverrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController)
        {
            name = new StringBuilder("Override " + NameOfClipToSwap + " with " + CorrespondingAnimationClip.name).ToString()
        };

        Animator.runtimeAnimatorController = _animatorOverrideController;
    }


    private void SwapClip()
    {
        _animatorOverrideController[NameOfClipToSwap] = CorrespondingAnimationClip;
    }


    [ContextMenu(nameof(SendPlayEvent))]
    public void SendPlayEvent()
    {
        CreateNewAOC();
        SwapClip();

        if (AnimationOffset > 0) // Animation trigger will happen later than Audio Event
        {
            StartAudio();
            Invoke(nameof(StartAnimation), AnimationOffset);
        }
        else if (AnimationOffset < 0) // Animation trigger will happen before Audio Event
        {
            StartAnimation();
            Invoke(nameof(StartAudio), Math.Abs(AnimationOffset));
        }
        else // Both will be send at the same time
        {
            StartAudio();
            StartAnimation();
        }
    }


    private void StartAudio()
    {
        PostEvent(WwiseEvent_play);
    }


    private void PostEvent(WwiseEvent wwiseEvent)
    {
        if (!wwiseEvent.IsValid())
        {
            Debug.LogWarning("Wwise event not valid");

            return;
        }

        AkSoundEngine.PostEvent(wwiseEvent.Name, gameObject);
    }


    private void StartAnimation()
    {
        Animator.SetTrigger(Trigger);
    }
}