using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour 
{
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMasterVolume(float volume)
    {
        //_audioMixer.SetFloat("masterVolume", volume);
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }
    public void SetBirdsVolume(float volume)
    {
        //_audioMixer.SetFloat("birdSoundsVolume", volume);
        _audioMixer.SetFloat("birdSoundsVolume", Mathf.Log10(volume) * 20f);
    }
    public void SetUiVolume(float volume)
    {
        //_audioMixer.SetFloat("uiVolume", volume);
        _audioMixer.SetFloat("uiVolume", Mathf.Log10(volume) * 20f);
    }
}
