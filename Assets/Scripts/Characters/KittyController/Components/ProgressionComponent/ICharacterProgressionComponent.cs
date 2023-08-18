using System;

namespace Arenar.Character
{
    public interface ICharacterProgressionComponent : ICharacterComponent
    {
        event Action<int, int> OnUpdateExperience; 
        event Action<int> OnUpdateLevel; 


        int Level { get; }
        
        int ExperienceMax { get; }
        
        int Experience { get; set; }
    }
}