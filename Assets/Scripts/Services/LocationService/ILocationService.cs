namespace Arenar.LocationService
{
    public interface ILocationService
    {
        void LoadLocation(LocationName locationName);
        
        void UnloadLastLoadedLocation();
        
        void UnloadLocation(LocationName locationName);
    }
}