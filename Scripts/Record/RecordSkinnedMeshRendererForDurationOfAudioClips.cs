using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Recorder;
using UnityEngine;


/// <summary>
///     This is designed to create recorded AnimationClips from a live SALSA session, but can theoretically be used for any
///     system which influences SkinnedMeshRenderer / Blendshapes.
///     Use this on a gameobject which is linked to a SALSA / EmoteR / Eyes component.
///     Let those components do their calculations based on the AudioClip(s) you're feeding it.
///     This records one AnimationClip per AudioClip, so that later you can take SALSA & Co offline, and use the
///     AnimationClips instead.
///     Designed because audio middleware such as Wwise does not want to play along with SALSA unfortunately.
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class RecordSkinnedMeshRendererForDurationOfAudioClips : MonoBehaviour
{
    [Tooltip("In case you don't want to use this GameObject's name as the start of the AnimationClip name")]
    public string BaseOutputName = "";
    [Tooltip("List of all audioclips you want to have the SkinnedMeshRenderer recorded")]
    public List<AudioClip> AudioClips;
    [Tooltip("If you want to re-record the same clip multiple times, perhaps because SALSA gives different results and you want to select the best recorded clip later")]
    [Range(1, 5)] public int NumberOfRecordingsPerClip = 1;
    [Tooltip("Some duration in between clips seems good, because then the system has some time to store clips and setup for new recording.")]
    [Range(0f, 5f)] public float DurationInBetweenRecordings = 1f;
    [Tooltip("The framerate to which the recording is made. Notice that Unity caps the framerate of the game to the same framerate during recording")]
    [Range(0, 60)] public float RecordingFrameRate = 24;
    public bool StartRecordingOnStart = true;


    private AnimationRecorderSettings _animationRecorderSettings;
    private AudioSource _audioSource;
    private GameObject _objectToRecord;
    private Coroutine _record;
    private RecorderController _recorderController;
    private RecorderControllerSettings _settings;
    private SkinnedMeshRenderer _skinnedMeshRenderer;


    private void Awake()
    {
        if (_objectToRecord == null)
        {
            _objectToRecord = gameObject;
        }

        _animationRecorderSettings = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
        var animationInputSettings = _animationRecorderSettings.AnimationInputSettings;
        animationInputSettings.gameObject = _objectToRecord;

        _skinnedMeshRenderer = _objectToRecord.GetComponent<SkinnedMeshRenderer>();

        if (_skinnedMeshRenderer == null)
        {
            Debug.LogError("No SkinnedMeshRenderer found on ObjectToRecord");

            return;
        }

        animationInputSettings.AddComponentToRecord(_skinnedMeshRenderer.GetType());

        _audioSource = _objectToRecord.GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("No AudioSource found on ObjectToRecord");

            return;
        }

        _settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        _settings.FrameRate = RecordingFrameRate;
        _settings.AddRecorderSettings(_animationRecorderSettings);
        _settings.Save();
    }


    private void CreateOutputName(string clipName)
    {
        var take = 1;

        CreateFileName(clipName, take);

        while (File.Exists(_animationRecorderSettings.FileNameGenerator.BuildAbsolutePath(new RecordingSession())))
        {
            take++;

            CreateFileName(clipName, take);
        }
    }


    private void CreateFileName(string clipName, int take)
    {
        var commonName = new StringBuilder("_" + clipName + "_take-" + take + "_FR-" + RecordingFrameRate).ToString();

        _animationRecorderSettings.FileNameGenerator.Leaf = "Animation/Recordings/";

        if (string.IsNullOrEmpty(BaseOutputName) || string.IsNullOrWhiteSpace(BaseOutputName))
        {
            _animationRecorderSettings.FileNameGenerator.FileName = new StringBuilder(_objectToRecord.name + commonName).ToString();
        }
        else
        {
            _animationRecorderSettings.FileNameGenerator.FileName = new StringBuilder(BaseOutputName + commonName).ToString();
        }
    }


    private void Start()
    {
        if (StartRecordingOnStart)
        {
            StartRecording();
        }
    }


    [ContextMenu(nameof(StartRecording))]
    private void StartRecording()
    {
        if (AudioClips == null || AudioClips.Count == 0)
        {
            Debug.LogError("No AudioClips have been entered, therefore recording cannot continue");

            return;
        }

        if (_record == null)
        {
            _record = StartCoroutine(Record());
        }
        else
        {
            StopCoroutine(_record);
            _record = null;
            _record = StartCoroutine(Record());
        }
    }


    private IEnumerator Record()
    {
        foreach (var audioClip in AudioClips)
        {
            for (var i = 0; i < NumberOfRecordingsPerClip; i++)
            {
                Debug.LogFormat("Waiting for {0} seconds in between recordings", DurationInBetweenRecordings);

                yield return new WaitForSeconds(DurationInBetweenRecordings);

                _recorderController = new RecorderController(_settings);
                _recorderController.Settings.SetRecordModeToManual();
                _recorderController.PrepareRecording();

                CreateOutputName(audioClip.name);
                _audioSource.clip = audioClip;

                _recorderController.StartRecording();
                Debug.LogFormat("Started recording of {0}", _animationRecorderSettings.FileNameGenerator.FileName);
                _audioSource.Play();

                while (_audioSource.isPlaying)
                {
                    yield return null;
                }

                _recorderController.StopRecording();
                Debug.LogFormat("Stopped recording. Saved as {0}", _animationRecorderSettings.FileNameGenerator.FileName);

                StoreClipInSO.CreateSOAndStoreClip(_animationRecorderSettings.FileNameGenerator.Leaf, _animationRecorderSettings.FileNameGenerator.FileName, audioClip);
            }
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}