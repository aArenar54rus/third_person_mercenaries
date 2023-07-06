using Arenar.Services.UI;


namespace Arenar.UI
{
    public class TransitionCanvasWindow : CanvasWindow, ITransitionWindow
    {
        public override bool OverrideSorting => true;
    }
}
