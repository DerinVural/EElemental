using UnityEngine;
using EElemental.Elements;
using EElemental.Combat;

namespace EElemental.Weapons
{
    /// <summary>
    /// Integrates elements with weapons, modifying stats.
    /// </summary>
    public class WeaponElementIntegrator : MonoBehaviour
    {
        [SerializeField] private WeaponData _baseWeapon;
        [SerializeField] private ElementData _currentElement;
        
        // Cached modified stats
        private WeaponStats _modifiedStats;
        private bool _statsDirty = true;
        
        // Properties
        public WeaponData BaseWeapon => _baseWeapon;
        public ElementData CurrentElement => _currentElement;
        public WeaponStats Stats
        {
            get
            {
                if (_statsDirty)
                {
                    _modifiedStats = CalculateStats();
                    _statsDirty = false;
                }
                return _modifiedStats;
            }
        }
        
        // Events
        public event System.Action<ElementData> OnElementChanged;
        public event System.Action<WeaponData> OnWeaponChanged;
        
        /// <summary>
        /// Apply an element to the weapon.
        /// </summary>
        public void ApplyElement(ElementData element)
        {
            if (element == _currentElement) return;
            
            _currentElement = element;
            _statsDirty = true;
            
            UpdateVisuals();
            OnElementChanged?.Invoke(element);
        }
        
        /// <summary>
        /// Remove the current element.
        /// </summary>
        public void RemoveElement()
        {
            ApplyElement(null);
        }
        
        /// <summary>
        /// Change the base weapon.
        /// </summary>
        public void SetWeapon(WeaponData weapon)
        {
            if (weapon == _baseWeapon) return;
            
            _baseWeapon = weapon;
            _statsDirty = true;
            
            OnWeaponChanged?.Invoke(weapon);
        }
        
        /// <summary>
        /// Check if an element is compatible with this weapon.
        /// </summary>
        public bool IsElementCompatible(ElementType elementType)
        {
            if (_baseWeapon == null || _baseWeapon.compatibleElements == null) return true;
            if (_baseWeapon.compatibleElements.Length == 0) return true;
            
            foreach (var compatible in _baseWeapon.compatibleElements)
            {
                if (compatible == elementType) return true;
            }
            return false;
        }
        
        private WeaponStats CalculateStats()
        {
            var stats = new WeaponStats();
            
            if (_baseWeapon == null) return stats;
            
            // Start with base stats
            stats.damage = _baseWeapon.baseDamage;
            stats.attackSpeed = _baseWeapon.attackSpeed;
            stats.range = _baseWeapon.range;
            stats.critChance = _baseWeapon.critChance;
            stats.critMultiplier = _baseWeapon.critMultiplier;
            stats.knockbackForce = _baseWeapon.knockbackForce;
            stats.elementType = ElementType.None;
            
            // Apply element modifiers
            if (_currentElement != null)
            {
                stats.damage *= _currentElement.damageMultiplier;
                stats.critChance += _currentElement.critChanceBonus;
                stats.critMultiplier += _currentElement.critDamageBonus;
                stats.elementType = _currentElement.type;
                
                // Compatibility bonus
                if (IsElementCompatible(_currentElement.type))
                {
                    stats.damage *= (1f + _baseWeapon.elementDamageBonus);
                }
            }
            
            return stats;
        }
        
        private void UpdateVisuals()
        {
            // Update weapon visuals based on element
            // This would include particle effects, color changes, etc.
            if (_currentElement != null)
            {
                Debug.Log($"[WeaponElementIntegrator] Applied {_currentElement.elementName} to {_baseWeapon?.weaponName}");
            }
        }
        
        /// <summary>
        /// Get damage info for an attack.
        /// </summary>
        public DamageInfo CreateDamageInfo(float attackMultiplier = 1f)
        {
            var stats = Stats;
            bool isCrit = Random.value < stats.critChance;
            
            float damage = stats.damage * attackMultiplier;
            if (isCrit)
            {
                damage *= stats.critMultiplier;
            }
            
            return new DamageInfo
            {
                amount = damage,
                element = stats.elementType,
                isCrit = isCrit,
                knockback = Vector2.right * stats.knockbackForce,
                hitstopFrames = isCrit ? 6 : 3,
                attacker = gameObject
            };
        }
    }
    
    [System.Serializable]
    public struct WeaponStats
    {
        public float damage;
        public float attackSpeed;
        public float range;
        public float critChance;
        public float critMultiplier;
        public float knockbackForce;
        public ElementType elementType;
    }
}
