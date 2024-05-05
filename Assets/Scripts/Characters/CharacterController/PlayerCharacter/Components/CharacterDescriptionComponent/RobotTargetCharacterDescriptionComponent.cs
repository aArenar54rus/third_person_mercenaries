namespace Arenar.Character
{
    public class RobotTargetCharacterDescriptionComponent : ICharacterDescriptionComponent
    {
        public string CharacterName => "loc_key_robot_target_name";
        public string CharacterDescription => "loc_key_robot_target_description";


        public void Initialize() { }

        public void DeInitialize() { }

        public void OnActivate() { }
        public void OnDeactivate() { }
    }
}