# SALSAWwisely
# Version 0.02

Wwise does not (easily) provide Unity with audio data for realtime LipSync approximation.
In this workaround, you can still keep all audiofiles in Wwise, and trigger recorded Animations simultaneously.

You'll need the Unity Recorder (available through the Package Manager). You also need some asset which modifies Blendshapes based on an AudioClip. This is designed for use with SALSA, so that's the best scenario here. Lastly, you'll need Wwise, but I'm sure you can make it work with other middleware such as FMOD.

In order for this to work, these are the steps to take:
# RECORDING:
1. Setup your character model using the standard procedures detailed in SALSA's documentation.
1. Import any voiceline clips you want to use in Unity.
1. Add the RecordSkinnedMeshRendererForDurationOfAudioClips to the model, at the level where the SkinnedMeshRenderer is located.
1. Add an AudioSource on that same GameObject.
1. Add the AudioClips you want to create AnimationClips for to the list AudioClips.
1. If you want multiple recordings of the same audioclip (since SALSA might analyse the clip slightly differently on each run. Maybe you want to select the best clip once recording is finished): set the 'NumberOfRecordingsPerClip' to the desired number, as long as it's 1 or higher.
7. If you set the StartOnStart to true, it will start the recording on entering PlayMode, otherwise hit right-mouse-button on the component, and hit StartRecording.
8. Wait until all clips have been recorded. Have some coffee. It's probably best to leave Unity to run in peace.
9. Find your recordings. Recorded clips are stored inside the 'Recordings' folder inside the 'Assets' folder.
10. If satisfied with the recordings, you can remove the RecordSkinnedMeshRendererForDurationOfAudioClips component, and also the SALSA component. If you want, you can keep the Eyes and EmoteR components live. At this stage you can also remove the AudioClips from Unity altogether, as long as you add them to your Wwise project.
11. Setup Wwise using the standard procedures detailed in the Wwise documentation.
12. Add your AudioClips to Wwise, and hook them up with well-named Events for each voiceline. Don't forget to generate your SoundBank. In Unity, you usually need to enter PlayMode once to load the SoundBank, but refer to Wwise documentation for more information.
13. Add the PostWwisePlayEventWithAnimation to your GameObject.
14. Select the desired WwisePlayEvent. Select the Animator you wish to have the AnimationClip played to. Add the AnimationClip which refers to the same voiceline as the WwisePlayEvent does.
15. In that Animator, you need to have a State with a default (possibly empty) AnimationClip. The name of that default/empty AnimationClip (so NOT the name of the State!) should be copied to the NameOfClipToSwap field. That State should have a Trigger Parameter, whose name should be in the Trigger field of the PostWwisePlayEventWithAnimation component. The upshot of this system is that you don't need to create a complicated Animator state machine: it simply swaps out the one clip when it's needed.
16. Then, when you want to start the Animation + Wwise Event, call the SendPlayEvent function (also available through right-mouse-button for easy testing).
17. Optionally, you can set the Animation Offset, in case there's a mismatch in when the audio is played from Wwise, and the Animation is played in Unity.

I hope this helps you get started. Feel free to make any changes to the code. This is provided as is, with limited to no support. However, if you do have any questions, feel free to reach out, and maybe I can help.
