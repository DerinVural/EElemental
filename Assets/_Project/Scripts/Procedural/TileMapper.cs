using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace EElemental.Procedural
{
    /// <summary>
    /// Converts generated dungeon data into Unity Tilemaps.
    /// Handles floor, wall, and decoration tile placement.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class TileMapper : MonoBehaviour
    {
        [Header("Tilemap References")]
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Tilemap _decorationTilemap;

        [Header("Default Tiles")]
        [SerializeField] private TileBase _defaultFloorTile;
        [SerializeField] private TileBase _defaultWallTile;
        [SerializeField] private TileBase _corridorFloorTile;

        [Header("Wall Settings")]
        [SerializeField] private int _wallHeight = 2;
        [SerializeField] private bool _autoGenerateWalls = true;

        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }

        /// <summary>
        /// Generate tilemap from dungeon data.
        /// </summary>
        public void GenerateTilemap(List<Room> rooms, List<Corridor> corridors)
        {
            ClearTilemaps();

            // Place corridor tiles first (so rooms can override)
            foreach (var corridor in corridors)
            {
                PlaceCorridorTiles(corridor);
            }

            // Place room tiles
            foreach (var room in rooms)
            {
                PlaceRoomTiles(room);
            }

            // Generate walls around rooms and corridors
            if (_autoGenerateWalls)
            {
                GenerateWalls(rooms, corridors);
            }
        }

        /// <summary>
        /// Clear all tilemaps.
        /// </summary>
        public void ClearTilemaps()
        {
            if (_floorTilemap != null) _floorTilemap.ClearAllTiles();
            if (_wallTilemap != null) _wallTilemap.ClearAllTiles();
            if (_decorationTilemap != null) _decorationTilemap.ClearAllTiles();
        }

        /// <summary>
        /// Place tiles for a room based on its template.
        /// </summary>
        private void PlaceRoomTiles(Room room)
        {
            if (_floorTilemap == null)
            {
                Debug.LogWarning("[TileMapper] Floor tilemap not assigned!");
                return;
            }

            // Determine tiles from template or use defaults
            TileBase floorTile = room.Template != null && room.Template.FloorTile != null
                ? room.Template.FloorTile
                : _defaultFloorTile;

            // Fill room with floor tiles
            for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
            {
                for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    _floorTilemap.SetTile(tilePos, floorTile);
                }
            }

            // Place decorations if template has them
            if (room.Template != null && room.Template.DecorationTile != null && _decorationTilemap != null)
            {
                PlaceRoomDecorations(room);
            }
        }

        /// <summary>
        /// Place corridor tiles.
        /// </summary>
        private void PlaceCorridorTiles(Corridor corridor)
        {
            if (_floorTilemap == null)
                return;

            TileBase corridorTile = _corridorFloorTile != null ? _corridorFloorTile : _defaultFloorTile;

            // Horizontal segment
            int minX = Mathf.Min(corridor.Start.x, corridor.Corner.x);
            int maxX = Mathf.Max(corridor.Start.x, corridor.Corner.x);
            int y = corridor.Start.y;

            for (int x = minX; x <= maxX; x++)
            {
                PlaceCorridorSegment(new Vector2Int(x, y), corridor.Width, corridorTile, true);
            }

            // Vertical segment
            int minY = Mathf.Min(corridor.Corner.y, corridor.End.y);
            int maxY = Mathf.Max(corridor.Corner.y, corridor.End.y);
            int x2 = corridor.End.x;

            for (int y2 = minY; y2 <= maxY; y2++)
            {
                PlaceCorridorSegment(new Vector2Int(x2, y2), corridor.Width, corridorTile, false);
            }
        }

        /// <summary>
        /// Place a segment of corridor with specified width.
        /// </summary>
        private void PlaceCorridorSegment(Vector2Int center, int width, TileBase tile, bool isHorizontal)
        {
            int halfWidth = width / 2;

            if (isHorizontal)
            {
                for (int dy = -halfWidth; dy <= halfWidth; dy++)
                {
                    Vector3Int tilePos = new Vector3Int(center.x, center.y + dy, 0);
                    _floorTilemap.SetTile(tilePos, tile);
                }
            }
            else
            {
                for (int dx = -halfWidth; dx <= halfWidth; dx++)
                {
                    Vector3Int tilePos = new Vector3Int(center.x + dx, center.y, 0);
                    _floorTilemap.SetTile(tilePos, tile);
                }
            }
        }

        /// <summary>
        /// Generate walls around all walkable areas.
        /// </summary>
        private void GenerateWalls(List<Room> rooms, List<Corridor> corridors)
        {
            if (_wallTilemap == null)
            {
                Debug.LogWarning("[TileMapper] Wall tilemap not assigned!");
                return;
            }

            TileBase wallTile = _defaultWallTile;

            // Get all floor positions
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

            // Collect floor positions from rooms
            foreach (var room in rooms)
            {
                for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
                {
                    for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
                    {
                        floorPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            // Collect floor positions from corridors
            foreach (var corridor in corridors)
            {
                int halfWidth = corridor.Width / 2;

                // Horizontal segment
                int minX = Mathf.Min(corridor.Start.x, corridor.Corner.x);
                int maxX = Mathf.Max(corridor.Start.x, corridor.Corner.x);
                for (int x = minX; x <= maxX; x++)
                {
                    for (int dy = -halfWidth; dy <= halfWidth; dy++)
                    {
                        floorPositions.Add(new Vector2Int(x, corridor.Start.y + dy));
                    }
                }

                // Vertical segment
                int minY = Mathf.Min(corridor.Corner.y, corridor.End.y);
                int maxY = Mathf.Max(corridor.Corner.y, corridor.End.y);
                for (int y = minY; y <= maxY; y++)
                {
                    for (int dx = -halfWidth; dx <= halfWidth; dx++)
                    {
                        floorPositions.Add(new Vector2Int(corridor.End.x + dx, y));
                    }
                }
            }

            // Place walls adjacent to floor tiles
            Vector2Int[] neighbors = new Vector2Int[]
            {
                new Vector2Int(0, 1),   // Up
                new Vector2Int(0, -1),  // Down
                new Vector2Int(1, 0),   // Right
                new Vector2Int(-1, 0),  // Left
                new Vector2Int(1, 1),   // Top-right
                new Vector2Int(-1, 1),  // Top-left
                new Vector2Int(1, -1),  // Bottom-right
                new Vector2Int(-1, -1)  // Bottom-left
            };

            foreach (var floorPos in floorPositions)
            {
                foreach (var offset in neighbors)
                {
                    Vector2Int neighborPos = floorPos + offset;

                    // If neighbor is not a floor, place wall
                    if (!floorPositions.Contains(neighborPos))
                    {
                        Vector3Int wallPos = new Vector3Int(neighborPos.x, neighborPos.y, 0);
                        _wallTilemap.SetTile(wallPos, wallTile);
                    }
                }
            }
        }

        /// <summary>
        /// Place decorations based on room template.
        /// </summary>
        private void PlaceRoomDecorations(Room room)
        {
            if (room.Template == null || room.Template.EnvironmentalFeatures.Count == 0)
                return;

            foreach (var feature in room.Template.EnvironmentalFeatures)
            {
                // Check spawn chance for optional features
                if (!feature.IsRequired && Random.value > feature.SpawnChance)
                    continue;

                // Adjust feature position to room bounds
                Vector2Int worldPos = new Vector2Int(
                    room.Bounds.x + feature.Position.x,
                    room.Bounds.y + feature.Position.y
                );

                // Place decoration tile or spawn prefab
                if (feature.Type == EnvironmentalFeature.FeatureType.Decoration)
                {
                    Vector3Int tilePos = new Vector3Int(worldPos.x, worldPos.y, 0);
                    if (room.Template.DecorationTile != null)
                    {
                        _decorationTilemap.SetTile(tilePos, room.Template.DecorationTile);
                    }
                }
                else if (feature.Prefab != null)
                {
                    // Spawn prefab (hazards, interactables, platforms)
                    Vector3 spawnPos = _grid.CellToWorld(new Vector3Int(worldPos.x, worldPos.y, 0));
                    Instantiate(feature.Prefab, spawnPos, Quaternion.identity, transform);
                }
            }
        }

        #region Public Helpers

        /// <summary>
        /// Convert world position to tile coordinates.
        /// </summary>
        public Vector3Int WorldToTilePosition(Vector3 worldPos)
        {
            return _grid.WorldToCell(worldPos);
        }

        /// <summary>
        /// Convert tile coordinates to world position.
        /// </summary>
        public Vector3 TileToWorldPosition(Vector3Int tilePos)
        {
            return _grid.CellToWorld(tilePos);
        }

        /// <summary>
        /// Check if a tile position has a floor tile.
        /// </summary>
        public bool HasFloorTile(Vector3Int tilePos)
        {
            return _floorTilemap != null && _floorTilemap.HasTile(tilePos);
        }

        /// <summary>
        /// Check if a tile position has a wall tile.
        /// </summary>
        public bool HasWallTile(Vector3Int tilePos)
        {
            return _wallTilemap != null && _wallTilemap.HasTile(tilePos);
        }

        #endregion
    }
}
