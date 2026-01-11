using UnityEngine;
using System.Collections.Generic;
using EElemental.Elements;

namespace EElemental.Player
{
    /// <summary>
    /// Manages player statistics like health, mana, defense, and element resistances.
    /// Merged version combining health/defense system with mana and elemental mechanics.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;

        [Header("Mana")]
        [SerializeField] private int _maxMana = 100;
        [SerializeField] private int _currentMana;
        [SerializeField] private float _manaRegenRate = 5f; // per second

        [Header("Defense")]
        [SerializeField] private float _defense = 0f;
        [SerializeField] private float _damageReduction = 0f;

        [Header("Offense")]
        [SerializeField] private float _attackPower = 1f;
        [SerializeField] private float _critChance = 0.05f;
        [SerializeField] private float _critMultiplier = 1.5f;

        [Header("Element Resistances")]
        [SerializeField] private float _fireResistance = 0f;
        [SerializeField] private float _waterResistance = 0f;
        [SerializeField] private float _earthResistance = 0f;
        [SerializeField] private float _airResistance = 0f;
        
        // Properties - Health
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float HealthPercent => _currentHealth / _maxHealth;
        public bool IsAlive => _currentHealth > 0;

        // Properties - Mana
        public int MaxMana => _maxMana;
        public int CurrentMana => _currentMana;
        public float ManaPercent => (float)_currentMana / _maxMana;

        // Properties - Combat
        public float Defense => _defense;
        public float AttackPower => _attackPower;
        public float CritChance => _critChance;
        public float CritMultiplier => _critMultiplier;

        // Events
        public event System.Action<float, float> OnHealthChanged; // current, max
        public event System.Action<float> OnDamageTaken;
        public event System.Action<float> OnHealed;
        public event System.Action<int, int> OnManaChanged; // current, max
        public event System.Action OnDeath;
        
        private void Awake()
        {
            _currentHealth = _maxHealth;
            _currentMana = _maxMana;
        }

        private void Update()
        {
            // Passive mana regeneration
            if (_currentMana < _maxMana)
            {
                RestoreMana(Mathf.CeilToInt(_manaRegenRate * Time.deltaTime));
            }
        }

        public void TakeDamage(float damage, ElementType damageElement = ElementType.None)
        {
            if (!IsAlive) return;

            // Apply element resistance
            float elementalMultiplier = GetElementalDamageMultiplier(damageElement);

            // Apply defense
            float reduction = _defense / (_defense + 100f);
            float finalDamage = damage * elementalMultiplier * (1f - reduction) * (1f - _damageReduction);
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

        private float GetElementalDamageMultiplier(ElementType element)
        {
            float resistance = element switch
            {
                ElementType.Fire => _fireResistance,
                ElementType.Water => _waterResistance,
                ElementType.Earth => _earthResistance,
                ElementType.Air => _airResistance,
                _ => 0f
            };

            // Resistance reduces damage (0.5 resistance = 50% damage reduction)
            return Mathf.Max(0f, 1f - resistance);
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

        #region Mana Management

        public bool ConsumeMana(int amount)
        {
            if (_currentMana >= amount)
            {
                _currentMana -= amount;
                OnManaChanged?.Invoke(_currentMana, _maxMana);
                return true;
            }
            return false;
        }

        public void RestoreMana(int amount)
        {
            int previousMana = _currentMana;
            _currentMana = Mathf.Min(_currentMana + amount, _maxMana);

            if (_currentMana != previousMana)
            {
                OnManaChanged?.Invoke(_currentMana, _maxMana);
            }
        }

        public void FullRestoreMana()
        {
            RestoreMana(_maxMana);
        }

        #endregion

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

        public void ModifyMaxMana(int amount)
        {
            _maxMana += amount;
            _currentMana = Mathf.Min(_currentMana, _maxMana);
            OnManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public void ModifyElementResistance(ElementType element, float amount)
        {
            switch (element)
            {
                case ElementType.Fire:
                    _fireResistance = Mathf.Clamp01(_fireResistance + amount);
                    break;
                case ElementType.Water:
                    _waterResistance = Mathf.Clamp01(_waterResistance + amount);
                    break;
                case ElementType.Earth:
                    _earthResistance = Mathf.Clamp01(_earthResistance + amount);
                    break;
                case ElementType.Air:
                    _airResistance = Mathf.Clamp01(_airResistance + amount);
                    break;
            }
        }

        public float GetElementResistance(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => _fireResistance,
                ElementType.Water => _waterResistance,
                ElementType.Earth => _earthResistance,
                ElementType.Air => _airResistance,
                _ => 0f
            };
        }

        #endregion
    }
}
