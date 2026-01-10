using UnityEngine;

namespace EElemental.Elements
{
    /// <summary>
    /// ScriptableObject that defines an element's properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewElement", menuName = "EElemental/Element Data")]
    public class ElementData : ScriptableObject
    {
        [Header("Identity")]
        public string elementName;
        public ElementType type;
        public Sprite icon;
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
        
        [Header("Combat Stats")]
        [Tooltip("Multiplier applied to base weapon damage")]
        public float damageMultiplier = 1f;
        
        [Tooltip("Bonus critical hit chance (0-1)")]
        [Range(0f, 1f)]
        public float critChanceBonus = 0f;
        
        [Tooltip("Bonus critical damage multiplier")]
        public float critDamageBonus = 0f;
        
        [Header("Status Effect")]
        public StatusEffectType statusEffect;
        
        [Tooltip("Chance to apply status effect (0-1)")]
        [Range(0f, 1f)]
        public float effectChance = 0.3f;
        
        [Tooltip("Duration of status effect in seconds")]
        public float effectDuration = 3f;
        
        [Tooltip("Strength/intensity of the effect")]
        public float effectStrength = 1f;
        
        [Header("Combinations")]
        public ElementCombination[] combinations;
        
        [Header("VFX/SFX")]
        public GameObject hitVFX;
        public GameObject trailVFX;
        public AudioClip hitSFX;
        public AudioClip applySFX;
    }
    
    [System.Serializable]
    public class ElementCombination
    {
        public ElementType combinesWith;
        public ElementData result;
    }
    
    public enum ElementType
    {
        None = 0,
        
        // Base Elements
        Fire = 1,
        Water = 2,
        Earth = 3,
        Air = 4,
        
        // Combined Elements
        Steam = 10,     // Fire + Water
        Magma = 11,     // Fire + Earth
        Inferno = 12,   // Fire + Air
        Mud = 13,       // Water + Earth
        Ice = 14,       // Water + Air
        Sand = 15       // Earth + Air
    }
    
    public enum StatusEffectType
    {
        None,
        Burn,       // Fire - DoT
        Slow,       // Water - Movement/Attack speed reduction
        Stun,       // Earth - Cannot act
        Knockback,  // Air - Push away
        Blind,      // Steam - Miss chance
        Root,       // Mud - Cannot move
        Freeze,     // Ice - Frozen solid
        Bleed       // Sand - DoT + vision impair
    }
}
