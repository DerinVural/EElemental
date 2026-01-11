using UnityEngine;

namespace EElemental.Player.States
{
    /// <summary>
    /// Dash state - invincible dash movement.
    /// </summary>
    public class PlayerDashState : PlayerStateBase
    {
        private float _dashDuration = 0.15f;
        
        public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            
            // Get dash direction from input or facing direction
            Vector2 dashDir = Controller.MoveInput;
            if (dashDir.magnitude < 0.1f)
            {
                dashDir = new Vector2(Movement.FacingDirection, 0);
            }
            
            Movement.StartDash(dashDir);
            
            // Enable i-frames during dash
            if (StateMachine.IFrameController != null)
            {
                StateMachine.IFrameController.StartIFrames(_dashDuration);
            }
            
            StateMachine.SetAnimatorTrigger("Dash");
        }
        
        public override void Execute()
        {
            // Don't call base - dash cannot be interrupted by common transitions
            StateTimer += Time.deltaTime;
            
            // End dash when duration is over
            if (!Movement.IsDashing)
            {
                EndDash();
            }
        }
        
        public override void FixedExecute()
        {
            Movement.UpdateDash();
        }
        
        public override void Exit()
        {
            base.Exit();
            if (Movement.IsDashing)
            {
                Movement.EndDash();
            }
        }
        
        private void EndDash()
        {
            // Transition based on current state
            if (!Movement.IsGrounded)
            {
                StateMachine.TransitionTo(StateMachine.FallState);
            }
            else if (Mathf.Abs(Controller.MoveInput.x) > 0.1f)
            {
                StateMachine.TransitionTo(StateMachine.RunState);
            }
            else
            {
                StateMachine.TransitionTo(StateMachine.IdleState);
            }
        }
        
        protected override void CheckCommonTransitions()
        {
            // Dash cannot be interrupted
        }
    }
}
