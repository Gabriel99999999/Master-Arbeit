using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource _soundFXObject;
    [SerializeField] private AudioSource _birdSoundFXObject;

    private readonly List<AudioSource> _activeBirdSounds = new();
    private bool _allowMultipleBirdSounds = false; // default: Memory mode


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // falls gewünscht über Szenen hinweg
    }

    private void OnEnable()
    {
        UIEvents.OnPlaySoundRequested += PlaySoundFXClip;
        UIEvents.OnPlayBirdCallRequested += PlayBirdSoundFXClip;
    }
    private void OnDisable()
    {
        UIEvents.OnPlaySoundRequested -= PlaySoundFXClip;
        UIEvents.OnPlayBirdCallRequested -= PlayBirdSoundFXClip;
    }

    public void SetBirdSoundMode(bool allowMultiple)
    {
        _allowMultipleBirdSounds = allowMultiple;
    }

    /*private void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(_soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);


    }*/


    private void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        PlayClip(_soundFXObject, clip, spawnTransform, volume);
    }

    public void StopAllBirdSounds()
    {
        Debug.Log("SoundFxManager: Stopping all bird sounds1.");
        foreach (var src in _activeBirdSounds)
        {
            if (src != null)
                Destroy(src.gameObject);
        }
        _activeBirdSounds.Clear();
    }
    private void PlayBirdSoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (!_allowMultipleBirdSounds)
        {
            Debug.Log("SoundFxManager: Stopping all bird sounds2.");
            StopAllBirdSounds();
        }

        var srcInstance = PlayClip(_birdSoundFXObject, clip, spawnTransform, volume);
        _activeBirdSounds.Add(srcInstance);
    }

    private AudioSource PlayClip(AudioSource prefab, AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null || prefab == null) return null;

        AudioSource audioSource = Instantiate(prefab, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
        return audioSource;
    }
}
