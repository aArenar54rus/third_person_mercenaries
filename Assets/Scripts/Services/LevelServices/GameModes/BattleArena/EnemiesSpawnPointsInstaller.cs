using System;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class EnemiesSpawnPointsInstaller : MonoInstaller
    {
        [SerializeField] private EnemySpawnPoints data;
		
		
        public override void InstallBindings()
        {
            Container.BindInstance(data)
                .AsSingle()
                .NonLazy();
        }
    }
    
    
        
    [Serializable]
    public class EnemySpawnPoints
    {
        [SerializeField]
        private Transform[] playerSpawnPoint;

        private Transform lastPoint;


        public Vector3 GetRandomPointPosition()
        {
            Transform nextRandomPoint;
            
            do
            {
                nextRandomPoint = playerSpawnPoint[UnityEngine.Random.Range(0, playerSpawnPoint.Length)];
            } while (nextRandomPoint == lastPoint);
            
            lastPoint = nextRandomPoint;
            return nextRandomPoint.position;
        }
    }
}