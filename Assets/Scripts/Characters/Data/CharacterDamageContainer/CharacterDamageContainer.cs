using UnityEngine;


namespace Arenar.Character
{
	[RequireComponent(typeof(Collider))]
	public class CharacterDamageContainer : MonoBehaviour
	{
		[SerializeField] private Collider collider;
		[SerializeField] private CharacterDamageContainerBodyType bodyType;
		
		private ICharacterEntity characterEntity;
		private ICharacterLiveComponent characterLiveComponent;
		
		
		public void Initialize(ICharacterEntity characterEntity)
		{
			collider ??= GetComponent<Collider>();
			
			this.characterEntity = characterEntity;
		}
		
		public void GetDamage(DamageData damageData)
		{
			
		}
	}
}