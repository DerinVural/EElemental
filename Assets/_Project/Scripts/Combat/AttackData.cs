using UnityEngine;
using EElemental.Elements;

namespace EElemental.Combat
{
    /// <summary>
    /// ScriptableObject defining attack properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAttack", menuName = "EElemental/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [Header("Identity")]
        public string attackName;
        public AttackType type = AttackType.Light;
        
        [Header("Timing (frames at 60fps)")]
        [Tooltip("Wind-up frames before hitbox becomes active")]
        public int startupFrames = 5;
        
        [Tooltip("Frames where hitbox is active and can deal damage")]
        public int activeFrames = 3;
        
        [Tooltip("Recovery frames after active frames end")]
        public int recoveryFrames = 10;
        
        [Header("Damage")]
        public float baseDamage = 10f;
        public float critMultiplier = 1.5f;
        [Range(0f, 1f)]
        public float critChance = 0.1f;
        
        [Header("Knockback")]
        public Vector2 knockbackDirection = Vector2.right;
        public float knockbackForce = 5f;
        
        [Header("Hitstop")]
        [Tooltip("Frames to pause on hit for impact feel")]
        public int hitstopFrames = 3;
        public float screenShakeIntensity = 0.1f;
        
        [Header("Cancels")]
        public bool canCancelIntoLight = false;
        public bool canCancelIntoHeavy = false;
        public bool canCancelIntoDash = true;
        [Tooltip("Frame where cancel window opens")]
        public int cancelWindowStart = 8;
        
        [Header("Hitbox")]
        public Vector2 hitboxOffset = new Vector2(0.5f, 0f);
        public Vector2 hitboxSize = new Vector2(1f, 1f);
        
        [Header("Animation")]
        public string animationTrigger;
        
        [Header("VFX/SFX")]
        public GameObject hitVFX;
        public GameObject swingVFX;
        public AudioClip hitSFX;
        public AudioClip swingSFX;
        
        /// <summary>
        /// Total frames for this attack.
        /// </summary>
        public int TotalFrames => startupFrames + activeFrames + recoveryFrames;
        
        /// <summary>
        /// Check if we're in the active frames.
        /// </summary>
        public bool IsActiveFrame(int currentFrame)
        {
            return currentFrame >= startupFrames && 
                   currentFrame < startupFrames + activeFrames;
        }
        
        /// <summary>
        /// Check if we're in the cancel window.
        /// </summary>
        public bool CanCancelAtFrame(int currentFrame)
        {
            return currentFrame >= cancelWindowStart;
        }
    }
    
    public enum AttackType
    {
        Light,
        Heavy,
        Special,
        Air,
        Dash,
        Finisher
    }
}
