using UnityEngine;

namespace EElemental.Player
{
    /// <summary>
    /// Manages player statistics like health, stamina, etc.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
        
        [Header("Defense")]
        [SerializeField] private float _defense = 0f;
        [SerializeField] private float _damageReduction = 0f;
        
        [Header("Offense")]
        [SerializeField] private float _attackPower = 1f;
        [SerializeField] private float _critChance = 0.05f;
        [SerializeField] private float _critMultiplier = 1.5f;
        
        // Properties
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float HealthPercent => _currentHealth / _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        public float Defense => _defense;
        public float AttackPower => _attackPower;
        public float CritChance => _critChance;
        public float CritMultiplier => _critMultiplier;
        
        // Events
        public event System.Action<float, float> OnHealthChanged; // current, max
        public event System.Action<float> OnDamageTaken;
        public event System.Action<float> OnHealed;
        public event System.Action OnDeath;
        
        private void Awake()
        {
            _currentHealth = _maxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            // Apply defense
            float reduction = _defense / (_defense + 100f);
            float finalDamage = damage * (1f - reduction) * (1f - _damageReduction);
            finalDamage = Mathf.Max(1f, finalDamage); // Minimum 1 damage
            
            _currentHealth -= finalDamage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            
            OnDamageTaken?.Invoke(finalDamage);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            if (!IsAlive)
            {
                Die();
            }
        }
        
        public void Heal(float amount)
        {
            if (!IsAlive) return;
            
            float previousHealth = _currentHealth;
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            
            float actualHeal = _currentHealth - previousHealth;
            if (actualHeal > 0)
            {
                OnHealed?.Invoke(actualHeal);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }
        
        public void FullHeal()
        {
            Heal(_maxHealth);
        }
        
        private void Die()
        {
            OnDeath?.Invoke();
            Debug.Log("[PlayerStats] Player died!");
        }
        
        #region Stat Modifiers
        
        public void ModifyMaxHealth(float amount)
        {
            _maxHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
        
        public void ModifyDefense(float amount)
        {
            _defense += amount;
        }
        
        public void ModifyAttackPower(float amount)
        {
            _attackPower += amount;
        }
        
        public void ModifyCritChance(float amount)
        {
            _critChance = Mathf.Clamp01(_critChance + amount);
        }
        
        public void ModifyCritMultiplier(float amount)
        {
            _critMultiplier += amount;
        }
        
        public void SetDamageReduction(float percent)
        {
            _damageReduction = Mathf.Clamp01(percent);
        }
        
        #endregion
    }
}
