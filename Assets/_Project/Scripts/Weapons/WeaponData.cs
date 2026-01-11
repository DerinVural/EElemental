using UnityEngine;
using EElemental.Elements;

namespace EElemental.Weapons
{
    /// <summary>
    /// ScriptableObject that defines a weapon's base properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "EElemental/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponName;
        public WeaponType type;
        public Sprite icon;
        public GameObject prefab;
        
        [Header("Base Stats")]
        public float baseDamage = 10f;
        public float attackSpeed = 1f;
        public float range = 1.5f;
        
        [Header("Critical")]
        [Range(0f, 1f)]
        public float critChance = 0.05f;
        public float critMultiplier = 1.5f;
        
        [Header("Knockback")]
        public float knockbackForce = 5f;
        
        [Header("Combo")]
        public Combat.ComboData[] combos;
        
        [Header("Element Compatibility")]
        public ElementType[] compatibleElements;
        public float elementDamageBonus = 0.2f; // 20% bonus when element matches
        
        [Header("VFX/SFX")]
        public GameObject swingVFX;
        public AudioClip swingSFX;
        public AudioClip hitSFX;
    }
    
    public enum WeaponType
    {
        Sword,
        Axe,
        Spear,
        Bow,
        Staff,
        Dagger
    }
}
