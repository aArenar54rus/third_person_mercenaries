namespace Arenar.Character
{
    public abstract class NpcComponentCharacterController : ComponentCharacterController
    {
        protected AiStateMachineController AiStateMachineController;


        public void InitAiController(AiStateMachineController aiStateMachineController) =>
            this.AiStateMachineController = aiStateMachineController;
    }
}