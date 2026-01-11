using UnityEngine;
using EElemental.Elements;
using EElemental.Combat;

namespace EElemental.Weapons
{
    /// <summary>
    /// Base class for weapon behavior.
    /// Attach to a weapon prefab to enable attacks.
    /// </summary>
    public class WeaponBase : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private WeaponElementIntegrator _elementIntegrator;
        [SerializeField] private HitboxManager _hitboxManager;
        [SerializeField] private Transform _attackPoint;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private ParticleSystem _elementParticles;
        
        // State
        private bool _isAttacking;
        
        // Properties
        public WeaponData Data => _weaponData;
        public WeaponElementIntegrator ElementIntegrator => _elementIntegrator;
        public bool IsAttacking => _isAttacking;
        
        private void Awake()
        {
            if (_elementIntegrator == null)
                _elementIntegrator = GetComponent<WeaponElementIntegrator>();
                
            if (_hitboxManager == null)
                _hitboxManager = GetComponentInChildren<HitboxManager>();
        }
        
        private void Start()
        {
            if (_elementIntegrator != null)
            {
                _elementIntegrator.OnElementChanged += OnElementChanged;
            }
        }
        
        /// <summary>
        /// Start an attack animation/hitbox.
        /// </summary>
        public void StartAttack(AttackData attackData)
        {
            if (_isAttacking) return;
            
            _isAttacking = true;
            
            // Enable trail
            if (_trailRenderer != null)
            {
                _trailRenderer.emitting = true;
            }
            
            // Play swing VFX
            if (_weaponData?.swingVFX != null && _attackPoint != null)
            {
                Instantiate(_weaponData.swingVFX, _attackPoint.position, _attackPoint.rotation);
            }
            
            // Play swing SFX
            if (_weaponData?.swingSFX != null)
            {
                AudioSource.PlayClipAtPoint(_weaponData.swingSFX, transform.position);
            }
        }
        
        /// <summary>
        /// End the attack.
        /// </summary>
        public void EndAttack()
        {
            _isAttacking = false;
            
            // Disable trail
            if (_trailRenderer != null)
            {
                _trailRenderer.emitting = false;
            }
        }
        
        /// <summary>
        /// Called when an enemy is hit.
        /// </summary>
        public void OnHit(GameObject target, DamageInfo damage)
        {
            // Play hit VFX
            if (_weaponData?.swingVFX != null)
            {
                // Hit effect at target position
            }
            
            // Play hit SFX
            if (_weaponData?.hitSFX != null)
            {
                AudioSource.PlayClipAtPoint(_weaponData.hitSFX, target.transform.position);
            }
            
            // Apply element visual effect
            if (_elementIntegrator?.CurrentElement != null)
            {
                var element = _elementIntegrator.CurrentElement;
                if (element.hitVFX != null)
                {
                    Instantiate(element.hitVFX, target.transform.position, Quaternion.identity);
                }
            }
        }
        
        private void OnElementChanged(ElementData element)
        {
            UpdateElementVisuals(element);
        }
        
        private void UpdateElementVisuals(ElementData element)
        {
            if (element == null)
            {
                // Reset to default
                if (_trailRenderer != null)
                {
                    _trailRenderer.startColor = Color.white;
                    _trailRenderer.endColor = Color.clear;
                }
                
                if (_elementParticles != null)
                {
                    _elementParticles.Stop();
                }
            }
            else
            {
                // Apply element colors
                if (_trailRenderer != null)
                {
                    _trailRenderer.startColor = element.primaryColor;
                    _trailRenderer.endColor = element.secondaryColor;
                }
                
                if (_elementParticles != null)
                {
                    var main = _elementParticles.main;
                    main.startColor = element.primaryColor;
                    _elementParticles.Play();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (_elementIntegrator != null)
            {
                _elementIntegrator.OnElementChanged -= OnElementChanged;
            }
        }
    }
}
