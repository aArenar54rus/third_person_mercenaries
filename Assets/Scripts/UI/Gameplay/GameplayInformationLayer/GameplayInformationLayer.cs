using TMPro;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class GameplayInformationLayer : CanvasWindowLayer
    {
        [SerializeField] private EnemyTargetInformationPanel enemyTargetInformationPanel;
        [SerializeField] private TMP_Text informationText;


        public EnemyTargetInformationPanel EnemyTargetInformationPanel => enemyTargetInformationPanel;
        public TMP_Text InformationText => informationText;
    }
}