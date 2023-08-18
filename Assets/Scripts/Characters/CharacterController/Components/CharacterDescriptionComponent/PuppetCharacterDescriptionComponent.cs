namespace Arenar.Character
{
    public class PuppetCharacterDescriptionComponent : ICharacterDescriptionComponent
    {
        public string CharacterName => "Puppet";
        public string CharacterDescription => "It's just a puppet.";
        
        
        public void Initialize() { }

        public void DeInitialize() { }

        public void OnStart() { }
    }
}