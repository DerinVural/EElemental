using UnityEngine;

namespace EElemental.Player
{
    /// <summary>
    /// Handles player movement physics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _acceleration = 50f;
        [SerializeField] private float _deceleration = 50f;
        [SerializeField] private float _velocityPower = 0.9f;
        
        [Header("Jump")]
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _jumpCutMultiplier = 0.5f;
        [SerializeField] private float _fallGravityMultiplier = 2.5f;
        [SerializeField] private float _maxFallSpeed = 20f;
        [SerializeField] private float _coyoteTime = 0.1f;
        [SerializeField] private float _jumpBufferTime = 0.1f;
        
        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 20f;
        [SerializeField] private float _dashDuration = 0.15f;
        [SerializeField] private float _dashCooldown = 0.5f;
        
        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.8f, 0.1f);
        [SerializeField] private LayerMask _groundLayer;
        
        // State
        public bool IsGrounded { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsFalling => _rb.velocity.y < 0 && !IsGrounded;
        public bool CanDash => !IsDashing && _dashCooldownTimer <= 0;
        public int FacingDirection { get; private set; } = 1;
        
        // Components
        private Rigidbody2D _rb;
        
        // Timers
        private float _coyoteTimeCounter;
        private float _dashTimer;
        private float _dashCooldownTimer;
        
        // Cached
        private Vector2 _dashDirection;
        private float _defaultGravity;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _defaultGravity = _rb.gravityScale;
        }
        
        private void FixedUpdate()
        {
            CheckGround();
            ApplyGravityModifiers();
            UpdateTimers();
        }
        
        private void UpdateTimers()
        {
            if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.fixedDeltaTime;
            
            if (IsGrounded)
                _coyoteTimeCounter = _coyoteTime;
            else
                _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        
        private void CheckGround()
        {
            if (_groundCheck == null) return;
            
            IsGrounded = Physics2D.OverlapBox(
                _groundCheck.position, 
                _groundCheckSize, 
                0f, 
                _groundLayer
            );
        }
        
        private void ApplyGravityModifiers()
        {
            if (IsDashing) return;
            
            if (IsFalling)
            {
                _rb.gravityScale = _defaultGravity * _fallGravityMultiplier;
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_maxFallSpeed));
            }
            else
            {
                _rb.gravityScale = _defaultGravity;
            }
        }
        
        #region Movement Methods
        
        public void Move(float inputX)
        {
            if (IsDashing) return;
            
            // Update facing direction
            if (inputX != 0)
            {
                FacingDirection = inputX > 0 ? 1 : -1;
            }
            
            // Calculate target speed
            float targetSpeed = inputX * _moveSpeed;
            float speedDiff = targetSpeed - _rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _deceleration;
            
            // Apply acceleration with velocity power for snappier feel
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, _velocityPower) * Mathf.Sign(speedDiff);
            
            _rb.AddForce(movement * Vector2.right);
        }
        
        public void StopHorizontalMovement()
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        
        #endregion
        
        #region Jump Methods
        
        public bool CanJump()
        {
            return _coyoteTimeCounter > 0;
        }
        
        public void Jump()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            _coyoteTimeCounter = 0;
        }
        
        public void CutJump()
        {
            if (_rb.velocity.y > 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _jumpCutMultiplier);
            }
        }
        
        #endregion
        
        #region Dash Methods
        
        public void StartDash(Vector2 direction)
        {
            if (!CanDash) return;
            
            IsDashing = true;
            _dashTimer = _dashDuration;
            _dashDirection = direction.normalized;
            
            if (_dashDirection == Vector2.zero)
            {
                _dashDirection = new Vector2(FacingDirection, 0);
            }
            
            _rb.gravityScale = 0;
            _rb.velocity = _dashDirection * _dashSpeed;
        }
        
        public void UpdateDash()
        {
            if (!IsDashing) return;
            
            _dashTimer -= Time.fixedDeltaTime;
            
            if (_dashTimer <= 0)
            {
                EndDash();
            }
        }
        
        public void EndDash()
        {
            IsDashing = false;
            _dashCooldownTimer = _dashCooldown;
            _rb.gravityScale = _defaultGravity;
            _rb.velocity = new Vector2(_rb.velocity.x * 0.5f, 0);
        }
        
        #endregion
        
        #region Utility
        
        public void SetVelocity(Vector2 velocity)
        {
            _rb.velocity = velocity;
        }
        
        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Impulse)
        {
            _rb.AddForce(force, mode);
        }
        
        public void Flip()
        {
            FacingDirection *= -1;
            transform.localScale = new Vector3(FacingDirection, 1, 1);
        }
        
        #endregion
        
        private void OnDrawGizmosSelected()
        {
            if (_groundCheck == null) return;
            
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
        }
    }
}
