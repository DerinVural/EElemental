namespace EElemental.Elements.Effects
{
    /// <summary>
    /// Interface for objects that can receive damage.
    /// </summary>
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        
        void TakeDamage(DamageInfo damage);
        void Heal(float amount);
    }
    
    /// <summary>
    /// Interface for objects that can move.
    /// </summary>
    public interface IMoveable
    {
        float MoveSpeed { get; set; }
        bool CanMove { get; set; }
    }
    
    /// <summary>
    /// Interface for objects that can be stunned.
    /// </summary>
    public interface IStunnable
    {
        bool IsStunned { get; }
        void SetStunned(bool stunned);
        void InterruptCurrentAction();
    }
    
    /// <summary>
    /// Interface for objects that can be knocked back.
    /// </summary>
    public interface IKnockbackable
    {
        void OnKnockbackStart();
        void OnKnockbackEnd();
    }
    
    /// <summary>
    /// Damage information structure.
    /// </summary>
    public struct DamageInfo
    {
        public float amount;
        public DamageType damageType;
        public ElementType element;
        public bool isCrit;
        public UnityEngine.Vector2 knockbackDirection;
        public float knockbackForce;
        public int hitstopFrames;
        public UnityEngine.GameObject attacker;
    }
    
    /// <summary>
    /// Types of damage.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Fire,
        Water,
        Earth,
        Air,
        True // Ignores defense
    }
}
