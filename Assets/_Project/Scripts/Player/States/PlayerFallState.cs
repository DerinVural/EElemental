namespace EElemental.Player.States
{
    /// <summary>
    /// Fall state - player descending.
    /// </summary>
    public class PlayerFallState : PlayerStateBase
    {
        public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StateMachine.SetAnimatorBool("IsFalling", true);
            StateMachine.SetAnimatorBool("IsGrounded", false);
        }
        
        public override void Exit()
        {
            base.Exit();
            StateMachine.SetAnimatorBool("IsFalling", false);
        }
        
        public override void Execute()
        {
            base.Execute();
            
            // Check for attack (air attack)
            if (CheckAttackTransition()) return;
            
            // Land when grounded
            if (Movement.IsGrounded)
            {
                // Transition based on input
                if (UnityEngine.Mathf.Abs(Controller.MoveInput.x) > 0.1f)
                {
                    StateMachine.TransitionTo(StateMachine.RunState);
                }
                else
                {
                    StateMachine.TransitionTo(StateMachine.IdleState);
                }
                
                StateMachine.SetAnimatorTrigger("Land");
                return;
            }
        }
        
        public override void FixedExecute()
        {
            // Allow air control
            Movement.Move(Controller.MoveInput.x);
        }
    }
}
