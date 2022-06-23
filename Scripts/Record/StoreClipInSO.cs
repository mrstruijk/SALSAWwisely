using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public static class StoreClipInSO
{
    public static void CreateSOAndStoreClip(string path, string fileName, AudioClip audioClip)
    {
        var pathAndName = new StringBuilder("Assets/" + path + "/" + fileName + ".anim").ToString();

        if (!File.Exists(pathAndName))
        {
            Debug.LogErrorFormat("No file found at {0}", pathAndName);
            return;
        }

        var createdClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(pathAndName, typeof(AnimationClip));
        AnimationAudioSO.CreateInstance(fileName, createdClip, audioClip);
    }
}