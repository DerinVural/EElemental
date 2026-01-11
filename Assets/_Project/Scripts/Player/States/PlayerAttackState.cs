using UnityEngine;
using EElemental.Combat;

namespace EElemental.Player.States
{
    /// <summary>
    /// Attack state - executing attacks and combos.
    /// </summary>
    public class PlayerAttackState : PlayerStateBase
    {
        private AttackData _currentAttack;
        private int _currentFrame;
        private bool _hitboxActivated;
        private bool _canCancel;
        
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            _currentFrame = 0;
            _hitboxActivated = false;
            _canCancel = false;
            
            // Determine attack type from input
            AttackInput attackInput = Controller.LightAttackPressed ? AttackInput.Light : AttackInput.Heavy;
            
            // Register with combo handler to get next attack
            if (StateMachine.ComboHandler != null)
            {
                StateMachine.ComboHandler.RegisterInput(attackInput);
                _currentAttack = StateMachine.ComboHandler.GetCurrentAttack();
            }
            
            if (_currentAttack != null)
            {
                StateMachine.SetAnimatorTrigger(_currentAttack.animationTrigger);
                
                // Stop horizontal movement during attack
                Movement.StopHorizontalMovement();
            }
            else
            {
                // No attack data, return to idle
                StateMachine.TransitionTo(StateMachine.IdleState);
            }
        }
        
        public override void Execute()
        {
            // Don't call base - custom transition logic
            StateTimer += Time.deltaTime;
            _currentFrame++;
            
            if (_currentAttack == null)
            {
                StateMachine.TransitionTo(StateMachine.IdleState);
                return;
            }
            
            // Startup frames - vulnerable, no hitbox
            if (_currentFrame < _currentAttack.startupFrames)
            {
                return;
            }
            
            // Active frames - hitbox on
            int activeEnd = _currentAttack.startupFrames + _currentAttack.activeFrames;
            if (_currentFrame < activeEnd)
            {
                if (!_hitboxActivated)
                {
                    ActivateHitbox();
                }
                return;
            }
            else if (_hitboxActivated)
            {
                DeactivateHitbox();
            }
            
            // Recovery frames - can cancel after window
            if (_currentFrame >= _currentAttack.cancelWindowStart)
            {
                _canCancel = true;
                HandleCancelWindow();
            }
            
            // Attack complete
            int totalFrames = _currentAttack.startupFrames + _currentAttack.activeFrames + _currentAttack.recoveryFrames;
            if (_currentFrame >= totalFrames)
            {
                EndAttack();
            }
        }
        
        public override void FixedExecute()
        {
            // Slight forward momentum during attack
            if (_currentAttack != null && _currentFrame < _currentAttack.startupFrames + _currentAttack.activeFrames)
            {
                Movement.Move(Movement.FacingDirection * 0.3f);
            }
        }
        
        public override void Exit()
        {
            base.Exit();
            DeactivateHitbox();
        }
        
        private void ActivateHitbox()
        {
            _hitboxActivated = true;
            if (StateMachine.HitboxManager != null && _currentAttack != null)
            {
                StateMachine.HitboxManager.ActivateHitbox(_currentAttack);
            }
        }
        
        private void DeactivateHitbox()
        {
            _hitboxActivated = false;
            if (StateMachine.HitboxManager != null)
            {
                StateMachine.HitboxManager.DeactivateHitbox();
            }
        }
        
        private void HandleCancelWindow()
        {
            // Check for buffered inputs
            if (StateMachine.InputBuffer != null)
            {
                var bufferedInput = StateMachine.InputBuffer.ConsumeBuffer();
                
                if (bufferedInput.HasValue)
                {
                    if ((bufferedInput.Value == AttackInput.Light && _currentAttack.canCancelIntoLight) ||
                        (bufferedInput.Value == AttackInput.Heavy && _currentAttack.canCancelIntoHeavy))
                    {
                        // Continue combo - re-enter attack state
                        StateMachine.TransitionTo(StateMachine.AttackState);
                        return;
                    }
                }
            }
            
            // Check for dash cancel
            if (Controller.DashPressed && _currentAttack.canCancelIntoDash && Movement.CanDash)
            {
                StateMachine.TransitionTo(StateMachine.DashState);
            }
        }
        
        private void EndAttack()
        {
            // Reset combo if no input
            if (StateMachine.ComboHandler != null)
            {
                StateMachine.ComboHandler.CheckComboTimeout();
            }
            
            // Transition based on state
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
            // Attacks can only be cancelled during cancel window
        }
    }
}
