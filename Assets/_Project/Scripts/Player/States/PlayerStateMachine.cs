using UnityEngine;
using EElemental.Core;
using EElemental.Combat;

namespace EElemental.Player.States
{
    /// <summary>
    /// Player-specific state machine.
    /// Manages all player states and provides access to player components.
    /// </summary>
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlayerController _controller;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private Animator _animator;
        [SerializeField] private HitboxManager _hitboxManager;
        [SerializeField] private HurtboxManager _hurtboxManager;
        [SerializeField] private IFrameController _iFrameController;
        [SerializeField] private ComboHandler _comboHandler;
        [SerializeField] private InputBuffer _inputBuffer;
        
        // State Machine
        private StateMachine _stateMachine;
        
        // States
        public PlayerIdleState IdleState { get; private set; }
        public PlayerRunState RunState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerHurtState HurtState { get; private set; }
        public PlayerDeathState DeathState { get; private set; }
        
        // Component Accessors
        public PlayerController Controller => _controller;
        public PlayerMovement Movement => _movement;
        public PlayerStats Stats => _stats;
        public Animator Animator => _animator;
        public HitboxManager HitboxManager => _hitboxManager;
        public HurtboxManager HurtboxManager => _hurtboxManager;
        public IFrameController IFrameController => _iFrameController;
        public ComboHandler ComboHandler => _comboHandler;
        public InputBuffer InputBuffer => _inputBuffer;
        
        // State accessors
        public IState CurrentState => _stateMachine.CurrentState;
        public IState PreviousState => _stateMachine.PreviousState;
        
        private void Awake()
        {
            // Get components if not assigned
            if (_controller == null) _controller = GetComponent<PlayerController>();
            if (_movement == null) _movement = GetComponent<PlayerMovement>();
            if (_stats == null) _stats = GetComponent<PlayerStats>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            if (_hitboxManager == null) _hitboxManager = GetComponentInChildren<HitboxManager>();
            if (_hurtboxManager == null) _hurtboxManager = GetComponentInChildren<HurtboxManager>();
            if (_iFrameController == null) _iFrameController = GetComponent<IFrameController>();
            if (_comboHandler == null) _comboHandler = GetComponent<ComboHandler>();
            if (_inputBuffer == null) _inputBuffer = GetComponent<InputBuffer>();
            
            // Create state machine
            _stateMachine = new StateMachine();
            
            // Create states
            IdleState = new PlayerIdleState(this);
            RunState = new PlayerRunState(this);
            JumpState = new PlayerJumpState(this);
            FallState = new PlayerFallState(this);
            DashState = new PlayerDashState(this);
            AttackState = new PlayerAttackState(this);
            HurtState = new PlayerHurtState(this);
            DeathState = new PlayerDeathState(this);
        }
        
        private void Start()
        {
            // Subscribe to events
            if (_hurtboxManager != null)
            {
                _hurtboxManager.OnDamageReceived += OnDamageReceived;
            }
            
            if (_stats != null)
            {
                _stats.OnDeath += OnPlayerDeath;
            }
            
            // Initialize state machine
            _stateMachine.Initialize(IdleState);
        }
        
        private void Update()
        {
            _stateMachine.Execute();
        }
        
        private void FixedUpdate()
        {
            _stateMachine.FixedExecute();
        }
        
        public void TransitionTo(IState newState)
        {
            _stateMachine.TransitionTo(newState);
        }
        
        private void OnDamageReceived(DamageInfo damage)
        {
            if (_iFrameController != null && _iFrameController.IsInvincible) return;
            
            _stats?.TakeDamage(damage.amount);
            
            // Transition to hurt state if not dead
            if (_stats.IsAlive && CurrentState != HurtState)
            {
                TransitionTo(HurtState);
            }
        }
        
        private void OnPlayerDeath()
        {
            TransitionTo(DeathState);
        }
        
        private void OnDestroy()
        {
            if (_hurtboxManager != null)
            {
                _hurtboxManager.OnDamageReceived -= OnDamageReceived;
            }
            
            if (_stats != null)
            {
                _stats.OnDeath -= OnPlayerDeath;
            }
        }
        
        #region Animation Helpers
        
        public void SetAnimatorBool(string param, bool value)
        {
            if (_animator != null) _animator.SetBool(param, value);
        }
        
        public void SetAnimatorTrigger(string param)
        {
            if (_animator != null) _animator.SetTrigger(param);
        }
        
        public void SetAnimatorFloat(string param, float value)
        {
            if (_animator != null) _animator.SetFloat(param, value);
        }
        
        #endregion
    }
}
