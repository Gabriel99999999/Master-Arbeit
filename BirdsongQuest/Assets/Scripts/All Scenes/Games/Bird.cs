using Assets.Scripts.All_Scenes.Games;
using UnityEngine;

public enum GermanArticle { Der, Die, Das, Den }

[CreateAssetMenu(menuName = "Birdsong/Bird", fileName = "Bird")]
public class Bird : ScriptableObject
{
    public string id;
    public string displayName;
    [SerializeField] private GermanArticle germanArticle;
    public string GermanArticleString
    {
        get
        {
            return germanArticle switch
            {
                GermanArticle.Der => "der",
                GermanArticle.Die => "die",
                GermanArticle.Das => "das",
                GermanArticle.Den => "den",
                _ => "",
            };
        }
    }
    public Sprite image;
    public AudioClip[] calls;

    public void AddCorrect()
    {
        string key = $"BirdCorrect_{id}";
        int current = PlayerPrefs.GetInt(key, 0);

        // Max 6 (1 unlock + 5 Sterne)
        current = Mathf.Clamp(current + 1, 0, 6);

        PlayerPrefs.SetInt(key, current);
        PlayerPrefs.Save();
    }

    public void MarkAsFoundInGame(GameId gameId)
    {
        string key = $"BirdFound_{id}_{(int)gameId}";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
}