using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Chase state - Hedefi takip et
    /// </summary>
    public class EnemyChaseState : EnemyStateBase
    {
        private float chaseSpeedMultiplier = 1.3f;
        private float maxChaseTime = 10f;
        
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            SetAnimatorBool("IsMoving", true);
            SetAnimatorBool("IsChasing", true);
            ai.ChangeState(AIState.Chase);
        }
        
        public override void Exit()
        {
            base.Exit();
            SetAnimatorBool("IsChasing", false);
        }
        
        public override void Update()
        {
            base.Update();
            
            if (!ai.HasTarget)
            {
                return;
            }
            
            // Hedefe doğru hareket
            Vector2 direction = enemy.GetDirectionToTarget();
            enemy.Move(direction.x * chaseSpeedMultiplier);
        }
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            // Duvar kontrolü - zıplama veya alternatif yol bulma
            if (enemy.IsWallAhead())
            {
                // TODO: Zıplama veya yol bulma mantığı
            }
        }
        
        protected override void CheckTransitions()
        {
            // Hedef kayboldu - Patrol'a dön
            if (!ai.HasTarget)
            {
                stateMachine.ChangeState(new EnemyPatrolState(stateMachine));
                return;
            }
            
            float distanceToTarget = enemy.GetDistanceToTarget();
            
            // Saldırı menzilindeyiz ve saldırabiliriz
            if (distanceToTarget <= stats.AttackRange && stats.CanAttack)
            {
                stateMachine.ChangeState(new EnemyAttackState(stateMachine));
                return;
            }
            
            // Hedef çok uzaklaştı
            if (distanceToTarget > stats.ChaseRange)
            {
                ai.ChangeState(AIState.Patrol);
                stateMachine.ChangeState(new EnemyPatrolState(stateMachine));
                return;
            }
            
            // Çok uzun süredir kovalıyoruz - vazgeç
            if (stateTimer >= maxChaseTime)
            {
                stateMachine.ChangeState(new EnemyPatrolState(stateMachine));
            }
        }
    }
}
