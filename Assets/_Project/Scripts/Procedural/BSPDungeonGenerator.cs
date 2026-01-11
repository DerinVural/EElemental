using UnityEngine;
using System.Collections.Generic;
using EElemental.Core;

namespace EElemental.Procedural
{
    /// <summary>
    /// Binary Space Partitioning (BSP) based dungeon generator.
    /// Recursively splits space into rooms and connects them with corridors.
    /// </summary>
    public class BSPDungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        [SerializeField] private int _dungeonWidth = 100;
        [SerializeField] private int _dungeonHeight = 100;
        [SerializeField] private int _minRoomSize = 8;
        [SerializeField] private int _maxRoomSize = 20;
        [SerializeField] private int _maxDepth = 4;

        [Header("Room Settings")]
        [SerializeField] private int _minRoomPadding = 2;
        [SerializeField] private float _roomSizeVariance = 0.3f;

        [Header("Corridor Settings")]
        [SerializeField] private int _corridorWidth = 3;

        [Header("References")]
        [SerializeField] private RoomDatabase _roomDatabase;
        [SerializeField] private TileMapper _tileMapper;

        private RoomNode _rootNode;
        private List<Room> _generatedRooms = new List<Room>();
        private List<Corridor> _corridors = new List<Corridor>();
        private int _currentSeed;

        public List<Room> GeneratedRooms => _generatedRooms;
        public int CurrentSeed => _currentSeed;

        /// <summary>
        /// Generate a new dungeon with the given seed.
        /// </summary>
        public DungeonData Generate(int seed)
        {
            _currentSeed = seed;
            Random.InitState(seed);

            GameEvents.TriggerDungeonGenerationStarted(seed);

            // Clear previous data
            _generatedRooms.Clear();
            _corridors.Clear();

            // Create root node spanning entire dungeon area
            _rootNode = new RoomNode(new RectInt(0, 0, _dungeonWidth, _dungeonHeight));

            // Recursively split space
            Split(_rootNode, 0);

            // Create rooms in leaf nodes
            CreateRooms(_rootNode);

            // Connect rooms with corridors
            CreateCorridors(_rootNode);

            // Assign room templates from database
            AssignRoomTemplates();

            // Generate tilemap
            if (_tileMapper != null)
            {
                _tileMapper.GenerateTilemap(_generatedRooms, _corridors);
            }

            GameEvents.TriggerDungeonGenerationCompleted(_generatedRooms.Count);

            return new DungeonData
            {
                Seed = seed,
                Rooms = _generatedRooms,
                Corridors = _corridors,
                Width = _dungeonWidth,
                Height = _dungeonHeight
            };
        }

        /// <summary>
        /// Recursively split node using BSP algorithm.
        /// </summary>
        private void Split(RoomNode node, int depth)
        {
            if (depth >= _maxDepth)
                return;

            RectInt rect = node.Bounds;

            // Check if room is too small to split
            if (rect.width < _minRoomSize * 2 + _minRoomPadding ||
                rect.height < _minRoomSize * 2 + _minRoomPadding)
                return;

            // Decide split direction (horizontal or vertical)
            bool splitHorizontal;
            if (rect.width > rect.height && (float)rect.width / rect.height >= 1.25f)
                splitHorizontal = false; // Split vertically
            else if (rect.height > rect.width && (float)rect.height / rect.width >= 1.25f)
                splitHorizontal = true; // Split horizontally
            else
                splitHorizontal = Random.value > 0.5f; // Random

            // Calculate split position
            int splitPos;
            if (splitHorizontal)
            {
                int minSplit = rect.y + _minRoomSize + _minRoomPadding;
                int maxSplit = rect.y + rect.height - _minRoomSize - _minRoomPadding;
                if (minSplit >= maxSplit) return; // Can't split
                splitPos = Random.Range(minSplit, maxSplit);
            }
            else
            {
                int minSplit = rect.x + _minRoomSize + _minRoomPadding;
                int maxSplit = rect.x + rect.width - _minRoomSize - _minRoomPadding;
                if (minSplit >= maxSplit) return; // Can't split
                splitPos = Random.Range(minSplit, maxSplit);
            }

            // Create child nodes
            if (splitHorizontal)
            {
                node.LeftChild = new RoomNode(new RectInt(
                    rect.x, rect.y,
                    rect.width, splitPos - rect.y
                ));
                node.RightChild = new RoomNode(new RectInt(
                    rect.x, splitPos,
                    rect.width, rect.y + rect.height - splitPos
                ));
            }
            else
            {
                node.LeftChild = new RoomNode(new RectInt(
                    rect.x, rect.y,
                    splitPos - rect.x, rect.height
                ));
                node.RightChild = new RoomNode(new RectInt(
                    splitPos, rect.y,
                    rect.x + rect.width - splitPos, rect.height
                ));
            }

            // Recursively split children
            Split(node.LeftChild, depth + 1);
            Split(node.RightChild, depth + 1);
        }

        /// <summary>
        /// Create rooms in leaf nodes.
        /// </summary>
        private void CreateRooms(RoomNode node)
        {
            if (node.LeftChild != null && node.RightChild != null)
            {
                // Not a leaf, recurse
                CreateRooms(node.LeftChild);
                CreateRooms(node.RightChild);
            }
            else
            {
                // Leaf node - create a room
                RectInt bounds = node.Bounds;

                // Add random variance to room size within bounds
                int padding = _minRoomPadding;
                int roomWidth = Mathf.Min(
                    bounds.width - padding * 2,
                    Random.Range(_minRoomSize, Mathf.Min(_maxRoomSize, bounds.width - padding * 2))
                );
                int roomHeight = Mathf.Min(
                    bounds.height - padding * 2,
                    Random.Range(_minRoomSize, Mathf.Min(_maxRoomSize, bounds.height - padding * 2))
                );

                // Center room within bounds with random offset
                int offsetX = Random.Range(padding, bounds.width - roomWidth - padding + 1);
                int offsetY = Random.Range(padding, bounds.height - roomHeight - padding + 1);

                Room room = new Room
                {
                    Id = $"room_{_generatedRooms.Count}",
                    Bounds = new RectInt(
                        bounds.x + offsetX,
                        bounds.y + offsetY,
                        roomWidth,
                        roomHeight
                    ),
                    Center = new Vector2Int(
                        bounds.x + offsetX + roomWidth / 2,
                        bounds.y + offsetY + roomHeight / 2
                    )
                };

                node.Room = room;
                _generatedRooms.Add(room);
            }
        }

        /// <summary>
        /// Connect rooms with corridors.
        /// </summary>
        private void CreateCorridors(RoomNode node)
        {
            if (node.LeftChild == null || node.RightChild == null)
                return;

            // Get rooms from left and right subtrees
            Room leftRoom = GetRandomRoomFromNode(node.LeftChild);
            Room rightRoom = GetRandomRoomFromNode(node.RightChild);

            if (leftRoom != null && rightRoom != null)
            {
                CreateCorridor(leftRoom.Center, rightRoom.Center);
            }

            // Recurse
            CreateCorridors(node.LeftChild);
            CreateCorridors(node.RightChild);
        }

        /// <summary>
        /// Get a random room from this node or its children.
        /// </summary>
        private Room GetRandomRoomFromNode(RoomNode node)
        {
            if (node.Room != null)
                return node.Room;

            List<Room> rooms = new List<Room>();
            CollectRooms(node, rooms);

            return rooms.Count > 0 ? rooms[Random.Range(0, rooms.Count)] : null;
        }

        private void CollectRooms(RoomNode node, List<Room> rooms)
        {
            if (node.Room != null)
            {
                rooms.Add(node.Room);
            }
            else
            {
                if (node.LeftChild != null) CollectRooms(node.LeftChild, rooms);
                if (node.RightChild != null) CollectRooms(node.RightChild, rooms);
            }
        }

        /// <summary>
        /// Create an L-shaped corridor between two points.
        /// </summary>
        private void CreateCorridor(Vector2Int start, Vector2Int end)
        {
            // Choose random corner point for L-shape
            Vector2Int corner = Random.value > 0.5f
                ? new Vector2Int(end.x, start.y) // Horizontal first
                : new Vector2Int(start.x, end.y); // Vertical first

            Corridor corridor = new Corridor
            {
                Start = start,
                End = end,
                Corner = corner,
                Width = _corridorWidth
            };

            _corridors.Add(corridor);
        }

        /// <summary>
        /// Assign room templates from database based on room characteristics.
        /// </summary>
        private void AssignRoomTemplates()
        {
            if (_roomDatabase == null)
            {
                Debug.LogWarning("[BSPDungeonGenerator] RoomDatabase not assigned!");
                return;
            }

            // First room is always spawn
            if (_generatedRooms.Count > 0)
            {
                _generatedRooms[0].Type = RoomType.Spawn;
                _generatedRooms[0].Template = _roomDatabase.GetSpawnRoomTemplate();
            }

            // Last room is always boss
            if (_generatedRooms.Count > 1)
            {
                int lastIndex = _generatedRooms.Count - 1;
                _generatedRooms[lastIndex].Type = RoomType.Boss;
                _generatedRooms[lastIndex].Template = _roomDatabase.GetBossRoomTemplate();
            }

            // Assign templates to remaining rooms
            for (int i = 1; i < _generatedRooms.Count - 1; i++)
            {
                Room room = _generatedRooms[i];

                // Randomly assign room type
                float roll = Random.value;
                if (roll < 0.1f)
                {
                    room.Type = RoomType.Treasure;
                    room.Template = _roomDatabase.GetTreasureRoomTemplate();
                }
                else if (roll < 0.25f)
                {
                    room.Type = RoomType.Elite;
                    room.Template = _roomDatabase.GetEliteRoomTemplate();
                }
                else
                {
                    room.Type = RoomType.Combat;
                    room.Template = _roomDatabase.GetCombatRoomTemplate();
                }
            }
        }

        #region Debug Visualization

        private void OnDrawGizmos()
        {
            if (_rootNode == null) return;

            // Draw all room bounds
            Gizmos.color = Color.white;
            foreach (var room in _generatedRooms)
            {
                Vector3 center = new Vector3(room.Bounds.center.x, room.Bounds.center.y, 0);
                Vector3 size = new Vector3(room.Bounds.width, room.Bounds.height, 0);
                Gizmos.DrawWireCube(center, size);

                // Color code by type
                switch (room.Type)
                {
                    case RoomType.Spawn:
                        Gizmos.color = Color.green;
                        break;
                    case RoomType.Boss:
                        Gizmos.color = Color.red;
                        break;
                    case RoomType.Treasure:
                        Gizmos.color = Color.yellow;
                        break;
                    case RoomType.Elite:
                        Gizmos.color = Color.magenta;
                        break;
                    default:
                        Gizmos.color = Color.white;
                        break;
                }

                Gizmos.DrawSphere(new Vector3(room.Center.x, room.Center.y, 0), 0.5f);
            }

            // Draw corridors
            Gizmos.color = Color.cyan;
            foreach (var corridor in _corridors)
            {
                Vector3 start = new Vector3(corridor.Start.x, corridor.Start.y, 0);
                Vector3 corner = new Vector3(corridor.Corner.x, corridor.Corner.y, 0);
                Vector3 end = new Vector3(corridor.End.x, corridor.End.y, 0);

                Gizmos.DrawLine(start, corner);
                Gizmos.DrawLine(corner, end);
            }
        }

        #endregion
    }

    /// <summary>
    /// BSP tree node representing a spatial partition.
    /// </summary>
    public class RoomNode
    {
        public RectInt Bounds;
        public RoomNode LeftChild;
        public RoomNode RightChild;
        public Room Room;

        public RoomNode(RectInt bounds)
        {
            Bounds = bounds;
        }

        public bool IsLeaf => LeftChild == null && RightChild == null;
    }

    /// <summary>
    /// Data structure representing a generated room.
    /// </summary>
    [System.Serializable]
    public class Room
    {
        public string Id;
        public RectInt Bounds;
        public Vector2Int Center;
        public RoomType Type;
        public RoomTemplate Template;
        public bool IsCleared;
        public List<GameObject> Enemies = new List<GameObject>();
    }

    /// <summary>
    /// Room type classification.
    /// </summary>
    public enum RoomType
    {
        Spawn,
        Combat,
        Elite,
        Boss,
        Treasure,
        Rest,
        Secret
    }

    /// <summary>
    /// Corridor connecting two rooms.
    /// </summary>
    [System.Serializable]
    public class Corridor
    {
        public Vector2Int Start;
        public Vector2Int End;
        public Vector2Int Corner; // L-shaped corner point
        public int Width;
    }

    /// <summary>
    /// Complete dungeon data structure.
    /// </summary>
    public class DungeonData
    {
        public int Seed;
        public List<Room> Rooms;
        public List<Corridor> Corridors;
        public int Width;
        public int Height;
    }
}
