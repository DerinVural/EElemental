using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EElemental.Procedural
{
    /// <summary>
    /// ScriptableObject database containing all room templates.
    /// Provides query methods for retrieving appropriate templates.
    /// </summary>
    [CreateAssetMenu(fileName = "RoomDatabase", menuName = "EElemental/Procedural/Room Database")]
    public class RoomDatabase : ScriptableObject
    {
        [Header("Room Templates by Type")]
        [SerializeField] private List<RoomTemplate> _spawnRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _combatRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _eliteRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _bossRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _treasureRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _restRoomTemplates = new List<RoomTemplate>();
        [SerializeField] private List<RoomTemplate> _secretRoomTemplates = new List<RoomTemplate>();

        [Header("Fallbacks")]
        [SerializeField] private RoomTemplate _defaultTemplate;

        private Dictionary<RoomType, List<RoomTemplate>> _templateCache;

        private void OnEnable()
        {
            BuildCache();
        }

        /// <summary>
        /// Build template cache for fast lookups.
        /// </summary>
        private void BuildCache()
        {
            _templateCache = new Dictionary<RoomType, List<RoomTemplate>>
            {
                { RoomType.Spawn, _spawnRoomTemplates },
                { RoomType.Combat, _combatRoomTemplates },
                { RoomType.Elite, _eliteRoomTemplates },
                { RoomType.Boss, _bossRoomTemplates },
                { RoomType.Treasure, _treasureRoomTemplates },
                { RoomType.Rest, _restRoomTemplates },
                { RoomType.Secret, _secretRoomTemplates }
            };
        }

        /// <summary>
        /// Get a random template of the specified type that fits the room size.
        /// </summary>
        public RoomTemplate GetTemplate(RoomType type, int roomWidth, int roomHeight)
        {
            if (_templateCache == null)
                BuildCache();

            if (!_templateCache.ContainsKey(type))
            {
                Debug.LogWarning($"[RoomDatabase] No templates found for type: {type}");
                return _defaultTemplate;
            }

            // Filter templates that fit the room size
            var validTemplates = _templateCache[type]
                .Where(t => t != null && t.FitsRoomSize(roomWidth, roomHeight))
                .ToList();

            if (validTemplates.Count == 0)
            {
                Debug.LogWarning($"[RoomDatabase] No templates fit room size {roomWidth}x{roomHeight} for type {type}. Using default.");
                return _defaultTemplate;
            }

            // Return random template from valid options
            return validTemplates[Random.Range(0, validTemplates.Count)];
        }

        #region Specific Room Type Getters

        public RoomTemplate GetSpawnRoomTemplate()
        {
            if (_spawnRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No spawn room templates! Using default.");
                return _defaultTemplate;
            }
            return _spawnRoomTemplates[Random.Range(0, _spawnRoomTemplates.Count)];
        }

        public RoomTemplate GetCombatRoomTemplate()
        {
            if (_combatRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No combat room templates! Using default.");
                return _defaultTemplate;
            }
            return _combatRoomTemplates[Random.Range(0, _combatRoomTemplates.Count)];
        }

        public RoomTemplate GetEliteRoomTemplate()
        {
            if (_eliteRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No elite room templates! Using default.");
                return _defaultTemplate;
            }
            return _eliteRoomTemplates[Random.Range(0, _eliteRoomTemplates.Count)];
        }

        public RoomTemplate GetBossRoomTemplate()
        {
            if (_bossRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No boss room templates! Using default.");
                return _defaultTemplate;
            }
            return _bossRoomTemplates[Random.Range(0, _bossRoomTemplates.Count)];
        }

        public RoomTemplate GetTreasureRoomTemplate()
        {
            if (_treasureRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No treasure room templates! Using default.");
                return _defaultTemplate;
            }
            return _treasureRoomTemplates[Random.Range(0, _treasureRoomTemplates.Count)];
        }

        public RoomTemplate GetRestRoomTemplate()
        {
            if (_restRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No rest room templates! Using default.");
                return _defaultTemplate;
            }
            return _restRoomTemplates[Random.Range(0, _restRoomTemplates.Count)];
        }

        public RoomTemplate GetSecretRoomTemplate()
        {
            if (_secretRoomTemplates.Count == 0)
            {
                Debug.LogWarning("[RoomDatabase] No secret room templates! Using default.");
                return _defaultTemplate;
            }
            return _secretRoomTemplates[Random.Range(0, _secretRoomTemplates.Count)];
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Get all templates of a specific type.
        /// </summary>
        public List<RoomTemplate> GetAllTemplatesOfType(RoomType type)
        {
            if (_templateCache == null)
                BuildCache();

            return _templateCache.ContainsKey(type) ? _templateCache[type] : new List<RoomTemplate>();
        }

        /// <summary>
        /// Get templates filtered by difficulty range.
        /// </summary>
        public List<RoomTemplate> GetTemplatesByDifficulty(RoomType type, int minDifficulty, int maxDifficulty)
        {
            if (_templateCache == null)
                BuildCache();

            if (!_templateCache.ContainsKey(type))
                return new List<RoomTemplate>();

            return _templateCache[type]
                .Where(t => t != null && t.Difficulty >= minDifficulty && t.Difficulty <= maxDifficulty)
                .ToList();
        }

        /// <summary>
        /// Get total count of templates in database.
        /// </summary>
        public int GetTotalTemplateCount()
        {
            int count = 0;
            count += _spawnRoomTemplates.Count;
            count += _combatRoomTemplates.Count;
            count += _eliteRoomTemplates.Count;
            count += _bossRoomTemplates.Count;
            count += _treasureRoomTemplates.Count;
            count += _restRoomTemplates.Count;
            count += _secretRoomTemplates.Count;
            return count;
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        /// <summary>
        /// Validate database integrity in editor.
        /// </summary>
        private void OnValidate()
        {
            if (_defaultTemplate == null)
            {
                Debug.LogWarning("[RoomDatabase] Default template is not assigned!");
            }

            ValidateTemplateList(_spawnRoomTemplates, RoomType.Spawn);
            ValidateTemplateList(_combatRoomTemplates, RoomType.Combat);
            ValidateTemplateList(_eliteRoomTemplates, RoomType.Elite);
            ValidateTemplateList(_bossRoomTemplates, RoomType.Boss);
            ValidateTemplateList(_treasureRoomTemplates, RoomType.Treasure);
            ValidateTemplateList(_restRoomTemplates, RoomType.Rest);
            ValidateTemplateList(_secretRoomTemplates, RoomType.Secret);
        }

        private void ValidateTemplateList(List<RoomTemplate> templates, RoomType expectedType)
        {
            foreach (var template in templates)
            {
                if (template != null && template.RoomType != expectedType)
                {
                    Debug.LogWarning($"[RoomDatabase] Template '{template.TemplateName}' has type {template.RoomType} but is in {expectedType} list!");
                }
            }
        }

        /// <summary>
        /// Debug: Print database statistics.
        /// </summary>
        [ContextMenu("Print Database Statistics")]
        private void PrintStatistics()
        {
            Debug.Log($"[RoomDatabase] Total Templates: {GetTotalTemplateCount()}");
            Debug.Log($"  - Spawn: {_spawnRoomTemplates.Count}");
            Debug.Log($"  - Combat: {_combatRoomTemplates.Count}");
            Debug.Log($"  - Elite: {_eliteRoomTemplates.Count}");
            Debug.Log($"  - Boss: {_bossRoomTemplates.Count}");
            Debug.Log($"  - Treasure: {_treasureRoomTemplates.Count}");
            Debug.Log($"  - Rest: {_restRoomTemplates.Count}");
            Debug.Log($"  - Secret: {_secretRoomTemplates.Count}");
        }
#endif

        #endregion
    }
}
