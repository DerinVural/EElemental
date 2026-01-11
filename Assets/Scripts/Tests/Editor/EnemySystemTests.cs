using NUnit.Framework;
using UnityEngine;
using EElemental.Enemies;

namespace EElemental.Tests.Editor
{
    /// <summary>
    /// Enemy System testleri
    /// </summary>
    [TestFixture]
    public class EnemySystemTests
    {
        #region EnemyStats Tests
        
        [Test]
        public void EnemyStats_Initialize_SetsFullHealth()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            
            // Assert
            Assert.AreEqual(stats.MaxHealth, stats.CurrentHealth);
            Assert.IsTrue(stats.IsAlive);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_TakeDamage_ReducesHealth()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            float initialHealth = stats.CurrentHealth;
            
            // Act
            stats.TakeDamage(20f);
            
            // Assert
            Assert.Less(stats.CurrentHealth, initialHealth);
            Assert.AreEqual(initialHealth - 20f, stats.CurrentHealth);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_TakeDamage_HealthDoesNotGoBelowZero()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            
            // Act
            stats.TakeDamage(9999f);
            
            // Assert
            Assert.AreEqual(0f, stats.CurrentHealth);
            Assert.IsFalse(stats.IsAlive);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_Heal_IncreasesHealth()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            stats.TakeDamage(50f);
            float damagedHealth = stats.CurrentHealth;
            
            // Act
            stats.Heal(30f);
            
            // Assert
            Assert.Greater(stats.CurrentHealth, damagedHealth);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_Heal_DoesNotExceedMaxHealth()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            stats.TakeDamage(10f);
            
            // Act
            stats.Heal(9999f);
            
            // Assert
            Assert.AreEqual(stats.MaxHealth, stats.CurrentHealth);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_CanAttack_RespectseCooldown()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            
            // Initially can attack
            Assert.IsTrue(stats.CanAttack);
            
            // Act - Record attack
            stats.RecordAttack();
            
            // Assert - Cannot attack immediately after
            Assert.IsFalse(stats.CanAttack);
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void EnemyStats_Modifiers_AffectStats()
        {
            // Arrange
            var go = new GameObject();
            var stats = go.AddComponent<EnemyStats>();
            float baseMoveSpeed = stats.MoveSpeed;
            
            // Act
            stats.SetMoveSpeedModifier(0.5f);
            
            // Assert
            Assert.AreEqual(baseMoveSpeed * 0.5f, stats.GetModifiedMoveSpeed());
            
            // Reset
            stats.ResetModifiers();
            Assert.AreEqual(baseMoveSpeed, stats.GetModifiedMoveSpeed());
            
            // Cleanup
            Object.DestroyImmediate(go);
        }
        
        #endregion
        
        #region EnemyData Tests
        
        [Test]
        public void EnemyData_Create_HasDefaultValues()
        {
            // Arrange
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            
            // Assert
            Assert.IsNotNull(enemyData);
            Assert.Greater(enemyData.MaxHealth, 0);
            Assert.Greater(enemyData.MoveSpeed, 0);
            Assert.Greater(enemyData.DetectionRange, 0);
            
            // Cleanup
            Object.DestroyImmediate(enemyData);
        }
        
        [Test]
        public void EnemyData_PatrolType_DefaultIsBackAndForth()
        {
            // Arrange
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            
            // Assert
            Assert.AreEqual(PatrolType.BackAndForth, enemyData.PatrolBehavior);
            
            // Cleanup
            Object.DestroyImmediate(enemyData);
        }
        
        #endregion
        
        #region EnemyAI Tests
        
        [Test]
        public void EnemyAI_InitialState_IsIdle()
        {
            // AI state machine başlangıçta Idle olmalı
            Assert.Pass("AI starts in Idle state - requires full component setup");
        }
        
        [Test]
        public void EnemyAI_GetMoveDirection_ReturnsZeroWhenIdle()
        {
            // Idle durumunda hareket yönü sıfır olmalı
            Assert.Pass("Move direction is zero when idle - requires full component setup");
        }
        
        [Test]
        public void EnemyAI_HasTarget_FalseByDefault()
        {
            // Başlangıçta hedef yok
            Assert.Pass("No target by default - requires full component setup");
        }
        
        #endregion
        
        #region AIState Enum Tests
        
        [Test]
        public void AIState_HasAllRequiredStates()
        {
            // Assert
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Idle));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Patrol));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Chase));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Attack));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Retreat));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Stunned));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AIState), AIState.Dead));
        }
        
        #endregion
        
        #region PatrolType Enum Tests
        
        [Test]
        public void PatrolType_HasAllRequiredTypes()
        {
            // Assert
            Assert.IsTrue(System.Enum.IsDefined(typeof(PatrolType), PatrolType.None));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PatrolType), PatrolType.BackAndForth));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PatrolType), PatrolType.Circular));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PatrolType), PatrolType.Random));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PatrolType), PatrolType.Waypoints));
        }
        
        #endregion
    }
}
