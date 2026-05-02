using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnCloseApplication()
    {
        Debug.Log("[MainMenuController] Anwendung wird geschlossen.");
        Application.Quit();
    }
}
