using System.Collections.Generic;
using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Manages status effects on a game object.
    /// </summary>
    public class StatusEffectManager : MonoBehaviour
    {
        [SerializeField] private List<StatusEffect> _activeEffects = new List<StatusEffect>();
        
        // Prefabs for each effect type (assign in inspector or load from resources)
        [Header("Effect Prefabs")]
        [SerializeField] private StatusEffect _burnPrefab;
        [SerializeField] private StatusEffect _slowPrefab;
        [SerializeField] private StatusEffect _stunPrefab;
        [SerializeField] private StatusEffect _knockbackPrefab;
        
        // Events
        public event System.Action<StatusEffect> OnEffectApplied;
        public event System.Action<StatusEffect> OnEffectRemoved;
        
        public IReadOnlyList<StatusEffect> ActiveEffects => _activeEffects;
        
        /// <summary>
        /// Apply a status effect to this object.
        /// </summary>
        public StatusEffect ApplyEffect(StatusEffectType type, float duration, float strength)
        {
            // Check for existing effect
            StatusEffect existing = GetEffect(type);
            
            if (existing != null)
            {
                existing.Refresh();
                return existing;
            }
            
            // Create new effect
            StatusEffect effect = CreateEffect(type);
            
            if (effect == null)
            {
                Debug.LogWarning($"[StatusEffectManager] Could not create effect: {type}");
                return null;
            }
            
            _activeEffects.Add(effect);
            effect.OnEffectEnd += HandleEffectEnded;
            effect.Initialize(duration, strength);
            
            OnEffectApplied?.Invoke(effect);
            
            return effect;
        }
        
        /// <summary>
        /// Apply a knockback effect with direction.
        /// </summary>
        public KnockbackEffect ApplyKnockback(Vector2 direction, float duration, float strength)
        {
            var effect = ApplyEffect(StatusEffectType.Knockback, duration, strength) as KnockbackEffect;
            effect?.SetDirection(direction);
            return effect;
        }
        
        private StatusEffect CreateEffect(StatusEffectType type)
        {
            StatusEffect prefab = type switch
            {
                StatusEffectType.Burn => _burnPrefab,
                StatusEffectType.Slow => _slowPrefab,
                StatusEffectType.Stun => _stunPrefab,
                StatusEffectType.Knockback => _knockbackPrefab,
                _ => null
            };
            
            if (prefab != null)
            {
                return Instantiate(prefab, transform);
            }
            
            // Fallback: Add component directly
            return type switch
            {
                StatusEffectType.Burn => gameObject.AddComponent<BurnEffect>(),
                StatusEffectType.Slow => gameObject.AddComponent<SlowEffect>(),
                StatusEffectType.Stun => gameObject.AddComponent<StunEffect>(),
                StatusEffectType.Knockback => gameObject.AddComponent<KnockbackEffect>(),
                _ => null
            };
        }
        
        /// <summary>
        /// Check if an effect is currently active.
        /// </summary>
        public bool HasEffect(StatusEffectType type)
        {
            return GetEffect(type) != null;
        }
        
        /// <summary>
        /// Get an active effect by type.
        /// </summary>
        public StatusEffect GetEffect(StatusEffectType type)
        {
            return _activeEffects.Find(e => e.effectType == type && e.IsActive);
        }
        
        /// <summary>
        /// Remove a specific effect type.
        /// </summary>
        public void RemoveEffect(StatusEffectType type)
        {
            var effect = GetEffect(type);
            effect?.ForceEnd();
        }
        
        /// <summary>
        /// Remove all active effects.
        /// </summary>
        public void ClearAllEffects()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                _activeEffects[i].ForceEnd();
            }
        }
        
        private void HandleEffectEnded(StatusEffect effect)
        {
            effect.OnEffectEnd -= HandleEffectEnded;
            _activeEffects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
        }
        
        private void OnDestroy()
        {
            ClearAllEffects();
        }
    }
}
