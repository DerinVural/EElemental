using UnityEngine;
using EElemental.Elements.Effects;

namespace EElemental.Combat
{
    /// <summary>
    /// Manages the hurtbox for receiving damage.
    /// </summary>
    public class HurtboxManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _canReceiveDamage = true;
        
        private IFrameController _iFrames;
        private StatusEffectManager _statusEffects;
        
        // Events
        public event System.Action<DamageInfo> OnDamageReceived;
        public event System.Action OnDamageBlocked;
        
        public bool CanReceiveDamage
        {
            get => _canReceiveDamage && (_iFrames == null || !_iFrames.IsInvincible);
            set => _canReceiveDamage = value;
        }
        
        private void Awake()
        {
            _iFrames = GetComponent<IFrameController>();
            _statusEffects = GetComponent<StatusEffectManager>();
        }
        
        /// <summary>
        /// Attempt to receive damage. Returns true if damage was applied.
        /// </summary>
        public bool TakeDamage(DamageInfo damage)
        {
            // Check if invincible
            if (!CanReceiveDamage)
            {
                OnDamageBlocked?.Invoke();
                return false;
            }
            
            // Apply damage
            OnDamageReceived?.Invoke(damage);
            
            return true;
        }
        
        /// <summary>
        /// Set temporary invincibility.
        /// </summary>
        public void SetInvincible(bool invincible)
        {
            if (_iFrames != null)
            {
                if (invincible)
                    _iFrames.StartIFrames(999f); // Long duration, call StopIFrames to end
                else
                    _iFrames.StopIFrames();
            }
        }
    }
}
