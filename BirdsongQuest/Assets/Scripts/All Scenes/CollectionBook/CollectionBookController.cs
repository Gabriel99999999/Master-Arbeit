using Assets.Scripts.All_Scenes.CollectionBook;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectionBookController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CollectionBookElement[] entries; // size = 10, Reihenfolge = Vogel 1..10

    [Header("birdSet")]
    [SerializeField] private BirdSet birdSet;

    [Header("Unlock Settings")]
    [SerializeField] private Material grayscaleMat;
    [SerializeField] private Color unlockedColor = Color.white;

    [Header("PlayerPrefs")]
    [SerializeField] private string unlockedLevelKey = "UnlockedLevel"; // falls du anders speicherst: ändern
    [SerializeField] private int maxBirds = 10;

    private void Start()
    {
        ApplyEntries();
    }


    private void ApplyEntries()
    {
        int unlockedLevel = PlayerPrefs.GetInt(unlockedLevelKey, 0);

        // deine Unlock-Regel (wie in deinem Code)
        // Level 0 -> 0
        // Level 1 -> 3 (laut Beschreibung)
        // in deinem Code war es: if(level > 1) ...
        // Ich mache es hier so, wie du es beschrieben hast:
        int unlockedCount = 0;
        if (unlockedLevel > 1) //unlockedLevel ist das Level das freigeschalten ist aber noch nicht geschaft wurde
            unlockedCount = Mathf.Clamp(3 + (unlockedLevel - 2), 0, maxBirds);

        int count = Mathf.Min(entries.Length, birdSet.birds.Count, maxBirds);

        for (int i = 0; i < count; i++)
        {
            var bird = birdSet.birds[i];

            bool unlockedByLevel = (i < unlockedCount);

            // Wichtig: wenn du willst, dass ein Vogel NUR durch Level unlocked wird,
            // dann ignorieren wir BirdCorrect für locked/unlocked.
            // Sterne werden nur angezeigt, wenn unlockedByLevel true ist.
            entries[i].Refresh(bird, unlockedByLevel);
        }

        // Falls entries länger als birdSet ist: leere Slots deaktivieren
        for (int i = count; i < entries.Length; i++)
        {
            if (entries[i] != null)
                entries[i].gameObject.SetActive(false);
        }
    }

    public void OnBack()
    {
        SoundFXManager.Instance.StopAllBirdSounds();
    }
}
