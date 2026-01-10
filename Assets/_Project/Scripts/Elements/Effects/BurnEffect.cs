using UnityEngine;

namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Burn effect - Deals damage over time.
    /// Fire element status effect.
    /// </summary>
    public class BurnEffect : StatusEffect
    {
        [Header("Burn Settings")]
        public float damagePerTick = 5f;
        public float tickInterval = 0.5f;
        public float spreadRadius = 2f;
        [Range(0f, 1f)]
        public float spreadChance = 0.1f;
        
        private float _tickTimer;
        private IDamageable _target;
        
        private void Awake()
        {
            effectType = StatusEffectType.Burn;
        }
        
        public override void Initialize(float duration, float strength)
        {
            _target = GetComponent<IDamageable>();
            damagePerTick *= strength;
            base.Initialize(duration, strength);
        }
        
        protected override void ApplyEffect()
        {
            _tickTimer = 0f;
            // Visual: Add fire tint/particles
        }
        
        protected override void TickEffect()
        {
            _tickTimer += Time.deltaTime;
            
            if (_tickTimer >= tickInterval)
            {
                _tickTimer = 0f;
                DealBurnDamage();
                TrySpreadFire();
            }
        }
        
        private void DealBurnDamage()
        {
            float totalDamage = damagePerTick * _currentStacks;
            
            if (_target != null)
            {
                _target.TakeDamage(new DamageInfo
                {
                    amount = totalDamage,
                    damageType = DamageType.Fire,
                    isCrit = false,
                    attacker = null
                });
            }
        }
        
        private void TrySpreadFire()
        {
            if (Random.value > spreadChance) return;
            
            // Find nearby enemies and try to spread
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, spreadRadius);
            
            foreach (var col in nearby)
            {
                if (col.gameObject == gameObject) continue;
                
                var targetEffects = col.GetComponent<StatusEffectManager>();
                if (targetEffects != null && !targetEffects.HasEffect(StatusEffectType.Burn))
                {
                    // Spread with reduced duration
                    targetEffects.ApplyEffect(StatusEffectType.Burn, duration * 0.5f, strength * 0.5f);
                    break; // Only spread to one target per tick
                }
            }
        }
        
        protected override void RemoveEffect()
        {
            // Cleanup burn visuals
        }
    }
}
