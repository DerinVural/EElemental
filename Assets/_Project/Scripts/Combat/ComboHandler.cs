using System.Collections.Generic;
using UnityEngine;

namespace EElemental.Combat
{
    /// <summary>
    /// Handles combo detection and progression.
    /// </summary>
    public class ComboHandler : MonoBehaviour
    {
        [Header("Combos")]
        [SerializeField] private ComboData[] _availableCombos;
        
        [Header("Settings")]
        [SerializeField] private float _comboWindowTime = 0.5f;
        [SerializeField] private int _maxInputBuffer = 5;
        
        private List<AttackInput> _inputBuffer = new List<AttackInput>();
        private float _lastInputTime;
        private int _currentComboStep = 0;
        private ComboData _currentCombo;
        
        // Events
        public event System.Action<AttackData> OnAttackTriggered;
        public event System.Action<ComboData> OnComboStarted;
        public event System.Action<ComboData> OnComboCompleted;
        public event System.Action OnComboDropped;
        
        public ComboData CurrentCombo => _currentCombo;
        public int CurrentStep => _currentComboStep;
        public bool IsInCombo => _currentCombo != null;
        
        private void Update()
        {
            // Check if combo window expired
            if (_inputBuffer.Count > 0 && Time.time - _lastInputTime > _comboWindowTime)
            {
                DropCombo();
            }
        }
        
        /// <summary>
        /// Register an attack input.
        /// </summary>
        public AttackData RegisterInput(AttackInput input)
        {
            float currentTime = Time.time;
            
            // Reset if window expired
            if (currentTime - _lastInputTime > _comboWindowTime)
            {
                ResetBuffer();
            }
            
            // Add to buffer
            _inputBuffer.Add(input);
            _lastInputTime = currentTime;
            
            // Trim buffer if too long
            if (_inputBuffer.Count > _maxInputBuffer)
            {
                _inputBuffer.RemoveAt(0);
            }
            
            // Find matching combo and get next attack
            return GetNextComboAttack();
        }
        
        private AttackData GetNextComboAttack()
        {
            AttackInput[] currentSequence = _inputBuffer.ToArray();
            
            // First, check if current combo still matches
            if (_currentCombo != null && _currentCombo.MatchesSequence(currentSequence))
            {
                _currentComboStep++;
                
                // Check if combo completed
                if (_currentComboStep >= _currentCombo.attacks.Length)
                {
                    var finalAttack = _currentCombo.GetAttackAtStep(_currentCombo.attacks.Length - 1);
                    CompleteCombo();
                    return finalAttack;
                }
                
                var attack = _currentCombo.GetAttackAtStep(_currentComboStep);
                OnAttackTriggered?.Invoke(attack);
                return attack;
            }
            
            // Look for a new matching combo
            foreach (var combo in _availableCombos)
            {
                if (combo.MatchesSequence(currentSequence))
                {
                    // Start this combo
                    if (_currentCombo != combo)
                    {
                        _currentCombo = combo;
                        _currentComboStep = currentSequence.Length - 1;
                        OnComboStarted?.Invoke(combo);
                    }
                    
                    var attack = combo.GetAttackAtStep(_currentComboStep);
                    OnAttackTriggered?.Invoke(attack);
                    return attack;
                }
            }
            
            // No combo matches - reset and return first attack if available
            if (_currentCombo == null && _inputBuffer.Count == 1)
            {
                // Try to find a combo starting with this input
                foreach (var combo in _availableCombos)
                {
                    if (combo.sequence.Length > 0 && combo.sequence[0] == _inputBuffer[0])
                    {
                        _currentCombo = combo;
                        _currentComboStep = 0;
                        OnComboStarted?.Invoke(combo);
                        
                        var attack = combo.GetAttackAtStep(0);
                        OnAttackTriggered?.Invoke(attack);
                        return attack;
                    }
                }
            }
            
            DropCombo();
            return null;
        }
        
        private void CompleteCombo()
        {
            if (_currentCombo != null)
            {
                OnComboCompleted?.Invoke(_currentCombo);
            }
            ResetBuffer();
        }
        
        private void DropCombo()
        {
            if (_currentCombo != null)
            {
                OnComboDropped?.Invoke();
            }
            ResetBuffer();
        }
        
        private void ResetBuffer()
        {
            _inputBuffer.Clear();
            _currentCombo = null;
            _currentComboStep = 0;
        }
        
        /// <summary>
        /// Force reset the combo state.
        /// </summary>
        public void ForceReset()
        {
            ResetBuffer();
        }
    }
}
