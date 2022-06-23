using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AnimationAudioSO))]
public class AnimationAudioEditor : Editor
{
    private AnimationAudioSO _target;
    private float _defaultLabelWidth;

    private void OnEnable()
    {
        _target = (AnimationAudioSO) target;
    }


    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        CreateCustomInspector();
    }


    private void CreateCustomInspector()
    {
        EditorGUIUtility.labelWidth = 175f;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(nameof(_target.WwiseEvent_play), _target.WwiseEvent_play);
        EditorGUILayout.ObjectField(nameof(_target.AnimationClip), _target.AnimationClip, typeof(AnimationClip), false);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField(new GUIContent(nameof(_target.AnimationClipLength)), _target.AnimationClipLength);
        EditorGUILayout.Space(10);
        EditorGUILayout.FloatField(new GUIContent(nameof(_target.AudioClipLength)), _target.AudioClipLength);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        EditorGUILayout.FloatField(new GUIContent(nameof(_target.PlayBackSpeedMultiplier)), _target.PlayBackSpeedMultiplier);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");
        _target.AnimationOffset = EditorGUILayout.Slider(new GUIContent(nameof(_target.AnimationOffset), "If the AnimationClip and the WwiseEvent don't line up perfectly, you can set the relative start time here"), _target.AnimationOffset, -2.5f, 2.5f);
        EditorGUILayout.EndVertical();
    }
}