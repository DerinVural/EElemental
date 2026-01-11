using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Patrol state - Belirlenen noktalar arasında devriye
    /// </summary>
    public class EnemyPatrolState : EnemyStateBase
    {
        private float patrolSpeed = 1.5f;
        private float waypointReachThreshold = 0.5f;
        
        public EnemyPatrolState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            SetAnimatorBool("IsMoving", true);
            ai.ChangeState(AIState.Patrol);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Patrol hedefine doğru hareket
            Vector2 direction = ai.GetMoveDirection();
            enemy.Move(direction.x * patrolSpeed / stats.MoveSpeed);
        }
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            // Duvar veya uçurum kontrolü
            if (enemy.IsWallAhead() || enemy.IsEdgeAhead())
            {
                enemy.Flip();
            }
        }
        
        protected override void CheckTransitions()
        {
            // Hedef tespit edildi - Chase'e geç
            if (ai.HasTarget)
            {
                stateMachine.ChangeState(new EnemyChaseState(stateMachine));
                return;
            }
            
            // Patrol noktasına ulaştık - Idle'a geç
            float distanceToTarget = Vector2.Distance(
                enemy.transform.position, 
                ai.PatrolTarget
            );
            
            if (distanceToTarget < waypointReachThreshold)
            {
                stateMachine.ChangeState(new EnemyIdleState(stateMachine));
            }
        }
    }
}
