using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Base class for all status effects.
    /// </summary>
    public abstract class StatusEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        public StatusEffectType effectType;
        public float duration = 3f;
        public float strength = 1f;
        public bool canStack = false;
        public int maxStacks = 1;
        
        [Header("Visual")]
        public GameObject effectVFX;
        public Color tintColor = Color.white;
        
        protected float _remainingDuration;
        protected int _currentStacks = 1;
        protected GameObject _activeVFX;
        
        public float RemainingDuration => _remainingDuration;
        public int CurrentStacks => _currentStacks;
        public bool IsActive => _remainingDuration > 0;
        
        // Events
        public event System.Action<StatusEffect> OnEffectStart;
        public event System.Action<StatusEffect> OnEffectEnd;
        public event System.Action<StatusEffect, int> OnStackChanged;
        
        public virtual void Initialize(float duration, float strength)
        {
            this.duration = duration;
            this.strength = strength;
            _remainingDuration = duration;
            _currentStacks = 1;
            
            ApplyEffect();
            SpawnVFX();
            
            OnEffectStart?.Invoke(this);
        }
        
        public virtual void Refresh(float additionalDuration = 0f)
        {
            if (canStack && _currentStacks < maxStacks)
            {
                _currentStacks++;
                OnStackChanged?.Invoke(this, _currentStacks);
            }
            
            // Refresh or extend duration
            _remainingDuration = Mathf.Max(_remainingDuration, duration + additionalDuration);
        }
        
        protected virtual void Update()
        {
            if (!IsActive) return;
            
            _remainingDuration -= Time.deltaTime;
            
            TickEffect();
            
            if (_remainingDuration <= 0)
            {
                EndEffect();
            }
        }
        
        /// <summary>
        /// Apply the initial effect (e.g., slow movement).
        /// </summary>
        protected abstract void ApplyEffect();
        
        /// <summary>
        /// Called every frame while effect is active.
        /// </summary>
        protected virtual void TickEffect() { }
        
        /// <summary>
        /// Remove the effect and cleanup.
        /// </summary>
        protected virtual void RemoveEffect() { }
        
        public virtual void EndEffect()
        {
            _remainingDuration = 0;
            RemoveEffect();
            CleanupVFX();
            
            OnEffectEnd?.Invoke(this);
            
            Destroy(this);
        }
        
        public void ForceEnd()
        {
            EndEffect();
        }
        
        protected virtual void SpawnVFX()
        {
            if (effectVFX != null)
            {
                _activeVFX = Instantiate(effectVFX, transform);
            }
        }
        
        protected virtual void CleanupVFX()
        {
            if (_activeVFX != null)
            {
                Destroy(_activeVFX);
            }
        }
    }
}
