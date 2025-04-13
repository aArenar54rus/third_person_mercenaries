namespace Arenar.Character
{
    public class SimpleCharacterAimComponent : ICharacterAimComponent
    {
        public bool IsAim { get; set; }
        public float AimProgress { get; private set; }
        
        
        public void Initialize()
        {
            IsAim = false;
            AimProgress = 0;
        }
        
        public void DeInitialize()
        {
        }
        
        public void OnActivate()
        {
        }

        public void OnDeactivate()
        {
        }
    }
}