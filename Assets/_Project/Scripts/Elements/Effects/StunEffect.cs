using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Stun effect - Target cannot move or attack.
    /// Earth element status effect.
    /// </summary>
    public class StunEffect : StatusEffect
    {
        [Header("Stun Settings")]
        public bool interruptCurrentAction = true;
        
        private IStunnable _stunnable;
        
        private void Awake()
        {
            effectType = StatusEffectType.Stun;
            tintColor = new Color(0.6f, 0.5f, 0.3f, 1f); // Brown/rocky tint
            canStack = false; // Stun doesn't stack, just refreshes
        }
        
        protected override void ApplyEffect()
        {
            _stunnable = GetComponent<IStunnable>();
            
            if (_stunnable != null)
            {
                _stunnable.SetStunned(true);
                
                if (interruptCurrentAction)
                {
                    _stunnable.InterruptCurrentAction();
                }
            }
        }
        
        protected override void RemoveEffect()
        {
            if (_stunnable != null)
            {
                _stunnable.SetStunned(false);
            }
        }
    }
}
