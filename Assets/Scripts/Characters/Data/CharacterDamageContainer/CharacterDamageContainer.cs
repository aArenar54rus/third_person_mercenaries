using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
	[RequireComponent(typeof(Collider))]
	public class CharacterDamageContainer : MonoBehaviour
	{
		[SerializeField] private Collider collider;
		private ICharacterEntity _characterEntity;
		
		public void Initialize(ICharacterEntity characterEntity)
		{
			collider ??= GetComponent<Collider>();
			
			_characterEntity = characterEntity;
		}


		/*
		public void (Collider other)
		{
			
		}*/
	}
}