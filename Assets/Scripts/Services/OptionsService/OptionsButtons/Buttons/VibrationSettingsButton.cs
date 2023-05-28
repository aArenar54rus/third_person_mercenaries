using Arenar.AudioSystem;
using TakeTop.Options;
using Zenject;


namespace TakeTop.UI
{
    public class VibrationSettingsButton : SettingsButton
    {
        private VibrationOption vibrationOption;


        [Inject]
        private void Initialize(IUiSoundManager uiSoundManager,
            IOptionsController optionsController)
        {
            this.uiSoundManager = uiSoundManager;
            this.optionsController = optionsController;

            Initialize();
        }

		protected override void OptionsButtonClick()
        {
            VibrationOption vibrationOption = optionsController.GetOption<VibrationOption>();
            vibrationOption.isActive = !vibrationOption.IsActive;

            optionsController.SetOption(vibrationOption);

            SetIcons(vibrationOption.isActive);
        }

		protected override void SetButtonStatus()
		{
			SetIcons(optionsController.GetOption<VibrationOption>().isActive);
		}
	}
}
