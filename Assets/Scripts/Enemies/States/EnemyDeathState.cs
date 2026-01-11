using UnityEngine;

namespace EElemental.Enemies.States
{
    /// <summary>
    /// Düşman Death state - Ölüm animasyonu ve cleanup
    /// </summary>
    public class EnemyDeathState : EnemyStateBase
    {
        private float deathDuration = 1.5f;
        private bool hasTriggeredDeath = false;
        
        public EnemyDeathState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            
            // Hareketi durdur
            enemy.Stop();
            
            // Death animasyonu
            SetAnimatorTrigger("Death");
            
            // AI'ı devre dışı bırak
            ai.EnableAI(false);
            ai.ChangeState(AIState.Dead);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Ölüm işlemlerini tetikle
            if (!hasTriggeredDeath && stateTimer >= 0.5f)
            {
                TriggerDeathEffects();
                hasTriggeredDeath = true;
            }
        }
        
        private void TriggerDeathEffects()
        {
            // Loot drop
            DropLoot();
            
            // Death event
            // GameEvents.OnEnemyDeath?.Invoke(enemy);
            
            // XP ver
            // GameManager.Instance.AddExperience(stats.ExperienceValue);
        }
        
        private void DropLoot()
        {
            // Loot table'dan item drop et
            // TODO: LootManager.DropLoot(enemy.transform.position, enemyData.LootTable);
        }
        
        protected override void CheckTransitions()
        {
            // Death state'ten çıkış yok - obje destroy edilecek
            if (stateTimer >= deathDuration)
            {
                // Enemy objesini yok et
                Object.Destroy(enemy.gameObject);
            }
        }
    }
}
