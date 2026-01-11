using UnityEngine;
using UnityEditor;

namespace EElemental.Editor
{
    /// <summary>
    /// Editor script to quickly setup a test scene with all required objects.
    /// Use via: Tools → EElemental → Setup Test Scene
    /// </summary>
    public static class TestSceneSetup
    {
#if UNITY_EDITOR
        [MenuItem("Tools/EElemental/Setup Test Scene")]
        public static void SetupTestScene()
        {
            Debug.Log("[EElemental] Setting up test scene...");
            
            // Create managers
            CreateManagers();
            
            // Create environment
            CreateEnvironment();
            
            // Create player spawn point
            CreatePlayerSpawn();
            
            // Create enemy spawn points
            CreateEnemySpawns();
            
            // Create UI canvas
            CreateUICanvas();
            
            // Setup camera
            SetupCamera();
            
            Debug.Log("[EElemental] Test scene setup complete!");
        }
        
        private static void CreateManagers()
        {
            // Create Managers parent
            var managersParent = new GameObject("--- MANAGERS ---");
            
            // GameManager
            var gameManager = new GameObject("GameManager");
            gameManager.transform.SetParent(managersParent.transform);
            // Add GameManager component if it exists
            var gmType = System.Type.GetType("EElemental.Core.GameManager, Assembly-CSharp");
            if (gmType != null)
            {
                gameManager.AddComponent(gmType);
            }
            
            // EventManager
            var eventManager = new GameObject("EventManager");
            eventManager.transform.SetParent(managersParent.transform);
        }
        
        private static void CreateEnvironment()
        {
            // Create Environment parent
            var envParent = new GameObject("--- ENVIRONMENT ---");
            
            // Ground
            var ground = new GameObject("Ground");
            ground.transform.SetParent(envParent.transform);
            ground.transform.position = new Vector3(0, -3, 0);
            ground.transform.localScale = new Vector3(30, 1, 1);
            
            var groundSprite = ground.AddComponent<SpriteRenderer>();
            groundSprite.color = new Color(0.4f, 0.3f, 0.2f); // Brown
            groundSprite.sortingLayerName = "Environment";
            
            var groundCollider = ground.AddComponent<BoxCollider2D>();
            ground.layer = LayerMask.NameToLayer("Ground");
            
            // Left Wall
            var leftWall = new GameObject("LeftWall");
            leftWall.transform.SetParent(envParent.transform);
            leftWall.transform.position = new Vector3(-15, 0, 0);
            leftWall.transform.localScale = new Vector3(1, 10, 1);
            
            var leftWallSprite = leftWall.AddComponent<SpriteRenderer>();
            leftWallSprite.color = new Color(0.3f, 0.3f, 0.3f);
            leftWall.AddComponent<BoxCollider2D>();
            
            // Right Wall
            var rightWall = new GameObject("RightWall");
            rightWall.transform.SetParent(envParent.transform);
            rightWall.transform.position = new Vector3(15, 0, 0);
            rightWall.transform.localScale = new Vector3(1, 10, 1);
            
            var rightWallSprite = rightWall.AddComponent<SpriteRenderer>();
            rightWallSprite.color = new Color(0.3f, 0.3f, 0.3f);
            rightWall.AddComponent<BoxCollider2D>();
            
            // Platforms
            CreatePlatform(envParent.transform, "Platform1", new Vector3(-5, -1, 0), new Vector3(4, 0.5f, 1));
            CreatePlatform(envParent.transform, "Platform2", new Vector3(5, 0, 0), new Vector3(4, 0.5f, 1));
            CreatePlatform(envParent.transform, "Platform3", new Vector3(0, 1.5f, 0), new Vector3(3, 0.5f, 1));
        }
        
        private static void CreatePlatform(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            var platform = new GameObject(name);
            platform.transform.SetParent(parent);
            platform.transform.position = position;
            platform.transform.localScale = scale;
            
            var sprite = platform.AddComponent<SpriteRenderer>();
            sprite.color = new Color(0.5f, 0.4f, 0.3f);
            platform.AddComponent<BoxCollider2D>();
            platform.layer = LayerMask.NameToLayer("Ground");
        }
        
        private static void CreatePlayerSpawn()
        {
            var playerParent = new GameObject("--- PLAYER ---");
            
            var playerSpawn = new GameObject("PlayerSpawnPoint");
            playerSpawn.transform.SetParent(playerParent.transform);
            playerSpawn.transform.position = Vector3.zero;
            
            // Add a visual indicator
            var indicator = playerSpawn.AddComponent<SpriteRenderer>();
            indicator.color = Color.green;
            indicator.sortingOrder = 100;
            
            // Create player placeholder
            var playerPlaceholder = new GameObject("Player (Add Prefab Here)");
            playerPlaceholder.transform.SetParent(playerParent.transform);
            playerPlaceholder.transform.position = Vector3.zero;
            playerPlaceholder.tag = "Player";
            
            // Add basic components for testing
            playerPlaceholder.AddComponent<Rigidbody2D>();
            var collider = playerPlaceholder.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 2);
            
            var sprite = playerPlaceholder.AddComponent<SpriteRenderer>();
            sprite.color = Color.blue;
            sprite.sortingLayerName = "Player";
        }
        
        private static void CreateEnemySpawns()
        {
            var enemyParent = new GameObject("--- ENEMIES ---");
            
            // Create spawn points
            CreateEnemySpawnPoint(enemyParent.transform, "EnemySpawn1", new Vector3(5, 0, 0));
            CreateEnemySpawnPoint(enemyParent.transform, "EnemySpawn2", new Vector3(-5, 0, 0));
            CreateEnemySpawnPoint(enemyParent.transform, "EnemySpawn3", new Vector3(8, 0, 0));
            
            // Create enemy placeholders
            CreateEnemyPlaceholder(enemyParent.transform, "Slime (Placeholder)", new Vector3(5, -2, 0), Color.green);
            CreateEnemyPlaceholder(enemyParent.transform, "Skeleton (Placeholder)", new Vector3(-5, -2, 0), Color.gray);
        }
        
        private static void CreateEnemySpawnPoint(Transform parent, string name, Vector3 position)
        {
            var spawn = new GameObject(name);
            spawn.transform.SetParent(parent);
            spawn.transform.position = position;
            
            var indicator = spawn.AddComponent<SpriteRenderer>();
            indicator.color = Color.red;
            indicator.sortingOrder = 100;
        }
        
        private static void CreateEnemyPlaceholder(Transform parent, string name, Vector3 position, Color color)
        {
            var enemy = new GameObject(name);
            enemy.transform.SetParent(parent);
            enemy.transform.position = position;
            enemy.tag = "Enemy";
            enemy.layer = LayerMask.NameToLayer("Enemy");
            
            enemy.AddComponent<Rigidbody2D>();
            var collider = enemy.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 1);
            
            var sprite = enemy.AddComponent<SpriteRenderer>();
            sprite.color = color;
            sprite.sortingLayerName = "Enemies";
        }
        
        private static void CreateUICanvas()
        {
            var uiParent = new GameObject("--- UI ---");
            
            // Main Canvas
            var canvas = new GameObject("GameCanvas");
            canvas.transform.SetParent(uiParent.transform);
            
            var canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = 100;
            
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // HUD Container
            var hudContainer = new GameObject("HUD");
            hudContainer.transform.SetParent(canvas.transform);
            var hudRect = hudContainer.AddComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;
            
            // Health Bar placeholder
            CreateUIPlaceholder(hudContainer.transform, "HealthBar", new Vector2(0, 1), new Vector2(0, 1), 
                new Vector2(150, -30), new Vector2(300, 30), Color.red);
            
            // Mana Bar placeholder
            CreateUIPlaceholder(hudContainer.transform, "ManaBar", new Vector2(0, 1), new Vector2(0, 1), 
                new Vector2(150, -70), new Vector2(250, 20), Color.blue);
            
            // Element UI placeholder
            CreateUIPlaceholder(hudContainer.transform, "ElementUI", new Vector2(0.5f, 0), new Vector2(0.5f, 0), 
                new Vector2(0, 50), new Vector2(200, 60), Color.yellow);
            
            // Combat UI placeholder
            CreateUIPlaceholder(hudContainer.transform, "CombatUI", new Vector2(1, 1), new Vector2(1, 1), 
                new Vector2(-100, -30), new Vector2(150, 40), Color.white);
            
            // EventSystem
            var eventSystem = new GameObject("EventSystem");
            eventSystem.transform.SetParent(uiParent.transform);
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        private static void CreateUIPlaceholder(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, 
            Vector2 position, Vector2 size, Color color)
        {
            var uiElement = new GameObject(name);
            uiElement.transform.SetParent(parent);
            
            var rect = uiElement.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            var image = uiElement.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(color.r, color.g, color.b, 0.5f);
            
            // Add label
            var label = new GameObject("Label");
            label.transform.SetParent(uiElement.transform);
            var labelRect = label.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            
            var text = label.AddComponent<UnityEngine.UI.Text>();
            text.text = name;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.fontSize = 14;
        }
        
        private static void SetupCamera()
        {
            var cameraParent = new GameObject("--- CAMERA ---");
            
            // Find or create main camera
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var cameraGO = new GameObject("Main Camera");
                cameraGO.transform.SetParent(cameraParent.transform);
                mainCamera = cameraGO.AddComponent<Camera>();
                cameraGO.AddComponent<AudioListener>();
                cameraGO.tag = "MainCamera";
            }
            else
            {
                mainCamera.transform.SetParent(cameraParent.transform);
            }
            
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }
        
        [MenuItem("Tools/EElemental/Setup Layers and Tags")]
        public static void SetupLayersAndTags()
        {
            Debug.Log("[EElemental] Setting up layers and tags...");
            Debug.Log("Please manually add these layers in Edit → Project Settings → Tags and Layers:");
            Debug.Log("  Layer 8: Ground");
            Debug.Log("  Layer 9: Player");
            Debug.Log("  Layer 10: Enemy");
            Debug.Log("  Layer 11: Projectile");
            Debug.Log("  Layer 12: Interactable");
            Debug.Log("");
            Debug.Log("And these Sorting Layers:");
            Debug.Log("  0: Background");
            Debug.Log("  1: Environment");
            Debug.Log("  2: Enemies");
            Debug.Log("  3: Player");
            Debug.Log("  4: Foreground");
            Debug.Log("  5: UI");
            Debug.Log("");
            Debug.Log("Tags to add:");
            Debug.Log("  - Player");
            Debug.Log("  - Enemy");
            Debug.Log("  - Weapon");
            Debug.Log("  - Interactable");
        }
        
        [MenuItem("Tools/EElemental/Create ScriptableObject Folders")]
        public static void CreateSOFolders()
        {
            string[] folders = new string[]
            {
                "Assets/_Project",
                "Assets/_Project/ScriptableObjects",
                "Assets/_Project/ScriptableObjects/Elements",
                "Assets/_Project/ScriptableObjects/Enemies",
                "Assets/_Project/ScriptableObjects/Weapons",
                "Assets/_Project/ScriptableObjects/Rooms",
                "Assets/_Project/Prefabs",
                "Assets/_Project/Prefabs/Player",
                "Assets/_Project/Prefabs/Enemies",
                "Assets/_Project/Prefabs/Weapons",
                "Assets/_Project/Prefabs/UI",
                "Assets/_Project/Scenes",
                "Assets/_Project/Art",
                "Assets/_Project/Art/Sprites",
                "Assets/_Project/Art/Animations",
                "Assets/_Project/Audio",
                "Assets/_Project/Audio/SFX",
                "Assets/_Project/Audio/Music"
            };
            
            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    var parts = folder.Split('/');
                    var parent = string.Join("/", parts, 0, parts.Length - 1);
                    var newFolder = parts[parts.Length - 1];
                    AssetDatabase.CreateFolder(parent, newFolder);
                    Debug.Log($"Created folder: {folder}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log("[EElemental] Folder structure created!");
        }
#endif
    }
}
