namespace Arenar
{
    public interface IClipComponent : IEquippedItemComponent
    {
        int ClipSizeMax { get; }
        
        int ClipSize { get; set; }
        
        bool IsReloadProcess { get; }
        
        bool IsInfinityClip { get; }
        
        
        void Reload();
    }
}