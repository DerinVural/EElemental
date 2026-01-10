using System.Collections.Generic;
using UnityEngine;
using EElemental.Elements.Effects;

namespace EElemental.Combat
{
    /// <summary>
    /// Manages hitbox activation and collision detection for attacks.
    /// </summary>
    public class HitboxManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _hitLayers;
        [SerializeField] private Transform _hitboxOrigin;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebugHitbox = true;
        [SerializeField] private Color _debugColor = Color.red;
        
        private AttackData _currentAttack;
        private bool _hitboxActive;
        private HashSet<Collider2D> _hitTargets = new HashSet<Collider2D>();
        
        // Events
        public event System.Action<Collider2D, DamageInfo> OnHit;
        
        private void Awake()
        {
            if (_hitboxOrigin == null)
                _hitboxOrigin = transform;
        }
        
        /// <summary>
        /// Activate hitbox for an attack.
        /// </summary>
        public void ActivateHitbox(AttackData attack)
        {
            _currentAttack = attack;
            _hitboxActive = true;
            _hitTargets.Clear();
        }
        
        /// <summary>
        /// Deactivate the hitbox.
        /// </summary>
        public void DeactivateHitbox()
        {
            _hitboxActive = false;
            _currentAttack = null;
        }
        
        private void FixedUpdate()
        {
            if (!_hitboxActive || _currentAttack == null) return;
            
            CheckHits();
        }
        
        private void CheckHits()
        {
            // Calculate hitbox position
            Vector2 origin = (Vector2)_hitboxOrigin.position;
            Vector2 offset = _currentAttack.hitboxOffset;
            
            // Flip offset based on facing direction
            if (_hitboxOrigin.lossyScale.x < 0 || 
                (_hitboxOrigin.parent != null && _hitboxOrigin.parent.lossyScale.x < 0))
            {
                offset.x *= -1;
            }
            
            Vector2 hitboxCenter = origin + offset;
            
            // Box cast for hits
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                hitboxCenter,
                _currentAttack.hitboxSize,
                0f,
                _hitLayers
            );
            
            foreach (var hit in hits)
            {
                // Skip if already hit this attack
                if (_hitTargets.Contains(hit)) continue;
                
                // Skip self
                if (hit.transform.IsChildOf(transform) || hit.transform == transform) continue;
                
                // Register hit
                _hitTargets.Add(hit);
                ProcessHit(hit);
            }
        }
        
        private void ProcessHit(Collider2D target)
        {
            // Calculate damage
            var damageInfo = new DamageInfo
            {
                amount = _currentAttack.baseDamage,
                damageType = DamageType.Physical,
                knockbackDirection = GetKnockbackDirection(target.transform.position),
                knockbackForce = _currentAttack.knockbackForce,
                hitstopFrames = _currentAttack.hitstopFrames,
                attacker = gameObject
            };
            
            // Apply damage if target is damageable
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damageInfo);
            
            // Fire event
            OnHit?.Invoke(target, damageInfo);
            
            // Spawn hit VFX
            if (_currentAttack.hitVFX != null)
            {
                Instantiate(_currentAttack.hitVFX, target.bounds.center, Quaternion.identity);
            }
            
            // Play hit SFX
            if (_currentAttack.hitSFX != null)
            {
                AudioSource.PlayClipAtPoint(_currentAttack.hitSFX, target.bounds.center);
            }
        }
        
        private Vector2 GetKnockbackDirection(Vector3 targetPosition)
        {
            Vector2 dir = _currentAttack.knockbackDirection;
            
            // Flip based on relative position
            if (targetPosition.x < transform.position.x)
            {
                dir.x *= -1;
            }
            
            return dir.normalized;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showDebugHitbox) return;
            if (_currentAttack == null) return;
            
            Transform origin = _hitboxOrigin != null ? _hitboxOrigin : transform;
            Vector2 offset = _currentAttack.hitboxOffset;
            
            if (origin.lossyScale.x < 0)
                offset.x *= -1;
            
            Vector2 center = (Vector2)origin.position + offset;
            
            Gizmos.color = _hitboxActive ? Color.red : Color.yellow;
            Gizmos.DrawWireCube(center, _currentAttack.hitboxSize);
        }
    }
}
