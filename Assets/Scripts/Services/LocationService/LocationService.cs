using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.LocationService
{
    public class LocationService : ILocationService
    {
        private ZenjectSceneLoader _zenjectSceneLoader;
        
        private List<LocationName> _loadedLocation = new List<LocationName>();
        private LocationName _lastLoadedLocation;


        public LocationService(ZenjectSceneLoader zenjectSceneLoader)
        {
            _zenjectSceneLoader = zenjectSceneLoader;
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
            _zenjectSceneLoader.LoadScene(_lastLoadedLocation.ToString(), LoadSceneMode.Additive);
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
            
            _loadedLocation.Remove(locationName);
            if (_loadedLocation.Count > 0)
                _lastLoadedLocation = _loadedLocation[^1];

            SceneManager.UnloadSceneAsync(locationName.ToString());
        }

        public void ChangeLoadedScene(LocationName newLocationName)
        {
            UnloadLastLoadedLocation();
            LoadLocation(newLocationName);
        }
    }
}