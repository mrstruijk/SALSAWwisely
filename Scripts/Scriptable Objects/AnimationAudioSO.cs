using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using WwiseEvent = AK.Wwise.Event;


[Serializable]
public class AnimationAudioSO : ScriptableObject
{
    private const string Path = "Assets/Animation/ScriptableObjects/";

    public string WwiseEvent_play;
    public AnimationClip AnimationClip;
    public float AnimationClipLength;
    public float AudioClipLength;
    public float PlayBackSpeedMultiplier = 1f;

    [Space(10)]
    [Tooltip("If the AnimationClip and the WwiseEvent don't line up perfectly, you can set the relative start time here")]
    [Range(-1f, 1f)] public float AnimationOffset;

    private float _tolerance = 0.001f;


    public static AnimationAudioSO CreateInstance(string assetName, AnimationClip clip, AudioClip audioClip)
    {
        var animationAudio = CreateInstance<AnimationAudioSO>();
        animationAudio.WwiseEvent_play = "Play_" + audioClip.name;
        animationAudio.AnimationClip = clip;
        animationAudio.AudioClipLength = audioClip.length;
        animationAudio.AnimationClipLength = animationAudio.AnimationClip.length;
        animationAudio.PlayBackSpeedMultiplier = animationAudio.CalculatePlaybackSpeed();

        var fileName = new StringBuilder(assetName + ".asset").ToString();
        var path = AssetDatabase.GenerateUniqueAssetPath(Path + fileName);

        AssetDatabase.CreateAsset(animationAudio, path);
        AssetDatabase.SaveAssets();

        return animationAudio;
    }


    private float CalculatePlaybackSpeed()
    {
        var playbackSpeed = 1f;

        if (Math.Abs(AnimationClipLength - AudioClipLength) > _tolerance)
        {
            playbackSpeed = AnimationClipLength / AudioClipLength;
        }

        return playbackSpeed;
    }
}