using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class AiStateMachineController : IFixedTickable
    {
	    private ICharacterEntity characterEntity;
	    private TickableManager tickableManager;

	    private ICharacterLiveComponent characterLiveComponent;
	    
	    private Dictionary<Type, IAIState> aiInitedStates;
	    private IAIState currentState;
	    private IAIState stateToSwitch;

	    private bool isControlBlocked;

	    private Task asyncUpdateTask;


		protected Vector3 MoveDirection { get; set; }

		protected Vector3 RotationDirection { get; set; }
		
		

		internal AiStateMachineController(
				ICharacterEntity characterEntity,
				TickableManager tickableManager,
				IAIState[] aiStates)
		{
			this.characterEntity = characterEntity;
			
			this.tickableManager = tickableManager;
			tickableManager.AddFixed(this);
			
			if (aiInitedStates == null || aiInitedStates.Count >= 0)
			{
				aiInitedStates = new Dictionary<Type, IAIState>();
				foreach (var state in aiStates)
				{
					state.SetupAiStateMachineBehaviour(this);
					aiInitedStates.Add(state.GetType(), state);
				}
			}
			
			currentState = aiStates[0];
			currentState.OnStateBegin();

			characterEntity.TryGetCharacterComponent(out characterLiveComponent);
		}


		public void FixedTick() =>
			HandleBaseLogic();

		public void Initialize()
		{
			foreach (var state in aiInitedStates.Values)
				state?.Initialize(characterEntity);
		}

		public void DeInitialize()
		{
			foreach (IAIState state in aiInitedStates.Values)
				state?.DeInitialize();

			asyncUpdateTask.Wait();
			asyncUpdateTask.Dispose();
			tickableManager.RemoveFixed(this);
		}

		public void AiBlockStatus(bool status) =>
			isControlBlocked = status;
		
		private void HandleBaseLogic()
		{
			if ((characterLiveComponent != null && !characterLiveComponent.IsAlive)
			    || isControlBlocked)
			{
				MoveDirection = Vector3.zero;
				return;
			}

			HandleCurrentStateUpdate();

			MoveDirection = currentState.MoveDirection;
			RotationDirection = currentState.RotationDirection;

			HandleAsyncStateSwitch();
		}

		public void SwitchState<T>() where T : IAIState
		{
			IAIState state = aiInitedStates[typeof(T)];
			SwitchState(state);
		}

		public void SwitchStateAsync<T>() where T : IAIState
		{
			stateToSwitch = aiInitedStates[typeof(T)];
		}

		public T GetStateInstance<T>() where T : IAIState
		{
			return (T)aiInitedStates[typeof(T)];
		}

		private void SwitchState(IAIState state)
		{
			currentState.OnStateEnd();
			currentState = state;
			currentState.OnStateBegin();
		}

		private void HandleAsyncStateSwitch()
		{
			if (stateToSwitch == null)
				return;

			SwitchState(stateToSwitch);
			stateToSwitch = null;
		}

		private void HandleCurrentStateUpdate()
		{
			asyncUpdateTask = Task.Factory.StartNew(CurrentStateAsyncUpdate,
				CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

			currentState.OnStateSyncUpdate();


			void CurrentStateAsyncUpdate() =>
				currentState.OnStateAsyncUpdate();
		}
    }
}