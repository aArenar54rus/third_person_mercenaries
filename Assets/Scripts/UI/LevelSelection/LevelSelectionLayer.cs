using UnityEngine;


namespace Arenar.Services.UI
{
    public class LevelSelectionLayer : CanvasWindowLayer
    {
        [SerializeField] private LevelSelectionButtonVisual _levelSelectionButtonVisualPrefab;
        [SerializeField] private RectTransform _levelButtonsContainer;

        
        public LevelSelectionButtonVisual LevelSelectionButtonVisualPrefab =>
            _levelSelectionButtonVisualPrefab;

        public RectTransform LevelButtonsContainer =>
            _levelButtonsContainer;
    }
}