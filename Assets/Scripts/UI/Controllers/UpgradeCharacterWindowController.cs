using Arenar.AudioSystem;
using Arenar.Character;
using Arenar.Services.LevelsService;
using Arenar.Services.PlayerInputService;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arenar.Services.UI
{
	public class UpgradeCharacterWindowController : CanvasWindowController
	{
		private UpgradeCharacterWindow upgradeCharacterWindow;
		private PlayerInput playerInputs;
		
		private ILevelsService levelsService;
		private PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService;
		private IUiSoundManager uiSoundManager;

		private UpgradeCharacterWindowLayer upgradeCharacterWindowLayer;
		
		private Dictionary<CharacterSkillUpgradeType, UpgradeParameterPanelVisual> upgradeParameterPanelVisuals;
		private Dictionary<CharacterSkillUpgradeType, int> addedScores;

		
		private bool IsClosed
		{
			get
			{
				if (canvasService.GetWindow<GameplayCanvasWindow>().gameObject.activeSelf
					&& !canvasService.GetWindow<UpgradeCharacterWindow>().gameObject.activeSelf)
					return true;

				return false;
			}
		}
		

		public UpgradeCharacterWindowController(IPlayerInputService playerInputService,
												ILevelsService levelsService,
												PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService,
												IUiSoundManager uiSoundManager)
			: base(playerInputService)
		{
			this.playerCharacterSkillUpgradeService = playerCharacterSkillUpgradeService;
			this.levelsService = levelsService;
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
				
				UpdateSkillVisual(parameter);
			}
			
			upgradeCharacterWindowLayer.ReturnUpgradeButton.onClick.AddListener(ReturnUpgradeScoresButtonHandler);
			upgradeCharacterWindowLayer.CloseButton.onClick.AddListener(CloseMenuHandler);
			upgradeCharacterWindowLayer.AcceptButton.onClick.AddListener(CloseUpgradeMenuButtonHandler);
			
			playerCharacterSkillUpgradeService.OnUpgradeScoreCountChange += UpdateScoreVisualHandler;
			UpdateScoreVisualHandler();

			playerInputs = (PlayerInput) playerInputService.InputActionCollection;
			if (playerInputs != null)
				playerInputs.Player.CharacterInformationMenu.canceled += InteractionMenuHandler;
			
			var gameplayPlayerParametersWindowLayer = canvasService.GetWindow<GameplayCanvasWindow>()
				.GetWindowLayer<GameplayPlayerParametersWindowLayer>();
			gameplayPlayerParametersWindowLayer.OpenUpgradeSkillsMenuButton.onClick.AddListener(() => InteractionMenuHandler(default));
		}
		
		private void UpdateScoreVisualHandler()
		{
			upgradeCharacterWindowLayer.ScoreText.text = playerCharacterSkillUpgradeService.UpgradeScore.ToString();
		}
		
		private void CloseUpgradeMenuButtonHandler()
		{
			AcceptUpgrades();
			CloseWindow();
		}
		
		private void CloseMenuHandler()
		{
			ReturnUpgradeScoresButtonHandler();
			CloseWindow();
		}
		
		private void CloseWindow()
		{
			SetButtonsStatus(false);
			uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
			playerInputService.SetInputControlType(InputActionMapType.UI, false);
			playerInputService.SetInputControlType(InputActionMapType.Gameplay, true);
			Time.timeScale = 1;
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
			SetButtonsStatus(true);/*
			
			if (playerInputService.InputActionCollection is PlayerInput playerInput)
				playerInput.UI.Decline.performed += OnInputAction_Decline;*/
		}
		
		protected override void OnWindowHideBegin_DeselectElements()
		{
			playerInputService.SetInputControlType(InputActionMapType.UI, false);
			playerInputService.SetInputControlType(InputActionMapType.Gameplay, true);
			Time.timeScale = 1.0f;
			SetButtonsStatus(false);/*
			
			if (playerInputService.InputActionCollection is PlayerInput playerInput)
				playerInput.UI.Decline.performed -= OnInputAction_Decline;*/
		}
		
		private void AcceptUpgrades()
		{
			var keys = new List<CharacterSkillUpgradeType>(addedScores.Keys);
			
			foreach (var addedScoresKey in keys)
			{
				int scoresToAdd = addedScores[addedScoresKey];
				playerCharacterSkillUpgradeService.UpgradeScore += scoresToAdd;
				if (scoresToAdd <= 0)
				{
					addedScores[addedScoresKey] = 0;
					continue;
				}

				while (scoresToAdd > 0)
				{
					playerCharacterSkillUpgradeService.TryUpgradeSkill(addedScoresKey);
					scoresToAdd--;
				}
				
				addedScores[addedScoresKey] = 0;
				UpdateSkillVisual(addedScoresKey);
			}
			
			CheckUpgradeButtonStatus();
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
			UpdateScoreVisualHandler();
			
			UpdateSkillVisual(upgradeType);
			CheckUpgradeButtonStatus();
		}
		
		private void ReturnUpgradeScoresButtonHandler()
		{
			uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
			
			int returnedScores = 0;

			var keys = new List<CharacterSkillUpgradeType>(addedScores.Keys);
			foreach (var addedScoresKey in keys)
			{
				returnedScores += addedScores[addedScoresKey];
				addedScores[addedScoresKey] = 0;
				UpdateSkillVisual(addedScoresKey);
			}

			if (returnedScores > 0)
			{
				playerCharacterSkillUpgradeService.UpgradeScore += returnedScores;
				UpdateScoreVisualHandler();
				CheckUpgradeButtonStatus();
			}
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
		
		private void InteractionMenuHandler(InputAction.CallbackContext context)
		{
			if (levelsService == null || levelsService.CurrentLevelContext == null || levelsService.CurrentLevelContext.GameMode != GameMode.Survival)
				return;
			
			if (IsClosed)
			{
				foreach (CharacterSkillUpgradeType parameter in addedScores.Keys)
				{
					UpdateSkillVisual(parameter);
				}
				
				uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
				
				playerInputService.SetInputControlType(InputActionMapType.UI, true);
				playerInputService.SetInputControlType(InputActionMapType.Gameplay, false);
				Time.timeScale = 0.0f;
				
				canvasService.TransitionController
					.PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
							GameplayCanvasWindow,
							UpgradeCharacterWindow>
						(false, false, null);
				
				SetButtonsStatus(true);
			}
			else
			{
				CloseMenuHandler();
			}
		}
	}
}