using UnityEngine;

namespace EElemental.Combat
{
    /// <summary>
    /// Controls invincibility frames (I-frames) for dodge/dash mechanics.
    /// </summary>
    public class IFrameController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _defaultDuration = 0.3f;
        
        [Header("Visual Feedback")]
        [SerializeField] private bool _flashDuringIFrames = true;
        [SerializeField] private float _flashInterval = 0.1f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private float _iFrameTimer;
        private float _flashTimer;
        private bool _spriteVisible = true;
        
        public bool IsInvincible => _iFrameTimer > 0;
        public float RemainingTime => _iFrameTimer;
        
        // Events
        public event System.Action OnIFrameStart;
        public event System.Action OnIFrameEnd;
        
        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        private void Update()
        {
            if (_iFrameTimer <= 0) return;
            
            _iFrameTimer -= Time.deltaTime;
            
            // Flash effect
            if (_flashDuringIFrames && _spriteRenderer != null)
            {
                _flashTimer -= Time.deltaTime;
                if (_flashTimer <= 0)
                {
                    _flashTimer = _flashInterval;
                    _spriteVisible = !_spriteVisible;
                    SetSpriteVisible(_spriteVisible);
                }
            }
            
            // End i-frames
            if (_iFrameTimer <= 0)
            {
                EndIFrames();
            }
        }
        
        /// <summary>
        /// Start invincibility frames.
        /// </summary>
        public void StartIFrames(float duration = -1f)
        {
            if (duration < 0) duration = _defaultDuration;
            
            _iFrameTimer = duration;
            _flashTimer = _flashInterval;
            
            OnIFrameStart?.Invoke();
        }
        
        /// <summary>
        /// Force stop invincibility frames.
        /// </summary>
        public void StopIFrames()
        {
            _iFrameTimer = 0;
            EndIFrames();
        }
        
        private void EndIFrames()
        {
            _iFrameTimer = 0;
            SetSpriteVisible(true);
            _spriteVisible = true;
            
            OnIFrameEnd?.Invoke();
        }
        
        private void SetSpriteVisible(bool visible)
        {
            if (_spriteRenderer != null)
            {
                Color c = _spriteRenderer.color;
                c.a = visible ? 1f : 0.3f;
                _spriteRenderer.color = c;
            }
        }
        
        /// <summary>
        /// Add additional I-frame time.
        /// </summary>
        public void ExtendIFrames(float additionalTime)
        {
            _iFrameTimer += additionalTime;
        }
    }
}
