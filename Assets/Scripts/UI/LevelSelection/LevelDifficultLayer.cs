using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class LevelDifficultLayer : CanvasWindowLayer
    {
        [SerializeField] private LevelDifficultButton[] _levelDifficultButtons;
        [SerializeField] private Button _levelStartButton;


        public LevelDifficultButton[] LevelDifficultButtons => _levelDifficultButtons;
        public Button LevelStartButton => _levelStartButton;
    }
}