using UnityEngine;

namespace EElemental.Core
{
    /// <summary>
    /// Core data structures and enums used throughout the game.
    /// </summary>

    /// <summary>
    /// Game state enum for managing high-level game flow.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        LoadingRun,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    /// <summary>
    /// Damage information struct passed between combat systems.
    /// </summary>
    public struct DamageInfo
    {
        public float Amount;
        public Elements.ElementType Element;
        public Vector2 KnockbackDirection;
        public float KnockbackForce;
        public bool IsCritical;
        public GameObject Source;

        public DamageInfo(float amount, Elements.ElementType element = Elements.ElementType.None,
                         Vector2 knockbackDir = default, float knockbackForce = 0f,
                         bool isCritical = false, GameObject source = null)
        {
            Amount = amount;
            Element = element;
            KnockbackDirection = knockbackDir;
            KnockbackForce = knockbackForce;
            IsCritical = isCritical;
            Source = source;
        }
    }

    /// <summary>
    /// Healing information struct.
    /// </summary>
    public struct HealInfo
    {
        public float Amount;
        public bool IsPercentage;
        public GameObject Source;

        public HealInfo(float amount, bool isPercentage = false, GameObject source = null)
        {
            Amount = amount;
            IsPercentage = isPercentage;
            Source = source;
        }
    }

    /// <summary>
    /// Direction enum for movement and facing.
    /// </summary>
    public enum Direction
    {
        Left = -1,
        Right = 1
    }

    /// <summary>
    /// Attack input type for combo system.
    /// </summary>
    public enum AttackInput
    {
        None,
        Light,
        Heavy
    }

    /// <summary>
    /// Attack type classification.
    /// </summary>
    public enum AttackType
    {
        Light,
        Heavy,
        Special,
        Ultimate
    }

    /// <summary>
    /// Layer mask helper for common collision checks.
    /// </summary>
    public static class GameLayers
    {
        public const string Ground = "Ground";
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Projectile = "Projectile";
        public const string Wall = "Wall";
        public const string Platform = "Platform";

        private static int? _groundMask;
        private static int? _enemyMask;
        private static int? _playerMask;

        public static int GroundMask => _groundMask ??= LayerMask.GetMask(Ground, Platform);
        public static int EnemyMask => _enemyMask ??= LayerMask.GetMask(Enemy);
        public static int PlayerMask => _playerMask ??= LayerMask.GetMask(Player);
    }

    /// <summary>
    /// Tag constants for GameObject identification.
    /// </summary>
    public static class GameTags
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Ground = "Ground";
        public const string Hazard = "Hazard";
        public const string Pickup = "Pickup";
    }
}

namespace EElemental.Elements
{
    /// <summary>
    /// Element types used in the game.
    /// </summary>
    public enum ElementType
    {
        None,
        Fire,
        Water,
        Earth,
        Air,

        // Combination elements
        Steam,      // Fire + Water
        Lava,       // Fire + Earth
        Lightning,  // Fire + Air
        Ice,        // Water + Air
        Mud,        // Water + Earth
        Dust        // Earth + Air
    }

    /// <summary>
    /// Status effect types.
    /// </summary>
    public enum StatusEffectType
    {
        None,
        Burn,
        Slow,
        Stun,
        Knockback,
        Poison,
        Freeze,
        Shock
    }
}
