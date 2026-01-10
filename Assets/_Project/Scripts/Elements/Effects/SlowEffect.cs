using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Slow effect - Reduces movement and attack speed.
    /// Water element status effect.
    /// </summary>
    public class SlowEffect : StatusEffect
    {
        [Header("Slow Settings")]
        [Range(0f, 0.9f)]
        public float movementReduction = 0.4f; // 40% slower
        [Range(0f, 0.9f)]
        public float attackSpeedReduction = 0.2f; // 20% slower attacks
        
        private IMoveable _moveable;
        private float _originalMoveSpeed;
        private float _originalAttackSpeed;
        
        private void Awake()
        {
            effectType = StatusEffectType.Slow;
            tintColor = new Color(0.5f, 0.7f, 1f, 1f); // Blue tint
        }
        
        public override void Initialize(float duration, float strength)
        {
            movementReduction = Mathf.Clamp(movementReduction * strength, 0f, 0.9f);
            attackSpeedReduction = Mathf.Clamp(attackSpeedReduction * strength, 0f, 0.9f);
            base.Initialize(duration, strength);
        }
        
        protected override void ApplyEffect()
        {
            _moveable = GetComponent<IMoveable>();
            
            if (_moveable != null)
            {
                _originalMoveSpeed = _moveable.MoveSpeed;
                _moveable.MoveSpeed *= (1f - movementReduction);
            }
            
            // TODO: Apply attack speed reduction when combat system is ready
        }
        
        protected override void RemoveEffect()
        {
            if (_moveable != null)
            {
                _moveable.MoveSpeed = _originalMoveSpeed;
            }
        }
    }
}
