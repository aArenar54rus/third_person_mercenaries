using System;
using UnityEngine;
using Zenject;


namespace Arenar
{
	public class PlayerSpawnPointInstaller : MonoInstaller
	{
		[SerializeField] private PlayerSpawnPoint data;
		
		
		public override void InstallBindings()
		{
			Container.BindInstance(data)
				.AsSingle()
				.NonLazy();
		}
	}
    
    
	[Serializable]
	public class PlayerSpawnPoint
	{
		[SerializeField]
		private Transform playerSpawnPoint;
		
		
		public Vector3 Position => playerSpawnPoint.position;
		public Quaternion Rotation => playerSpawnPoint.rotation;
	}
}