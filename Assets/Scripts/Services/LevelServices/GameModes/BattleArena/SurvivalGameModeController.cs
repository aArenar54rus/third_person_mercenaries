using Arenar.CameraService;
using Arenar.Character;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class SurvivalGameModeController : GameModeController
    {
        private CharacterSpawnController сharacterSpawnController;
        private SurvivalLevelInfoCollection survivalLevelInfoCollection;
        private PlayerCharacterSkillUpgradeService playerSkillUpgradeService;
        private ICameraService cameraService;
        private EffectsSpawner effectsSpawner;
        
        private ILevelsService levelsService;
        
        private DiContainer container;
        private PlayerSpawnPoint spawnPoint;
        private EnemySpawnPoints enemySpawnPoints;

        private int enemyCounter;
        private float nextSpawnTime;
        private float nextLevelSpawnTime;

        private Tween playerDeathTween;
        
        private bool isGameActive = false;
        

        private bool CanSpawnEnemy => (enemyCounter >= survivalLevelInfoCollection.MaxEnemyCountOnScene);


        public SurvivalGameModeController(
            ILevelsService levelsService,
            CharacterSpawnController сharacterSpawnController,
            ICameraService cameraService,
            SurvivalLevelInfoCollection survivalLevelInfoCollection,
            DiContainer container,
            PlayerCharacterSkillUpgradeService playerSkillUpgradeService,
            EffectsSpawner effectsSpawner
        )
        {
            this.levelsService = levelsService;
            this.сharacterSpawnController = сharacterSpawnController;
            this.cameraService = cameraService;
            this.survivalLevelInfoCollection = survivalLevelInfoCollection;
            this.container = container;
            this.playerSkillUpgradeService = playerSkillUpgradeService;
            this.effectsSpawner = effectsSpawner;
        }


        public override void StartGame()
        {
            GetNextLevelSpawnTime();
            
            spawnPoint = container.Resolve<PlayerSpawnPoint>();
            enemySpawnPoints = container.Resolve<EnemySpawnPoints>(); 
            
            var playerCharacter = сharacterSpawnController.GetCharacter(CharacterTypeKeys.Player);
            playerCharacter.EntityObjectTransform.position = spawnPoint.Position;
            playerCharacter.CharacterTransform.rotation = spawnPoint.Rotation;

            if (playerCharacter is PhysicalHumanoidComponentCharacterController player)
            {
                cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, player.CharacterTransform);
                cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
            }

            if (playerCharacter.TryGetCharacterComponent<ICharacterLiveComponent>(out var characterLiveComponent))
            {
                characterLiveComponent.OnCharacterDie += PlayerDieHandler;
            }
            
            playerCharacter.Activate();
            playerSkillUpgradeService.InitializeCharacter(playerCharacter);
            
            isGameActive = true;
        }
        
        public override void EndGame() {}
        
        public override void OnUpdate()
        {
            if (!isGameActive)
                return;

            nextSpawnTime += Time.deltaTime;
            if (nextSpawnTime < nextLevelSpawnTime)
                return;

            GetNextLevelSpawnTime();

            SurvivalLevelInfoCollection.SpawnPattern spawnPattern =
                survivalLevelInfoCollection.SpawnPatterns[Random.Range(0, survivalLevelInfoCollection.SpawnPatterns.Length)];

            if (CanSpawnEnemy)
                return;

            foreach (var spawnNpc in spawnPattern.SpawnNpcs)
            {
                ICharacterEntity enemy = сharacterSpawnController.GetCharacter(spawnNpc);

                if (enemy.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                    liveComponent.OnCharacterDie += EnemyCharacterDieHandler;

                var position = enemySpawnPoints.GetRandomPointPosition();
                enemy.EntityObjectTransform.position = position;
                
                enemy.Activate();
                
                enemyCounter++;
                if (CanSpawnEnemy)
                    return;
            }
        }
        
        private void GetNextLevelSpawnTime()
        {
            nextSpawnTime = 0;
            nextLevelSpawnTime = Random.Range(survivalLevelInfoCollection.MinSpawnInterval,
                survivalLevelInfoCollection.MaxSpawnInterval);
        }
        
        private void PlayerDieHandler(ICharacterEntity character)
        {
            playerDeathTween = DOVirtual.DelayedCall(1.0f, () =>
            {
                CompleteLevel();
            });
            
            isGameActive = false;
        }
        
        private void EnemyCharacterDieHandler(ICharacterEntity character)
        {
            playerSkillUpgradeService.UpgradeScore++;
            enemyCounter--;
            if (character.TryGetCharacterComponent<ICharacterLiveComponent>(out var liveComponent))
                liveComponent.OnCharacterDie -= EnemyCharacterDieHandler;
            
            playerDeathTween = DOVirtual.DelayedCall(1.0f, () =>
            {
                var effect = effectsSpawner.GetEffect(EffectType.RobotBlow);
                effect.transform.position = character.CharacterTransform.position;
                effect.gameObject.SetActive(true);
                effect.Play();
                
                сharacterSpawnController.ReturnCharacter(character);
            });
        }

        private void CompleteLevel()
        {
            levelsService.CompleteLevel();
            сharacterSpawnController.ReturnAllCharacters();
        }
    }
}