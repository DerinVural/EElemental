using UnityEngine;
using EElemental.Elements;
using EElemental.Elements.Effects;

namespace EElemental.Combat
{
    /// <summary>
    /// Handles damage calculation with element modifiers.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Calculate final damage with all modifiers.
        /// </summary>
        public static float Calculate(
            float baseDamage,
            float attackMultiplier,
            float elementMultiplier,
            float critMultiplier,
            float weaknessMultiplier,
            float defense)
        {
            // Base calculation
            float damage = baseDamage * attackMultiplier;
            
            // Element bonus
            damage *= elementMultiplier;
            
            // Critical hit
            damage *= critMultiplier;
            
            // Weakness multiplier
            damage *= weaknessMultiplier;
            
            // Defense reduction (diminishing returns formula)
            float reduction = defense / (defense + 100f);
            damage *= (1f - reduction);
            
            // Minimum 1 damage
            return Mathf.Max(1f, damage);
        }
        
        /// <summary>
        /// Calculate damage from attack data and element.
        /// </summary>
        public static DamageInfo CalculateFromAttack(
            AttackData attack,
            ElementData element,
            float attackerDamage,
            bool forceCrit = false)
        {
            // Roll for crit
            bool isCrit = forceCrit || Random.value < attack.critChance;
            float critMult = isCrit ? attack.critMultiplier : 1f;
            
            // Element modifier
            float elementMult = element != null ? element.damageMultiplier : 1f;
            
            // Calculate final damage
            float finalDamage = attack.baseDamage * attackerDamage * elementMult * critMult;
            
            return new DamageInfo
            {
                amount = finalDamage,
                damageType = GetDamageType(element),
                element = element?.type ?? ElementType.None,
                isCrit = isCrit,
                knockbackDirection = attack.knockbackDirection,
                knockbackForce = attack.knockbackForce,
                hitstopFrames = attack.hitstopFrames
            };
        }
        
        private static DamageType GetDamageType(ElementData element)
        {
            if (element == null) return DamageType.Physical;
            
            return element.type switch
            {
                ElementType.Fire => DamageType.Fire,
                ElementType.Water => DamageType.Water,
                ElementType.Earth => DamageType.Earth,
                ElementType.Air => DamageType.Air,
                _ => DamageType.Physical
            };
        }
        
        /// <summary>
        /// Get weakness multiplier for element matchups.
        /// </summary>
        public static float GetWeaknessMultiplier(ElementType attackElement, ElementType defenderWeakness)
        {
            if (attackElement == ElementType.None || defenderWeakness == ElementType.None)
                return 1f;
            
            if (attackElement == defenderWeakness)
                return 1.5f; // 50% bonus damage
            
            // Could add resistance logic here too
            return 1f;
        }
    }
}
