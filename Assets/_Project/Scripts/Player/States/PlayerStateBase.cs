using EElemental.Core;

namespace EElemental.Player.States
{
    /// <summary>
    /// Base class for all player states.
    /// </summary>
    public abstract class PlayerStateBase : IState
    {
        protected PlayerStateMachine StateMachine;
        protected PlayerController Controller => StateMachine.Controller;
        protected PlayerMovement Movement => StateMachine.Movement;
        protected PlayerStats Stats => StateMachine.Stats;
        
        protected float StateTimer;
        
        public PlayerStateBase(PlayerStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public virtual void Enter()
        {
            StateTimer = 0f;
        }
        
        public virtual void Execute()
        {
            StateTimer += UnityEngine.Time.deltaTime;
            CheckCommonTransitions();
        }
        
        public virtual void FixedExecute() { }
        
        public virtual void Exit() { }
        
        /// <summary>
        /// Check for transitions that can happen from any state.
        /// </summary>
        protected virtual void CheckCommonTransitions()
        {
            // Dash can interrupt most states
            if (Controller.DashPressed && Movement.CanDash)
            {
                StateMachine.TransitionTo(StateMachine.DashState);
            }
        }
        
        /// <summary>
        /// Check for attack inputs.
        /// </summary>
        protected bool CheckAttackTransition()
        {
            if (Controller.LightAttackPressed || Controller.HeavyAttackPressed)
            {
                StateMachine.TransitionTo(StateMachine.AttackState);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Check for jump input.
        /// </summary>
        protected bool CheckJumpTransition()
        {
            if (Controller.JumpPressed && Movement.CanJump())
            {
                StateMachine.TransitionTo(StateMachine.JumpState);
                return true;
            }
            return false;
        }
    }
}
