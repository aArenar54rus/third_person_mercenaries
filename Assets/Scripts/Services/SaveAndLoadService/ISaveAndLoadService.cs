namespace Arenar.Services.SaveAndLoad
{
    public interface ISaveAndLoadService
    {
        bool IsInitialize { get; }
    }

    
    public interface ISaveAndLoadService<T> : ISaveAndLoadService
        where T : SaveDelegate
    {
        T GetSaveData();

        void MakeSave(T saveData);
    }
}