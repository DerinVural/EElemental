using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Hurt state - Hasar alındığında
    /// </summary>
    public class EnemyHurtState : EnemyStateBase
    {
        private float hurtDuration = 0.3f;
        
        public EnemyHurtState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            
            // Hareketi durdur
            enemy.Stop();
            
            // Hurt animasyonu
            SetAnimatorTrigger("Hurt");
        }
        
        protected override void CheckTransitions()
        {
            // Hurt süresi doldu
            if (stateTimer >= hurtDuration)
            {
                // Ölmüşsek Death state'e geç
                if (!enemy.IsAlive)
                {
                    stateMachine.ChangeState(new EnemyDeathState(stateMachine));
                    return;
                }
                
                // Hedef varsa chase'e dön
                if (ai.HasTarget)
                {
                    stateMachine.ChangeState(new EnemyChaseState(stateMachine));
                }
                else
                {
                    stateMachine.ChangeState(new EnemyIdleState(stateMachine));
                }
            }
        }
    }
}
