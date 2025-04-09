using System;
using Zenject;


namespace Arenar.LocationService
{
    public interface ILocationService
    {
        void LoadLocation(LocationName locationName, Action<DiContainer> onComplete = null);
        
        void UnloadLastLoadedLocation();
        
        void UnloadLocation(LocationName locationName);

        void ChangeLoadedScene(LocationName newLocationName, Action<DiContainer> onComplete = null);
    }
}