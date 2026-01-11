using UnityEngine;

namespace EElemental.Player.States
{
    /// <summary>
    /// Hurt state - player taking damage.
    /// </summary>
    public class PlayerHurtState : PlayerStateBase
    {
        private float _hurtDuration = 0.3f;
        
        public PlayerHurtState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StateMachine.SetAnimatorTrigger("Hurt");
            
            // Stop movement
            Movement.StopHorizontalMovement();
            
            // Start i-frames after hit
            if (StateMachine.IFrameController != null)
            {
                StateMachine.IFrameController.StartIFrames(_hurtDuration + 0.5f);
            }
        }
        
        public override void Execute()
        {
            // Don't call base - hurt cannot be interrupted
            StateTimer += Time.deltaTime;
            
            // Recover after hurt duration
            if (StateTimer >= _hurtDuration)
            {
                Recover();
            }
        }
        
        private void Recover()
        {
            if (!Movement.IsGrounded)
            {
                StateMachine.TransitionTo(StateMachine.FallState);
            }
            else
            {
                StateMachine.TransitionTo(StateMachine.IdleState);
            }
        }
        
        protected override void CheckCommonTransitions()
        {
            // Hurt cannot be interrupted
        }
    }
}
