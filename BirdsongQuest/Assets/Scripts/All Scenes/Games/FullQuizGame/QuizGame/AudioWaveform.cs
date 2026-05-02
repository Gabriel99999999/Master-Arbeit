using UnityEngine;
using UnityEngine.UI;

public class AudioWaveform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage targetImage;

    [Header("Settings")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 256;
    [SerializeField] private Color waveformColor = Color.green;
    [SerializeField] private Color backgroundColor = Color.black;


    private AudioClip audioClip;

    private void Start()
    {
        if (audioClip == null || targetImage == null)
        {
            Debug.LogError("AudioClip oder RawImage nicht zugewiesen!");
            return;
        }

        Texture2D waveformTexture = GenerateWaveformTexture();
        targetImage.texture = waveformTexture;
    }

    private Texture2D GenerateWaveformTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        float[] waveform = new float[textureWidth];
        int packSize = Mathf.CeilToInt(samples.Length / (float)textureWidth);

        for (int i = 0; i < samples.Length; i += packSize)
        {
            waveform[i / packSize] = Mathf.Abs(samples[i]);
        }

        // Hintergrund setzen
        Color[] backgroundPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < backgroundPixels.Length; i++)
            backgroundPixels[i] = backgroundColor;
        texture.SetPixels(backgroundPixels);

        // Waveform zeichnen
        for (int x = 0; x < textureWidth; x++)
        {
            int amplitudeHeight = Mathf.RoundToInt(waveform[x] * textureHeight * 0.5f);
            for (int y = (textureHeight / 2) - amplitudeHeight; y <= (textureHeight / 2) + amplitudeHeight; y++)
            {
                texture.SetPixel(x, y, waveformColor);
            }
        }

        texture.Apply();
        return texture;
    }

    public void DisplayAudioClip(AudioClip audioClip)
    {
        this.audioClip = audioClip;
        Texture2D waveformTexture = GenerateWaveformTexture();
        targetImage.texture = waveformTexture;
    }

    public void PlaySound()
    {
        UIEvents.RequestBirdSound(audioClip, transform, 1f);
    }
}
