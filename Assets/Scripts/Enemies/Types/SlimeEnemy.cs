using UnityEngine;
using EElemental.Enemies.States;

namespace EElemental.Enemies
{
    /// <summary>
    /// Slime - Temel düşman tipi
    /// Basit AI, yavaş hareket, yakın mesafe saldırı
    /// </summary>
    public class SlimeEnemy : EnemyBase
    {
        [Header("Slime Settings")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 2f;
        
        private float lastJumpTime;
        
        protected override void InitializeStateMachine()
        {
            stateMachine = new EnemyStateMachine(this, Stats, AI);
            stateMachine.Initialize(new EnemyIdleState(stateMachine));
        }
        
        public override void Attack()
        {
            // Slime saldırısı - kendini hedefe fırlat
            if (Target == null) return;
            
            Vector2 direction = GetDirectionToTarget();
            
            // Küçük bir sıçrama ile saldır
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(direction.x * jumpForce * 0.5f, jumpForce * 0.3f), ForceMode2D.Impulse);
            
            Stats.RecordAttack();
        }
        
        /// <summary>
        /// Slime zıplama hareketi
        /// </summary>
        public void Jump()
        {
            if (!IsGrounded) return;
            if (Time.time - lastJumpTime < jumpCooldown) return;
            
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Slime özel gizmos
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
