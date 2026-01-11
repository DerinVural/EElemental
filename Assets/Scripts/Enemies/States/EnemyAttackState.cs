using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Attack state - Saldırı animasyonu ve hasar verme
    /// </summary>
    public class EnemyAttackState : EnemyStateBase
    {
        private float attackDuration = 0.5f;
        private bool hasDealtDamage = false;
        private float damageFrame = 0.3f; // Hasarın verildiği frame
        
        public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            
            hasDealtDamage = false;
            
            // Hareketi durdur
            enemy.Stop();
            
            // Hedefe dön
            FaceTarget();
            
            // Saldırı animasyonu
            SetAnimatorTrigger("Attack");
            ai.ChangeState(AIState.Attack);
            
            // Saldırı cooldown'ını kaydet
            stats.RecordAttack();
        }
        
        public override void Update()
        {
            base.Update();
            
            // Hasar frame'inde hasar ver
            if (!hasDealtDamage && stateTimer >= damageFrame)
            {
                DealDamage();
                hasDealtDamage = true;
            }
        }
        
        private void FaceTarget()
        {
            if (!ai.HasTarget) return;
            
            Vector2 direction = enemy.GetDirectionToTarget();
            
            // Sağa bakıyoruz ama hedef solda (veya tersi)
            if ((direction.x > 0 && !enemy.IsFacingRight) || 
                (direction.x < 0 && enemy.IsFacingRight))
            {
                enemy.Flip();
            }
        }
        
        private void DealDamage()
        {
            if (!ai.HasTarget) return;
            
            float distance = enemy.GetDistanceToTarget();
            
            // Hala menzildeyse hasar ver
            if (distance <= stats.AttackRange * 1.2f)
            {
                // Hedefin IDamageable interface'ini bul
                var damageable = ai.CurrentTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = enemy.GetDirectionToTarget();
                    damageable.TakeDamage(stats.GetModifiedAttackDamage(), knockbackDir);
                }
            }
        }
        
        protected override void CheckTransitions()
        {
            // Saldırı animasyonu bitti
            if (stateTimer >= attackDuration)
            {
                // Hedef hala varsa ve menzildeyse tekrar saldır
                if (ai.HasTarget)
                {
                    float distance = enemy.GetDistanceToTarget();
                    
                    if (distance <= stats.AttackRange && stats.CanAttack)
                    {
                        // Yeni saldırı state'i
                        stateMachine.ChangeState(new EnemyAttackState(stateMachine));
                    }
                    else
                    {
                        // Chase'e dön
                        stateMachine.ChangeState(new EnemyChaseState(stateMachine));
                    }
                }
                else
                {
                    // Hedef kayboldu
                    stateMachine.ChangeState(new EnemyIdleState(stateMachine));
                }
            }
        }
    }
    
    /// <summary>
    /// Hasar alabilir nesneler için interface
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector2 knockbackDirection = default);
        bool IsAlive { get; }
    }
}
