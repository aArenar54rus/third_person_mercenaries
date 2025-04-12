using Arenar.AudioSystem;
using Arenar.Character;
using Arenar.Services.PlayerInputService;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Services.UI
{
	public class UpgradeCharacterWindowController : CanvasWindowController
	{
		private UpgradeCharacterWindow upgradeCharacterWindow;
		
		private IPlayerInputService playerInputService;
		private PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService;
		private IUiSoundManager uiSoundManager;

		private UpgradeCharacterWindowLayer upgradeCharacterWindowLayer;
		
		private Dictionary<CharacterSkillUpgradeType, UpgradeParameterPanelVisual> upgradeParameterPanelVisuals;
		private Dictionary<CharacterSkillUpgradeType, int> addedScores;
		

		public UpgradeCharacterWindowController(IPlayerInputService playerInputService,
												PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService,
												IUiSoundManager uiSoundManager)
			: base(playerInputService)
		{
			this.playerInputService = playerInputService;
			this.playerCharacterSkillUpgradeService = playerCharacterSkillUpgradeService;
			this.uiSoundManager = uiSoundManager;
		}


		public override void Initialize(ICanvasService canvasService)
		{
			base.Initialize(canvasService);

			upgradeCharacterWindowLayer = canvasService.GetWindow<UpgradeCharacterWindow>()
				.GetWindowLayer<UpgradeCharacterWindowLayer>();

			var values = Enum.GetValues(typeof(CharacterSkillUpgradeType));
			
			upgradeParameterPanelVisuals = new(values.Length - 1);
			addedScores = new(values.Length - 1);
			
			foreach (CharacterSkillUpgradeType parameter in values)
			{
				if (parameter == CharacterSkillUpgradeType.None)
					continue;
				
				var skillVisual = GameObject.Instantiate(upgradeCharacterWindowLayer.UpgradeParameterPanelVisualPrefab,
					upgradeCharacterWindowLayer.UpgradePanelsContainer).GetComponent<UpgradeParameterPanelVisual>();

				skillVisual.UpgradeButton.onClick.AddListener(() => MakeSkillUpgradeButtonHandler(parameter));
				skillVisual.InitializeUpgradeParameter(parameter);
				
				upgradeParameterPanelVisuals.Add(parameter, skillVisual);
				addedScores.Add(parameter, 0);
			}
			
			upgradeCharacterWindowLayer.ReturnUpgradeButton.onClick.AddListener(ReturnUpgradeScoresButtonHandler);
			upgradeCharacterWindowLayer.CloseButton.onClick.AddListener(CloseUpgradeMenuButtonHandler);
		}
		
		private void CloseUpgradeMenuButtonHandler()
		{
			uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
			SetButtonsStatus(false);
			AcceptUpgrades();
			
			canvasService.TransitionController
				.PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
						UpgradeCharacterWindow,
						GameplayCanvasWindow>
					(false, false, null);
		}

		protected override void OnWindowShowEnd_SelectElements()
		{
			playerInputService.SetInputControlType(InputActionMapType.UI, true);
			playerInputService.SetInputControlType(InputActionMapType.Gameplay, false);
			Time.timeScale = 0.0f;
			SetButtonsStatus(true);
		}
		
		protected override void OnWindowHideBegin_DeselectElements()
		{
			playerInputService.SetInputControlType(InputActionMapType.UI, false);
			playerInputService.SetInputControlType(InputActionMapType.Gameplay, true);
			Time.timeScale = 1.0f;
			SetButtonsStatus(false);
		}
		
		private void AcceptUpgrades()
		{
			foreach (var addedScoresKey in addedScores.Keys)
			{
				int scoresToAdd = addedScores[addedScoresKey];
				while (scoresToAdd > 0)
				{
					playerCharacterSkillUpgradeService.TryUpgradeSkill(addedScoresKey);
					scoresToAdd--;
				}
				
				addedScores[addedScoresKey] = 0;
				UpdateSkillVisual(addedScoresKey);
			}
		}
		
		private void SetButtonsStatus(bool status)
		{
			if (status)
			{
				CheckUpgradeButtonStatus();
			}
			else
			{
				foreach (var visual in upgradeParameterPanelVisuals.Values)
					visual.SetButtonInteractable(false);
			}

			upgradeCharacterWindowLayer.ReturnUpgradeButton.interactable = status;
			upgradeCharacterWindowLayer.CloseButton.interactable = status;
		}
		
		private void CheckUpgradeButtonStatus()
		{
			foreach (var visual in upgradeParameterPanelVisuals)
			{
				visual.Value.SetButtonInteractable(
					playerCharacterSkillUpgradeService.CanUpgradeSkill(visual.Key));
			}
		}
		
		private void MakeSkillUpgradeButtonHandler(CharacterSkillUpgradeType upgradeType)
		{
			if (playerCharacterSkillUpgradeService.UpgradeScore <= 0)
				return;
			
			uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);

			addedScores[upgradeType]++;
			playerCharacterSkillUpgradeService.UpgradeScore--;
			
			UpdateSkillVisual(upgradeType);
			CheckUpgradeButtonStatus();
		}
		
		private void ReturnUpgradeScoresButtonHandler()
		{
			uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
			
			int returnedScores = 0;

			foreach (var addedScoresKey in addedScores.Keys)
			{
				returnedScores += addedScores[addedScoresKey];
				addedScores[addedScoresKey] = 0;
				UpdateSkillVisual(addedScoresKey);
			}

			playerCharacterSkillUpgradeService.UpgradeScore += returnedScores;
			CheckUpgradeButtonStatus();
		}
		
		private void UpdateSkillVisual(CharacterSkillUpgradeType upgradeType)
		{
			int levelWithUpgrades = GetSkillLevelWithAddedUpgrades(upgradeType);

			upgradeParameterPanelVisuals[upgradeType].SetUpgradeProgress(
				levelWithUpgrades,
				playerCharacterSkillUpgradeService.CanUpgradeSkill(upgradeType, levelWithUpgrades),
				addedScores[upgradeType] > 0,
				playerCharacterSkillUpgradeService.GetUpgradeSkillValueByLevel(upgradeType, levelWithUpgrades));
		}

		private int GetSkillLevelWithAddedUpgrades(CharacterSkillUpgradeType upgradeType)
		{
			return playerCharacterSkillUpgradeService.GetUpgradeSkillLevel(upgradeType)
				+ addedScores[upgradeType];
		}
	}
}