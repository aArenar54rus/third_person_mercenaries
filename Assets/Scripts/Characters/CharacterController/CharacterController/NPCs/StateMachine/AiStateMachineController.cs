using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Arenar.Character
{
    public class AiStateMachineController
    {
	    private ICharacterEntity characterEntity;

	    private ICharacterLiveComponent characterLiveComponent;
	    
	    private Dictionary<Type, IAIState> aiInitedStates;
	    private IAIState currentState;
	    private IAIState stateToSwitch;

	    private Task asyncUpdateTask;

	    protected Vector3 MoveDirection { get; set; }

		protected Vector3 RotationDirection { get; set; }
		
		
		internal AiStateMachineController(
				ICharacterEntity characterEntity,
				IAIState[] aiStates)
		{
			this.characterEntity = characterEntity;

			aiInitedStates = new Dictionary<Type, IAIState>();
			foreach (var state in aiStates)
			{
				state.SetupAiStateMachineBehaviour(this);
				aiInitedStates.Add(state.GetType(), state);
			}

			characterEntity.TryGetCharacterComponent(out characterLiveComponent);
		}


		public void OnFixedTick() =>
			HandleBaseLogic();

		public void Initialize()
		{
			foreach (var state in aiInitedStates.Values)
				state?.Initialize(characterEntity);
		}

		public void OnStart()
		{
			if (currentState == null)
			{
				currentState = aiInitedStates.First().Value;
				currentState.OnStateBegin();
			}
		}

		public void DeInitialize()
		{
			foreach (IAIState state in aiInitedStates.Values)
				state?.DeInitialize();

			asyncUpdateTask.Wait();
			asyncUpdateTask.Dispose();
		}

		private void HandleBaseLogic()
		{
			if ((characterLiveComponent != null && !characterLiveComponent.IsAlive))
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