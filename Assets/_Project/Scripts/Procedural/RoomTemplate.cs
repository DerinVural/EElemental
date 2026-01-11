using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace EElemental.Procedural
{
    /// <summary>
    /// ScriptableObject defining a room template with tiles, spawns, and metadata.
    /// Designer-friendly way to create predefined room layouts.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRoomTemplate", menuName = "EElemental/Procedural/Room Template")]
    public class RoomTemplate : ScriptableObject
    {
        [Header("Metadata")]
        [SerializeField] private string _templateName;
        [SerializeField] private RoomType _roomType;
        [TextArea(3, 5)]
        [SerializeField] private string _description;

        [Header("Dimensions")]
        [SerializeField] private int _minWidth = 8;
        [SerializeField] private int _maxWidth = 20;
        [SerializeField] private int _minHeight = 8;
        [SerializeField] private int _maxHeight = 20;

        [Header("Tile Data")]
        [SerializeField] private TileBase _floorTile;
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _decorationTile;

        [Header("Spawn Points")]
        [SerializeField] private List<SpawnPoint> _enemySpawnPoints = new List<SpawnPoint>();
        [SerializeField] private List<Vector2Int> _itemSpawnPoints = new List<Vector2Int>();
        [SerializeField] private Vector2Int _playerSpawnPoint;

        [Header("Features")]
        [SerializeField] private bool _hasEnvironmentalHazards;
        [SerializeField] private List<EnvironmentalFeature> _environmentalFeatures = new List<EnvironmentalFeature>();

        [Header("Difficulty")]
        [SerializeField] [Range(1, 10)] private int _difficulty = 5;
        [SerializeField] private int _minEnemyCount = 3;
        [SerializeField] private int _maxEnemyCount = 8;

        // Properties
        public string TemplateName => _templateName;
        public RoomType RoomType => _roomType;
        public string Description => _description;
        public int MinWidth => _minWidth;
        public int MaxWidth => _maxWidth;
        public int MinHeight => _minHeight;
        public int MaxHeight => _maxHeight;
        public TileBase FloorTile => _floorTile;
        public TileBase WallTile => _wallTile;
        public TileBase DecorationTile => _decorationTile;
        public List<SpawnPoint> EnemySpawnPoints => _enemySpawnPoints;
        public List<Vector2Int> ItemSpawnPoints => _itemSpawnPoints;
        public Vector2Int PlayerSpawnPoint => _playerSpawnPoint;
        public bool HasEnvironmentalHazards => _hasEnvironmentalHazards;
        public List<EnvironmentalFeature> EnvironmentalFeatures => _environmentalFeatures;
        public int Difficulty => _difficulty;
        public int MinEnemyCount => _minEnemyCount;
        public int MaxEnemyCount => _maxEnemyCount;

        /// <summary>
        /// Check if this template fits the given room dimensions.
        /// </summary>
        public bool FitsRoomSize(int width, int height)
        {
            return width >= _minWidth && width <= _maxWidth &&
                   height >= _minHeight && height <= _maxHeight;
        }

        /// <summary>
        /// Get spawn points adjusted to room bounds.
        /// </summary>
        public List<Vector2Int> GetAdjustedEnemySpawns(RectInt roomBounds)
        {
            List<Vector2Int> adjustedSpawns = new List<Vector2Int>();

            foreach (var spawnPoint in _enemySpawnPoints)
            {
                // Convert normalized position to world position within room
                Vector2Int worldPos = new Vector2Int(
                    roomBounds.x + Mathf.RoundToInt(spawnPoint.NormalizedPosition.x * roomBounds.width),
                    roomBounds.y + Mathf.RoundToInt(spawnPoint.NormalizedPosition.y * roomBounds.height)
                );
                adjustedSpawns.Add(worldPos);
            }

            return adjustedSpawns;
        }

        /// <summary>
        /// Get item spawn points adjusted to room bounds.
        /// </summary>
        public List<Vector2Int> GetAdjustedItemSpawns(RectInt roomBounds)
        {
            List<Vector2Int> adjustedSpawns = new List<Vector2Int>();

            foreach (var itemSpawn in _itemSpawnPoints)
            {
                // Assume item spawns are stored as normalized coordinates
                Vector2Int worldPos = new Vector2Int(
                    roomBounds.x + itemSpawn.x,
                    roomBounds.y + itemSpawn.y
                );
                adjustedSpawns.Add(worldPos);
            }

            return adjustedSpawns;
        }

        /// <summary>
        /// Get player spawn adjusted to room bounds.
        /// </summary>
        public Vector2Int GetAdjustedPlayerSpawn(RectInt roomBounds)
        {
            return new Vector2Int(
                roomBounds.x + _playerSpawnPoint.x,
                roomBounds.y + _playerSpawnPoint.y
            );
        }

        #region Editor Helpers

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure min/max constraints
            _minWidth = Mathf.Max(4, _minWidth);
            _maxWidth = Mathf.Max(_minWidth, _maxWidth);
            _minHeight = Mathf.Max(4, _minHeight);
            _maxHeight = Mathf.Max(_minHeight, _maxHeight);

            _minEnemyCount = Mathf.Max(0, _minEnemyCount);
            _maxEnemyCount = Mathf.Max(_minEnemyCount, _maxEnemyCount);
        }
#endif

        #endregion
    }

    /// <summary>
    /// Enemy spawn point with metadata.
    /// </summary>
    [System.Serializable]
    public class SpawnPoint
    {
        [Tooltip("Normalized position (0-1) within room bounds")]
        public Vector2 NormalizedPosition;

        [Tooltip("Preferred enemy type (optional, can be left empty)")]
        public string EnemyType;

        [Tooltip("Spawn weight (higher = more likely to spawn here)")]
        [Range(0f, 1f)]
        public float Weight = 1f;

        [Tooltip("Only spawn if difficulty meets threshold")]
        [Range(1, 10)]
        public int MinDifficulty = 1;

        public SpawnPoint(Vector2 normalizedPos, string enemyType = "", float weight = 1f)
        {
            NormalizedPosition = normalizedPos;
            EnemyType = enemyType;
            Weight = weight;
        }
    }

    /// <summary>
    /// Environmental feature (hazards, decorations, interactables).
    /// </summary>
    [System.Serializable]
    public class EnvironmentalFeature
    {
        public enum FeatureType
        {
            Hazard,
            Decoration,
            Interactable,
            Platform
        }

        public FeatureType Type;
        public Vector2Int Position;
        public GameObject Prefab;
        public bool IsRequired; // Must spawn
        public float SpawnChance = 1f; // For optional features
    }
}
