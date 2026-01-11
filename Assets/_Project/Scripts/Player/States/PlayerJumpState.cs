namespace EElemental.Player.States
{
    /// <summary>
    /// Jump state - player ascending.
    /// </summary>
    public class PlayerJumpState : PlayerStateBase
    {
        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            Movement.Jump();
            StateMachine.SetAnimatorTrigger("Jump");
            StateMachine.SetAnimatorBool("IsGrounded", false);
        }
        
        public override void Execute()
        {
            base.Execute();
            
            // Check for attack
            if (CheckAttackTransition()) return;
            
            // Variable jump height - cut jump if button released
            if (!Controller.JumpHeld)
            {
                Movement.CutJump();
            }
            
            // Transition to fall when starting to descend
            if (Movement.IsFalling)
            {
                StateMachine.TransitionTo(StateMachine.FallState);
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
