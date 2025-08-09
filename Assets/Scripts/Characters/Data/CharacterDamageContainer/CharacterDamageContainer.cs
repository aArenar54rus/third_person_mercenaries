using Cysharp.Threading.Tasks;
using RootMotion.Dynamics;
using UnityEngine;

namespace Arenar.Character
{
	[RequireComponent(typeof(Collider))]
	public class CharacterDamageContainer : MonoBehaviour
	{
		public float unpin = 10f;
		public float might = 10f;
		
		[SerializeField]
		private Collider _collider;
		[SerializeField]
		private Rigidbody _rigidbody;
		[SerializeField]
		private ECharacterDamageContainerBodyType _bodyType;
		[SerializeField]
		private MuscleCollisionBroadcaster _muscleBroadcaster;
		
		private ICharacterLiveComponent _characterLiveComponent;
		private IStunCharacterComponent _characterStunComponent;
		
		
		public ICharacterEntity CharacterEntity { get; private set; }
		
		
		public void Initialize(ICharacterEntity characterEntity)
		{
			_collider ??= GetComponent<Collider>();
			_rigidbody ??= GetComponent<Rigidbody>();
			
			CharacterEntity = characterEntity;
			CharacterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out _characterLiveComponent);
			CharacterEntity.TryGetCharacterComponent<IStunCharacterComponent>(out _characterStunComponent);
		}
		
		public void SetDamage(DamageData damageData, RaycastHit hit) {
			damageData.BodyPart = _bodyType;
			
			if (_muscleBroadcaster)
				UpdatePhysicalHit(damageData, hit).Forget();
			
			if (_characterLiveComponent != null) {
				_characterLiveComponent.SetDamage(damageData);
			}

			if (_characterStunComponent != null) {
				damageData.BodyPart = _bodyType;
				_characterStunComponent.AddStunPoints(damageData);
			}
		}
		
		private async UniTask UpdatePhysicalHit(DamageData damageData, RaycastHit hit) {
			var puppet = _muscleBroadcaster.puppetMaster;
			var muscle = puppet.GetMuscle(_rigidbody);
			
			float oldPin = muscle.props.pinWeight;
			float oldMuscle = muscle.props.muscleWeight;

			muscle.props.pinWeight = 0.1f;
			muscle.props.muscleWeight = 0.2f;

			_muscleBroadcaster.Hit(unpin, damageData.PhysicalMight.normalized * might, hit.point);
			_rigidbody.AddForceAtPosition(damageData.PhysicalMight.normalized * might, hit.point, ForceMode.Impulse);

			await UniTask.Delay(250);

			float duration = 0.2f;
			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / duration);
				muscle.props.pinWeight = Mathf.Lerp(0.1f, oldPin, t);
				muscle.props.muscleWeight = Mathf.Lerp(0.2f, oldMuscle, t);
				await UniTask.Yield();
			}

			muscle.props.pinWeight = oldPin;
			muscle.props.muscleWeight = oldMuscle;
		}
	}
}