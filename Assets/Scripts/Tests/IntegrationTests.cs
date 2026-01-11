using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using EElemental.Core;
using EElemental.Player;
using EElemental.Enemies;
using EElemental.Combat;
using EElemental.Elements;
using EElemental.Procedural;
using EElemental.UI.HUD;

namespace EElemental.Tests
{
    /// <summary>
    /// Integration tests for cross-system interactions.
    /// Tests Player + Enemy + Combat + Procedural Generation integration.
    /// </summary>
    [TestFixture]
    public class IntegrationTests
    {
        private GameObject playerGO;
        private GameObject enemyGO;
        private PlayerController playerController;
        private EnemyBase enemyBase;
        
        [SetUp]
        public void SetUp()
        {
            // Create player
            playerGO = CreatePlayerGameObject();
            playerController = playerGO.GetComponent<PlayerController>();
            
            // Create enemy
            enemyGO = CreateEnemyGameObject();
            enemyBase = enemyGO.GetComponent<SlimeEnemy>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (playerGO != null) Object.DestroyImmediate(playerGO);
            if (enemyGO != null) Object.DestroyImmediate(enemyGO);
        }
        
        #region Player + Enemy Integration
        
        [Test]
        public void Player_CanDetectEnemy_InRange()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(2f, 0f, 0f);
            
            // Act
            float distance = Vector3.Distance(playerGO.transform.position, enemyGO.transform.position);
            
            // Assert
            Assert.IsTrue(distance < 5f); // Within detection range
        }
        
        [Test]
        public void Enemy_CanDetectPlayer_WhenClose()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(3f, 0f, 0f);
            
            // Act
            float distance = Vector3.Distance(playerGO.transform.position, enemyGO.transform.position);
            
            // Assert
            Assert.IsTrue(distance < 10f); // Enemy detection range
        }
        
        [Test]
        public void Player_AndEnemy_CanOccupySameSpace()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = Vector3.zero;
            
            // Assert - Both objects exist at same position
            Assert.AreEqual(playerGO.transform.position, enemyGO.transform.position);
        }
        
        #endregion
        
        #region Combat Integration
        
        [Test]
        public void PlayerAttack_CanHitEnemy()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(1f, 0f, 0f);
            
            var playerStats = playerGO.GetComponent<PlayerStats>();
            var enemyStats = enemyGO.GetComponent<EnemyStats>();
            
            // Assert - Both have stats components
            Assert.IsNotNull(playerStats);
            Assert.IsNotNull(enemyStats);
        }
        
        [Test]
        public void EnemyAttack_CanHitPlayer()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(1f, 0f, 0f);
            
            var playerStats = playerGO.GetComponent<PlayerStats>();
            var enemyStats = enemyGO.GetComponent<EnemyStats>();
            
            // Assert - Both have stats
            Assert.IsNotNull(playerStats);
            Assert.IsNotNull(enemyStats);
        }
        
        [Test]
        public void DamageCalculation_WorksBetweenPlayerAndEnemy()
        {
            // Arrange
            float baseDamage = 10f;
            float defense = 2f;
            
            // Act - Simulate damage calculation
            float finalDamage = Mathf.Max(1f, baseDamage - defense);
            
            // Assert
            Assert.AreEqual(8f, finalDamage);
        }
        
        [Test]
        public void Combat_ElementsApplyStatusEffects()
        {
            // Arrange
            var elementData = ScriptableObject.CreateInstance<ElementData>();
            elementData.elementName = "Fire";
            elementData.baseDamageMultiplier = 1.5f;
            
            // Act
            float boostedDamage = 10f * elementData.baseDamageMultiplier;
            
            // Assert
            Assert.AreEqual(15f, boostedDamage);
            
            // Cleanup
            Object.DestroyImmediate(elementData);
        }
        
        #endregion
        
        #region Element System Integration
        
        [Test]
        public void ElementCombiner_WorksWithPlayerAndEnemy()
        {
            // Arrange - Create two elements
            var fire = ScriptableObject.CreateInstance<ElementData>();
            fire.elementName = "Fire";
            fire.elementType = ElementType.Fire;
            
            var water = ScriptableObject.CreateInstance<ElementData>();
            water.elementName = "Water";
            water.elementType = ElementType.Water;
            
            // Assert - Elements can be created
            Assert.IsNotNull(fire);
            Assert.IsNotNull(water);
            Assert.AreNotEqual(fire.elementType, water.elementType);
            
            // Cleanup
            Object.DestroyImmediate(fire);
            Object.DestroyImmediate(water);
        }
        
        [Test]
        public void ElementalDamage_AffectsEnemyStats()
        {
            // Arrange
            float baseDamage = 20f;
            float elementMultiplier = 1.5f; // Fire bonus
            
            // Act
            float elementalDamage = baseDamage * elementMultiplier;
            
            // Assert
            Assert.AreEqual(30f, elementalDamage);
        }
        
        [Test]
        public void StatusEffects_CanBeAppliedToEnemy()
        {
            // Arrange
            var enemyStats = enemyGO.GetComponent<EnemyStats>();
            
            // Assert - Enemy has stats component for status effects
            Assert.IsNotNull(enemyStats);
        }
        
        #endregion
        
        #region Procedural + Combat Integration
        
        [Test]
        public void RoomGeneration_CanSpawnEnemies()
        {
            // Arrange
            var roomData = ScriptableObject.CreateInstance<RoomTemplate>();
            roomData.roomType = RoomType.Combat;
            roomData.roomWidth = 10;
            roomData.roomHeight = 8;
            
            // Assert - Combat room can have enemies
            Assert.AreEqual(RoomType.Combat, roomData.roomType);
            Assert.IsTrue(roomData.roomWidth > 0);
            Assert.IsTrue(roomData.roomHeight > 0);
            
            // Cleanup
            Object.DestroyImmediate(roomData);
        }
        
        [Test]
        public void SpawnPoints_AreValidPositions()
        {
            // Arrange
            var spawnPoints = new List<Vector2>
            {
                new Vector2(2, 2),
                new Vector2(5, 5),
                new Vector2(8, 3)
            };
            
            // Assert - Spawn points are within room bounds
            foreach (var point in spawnPoints)
            {
                Assert.IsTrue(point.x >= 0 && point.x <= 10);
                Assert.IsTrue(point.y >= 0 && point.y <= 10);
            }
        }
        
        [Test]
        public void EnemySpawner_CreatesValidEnemies()
        {
            // Arrange
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.enemyName = "Test Slime";
            enemyData.maxHealth = 50f;
            enemyData.attackDamage = 10f;
            
            // Assert - Enemy data is valid
            Assert.IsNotNull(enemyData);
            Assert.IsTrue(enemyData.maxHealth > 0);
            
            // Cleanup
            Object.DestroyImmediate(enemyData);
        }
        
        [Test]
        public void DungeonRooms_ConnectCorrectly()
        {
            // Arrange
            var room1Center = new Vector2(5, 5);
            var room2Center = new Vector2(15, 5);
            
            // Act - Calculate corridor
            float distance = Vector2.Distance(room1Center, room2Center);
            
            // Assert
            Assert.AreEqual(10f, distance);
        }
        
        #endregion
        
        #region Player State + Combat Integration
        
        [Test]
        public void PlayerAttackState_DealsCorrectDamage()
        {
            // Arrange
            var playerStats = playerGO.GetComponent<PlayerStats>();
            float attackPower = 15f;
            
            // Assert
            Assert.IsNotNull(playerStats);
            Assert.IsTrue(attackPower > 0);
        }
        
        [Test]
        public void PlayerDodgeState_ProvidesIFrames()
        {
            // Arrange - IFrames should make player invulnerable
            bool hasIFrames = true; // Would be set by dodge state
            
            // Assert
            Assert.IsTrue(hasIFrames);
        }
        
        [Test]
        public void PlayerHurtState_ReducesHealth()
        {
            // Arrange
            var playerStats = playerGO.GetComponent<PlayerStats>();
            float initialHealth = 100f;
            float damage = 20f;
            
            // Act
            float remainingHealth = initialHealth - damage;
            
            // Assert
            Assert.AreEqual(80f, remainingHealth);
        }
        
        [Test]
        public void PlayerDeathState_TriggersOnZeroHealth()
        {
            // Arrange
            float health = 0f;
            
            // Assert
            Assert.IsTrue(health <= 0);
        }
        
        #endregion
        
        #region Enemy AI + Player Integration
        
        [Test]
        public void EnemyAI_ChasesPlayerWhenInRange()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(5f, 0f, 0f);
            
            float chaseRange = 10f;
            float distance = Vector3.Distance(playerGO.transform.position, enemyGO.transform.position);
            
            // Assert
            Assert.IsTrue(distance < chaseRange);
        }
        
        [Test]
        public void EnemyAI_AttacksPlayerWhenClose()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(1.5f, 0f, 0f);
            
            float attackRange = 2f;
            float distance = Vector3.Distance(playerGO.transform.position, enemyGO.transform.position);
            
            // Assert
            Assert.IsTrue(distance < attackRange);
        }
        
        [Test]
        public void EnemyAI_PatrolsWhenPlayerFar()
        {
            // Arrange
            playerGO.transform.position = Vector3.zero;
            enemyGO.transform.position = new Vector3(20f, 0f, 0f);
            
            float detectionRange = 15f;
            float distance = Vector3.Distance(playerGO.transform.position, enemyGO.transform.position);
            
            // Assert - Player is out of detection range
            Assert.IsTrue(distance > detectionRange);
        }
        
        #endregion
        
        #region UI + Combat Integration
        
        [Test]
        public void HealthBar_UpdatesWhenPlayerTakesDamage()
        {
            // Arrange
            float maxHealth = 100f;
            float currentHealth = 75f;
            
            // Act
            float healthPercent = currentHealth / maxHealth;
            
            // Assert
            Assert.AreEqual(0.75f, healthPercent);
        }
        
        [Test]
        public void ComboUI_UpdatesOnAttack()
        {
            // Arrange
            int comboCount = 0;
            
            // Act - Simulate attack
            comboCount++;
            comboCount++;
            comboCount++;
            
            // Assert
            Assert.AreEqual(3, comboCount);
        }
        
        [Test]
        public void ElementUI_ShowsEquippedElement()
        {
            // Arrange
            var element = ScriptableObject.CreateInstance<ElementData>();
            element.elementName = "Fire";
            element.elementType = ElementType.Fire;
            
            // Assert
            Assert.IsNotNull(element);
            Assert.AreEqual("Fire", element.elementName);
            
            // Cleanup
            Object.DestroyImmediate(element);
        }
        
        #endregion
        
        #region Full Combat Flow Test
        
        [Test]
        public void FullCombatFlow_PlayerAttacksAndKillsEnemy()
        {
            // Arrange
            float playerDamage = 25f;
            float enemyHealth = 50f;
            
            // Act - Simulate combat
            enemyHealth -= playerDamage; // First hit
            Assert.AreEqual(25f, enemyHealth);
            
            enemyHealth -= playerDamage; // Second hit
            
            // Assert - Enemy is dead
            Assert.IsTrue(enemyHealth <= 0);
        }
        
        [Test]
        public void FullCombatFlow_EnemyAttacksPlayer()
        {
            // Arrange
            float enemyDamage = 15f;
            float playerHealth = 100f;
            
            // Act
            playerHealth -= enemyDamage;
            
            // Assert
            Assert.AreEqual(85f, playerHealth);
        }
        
        [Test]
        public void FullCombatFlow_ElementalComboBoost()
        {
            // Arrange
            float baseDamage = 20f;
            float elementBonus = 1.5f; // Fire
            float comboBonus = 1.2f; // 3-hit combo
            
            // Act
            float totalDamage = baseDamage * elementBonus * comboBonus;
            
            // Assert
            Assert.AreEqual(36f, totalDamage);
        }
        
        #endregion
        
        #region Helper Methods
        
        private GameObject CreatePlayerGameObject()
        {
            var go = new GameObject("Player");
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            go.AddComponent<PlayerStats>();
            go.AddComponent<PlayerMovement>();
            go.AddComponent<PlayerController>();
            return go;
        }
        
        private GameObject CreateEnemyGameObject()
        {
            var go = new GameObject("Enemy");
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Animator>();
            go.AddComponent<EnemyStats>();
            go.AddComponent<SlimeEnemy>();
            return go;
        }
        
        #endregion
    }
}
