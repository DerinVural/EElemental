using UnityEngine;

namespace EElemental.Enemies
{
    /// <summary>
    /// Düşman istatistiklerini yöneten sınıf.
    /// ScriptableObject ile konfigüre edilebilir.
    /// </summary>
    public class EnemyStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] private EnemyData enemyData;
        
        [Header("Runtime Stats")]
        [SerializeField] private float currentHealth;
        
        // Base stats (from EnemyData or default values)
        public float MaxHealth => enemyData != null ? enemyData.MaxHealth : 100f;
        public float MoveSpeed => enemyData != null ? enemyData.MoveSpeed : 3f;
        public float AttackDamage => enemyData != null ? enemyData.AttackDamage : 10f;
        public float AttackRange => enemyData != null ? enemyData.AttackRange : 1.5f;
        public float AttackCooldown => enemyData != null ? enemyData.AttackCooldown : 1f;
        public float DetectionRange => enemyData != null ? enemyData.DetectionRange : 8f;
        public float ChaseRange => enemyData != null ? enemyData.ChaseRange : 12f;
        public float KnockbackResistance => enemyData != null ? enemyData.KnockbackResistance : 1f;
        public bool CanBeKnockedBack => enemyData != null ? enemyData.CanBeKnockedBack : true;
        public bool CanBeStunned => enemyData != null ? enemyData.CanBeStunned : true;
        
        // Runtime stats
        public float CurrentHealth => currentHealth;
        public bool IsAlive => currentHealth > 0;
        
        // Attack tracking
        public float LastAttackTime { get; private set; }
        public bool CanAttack => Time.time >= LastAttackTime + AttackCooldown;
        
        // Events
        public System.Action<float, float> OnHealthChanged; // current, max
        public System.Action OnDeath;
        
        private void Awake()
        {
            InitializeStats();
        }
        
        public void InitializeStats()
        {
            currentHealth = MaxHealth;
            LastAttackTime = -AttackCooldown; // Allow immediate first attack
        }
        
        public void InitializeStats(EnemyData data)
        {
            enemyData = data;
            InitializeStats();
        }
        
        #region Health Management
        
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            float actualDamage = CalculateDamage(damage);
            currentHealth = Mathf.Max(0, currentHealth - actualDamage);
            
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);
            
            if (!IsAlive)
            {
                OnDeath?.Invoke();
            }
        }
        
        public void Heal(float amount)
        {
            if (!IsAlive) return;
            
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);
        }
        
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0, MaxHealth);
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);
        }
        
        protected virtual float CalculateDamage(float incomingDamage)
        {
            // Base damage calculation
            // Can be extended for armor, resistances, etc.
            return incomingDamage;
        }
        
        #endregion
        
        #region Attack Management
        
        public void RecordAttack()
        {
            LastAttackTime = Time.time;
        }
        
        public float GetAttackCooldownRemaining()
        {
            return Mathf.Max(0, (LastAttackTime + AttackCooldown) - Time.time);
        }
        
        #endregion
        
        #region Stat Modifiers
        
        // Temporary stat modifiers for buffs/debuffs
        private float moveSpeedModifier = 1f;
        private float attackDamageModifier = 1f;
        
        public float GetModifiedMoveSpeed() => MoveSpeed * moveSpeedModifier;
        public float GetModifiedAttackDamage() => AttackDamage * attackDamageModifier;
        
        public void SetMoveSpeedModifier(float modifier)
        {
            moveSpeedModifier = Mathf.Max(0, modifier);
        }
        
        public void SetAttackDamageModifier(float modifier)
        {
            attackDamageModifier = Mathf.Max(0, modifier);
        }
        
        public void ResetModifiers()
        {
            moveSpeedModifier = 1f;
            attackDamageModifier = 1f;
        }
        
        #endregion
    }
}
