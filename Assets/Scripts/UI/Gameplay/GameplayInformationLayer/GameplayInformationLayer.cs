using Arenar.UI;
using TMPro;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class GameplayInformationLayer : CanvasWindowLayer
    {
        [SerializeField] private EnemyTargetInformationPanel enemyTargetInformationPanel;
        [SerializeField] private TMP_Text informationText;
        [SerializeField] private ProgressBarController _progressBarController;
        [SerializeField] private GameplayWeaponInformationPanel _weaponInfoPanel;


        public EnemyTargetInformationPanel EnemyTargetInformationPanel => enemyTargetInformationPanel;
        public TMP_Text InformationText => informationText;
        public ProgressBarController ProgressBarController => _progressBarController;
        public GameplayWeaponInformationPanel WeaponInfoPanel => _weaponInfoPanel;
    }
}