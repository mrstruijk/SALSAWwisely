# SALSAWwisely
### Version 0.02

Wwise does not (easily) provide Unity with audio data for realtime LipSync approximation.
In this workaround, you can still keep all audiofiles in Wwise, and trigger recorded Animations simultaneously.

You'll need the Unity Recorder (available through the Package Manager). You also need some asset which modifies Blendshapes based on an AudioClip. This is designed for use with SALSA, so that's the best scenario here. Lastly, you'll need Wwise, but I'm sure you can make it work with other middleware such as FMOD.

In order for this to work, these are the steps to take:

## RECORDING:
1. Setup your character model using the standard procedures detailed in SALSA's documentation.
1. Import any voiceline clips you want to use in Unity.
1. Add the RecordSkinnedMeshRendererForDurationOfAudioClips to the model, at the level where the SkinnedMeshRenderer is located.
1. Add an AudioSource on that same GameObject.
1. Add the AudioClips you want to create AnimationClips for to the list AudioClips.
1. If you want multiple recordings of the same audioclip (since SALSA might analyse the clip slightly differently on each run. Maybe you want to select the best clip once recording is finished): set the 'NumberOfRecordingsPerClip' to the desired number, as long as it's 1 or higher.
1. If you set the StartOnStart to true, it will start the recording on entering PlayMode, otherwise hit right-mouse-button on the component, and hit StartRecording.
1. Wait until all clips have been recorded. Have some coffee. It's probably best to leave Unity to run in peace.
1. Find your recordings. Recorded clips are stored inside the 'Assets/Animation/Recordings' folder.
1. Notice that the recorded AnimationClip may not have the exact same length as the AudioClip. This is due to the (exact) framerate of the Unity Editor during PlayMode Recording earlier.
1. A ScriptableObject with the name of the recording has been created in the 'Assets/Animation/ScriptableObjects' folder. There you can find a reference to the recording, and information on how long it is, compared to the length of the original audioclip. It has a PlayBackSpeedMultiplier for dealing with the difference in AnimationClip vs AudioClip length. It also contains the (suggested) name for you Wwise Event.
1. If satisfied with the recordings, you can remove the RecordSkinnedMeshRendererForDurationOfAudioClips component, and also the SALSA component. If you want, you can keep the Eyes and EmoteR components live. At this stage you can also remove the AudioClips from Unity altogether, as long as you add them to your Wwise project.

## PLAYING:
1. Setup Wwise using the standard procedures detailed in the Wwise documentation.
1. Add your AudioClips to Wwise, and hook them up with the same name as the WwiseEvent_play in the ScriptableObject for each voiceline.
1. (Don't forget to generate your SoundBank. In Unity, you usually need to enter PlayMode once to load the SoundBank, but refer to Wwise documentation for more information.)
1. Add the PostWwisePlayEventWithAnimation to your GameObject.
1. Add the created ScriptableObject to the PostWwisePlayEventWithAnimation component.
1. Select the Animator you wish to have the AnimationClip played to.
1. In that Animator, you need to have a State with a default (possibly empty) AnimationClip. The name of that default/empty AnimationClip (so NOT the name of the State!) should be the same as the NameOfClipToSwap field.
1. That State should have a Trigger Parameter, whose name should be in the TriggerParameter field of the PostWwisePlayEventWithAnimation component (by default this is 'Talking').
1. Add a Float Parameter in your Animator, by default that should be 'SpeedMultiplier' (make sure its identical to the SpeedMultiplierParameter field in the PostWwisePlayEventWithAnimation component). Hook that parameter up in the Inspector to the Speed function of the earlier mentioned State, so that it functions as a Speed multiplier.
1. Then, when you want to start the Animation + Wwise Event, call the SendPlayEvent function (also available through right-mouse-button for easy testing).
1. Optionally, you can set the Animation Offset in the ScriptableObject, in case there's still a mismatch in when the audio is played from Wwise, and the Animation is played in Unity.

I hope this helps you get started. Feel free to make any changes to the code. This is provided as is, with limited to no support. However, if you do have any questions, feel free to reach out, and maybe I can help.
