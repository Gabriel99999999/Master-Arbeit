using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAction : MonoBehaviour
{
    public enum ButtonActionType
    {
        None,
        LoadScene,
        ShowPanel,
        HidePanel
    }

    [Header("Aktionstyp")]
    [SerializeField] private ButtonActionType actionType = ButtonActionType.None;

    [Header("Ziel")]
    [SerializeField] private string targetScene;

    [Header("Klicksound")]
    [SerializeField] private AudioClip clickSound;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // 🔊 Klicksound
        if (clickSound != null)
            UIEvents.RequestSound(clickSound, transform, 1f);

        // 🔁 Aktion auslösen
        switch (actionType)
        {
            case ButtonActionType.LoadScene:
                UIEvents.RequestSceneLoad(targetScene, clickSound ? clickSound.length * 0.9f : 0f);
                break;

            case ButtonActionType.ShowPanel:
                UIEvents.RequestAudioPanel(true);
                break;

            case ButtonActionType.HidePanel:
                UIEvents.RequestAudioPanel(false);
                break;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
}
