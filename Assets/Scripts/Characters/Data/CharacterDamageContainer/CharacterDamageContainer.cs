using UnityEngine;

namespace Arenar.Character
{
	[RequireComponent(typeof(Collider))]
	public class CharacterDamageContainer : MonoBehaviour
	{
		[SerializeField]
		private Collider _collider;
		[SerializeField]
		private ECharacterDamageContainerBodyType _bodyType;
		
		private ICharacterLiveComponent _characterLiveComponent;
		private IStunCharacterComponent _characterStunComponent;
		
		
		public ICharacterEntity CharacterEntity { get; private set; }
		
		
		public void Initialize(ICharacterEntity characterEntity)
		{
			_collider ??= GetComponent<Collider>();
			
			CharacterEntity = characterEntity;
			CharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out _characterLiveComponent);
			CharacterEntity.TryGetCharacterComponent<IStunCharacterComponent>(out _characterStunComponent);
		}
		
		public void SetDamage(DamageData damageData)
		{
			if (_characterLiveComponent != null) {
				_characterLiveComponent.SetDamage(damageData);
			}

			if (_characterStunComponent != null) {
				damageData.BodyPart = _bodyType;
				_characterStunComponent.AddStunPoints(damageData);
			}
		}
	}
}