using System;
using UnityEngine;

public static class UIEvents
{
    public static event Action<string, float> OnSceneLoadRequested;
    public static event Action<bool> OnAudioPanelRequested;
    public static event Action<AudioClip, Transform, float> OnPlaySoundRequested;
    public static event Action<AudioClip, Transform, float> OnPlayBirdCallRequested;

    public static void RequestSceneLoad(string sceneName, float delay = 0f)
        => OnSceneLoadRequested?.Invoke(sceneName, delay);

    public static void RequestAudioPanel(bool active)
        => OnAudioPanelRequested?.Invoke(active);

    public static void RequestSound(AudioClip clip, Transform transform, float volume = 1f)
        => OnPlaySoundRequested?.Invoke(clip, transform, volume);

    public static void RequestBirdSound(AudioClip clip, Transform transform, float volume = 1f)
        => OnPlayBirdCallRequested?.Invoke(clip, transform, volume);

}
