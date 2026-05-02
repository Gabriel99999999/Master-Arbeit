using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public interface IGameSettingsCreator
    {
        void CreateSettings();
        string SceneName { get; }
    }
}
