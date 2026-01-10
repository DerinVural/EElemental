# Procedural Generation System 🗺️

## Genel Bakış

Dungeon generation sistemi her run'da benzersiz level'lar üretir. Room-based yaklaşım kullanılacak.

---

## Generation Stratejisi

### Seçilen Yaklaşım: Room-Based with BSP

```
1. BSP (Binary Space Partitioning) ile alan bölme
2. Her bölüme room template yerleştirme
3. Corridor'larla odaları bağlama
4. Spawn points ve loot yerleştirme
5. Difficulty scaling uygulama
```

---

## Room Types

| Room Type | Sembol | Açıklama |
|-----------|--------|----------|
| **Start** | `S` | Oyuncunun spawn olduğu oda |
| **Combat** | `C` | Düşman dalgaları |
| **Elite** | `E` | Güçlü düşman(lar) |
| **Boss** | `B` | Boss fight |
| **Treasure** | `T` | Loot odası |
| **Shop** | `$` | Item satın alma |
| **Rest** | `R` | Heal/upgrade |
| **Secret** | `?` | Gizli oda |
| **Transition** | `-` | Corridor/hallway |

---

## Dungeon Flow

```
Floor 1 (Easy):
┌───┐   ┌───┐   ┌───┐
│ S │───│ C │───│ C │
└───┘   └───┘   └─┬─┘
                  │
        ┌───┐   ┌─┴─┐
        │ T │───│ C │
        └───┘   └─┬─┘
                  │
                ┌─┴─┐
                │ E │
                └─┬─┘
                  │
                ┌─┴─┐
                │ B │
                └───┘

Floor N (Harder):
- Daha fazla combat room
- Daha az treasure
- Daha karmaşık layout
- Secret room şansı artar
```

---

## BSP Algorithm

```csharp
public class BSPDungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private int dungeonWidth = 100;
    [SerializeField] private int dungeonHeight = 100;
    [SerializeField] private int minRoomSize = 15;
    [SerializeField] private int maxRoomSize = 30;
    [SerializeField] private int maxIterations = 5;

    [Header("Room Settings")]
    [SerializeField] private int roomPadding = 2;
    [SerializeField] private int corridorWidth = 3;

    private List<RoomNode> allRooms = new List<RoomNode>();

    public DungeonData Generate(int seed)
    {
        Random.InitState(seed);
        allRooms.Clear();

        // Create root node
        RoomNode root = new RoomNode(
            new RectInt(0, 0, dungeonWidth, dungeonHeight)
        );

        // BSP split
        Split(root, 0);

        // Collect leaf nodes (actual rooms)
        List<RoomNode> leafRooms = new List<RoomNode>();
        CollectLeafNodes(root, leafRooms);

        // Create room bounds within partitions
        foreach (var room in leafRooms)
        {
            CreateRoomInPartition(room);
        }

        // Connect rooms with corridors
        List<Corridor> corridors = ConnectRooms(root);

        // Assign room types
        AssignRoomTypes(leafRooms);

        return new DungeonData
        {
            rooms = leafRooms,
            corridors = corridors,
            seed = seed
        };
    }

    private void Split(RoomNode node, int iteration)
    {
        if (iteration >= maxIterations) return;

        bool splitHorizontally = Random.value > 0.5f;

        // Force split direction based on aspect ratio
        if (node.bounds.width > node.bounds.height * 1.25f)
            splitHorizontally = false;
        else if (node.bounds.height > node.bounds.width * 1.25f)
            splitHorizontally = true;

        int maxSize = (splitHorizontally ? node.bounds.height : node.bounds.width) - minRoomSize;

        if (maxSize < minRoomSize) return; // Too small to split

        int splitPos = Random.Range(minRoomSize, maxSize);

        if (splitHorizontally)
        {
            node.left = new RoomNode(new RectInt(
                node.bounds.x,
                node.bounds.y,
                node.bounds.width,
                splitPos
            ));
            node.right = new RoomNode(new RectInt(
                node.bounds.x,
                node.bounds.y + splitPos,
                node.bounds.width,
                node.bounds.height - splitPos
            ));
        }
        else
        {
            node.left = new RoomNode(new RectInt(
                node.bounds.x,
                node.bounds.y,
                splitPos,
                node.bounds.height
            ));
            node.right = new RoomNode(new RectInt(
                node.bounds.x + splitPos,
                node.bounds.y,
                node.bounds.width - splitPos,
                node.bounds.height
            ));
        }

        Split(node.left, iteration + 1);
        Split(node.right, iteration + 1);
    }

    private void CreateRoomInPartition(RoomNode node)
    {
        int roomWidth = Random.Range(minRoomSize,
            Mathf.Min(maxRoomSize, node.bounds.width - roomPadding * 2));
        int roomHeight = Random.Range(minRoomSize,
            Mathf.Min(maxRoomSize, node.bounds.height - roomPadding * 2));

        int roomX = node.bounds.x + Random.Range(roomPadding,
            node.bounds.width - roomWidth - roomPadding);
        int roomY = node.bounds.y + Random.Range(roomPadding,
            node.bounds.height - roomHeight - roomPadding);

        node.roomBounds = new RectInt(roomX, roomY, roomWidth, roomHeight);
    }
}

public class RoomNode
{
    public RectInt bounds;
    public RectInt roomBounds;
    public RoomNode left;
    public RoomNode right;
    public RoomType type;

    public RoomNode(RectInt bounds)
    {
        this.bounds = bounds;
    }

    public bool IsLeaf => left == null && right == null;
}
```

---

## Room Templates

### Template Yapısı

```csharp
[CreateAssetMenu(fileName = "NewRoom", menuName = "EElemental/Room Template")]
public class RoomTemplate : ScriptableObject
{
    [Header("Identity")]
    public string roomName;
    public RoomType type;
    public RoomSize size; // Small, Medium, Large

    [Header("Layout")]
    public GameObject prefab;
    public Vector2Int gridSize;

    [Header("Spawn Points")]
    public List<SpawnPoint> enemySpawnPoints;
    public List<SpawnPoint> itemSpawnPoints;
    public Transform playerSpawnPoint;

    [Header("Connections")]
    public List<DoorPoint> doorPoints; // Where corridors can connect

    [Header("Requirements")]
    public int minFloor = 1;
    public int maxFloor = -1; // -1 = no limit
    public float spawnWeight = 1f;
}

[System.Serializable]
public class SpawnPoint
{
    public Vector2 position;
    public SpawnPointType type;
}

[System.Serializable]
public class DoorPoint
{
    public Vector2 position;
    public Direction direction; // Up, Down, Left, Right
}

public enum RoomType
{
    Start,
    Combat,
    Elite,
    Boss,
    Treasure,
    Shop,
    Rest,
    Secret,
    Transition
}

public enum RoomSize
{
    Small,   // 1x1 grid
    Medium,  // 2x1 or 1x2
    Large    // 2x2
}
```

---

## Room Type Distribution

```csharp
public class RoomTypeAssigner
{
    public void AssignTypes(List<RoomNode> rooms, int floorNumber)
    {
        // Sort by distance from center for placement logic
        var center = CalculateCenter(rooms);
        rooms.Sort((a, b) =>
            Vector2.Distance(GetRoomCenter(a), center)
            .CompareTo(Vector2.Distance(GetRoomCenter(b), center)));

        // Assign fixed rooms
        rooms[0].type = RoomType.Start;
        rooms[rooms.Count - 1].type = RoomType.Boss;

        // Calculate room distribution
        int combatCount = Mathf.FloorToInt(rooms.Count * 0.5f);
        int treasureCount = Mathf.Max(1, Mathf.FloorToInt(rooms.Count * 0.1f));
        int eliteCount = Mathf.Max(1, floorNumber / 2);

        // Assign remaining rooms
        List<RoomNode> unassigned = rooms.Skip(1).Take(rooms.Count - 2).ToList();
        Shuffle(unassigned);

        int index = 0;

        // Elites (place before boss)
        for (int i = 0; i < eliteCount && index < unassigned.Count; i++)
        {
            unassigned[index++].type = RoomType.Elite;
        }

        // Treasure
        for (int i = 0; i < treasureCount && index < unassigned.Count; i++)
        {
            unassigned[index++].type = RoomType.Treasure;
        }

        // Rest of combat
        while (index < unassigned.Count)
        {
            unassigned[index++].type = RoomType.Combat;
        }
    }
}
```

---

## Tilemap Integration

```csharp
public class TileMapper : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap decorationTilemap;

    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase[] decorationTiles;

    public void RenderDungeon(DungeonData dungeon)
    {
        ClearAllTilemaps();

        // Render rooms
        foreach (var room in dungeon.rooms)
        {
            RenderRoom(room);
        }

        // Render corridors
        foreach (var corridor in dungeon.corridors)
        {
            RenderCorridor(corridor);
        }

        // Add walls around floors
        GenerateWalls();

        // Add decorations
        AddDecorations();
    }

    private void RenderRoom(RoomNode room)
    {
        for (int x = room.roomBounds.x; x < room.roomBounds.xMax; x++)
        {
            for (int y = room.roomBounds.y; y < room.roomBounds.yMax; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
    }

    private void GenerateWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;

        for (int x = bounds.x - 1; x <= bounds.xMax; x++)
        {
            for (int y = bounds.y - 1; y <= bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // If no floor and adjacent to floor, place wall
                if (floorTilemap.GetTile(pos) == null && HasAdjacentFloor(pos))
                {
                    wallTilemap.SetTile(pos, wallTile);
                }
            }
        }
    }

    private bool HasAdjacentFloor(Vector3Int pos)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int checkPos = new Vector3Int(pos.x + dx, pos.y + dy, 0);
                if (floorTilemap.GetTile(checkPos) != null)
                    return true;
            }
        }
        return false;
    }
}
```

---

## Difficulty Scaling

```csharp
public class DifficultyScaler
{
    public DifficultySettings GetSettings(int floorNumber, int runNumber)
    {
        return new DifficultySettings
        {
            // Enemy scaling
            enemyHealthMultiplier = 1f + (floorNumber * 0.15f),
            enemyDamageMultiplier = 1f + (floorNumber * 0.1f),
            enemyCountMultiplier = 1f + (floorNumber * 0.2f),

            // Spawn chances
            eliteSpawnChance = Mathf.Min(0.3f, 0.05f + (floorNumber * 0.03f)),

            // Room distribution
            combatRoomRatio = Mathf.Min(0.7f, 0.5f + (floorNumber * 0.02f)),
            treasureRoomRatio = Mathf.Max(0.05f, 0.15f - (floorNumber * 0.01f)),

            // Meta progression bonus (future)
            playerStartBonus = runNumber * 0.01f
        };
    }
}

public struct DifficultySettings
{
    public float enemyHealthMultiplier;
    public float enemyDamageMultiplier;
    public float enemyCountMultiplier;
    public float eliteSpawnChance;
    public float combatRoomRatio;
    public float treasureRoomRatio;
    public float playerStartBonus;
}
```

---

## Enemy Spawning

```csharp
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyDatabase enemyDatabase;

    public void PopulateRoom(RoomNode room, DifficultySettings difficulty)
    {
        if (room.type != RoomType.Combat && room.type != RoomType.Elite)
            return;

        RoomTemplate template = room.template;

        int enemyCount = CalculateEnemyCount(room, difficulty);
        var spawnPoints = new List<SpawnPoint>(template.enemySpawnPoints);
        Shuffle(spawnPoints);

        for (int i = 0; i < enemyCount && i < spawnPoints.Count; i++)
        {
            EnemyData enemy = SelectEnemy(room, difficulty);
            SpawnEnemy(enemy, spawnPoints[i].position, difficulty);
        }
    }

    private EnemyData SelectEnemy(RoomNode room, DifficultySettings difficulty)
    {
        if (room.type == RoomType.Elite)
        {
            return enemyDatabase.GetRandomElite();
        }

        // Weighted random selection based on floor
        return enemyDatabase.GetRandomEnemy(difficulty);
    }

    private void SpawnEnemy(EnemyData data, Vector2 position, DifficultySettings difficulty)
    {
        GameObject enemy = Instantiate(data.prefab, position, Quaternion.identity);

        // Apply difficulty scaling
        var stats = enemy.GetComponent<EnemyStats>();
        stats.maxHealth *= difficulty.enemyHealthMultiplier;
        stats.damage *= difficulty.enemyDamageMultiplier;
    }
}
```

---

## Seed System

```csharp
public class RunManager : MonoBehaviour
{
    public int CurrentSeed { get; private set; }

    public void StartNewRun(int? customSeed = null)
    {
        CurrentSeed = customSeed ?? GenerateRandomSeed();

        // Initialize all random systems with seed
        Random.InitState(CurrentSeed);

        // Generate first floor
        GenerateFloor(1);
    }

    private int GenerateRandomSeed()
    {
        return System.DateTime.Now.GetHashCode();
    }

    public string GetSeedString()
    {
        // Convert to shareable format
        return System.Convert.ToBase64String(
            System.BitConverter.GetBytes(CurrentSeed)
        ).TrimEnd('=');
    }

    public int ParseSeedString(string seedString)
    {
        // Parse shared seed
        byte[] bytes = System.Convert.FromBase64String(seedString + "==");
        return System.BitConverter.ToInt32(bytes, 0);
    }
}
```

---

## Sonraki Adımlar

1. [ ] BSP generator implementasyonu
2. [ ] Room template sistemi
3. [ ] Tilemap renderer
4. [ ] Corridor generation
5. [ ] Room type assignment
6. [ ] Enemy spawner
7. [ ] Difficulty scaling
8. [ ] Seed system
9. [ ] Mini-map UI
