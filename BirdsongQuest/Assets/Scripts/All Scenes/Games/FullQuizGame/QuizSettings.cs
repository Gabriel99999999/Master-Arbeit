using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings
{
    public class QuizSettings
    {
        public int levelNumberForBestTimeAndNextBird;
        private readonly QuizLevel _level;
        public QuizLevel Level => _level;

        public bool nextLevelAddsANewBird;
        public QuizSettings(QuizLevel level, bool nextLevelAddsANewBird, int levelNumberForBestTimeAndNextBird)
        {
            this.levelNumberForBestTimeAndNextBird = levelNumberForBestTimeAndNextBird;
            _level = level;
            this.nextLevelAddsANewBird = nextLevelAddsANewBird;
        }
    }
}
