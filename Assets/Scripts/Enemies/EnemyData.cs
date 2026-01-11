using UnityEngine;

namespace EElemental.Enemies
{
    /// <summary>
    /// Düşman verileri için ScriptableObject.
    /// Her düşman tipi için bir tane oluşturulur.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "EElemental/Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string EnemyName = "Enemy";
        public string Description = "";
        public Sprite Icon;
        
        [Header("Health")]
        [Min(1)] public float MaxHealth = 100f;
        
        [Header("Movement")]
        [Min(0)] public float MoveSpeed = 3f;
        [Min(0)] public float PatrolSpeed = 1.5f;
        [Min(0)] public float ChaseSpeed = 4f;
        
        [Header("Combat")]
        [Min(0)] public float AttackDamage = 10f;
        [Min(0)] public float AttackRange = 1.5f;
        [Min(0)] public float AttackCooldown = 1f;
        [Range(0, 10)] public int AttackComboCount = 1;
        
        [Header("Detection")]
        [Min(0)] public float DetectionRange = 8f;
        [Min(0)] public float ChaseRange = 12f;
        [Min(0)] public float LoseTargetRange = 15f;
        [Range(0, 360)] public float DetectionAngle = 180f;
        
        [Header("Patrol")]
        public PatrolType PatrolBehavior = PatrolType.BackAndForth;
        [Min(0)] public float PatrolWaitTime = 2f;
        [Min(0)] public float PatrolDistance = 5f;
        
        [Header("Knockback")]
        public bool CanBeKnockedBack = true;
        [Range(0, 2)] public float KnockbackResistance = 1f;
        
        [Header("Stun")]
        public bool CanBeStunned = true;
        [Range(0, 2)] public float StunResistance = 1f;
        
        [Header("Element")]
        public EElemental.Elements.ElementData ElementType;
        public bool IsElemental = false;
        
        [Header("Loot")]
        [Min(0)] public int ExperienceValue = 10;
        [Range(0, 100)] public float DropChance = 50f;
        // public LootTable LootTable; // Future implementation
        
        [Header("Audio")]
        public AudioClip HurtSound;
        public AudioClip DeathSound;
        public AudioClip AttackSound;
        public AudioClip DetectSound;
        
        [Header("Visual")]
        public RuntimeAnimatorController AnimatorController;
        public GameObject DeathEffect;
        public GameObject HitEffect;
    }
    
    public enum PatrolType
    {
        None,           // Yerinde durur
        BackAndForth,   // İki nokta arasında gidip gelir
        Circular,       // Dairesel hareket
        Random,         // Rastgele hareket
        Waypoints       // Belirlenen noktaları takip eder
    }
}
