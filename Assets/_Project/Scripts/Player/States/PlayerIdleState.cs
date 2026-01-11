namespace EElemental.Player.States
{
    /// <summary>
    /// Idle state - player standing still.
    /// </summary>
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StateMachine.SetAnimatorBool("IsRunning", false);
            StateMachine.SetAnimatorBool("IsGrounded", true);
        }
        
        public override void Execute()
        {
            base.Execute();
            
            // Check for transitions
            if (CheckJumpTransition()) return;
            if (CheckAttackTransition()) return;
            
            // Transition to run if moving
            if (UnityEngine.Mathf.Abs(Controller.MoveInput.x) > 0.1f)
            {
                StateMachine.TransitionTo(StateMachine.RunState);
                return;
            }
            
            // Transition to fall if not grounded
            if (!Movement.IsGrounded)
            {
                StateMachine.TransitionTo(StateMachine.FallState);
                return;
            }
        }
        
        public override void FixedExecute()
        {
            // Apply slight deceleration
            Movement.Move(0);
        }
    }
}
