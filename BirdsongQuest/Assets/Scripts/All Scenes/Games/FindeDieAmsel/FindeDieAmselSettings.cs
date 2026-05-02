using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.All_Scenes.Games.FindeDieAmsel
{
    public class FindeDieAmselSettings
    {
        private FindeDieAmselLevel _level;

        public int levelNumberForBestTimeAndNextBird;
        public FindeDieAmselLevel Level => _level;

        public bool nextLevelAddsANewBird;

        public FindeDieAmselSettings(FindeDieAmselLevel level, bool nextLevelAddsANewBird, int levelNumberForBestTimeAndNextBird)
        {
            _level = level;
            this.nextLevelAddsANewBird = nextLevelAddsANewBird;
            this.levelNumberForBestTimeAndNextBird = levelNumberForBestTimeAndNextBird;
        }
    }
}
