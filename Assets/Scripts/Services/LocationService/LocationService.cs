using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.LocationService
{
    public class LocationService : ILocationService, ITickable
    {
        private ZenjectSceneLoader _zenjectSceneLoader;
        
        private List<LocationName> _loadedLocation = new();
        private LocationName _lastLoadedLocation;

        private LocationName _needToUnloadLocationName;
        private ILocationService locationServiceImplementation;


        public LocationService(ZenjectSceneLoader zenjectSceneLoader,
                               TickableManager tickableManager)
        {
            _zenjectSceneLoader = zenjectSceneLoader;
            tickableManager.Add(this);
        }
        
        
        public void LoadLocation(LocationName locationName, Action<DiContainer> onComplete)
        {
            LoadLocationAsync(locationName, onComplete);
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

        public void ChangeLoadedScene(LocationName newLocationName, Action<DiContainer> onComplete = null)
        {
            UnloadLastLoadedLocation();
            LoadLocationAsync(newLocationName, onComplete);
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
        
        private void LoadLocationAsync(LocationName locationName, Action<DiContainer> onComplete)
        {
            if (_loadedLocation.Contains(locationName))
            {
                Debug.LogError($"Location {locationName} load early!");
                return;
            }
            
            _loadedLocation.Add(locationName);
            _lastLoadedLocation = locationName;
            
            _zenjectSceneLoader.LoadSceneAsync(
                    _lastLoadedLocation.ToString(), LoadSceneMode.Additive)
                .completed += _ =>
            {
                var loadedScene = SceneManager.GetSceneByName(locationName.ToString());
                if (!loadedScene.IsValid() || !loadedScene.isLoaded)
                {
                    Debug.LogError($"Scene {locationName} did not load correctly!");
                    onComplete?.Invoke(null);
                    return;
                }

                SceneContext sceneContext = null;
                foreach (var rootObject in loadedScene.GetRootGameObjects())
                {
                    sceneContext = rootObject.GetComponentInChildren<SceneContext>();
                    if (sceneContext != null)
                        break;
                }
                
                onComplete?.Invoke(sceneContext.Container);
            };
        }
    }
}