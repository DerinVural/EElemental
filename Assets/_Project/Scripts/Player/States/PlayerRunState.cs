namespace EElemental.Player.States
{
    /// <summary>
    /// Run state - player moving horizontally on ground.
    /// </summary>
    public class PlayerRunState : PlayerStateBase
    {
        public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StateMachine.SetAnimatorBool("IsRunning", true);
            StateMachine.SetAnimatorBool("IsGrounded", true);
        }
        
        public override void Exit()
        {
            base.Exit();
            StateMachine.SetAnimatorBool("IsRunning", false);
        }
        
        public override void Execute()
        {
            base.Execute();
            
            // Check for transitions
            if (CheckJumpTransition()) return;
            if (CheckAttackTransition()) return;
            
            // Transition to idle if not moving
            if (UnityEngine.Mathf.Abs(Controller.MoveInput.x) < 0.1f)
            {
                StateMachine.TransitionTo(StateMachine.IdleState);
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
            Movement.Move(Controller.MoveInput.x);
        }
    }
}
