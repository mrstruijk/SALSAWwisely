using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class RemoveBindings : MonoBehaviour
{
    [Header("TO FIND OUT WHAT THE NAMES OF THE (IN)CORRECT BINDINGS ARE")]
    public AnimationClip AnimationClip;

    [Space(10)]
    [SerializeField] private EditorCurveBinding[] _curveBindings;
    [SerializeField] private string _nameOfCurveBindingsToKeep = "blendShape";

    [Space(10)]
    [SerializeField] private static EditorCurveBinding[] _objectReferenceBindings;
    [SerializeField] private string _nameOfObjectReferenceBindingsToRemove = "Material";


    [Header("TO REMOVE INCORRECT BINDINGS")]
    public List<AnimationClip> AnimationClips;

    /// <summary>
    ///     Use this to see which the name is of the bindings in the AnimationClip.
    /// </summary>
    [ContextMenu(nameof(GetNamesOfBindings))]
    public void GetNamesOfBindings()
    {
        _curveBindings = AnimationUtility.GetCurveBindings(AnimationClip);
        _objectReferenceBindings = AnimationUtility.GetObjectReferenceCurveBindings(AnimationClip);
    }

    /// <summary>
    ///     In the model's SkinnedMeshRenderer that I used, the blendshapes all contained the word 'blendShape',
    ///     and the ObjectReference all contained the word 'Material'. Using the words you want to keep/remove the below
    ///     methods allows you to clean up your AnimationClip.
    /// </summary>
    [ContextMenu(nameof(RemoveBindingsFromClip))]
    public void RemoveBindingsFromClip()
    {
        if (AnimationClip == null)
        {
            Debug.LogError("No clip present");
            return;
        }

        AnimationClip = RemoveCurvesExcept(AnimationClip, _nameOfCurveBindingsToKeep);
        AnimationClip = RemoveObjectReferenceBindingsContaining(AnimationClip, _nameOfObjectReferenceBindingsToRemove);
    }

    /// <summary>
    ///     To batch remove the same bindings from multiple clips
    /// </summary>
    [ContextMenu(nameof(RemoveBindingsFromMultipleClips))]
    public void RemoveBindingsFromMultipleClips()
    {
        if (AnimationClips == null || AnimationClips.Count == 0)
        {
            Debug.LogError("No clips present");
            return;
        }

        for (var i = 0; i < AnimationClips.Count; i++)
        {
            AnimationClips[i] = RemoveCurvesExcept(AnimationClips[i], _nameOfCurveBindingsToKeep);
            AnimationClips[i] = RemoveObjectReferenceBindingsContaining(AnimationClips[i], _nameOfObjectReferenceBindingsToRemove);
        }
    }


    /// <summary>
    ///     Allows you to remove any bindings from an AnimationClip, except when that binding contains a word.
    ///     Created to remove all extranaous bindings from Unity Recorder Skinned Mesh Renderer recordings that don't deal with
    ///     the BlendShapes
    /// </summary>
    public AnimationClip RemoveCurvesExcept(AnimationClip animationClip, string bindingContains = "blendShape")
    {
        _curveBindings = AnimationUtility.GetCurveBindings(animationClip);

        for (var i = _curveBindings.Length - 1; i >= 0; i--)
        {
            if (!_curveBindings[i].propertyName.Contains(bindingContains))
            {
                AnimationUtility.SetEditorCurve(animationClip, _curveBindings[i], null);
                Debug.Log("Deleted CurveBinding " + _curveBindings[i].propertyName);
            }
            else
            {
                Debug.Log("Kept CurveBinding " + _curveBindings[i].propertyName);
            }
        }

        return animationClip;
    }


    /// <summary>
    ///     Allows you to remove specific ObjectReference bindings from Animation clip.
    ///     Created to remove all extranaous bindings from Unity Recorder Skinned Mesh Renderer recordings that don't deal with
    ///     the BlendShapes
    /// </summary>
    public AnimationClip RemoveObjectReferenceBindingsContaining(AnimationClip animationClip, string bindingContains = "Material")
    {
        _objectReferenceBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);

        for (var i = _objectReferenceBindings.Length - 1; i >= 0; i--)
        {
            if (_objectReferenceBindings[i].propertyName.Contains(bindingContains))
            {
                AnimationUtility.SetObjectReferenceCurve(animationClip, _objectReferenceBindings[i], null);
                Debug.Log("Deleted ObjectReferenceBinding " + _objectReferenceBindings[i].propertyName);
            }
            else
            {
                Debug.Log("Kept ObjectReferenceBinding " + _objectReferenceBindings[i].propertyName);
            }
        }

        return animationClip;
    }
}