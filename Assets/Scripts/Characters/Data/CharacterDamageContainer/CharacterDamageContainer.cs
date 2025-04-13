using UnityEngine;


namespace Arenar.Character
{
	[RequireComponent(typeof(Collider))]
	public class CharacterDamageContainer : MonoBehaviour
	{
		[SerializeField] private Collider collider;
		[SerializeField] private CharacterDamageContainerBodyType bodyType;
		
		private ICharacterLiveComponent characterLiveComponent;
		
		
		public ICharacterEntity CharacterEntity { get; private set; }
		
		
		public void Initialize(ICharacterEntity characterEntity)
		{
			collider ??= GetComponent<Collider>();
			
			CharacterEntity = characterEntity;
			CharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out characterLiveComponent);
		}
		
		public void GetDamage(DamageData damageData)
		{
			if (characterLiveComponent == null)
				return;
			
			characterLiveComponent.SetDamage(damageData);
		}
	}
}