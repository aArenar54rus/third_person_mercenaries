using System;
using Arenar.Services.LevelsService;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private int _levelIndex;
        [SerializeField] private string _levelNameKey;
        [SerializeField] private string _sceneKey;
        [SerializeField] private Sprite _levelPortrait;
        [SerializeField] private SerializableDictionary<LevelDifficult, LevelDifficultData> _difficultData;
        [SerializeField] private GameMode _gameMode;


        public int LevelIndex => _levelIndex;
        public string LevelNameKey => _levelNameKey;
        public string SceneKey => _sceneKey;
        public Sprite LevelPortrait => _levelPortrait;
        public SerializableDictionary<LevelDifficult, LevelDifficultData> DifficultData => _difficultData;
        public GameMode GameMode => _gameMode;
        
        

        [Serializable]
        public class LevelDifficultData
        {
            [SerializeField] private int _minimalEnemyLevel;
            [SerializeField] private int _maximumEnemyLevel;


            public int MinimalEnemyLevel => _minimalEnemyLevel;
            public int MaximumEnemyLevel => _maximumEnemyLevel;
        }
    }
}