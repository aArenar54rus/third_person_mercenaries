using UnityEngine;
using Zenject;


namespace CatSimulator.Character
{
    public class CharacterRayCastComponent : ICharacterRayCastComponent
    {
        private ICharacterEntity characterEntity;
        private bool isGrounded = true;
        private PlayerCharacterParametersData playerCharacterParametersData;
   

        private Transform characterTransform =>
            characterEntity.CharacterTransform;


        [Inject]
        public void Construct(ICharacterEntity characterEntity, PlayerCharacterParametersData playerCharacterParametersData)
        {
            this.characterEntity = characterEntity;
            this.playerCharacterParametersData = playerCharacterParametersData;
        }

        public bool IsGroundedCheck()
        {
            Vector3 spherePosition = new Vector3(characterTransform.position.x, 
                characterTransform.position.y + playerCharacterParametersData.GroundedOffset,
                characterTransform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, playerCharacterParametersData.GroundedRadius, playerCharacterParametersData.GroundedLayers,
                QueryTriggerInteraction.Ignore);

            return isGrounded;
        }

        public void Initialize() { }

        public void DeInitialize() { }

        public void OnStart() { }
    }
}