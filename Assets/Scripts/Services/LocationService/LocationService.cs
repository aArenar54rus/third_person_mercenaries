using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.LocationService
{
    public class LocationService : ILocationService, ITickable
    {
        private ZenjectSceneLoader _zenjectSceneLoader;
        
        private List<LocationName> _loadedLocation = new List<LocationName>();
        private LocationName _lastLoadedLocation;

        private LocationName _needToUnloadLocationName;


        public LocationService(ZenjectSceneLoader zenjectSceneLoader,
            TickableManager tickableManager)
        {
            _zenjectSceneLoader = zenjectSceneLoader;
            tickableManager.Add(this);
        }
        
        
        public void LoadLocation(LocationName locationName)
        {
            if (_loadedLocation.Contains(locationName))
            {
                Debug.LogError($"Location {locationName} load early!");
                return;
            }

            _loadedLocation.Add(locationName);
            _lastLoadedLocation = locationName;
            SceneManager.LoadScene(_lastLoadedLocation.ToString(), LoadSceneMode.Additive);
            //_zenjectSceneLoader.LoadScene(_lastLoadedLocation.ToString(), LoadSceneMode.Additive);
        }

        public void UnloadLastLoadedLocation()
        {
            UnloadLocation(_lastLoadedLocation);
        }

        public void UnloadLocation(LocationName locationName)
        {
            if (!_loadedLocation.Contains(locationName))
            {
                Debug.LogError($"Location {locationName} not loaded!");
                return;
            }
            
            _needToUnloadLocationName = locationName;
        }

        public void ChangeLoadedScene(LocationName newLocationName)
        {
            UnloadLastLoadedLocation();
            LoadLocation(newLocationName);
        }

        public void Tick()
        {
            if (_needToUnloadLocationName == LocationName.None)
                return;
            
            _loadedLocation.Remove(_needToUnloadLocationName);
            if (_loadedLocation.Count > 0)
                _lastLoadedLocation = _loadedLocation[^1];

            SceneManager.UnloadScene(_needToUnloadLocationName.ToString());
            _needToUnloadLocationName = LocationName.None;
        }
    }
}