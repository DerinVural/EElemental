using UnityEngine;
using UnityEngine.InputSystem;

namespace EElemental.Player
{
    /// <summary>
    /// Handles player input using the new Input System.
    /// Provides input state for other components.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float _inputBufferTime = 0.1f;
        
        // Input State
        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool DashPressed { get; private set; }
        public bool LightAttackPressed { get; private set; }
        public bool HeavyAttackPressed { get; private set; }
        public bool SpecialPressed { get; private set; }
        
        // Buffered inputs
        private float _jumpBufferTimer;
        private float _dashBufferTimer;
        private float _lightAttackBufferTimer;
        private float _heavyAttackBufferTimer;
        
        // Components
        private PlayerInput _playerInput;
        
        // Events
        public event System.Action OnJumpInput;
        public event System.Action OnDashInput;
        public event System.Action OnLightAttackInput;
        public event System.Action OnHeavyAttackInput;
        public event System.Action OnSpecialInput;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        
        private void Update()
        {
            UpdateBufferTimers();
            ResetPressedStates();
        }
        
        private void UpdateBufferTimers()
        {
            if (_jumpBufferTimer > 0) _jumpBufferTimer -= Time.deltaTime;
            if (_dashBufferTimer > 0) _dashBufferTimer -= Time.deltaTime;
            if (_lightAttackBufferTimer > 0) _lightAttackBufferTimer -= Time.deltaTime;
            if (_heavyAttackBufferTimer > 0) _heavyAttackBufferTimer -= Time.deltaTime;
        }
        
        private void ResetPressedStates()
        {
            JumpPressed = false;
            DashPressed = false;
            LightAttackPressed = false;
            HeavyAttackPressed = false;
            SpecialPressed = false;
        }
        
        #region Input Callbacks
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                JumpPressed = true;
                JumpHeld = true;
                _jumpBufferTimer = _inputBufferTime;
                OnJumpInput?.Invoke();
            }
            else if (context.canceled)
            {
                JumpHeld = false;
            }
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                DashPressed = true;
                _dashBufferTimer = _inputBufferTime;
                OnDashInput?.Invoke();
            }
        }
        
        public void OnLightAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                LightAttackPressed = true;
                _lightAttackBufferTimer = _inputBufferTime;
                OnLightAttackInput?.Invoke();
            }
        }
        
        public void OnHeavyAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                HeavyAttackPressed = true;
                _heavyAttackBufferTimer = _inputBufferTime;
                OnHeavyAttackInput?.Invoke();
            }
        }
        
        public void OnSpecial(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SpecialPressed = true;
                OnSpecialInput?.Invoke();
            }
        }
        
        #endregion
        
        #region Buffer Consumption
        
        public bool ConsumeJumpBuffer()
        {
            if (_jumpBufferTimer > 0)
            {
                _jumpBufferTimer = 0;
                return true;
            }
            return false;
        }
        
        public bool ConsumeDashBuffer()
        {
            if (_dashBufferTimer > 0)
            {
                _dashBufferTimer = 0;
                return true;
            }
            return false;
        }
        
        public bool ConsumeLightAttackBuffer()
        {
            if (_lightAttackBufferTimer > 0)
            {
                _lightAttackBufferTimer = 0;
                return true;
            }
            return false;
        }
        
        public bool ConsumeHeavyAttackBuffer()
        {
            if (_heavyAttackBufferTimer > 0)
            {
                _heavyAttackBufferTimer = 0;
                return true;
            }
            return false;
        }
        
        #endregion
    }
}
