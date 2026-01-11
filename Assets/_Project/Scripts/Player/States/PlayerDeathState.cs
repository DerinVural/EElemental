using UnityEngine;

namespace EElemental.Player.States
{
    /// <summary>
    /// Death state - player is dead.
    /// </summary>
    public class PlayerDeathState : PlayerStateBase
    {
        public PlayerDeathState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        
        public override void Enter()
        {
            base.Enter();
            StateMachine.SetAnimatorTrigger("Death");
            
            // Stop all movement
            Movement.SetVelocity(Vector2.zero);
            
            // Disable player input/control
            if (StateMachine.Controller != null)
            {
                StateMachine.Controller.enabled = false;
            }
            
            // Notify game manager
            Core.GameManager.Instance?.EndRun(false);
        }
        
        public override void Execute()
        {
            // Death state does nothing - wait for respawn/restart
        }
        
        public override void FixedExecute()
        {
            // No physics updates in death state
        }
        
        protected override void CheckCommonTransitions()
        {
            // Death cannot be interrupted
        }
    }
}
