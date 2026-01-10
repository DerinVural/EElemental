using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Knockback effect - Pushes target away.
    /// Air element status effect.
    /// </summary>
    public class KnockbackEffect : StatusEffect
    {
        [Header("Knockback Settings")]
        public float knockbackForce = 10f;
        public float airborneTime = 0.3f;
        public Vector2 knockbackDirection = Vector2.right;
        
        private Rigidbody2D _rb;
        private IKnockbackable _knockbackable;
        
        private void Awake()
        {
            effectType = StatusEffectType.Knockback;
            duration = 0.3f; // Very short duration
            canStack = false;
        }
        
        public void SetDirection(Vector2 direction)
        {
            knockbackDirection = direction.normalized;
        }
        
        public override void Initialize(float duration, float strength)
        {
            knockbackForce *= strength;
            base.Initialize(duration, strength);
        }
        
        protected override void ApplyEffect()
        {
            _rb = GetComponent<Rigidbody2D>();
            _knockbackable = GetComponent<IKnockbackable>();
            
            if (_knockbackable != null)
            {
                _knockbackable.OnKnockbackStart();
            }
            
            ApplyForce();
        }
        
        private void ApplyForce()
        {
            if (_rb != null)
            {
                // Add slight upward component for "airborne" feel
                Vector2 force = knockbackDirection * knockbackForce;
                force.y += knockbackForce * 0.3f;
                
                _rb.velocity = Vector2.zero;
                _rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
        
        protected override void RemoveEffect()
        {
            if (_knockbackable != null)
            {
                _knockbackable.OnKnockbackEnd();
            }
        }
    }
}
