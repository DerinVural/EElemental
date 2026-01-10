using UnityEngine;

namespace EElemental.Combat
{
    /// <summary>
    /// ScriptableObject defining a combo sequence.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCombo", menuName = "EElemental/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("Identity")]
        public string comboName;
        
        [Header("Sequence")]
        [Tooltip("Input sequence required to perform this combo")]
        public AttackInput[] sequence;
        
        [Tooltip("Attacks that play for each step in the sequence")]
        public AttackData[] attacks;
        
        [Header("Requirements")]
        [Tooltip("Can this combo be started from idle?")]
        public bool canStartFromIdle = true;
        
        [Tooltip("Can this combo be started while airborne?")]
        public bool canStartInAir = false;
        
        [Header("Bonus")]
        [Tooltip("Damage multiplier for final hit")]
        public float finisherMultiplier = 1.5f;
        
        /// <summary>
        /// Check if input sequence matches this combo.
        /// </summary>
        public bool MatchesSequence(AttackInput[] inputs)
        {
            if (inputs == null || inputs.Length == 0) return false;
            if (inputs.Length > sequence.Length) return false;
            
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] != sequence[i]) return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get the attack for a specific step in the combo.
        /// </summary>
        public AttackData GetAttackAtStep(int step)
        {
            if (step < 0 || step >= attacks.Length) return null;
            return attacks[step];
        }
        
        /// <summary>
        /// Check if this is the final hit in the combo.
        /// </summary>
        public bool IsFinalHit(int step)
        {
            return step >= attacks.Length - 1;
        }
    }
    
    public enum AttackInput
    {
        Light,
        Heavy,
        Special
    }
}
