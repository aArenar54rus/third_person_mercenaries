namespace Arenar.Services.SaveAndLoad
{
    public class SaveAndLoadService : ISaveAndLoadService<SaveDelegate>
    {
        public bool IsInitialize { get; private set; } = false;

        
        public SaveDelegate GetSaveData()
        {
            throw new System.NotImplementedException();
        }

        public void MakeSave(SaveDelegate saveData)
        {
            throw new System.NotImplementedException();
        }
    }
}