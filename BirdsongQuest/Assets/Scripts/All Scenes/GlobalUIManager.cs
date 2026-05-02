using UnityEngine;
using UnityEngine.SceneManagement;


//Global UIManager: Singleton zur Verwaltung globaler UI-Elemente wie Sound-Menüs.
public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }
    [SerializeField] private GameObject soundMenuPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => UIEvents.OnAudioPanelRequested += ShowSoundMenu;
    private void OnDisable() => UIEvents.OnAudioPanelRequested -= ShowSoundMenu;

    private void ShowSoundMenu(bool active)
    {
        soundMenuPrefab.SetActive(active);
    }
}
