using UnityEngine;
using UnityEditor;

namespace EElemental.Editor
{
    /// <summary>
    /// Editor script to quickly create prefabs for the game.
    /// Use via: Tools → EElemental → Create Prefabs
    /// </summary>
    public static class PrefabCreator
    {
#if UNITY_EDITOR
        [MenuItem("Tools/EElemental/Create Player Prefab")]
        public static void CreatePlayerPrefab()
        {
            // Create player GameObject
            var player = new GameObject("Player");
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Player");
            
            // Rigidbody2D
            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // Collider
            var collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.8f);
            collider.offset = new Vector2(0, 0);
            
            // Sprite
            var sprite = player.AddComponent<SpriteRenderer>();
            sprite.color = Color.blue;
            sprite.sortingLayerName = "Player";
            sprite.sortingOrder = 0;
            
            // Animator
            player.AddComponent<Animator>();
            
            // Add player scripts
            AddComponentByName(player, "EElemental.Player.PlayerStats");
            AddComponentByName(player, "EElemental.Player.PlayerMovement");
            AddComponentByName(player, "EElemental.Player.PlayerController");
            
            // Create child objects
            CreateChildCheck(player.transform, "GroundCheck", new Vector3(0, -1f, 0));
            CreateChildCheck(player.transform, "WallCheck", new Vector3(0.5f, 0, 0));
            CreateChildCheck(player.transform, "CeilingCheck", new Vector3(0, 1f, 0));
            
            // Attack hitbox
            var attackHitbox = new GameObject("AttackHitbox");
            attackHitbox.transform.SetParent(player.transform);
            attackHitbox.transform.localPosition = new Vector3(1f, 0, 0);
            var hitboxCollider = attackHitbox.AddComponent<BoxCollider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.size = new Vector2(1.5f, 1.5f);
            attackHitbox.SetActive(false);
            
            // Save as prefab
            SavePrefab(player, "Assets/_Project/Prefabs/Player/Player.prefab");
            
            Debug.Log("[EElemental] Player prefab created!");
        }
        
        [MenuItem("Tools/EElemental/Create Slime Prefab")]
        public static void CreateSlimePrefab()
        {
            var slime = CreateEnemyBase("Slime", Color.green, new Vector2(1f, 1f));
            AddComponentByName(slime, "EElemental.Enemies.SlimeEnemy");
            SavePrefab(slime, "Assets/_Project/Prefabs/Enemies/Slime.prefab");
            Debug.Log("[EElemental] Slime prefab created!");
        }
        
        [MenuItem("Tools/EElemental/Create Skeleton Prefab")]
        public static void CreateSkeletonPrefab()
        {
            var skeleton = CreateEnemyBase("SkeletonWarrior", Color.gray, new Vector2(0.8f, 1.8f));
            AddComponentByName(skeleton, "EElemental.Enemies.SkeletonWarrior");
            SavePrefab(skeleton, "Assets/_Project/Prefabs/Enemies/SkeletonWarrior.prefab");
            Debug.Log("[EElemental] Skeleton Warrior prefab created!");
        }
        
        private static GameObject CreateEnemyBase(string name, Color color, Vector2 colliderSize)
        {
            var enemy = new GameObject(name);
            enemy.tag = "Enemy";
            enemy.layer = LayerMask.NameToLayer("Enemy");
            
            // Rigidbody2D
            var rb = enemy.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;
            rb.freezeRotation = true;
            
            // Collider
            var collider = enemy.AddComponent<BoxCollider2D>();
            collider.size = colliderSize;
            
            // Sprite
            var sprite = enemy.AddComponent<SpriteRenderer>();
            sprite.color = color;
            sprite.sortingLayerName = "Enemies";
            
            // Animator
            enemy.AddComponent<Animator>();
            
            // Add enemy scripts
            AddComponentByName(enemy, "EElemental.Enemies.EnemyStats");
            
            // Create child objects
            CreateChildCheck(enemy.transform, "GroundCheck", new Vector3(0, -colliderSize.y / 2, 0));
            CreateChildCheck(enemy.transform, "PlayerDetector", new Vector3(0, 0, 0));
            
            // Attack hitbox
            var attackHitbox = new GameObject("AttackHitbox");
            attackHitbox.transform.SetParent(enemy.transform);
            attackHitbox.transform.localPosition = new Vector3(1f, 0, 0);
            var hitboxCollider = attackHitbox.AddComponent<BoxCollider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.size = new Vector2(1f, 1f);
            attackHitbox.SetActive(false);
            
            // Hurtbox
            var hurtbox = new GameObject("Hurtbox");
            hurtbox.transform.SetParent(enemy.transform);
            hurtbox.transform.localPosition = Vector3.zero;
            var hurtboxCollider = hurtbox.AddComponent<BoxCollider2D>();
            hurtboxCollider.isTrigger = true;
            hurtboxCollider.size = colliderSize;
            
            return enemy;
        }
        
        private static void CreateChildCheck(Transform parent, string name, Vector3 localPosition)
        {
            var check = new GameObject(name);
            check.transform.SetParent(parent);
            check.transform.localPosition = localPosition;
        }
        
        private static void AddComponentByName(GameObject go, string typeName)
        {
            var type = System.Type.GetType(typeName + ", Assembly-CSharp");
            if (type != null)
            {
                go.AddComponent(type);
            }
            else
            {
                Debug.LogWarning($"Could not find type: {typeName}");
            }
        }
        
        private static void SavePrefab(GameObject go, string path)
        {
            // Ensure directory exists
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                var parts = directory.Replace("\\", "/").Split('/');
                var parent = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var newPath = parent + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(parent, parts[i]);
                    }
                    parent = newPath;
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }
        
        [MenuItem("Tools/EElemental/Create All Prefabs")]
        public static void CreateAllPrefabs()
        {
            CreatePlayerPrefab();
            CreateSlimePrefab();
            CreateSkeletonPrefab();
            Debug.Log("[EElemental] All prefabs created!");
        }
#endif
    }
}
