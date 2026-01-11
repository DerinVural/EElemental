using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Idle state - Beklemede, çevreyi gözlüyor
    /// </summary>
    public class EnemyIdleState : EnemyStateBase
    {
        private float idleDuration = 2f;
        
        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            enemy.Stop();
            SetAnimatorBool("IsMoving", false);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Yavaşça çevreye bak (opsiyonel animasyon)
        }
        
        protected override void CheckTransitions()
        {
            // Hedef tespit edildi - Chase'e geç
            if (ai.HasTarget)
            {
                stateMachine.ChangeState(new EnemyChaseState(stateMachine));
                return;
            }
            
            // Bekleme süresi doldu - Patrol'a geç
            if (stateTimer >= idleDuration)
            {
                stateMachine.ChangeState(new EnemyPatrolState(stateMachine));
            }
        }
    }
}
