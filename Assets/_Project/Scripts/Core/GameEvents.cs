using UnityEngine;
using EElemental.Elements;

namespace EElemental.Core
{
    /// <summary>
    /// Central event system for game-wide communication.
    /// Uses C# events for type-safe, loosely coupled messaging.
    /// </summary>
    public static class GameEvents
    {
        #region Player Events

        /// <summary>
        /// Fired when player health changes.
        /// Parameters: currentHealth, maxHealth
        /// </summary>
        public static event System.Action<float, float> OnPlayerHealthChanged;

        /// <summary>
        /// Fired when player mana changes.
        /// Parameters: currentMana, maxMana
        /// </summary>
        public static event System.Action<int, int> OnPlayerManaChanged;

        /// <summary>
        /// Fired when player takes damage.
        /// Parameters: damageAmount
        /// </summary>
        public static event System.Action<float> OnPlayerDamaged;

        /// <summary>
        /// Fired when player dies.
        /// </summary>
        public static event System.Action OnPlayerDeath;

        /// <summary>
        /// Fired when player respawns.
        /// </summary>
        public static event System.Action OnPlayerRespawn;

        /// <summary>
        /// Fired when player's active element changes.
        /// Parameters: newElement
        /// </summary>
        public static event System.Action<ElementType> OnPlayerElementChanged;

        #endregion

        #region Combat Events

        /// <summary>
        /// Fired when any attack lands a hit.
        /// Parameters: damageInfo, targetGameObject
        /// </summary>
        public static event System.Action<DamageInfo, GameObject> OnAttackHit;

        /// <summary>
        /// Fired when a critical hit occurs.
        /// Parameters: damageAmount, targetGameObject
        /// </summary>
        public static event System.Action<float, GameObject> OnCriticalHit;

        /// <summary>
        /// Fired when a combo is completed.
        /// Parameters: comboCount
        /// </summary>
        public static event System.Action<int> OnComboCompleted;

        /// <summary>
        /// Fired when a combo is broken/reset.
        /// </summary>
        public static event System.Action OnComboReset;

        #endregion

        #region Enemy Events

        /// <summary>
        /// Fired when an enemy is spawned.
        /// Parameters: enemyGameObject
        /// </summary>
        public static event System.Action<GameObject> OnEnemySpawned;

        /// <summary>
        /// Fired when an enemy dies.
        /// Parameters: enemyGameObject, killerGameObject
        /// </summary>
        public static event System.Action<GameObject, GameObject> OnEnemyDeath;

        /// <summary>
        /// Fired when an enemy enters alert state.
        /// Parameters: enemyGameObject, targetGameObject
        /// </summary>
        public static event System.Action<GameObject, GameObject> OnEnemyAlerted;

        #endregion

        #region Dungeon Events

        /// <summary>
        /// Fired when a new room is entered.
        /// Parameters: roomId
        /// </summary>
        public static event System.Action<string> OnRoomEntered;

        /// <summary>
        /// Fired when a room is cleared of enemies.
        /// Parameters: roomId
        /// </summary>
        public static event System.Action<string> OnRoomCleared;

        /// <summary>
        /// Fired when dungeon generation starts.
        /// Parameters: seed
        /// </summary>
        public static event System.Action<int> OnDungeonGenerationStarted;

        /// <summary>
        /// Fired when dungeon generation completes.
        /// Parameters: totalRooms
        /// </summary>
        public static event System.Action<int> OnDungeonGenerationCompleted;

        #endregion

        #region Game State Events

        /// <summary>
        /// Fired when game state changes.
        /// Parameters: newState
        /// </summary>
        public static event System.Action<GameState> OnGameStateChanged;

        /// <summary>
        /// Fired when a new run starts.
        /// Parameters: runSeed
        /// </summary>
        public static event System.Action<int> OnRunStarted;

        /// <summary>
        /// Fired when the current run ends.
        /// Parameters: wasVictory
        /// </summary>
        public static event System.Action<bool> OnRunEnded;

        /// <summary>
        /// Fired when game is paused.
        /// </summary>
        public static event System.Action OnGamePaused;

        /// <summary>
        /// Fired when game is resumed.
        /// </summary>
        public static event System.Action OnGameResumed;

        #endregion

        #region Item/Pickup Events

        /// <summary>
        /// Fired when player collects an item.
        /// Parameters: itemGameObject
        /// </summary>
        public static event System.Action<GameObject> OnItemCollected;

        /// <summary>
        /// Fired when player equips a weapon.
        /// Parameters: weaponGameObject
        /// </summary>
        public static event System.Action<GameObject> OnWeaponEquipped;

        #endregion

        #region Event Trigger Methods

        // Player Events
        public static void TriggerPlayerHealthChanged(float current, float max) => OnPlayerHealthChanged?.Invoke(current, max);
        public static void TriggerPlayerManaChanged(int current, int max) => OnPlayerManaChanged?.Invoke(current, max);
        public static void TriggerPlayerDamaged(float amount) => OnPlayerDamaged?.Invoke(amount);
        public static void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
        public static void TriggerPlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void TriggerPlayerElementChanged(ElementType element) => OnPlayerElementChanged?.Invoke(element);

        // Combat Events
        public static void TriggerAttackHit(DamageInfo damageInfo, GameObject target) => OnAttackHit?.Invoke(damageInfo, target);
        public static void TriggerCriticalHit(float damage, GameObject target) => OnCriticalHit?.Invoke(damage, target);
        public static void TriggerComboCompleted(int comboCount) => OnComboCompleted?.Invoke(comboCount);
        public static void TriggerComboReset() => OnComboReset?.Invoke();

        // Enemy Events
        public static void TriggerEnemySpawned(GameObject enemy) => OnEnemySpawned?.Invoke(enemy);
        public static void TriggerEnemyDeath(GameObject enemy, GameObject killer) => OnEnemyDeath?.Invoke(enemy, killer);
        public static void TriggerEnemyAlerted(GameObject enemy, GameObject target) => OnEnemyAlerted?.Invoke(enemy, target);

        // Dungeon Events
        public static void TriggerRoomEntered(string roomId) => OnRoomEntered?.Invoke(roomId);
        public static void TriggerRoomCleared(string roomId) => OnRoomCleared?.Invoke(roomId);
        public static void TriggerDungeonGenerationStarted(int seed) => OnDungeonGenerationStarted?.Invoke(seed);
        public static void TriggerDungeonGenerationCompleted(int totalRooms) => OnDungeonGenerationCompleted?.Invoke(totalRooms);

        // Game State Events
        public static void TriggerGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
        public static void TriggerRunStarted(int seed) => OnRunStarted?.Invoke(seed);
        public static void TriggerRunEnded(bool wasVictory) => OnRunEnded?.Invoke(wasVictory);
        public static void TriggerGamePaused() => OnGamePaused?.Invoke();
        public static void TriggerGameResumed() => OnGameResumed?.Invoke();

        // Item Events
        public static void TriggerItemCollected(GameObject item) => OnItemCollected?.Invoke(item);
        public static void TriggerWeaponEquipped(GameObject weapon) => OnWeaponEquipped?.Invoke(weapon);

        #endregion

        #region Cleanup

        /// <summary>
        /// Clears all event subscriptions. Call this when changing scenes or ending a run.
        /// </summary>
        public static void ClearAllEvents()
        {
            // Player Events
            OnPlayerHealthChanged = null;
            OnPlayerManaChanged = null;
            OnPlayerDamaged = null;
            OnPlayerDeath = null;
            OnPlayerRespawn = null;
            OnPlayerElementChanged = null;

            // Combat Events
            OnAttackHit = null;
            OnCriticalHit = null;
            OnComboCompleted = null;
            OnComboReset = null;

            // Enemy Events
            OnEnemySpawned = null;
            OnEnemyDeath = null;
            OnEnemyAlerted = null;

            // Dungeon Events
            OnRoomEntered = null;
            OnRoomCleared = null;
            OnDungeonGenerationStarted = null;
            OnDungeonGenerationCompleted = null;

            // Game State Events
            OnGameStateChanged = null;
            OnRunStarted = null;
            OnRunEnded = null;
            OnGamePaused = null;
            OnGameResumed = null;

            // Item Events
            OnItemCollected = null;
            OnWeaponEquipped = null;
        }

        #endregion
    }
}
