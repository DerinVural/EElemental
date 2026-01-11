using UnityEngine;
using UnityEditor;

namespace EElemental.Editor
{
    /// <summary>
    /// Editor script to create Element ScriptableObjects.
    /// Use via: Tools → EElemental → Create Elements
    /// </summary>
    public static class ElementCreator
    {
#if UNITY_EDITOR
        [MenuItem("Tools/EElemental/Create Element ScriptableObjects")]
        public static void CreateElements()
        {
            EnsureFolder("Assets/_Project/ScriptableObjects/Elements");
            
            // Fire Element
            CreateElementAsset("Fire", 0, 1.5f, new Color(1f, 0.3f, 0f), 
                "Deals burn damage over time", "Assets/_Project/ScriptableObjects/Elements/Fire.asset");
            
            // Water Element
            CreateElementAsset("Water", 1, 1.0f, new Color(0f, 0.7f, 1f), 
                "Applies wet status, enables healing", "Assets/_Project/ScriptableObjects/Elements/Water.asset");
            
            // Earth Element
            CreateElementAsset("Earth", 2, 1.2f, new Color(0.55f, 0.27f, 0.07f), 
                "Increases defense, can stun enemies", "Assets/_Project/ScriptableObjects/Elements/Earth.asset");
            
            // Air Element
            CreateElementAsset("Air", 3, 0.8f, new Color(0.7f, 0.9f, 1f), 
                "Increases speed, applies knockback", "Assets/_Project/ScriptableObjects/Elements/Air.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[EElemental] Element ScriptableObjects created!");
        }
        
        private static void CreateElementAsset(string name, int elementType, float damageMultiplier, 
            Color color, string description, string path)
        {
            // Try to find ElementData type
            var elementDataType = System.Type.GetType("EElemental.Elements.ElementData, Assembly-CSharp");
            
            if (elementDataType != null)
            {
                var asset = ScriptableObject.CreateInstance(elementDataType);
                
                // Set properties via reflection
                var nameField = elementDataType.GetField("elementName");
                if (nameField != null) nameField.SetValue(asset, name);
                
                var typeField = elementDataType.GetField("elementType");
                if (typeField != null) typeField.SetValue(asset, elementType);
                
                var multiplierField = elementDataType.GetField("baseDamageMultiplier");
                if (multiplierField != null) multiplierField.SetValue(asset, damageMultiplier);
                
                var colorField = elementDataType.GetField("primaryColor");
                if (colorField != null) colorField.SetValue(asset, color);
                
                var descField = elementDataType.GetField("description");
                if (descField != null) descField.SetValue(asset, description);
                
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"Created: {path}");
            }
            else
            {
                Debug.LogWarning("ElementData type not found. Please ensure the script is compiled.");
            }
        }
        
        [MenuItem("Tools/EElemental/Create Enemy ScriptableObjects")]
        public static void CreateEnemies()
        {
            EnsureFolder("Assets/_Project/ScriptableObjects/Enemies");
            
            // Slime
            CreateEnemyAsset("Slime", 50f, 10f, 3f, 5f, 2f, 
                "Assets/_Project/ScriptableObjects/Enemies/SlimeData.asset");
            
            // Skeleton Warrior
            CreateEnemyAsset("Skeleton Warrior", 80f, 15f, 4f, 8f, 2.5f, 
                "Assets/_Project/ScriptableObjects/Enemies/SkeletonWarriorData.asset");
            
            // Bat
            CreateEnemyAsset("Bat", 30f, 8f, 6f, 10f, 3f, 
                "Assets/_Project/ScriptableObjects/Enemies/BatData.asset");
            
            // Golem
            CreateEnemyAsset("Golem", 150f, 25f, 2f, 4f, 1.5f, 
                "Assets/_Project/ScriptableObjects/Enemies/GolemData.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[EElemental] Enemy ScriptableObjects created!");
        }
        
        private static void CreateEnemyAsset(string name, float maxHealth, float attackDamage, 
            float moveSpeed, float detectionRange, float attackRange, string path)
        {
            var enemyDataType = System.Type.GetType("EElemental.Enemies.EnemyData, Assembly-CSharp");
            
            if (enemyDataType != null)
            {
                var asset = ScriptableObject.CreateInstance(enemyDataType);
                
                SetFieldValue(asset, enemyDataType, "enemyName", name);
                SetFieldValue(asset, enemyDataType, "maxHealth", maxHealth);
                SetFieldValue(asset, enemyDataType, "attackDamage", attackDamage);
                SetFieldValue(asset, enemyDataType, "moveSpeed", moveSpeed);
                SetFieldValue(asset, enemyDataType, "detectionRange", detectionRange);
                SetFieldValue(asset, enemyDataType, "attackRange", attackRange);
                
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"Created: {path}");
            }
            else
            {
                Debug.LogWarning("EnemyData type not found. Please ensure the script is compiled.");
            }
        }
        
        [MenuItem("Tools/EElemental/Create Weapon ScriptableObjects")]
        public static void CreateWeapons()
        {
            EnsureFolder("Assets/_Project/ScriptableObjects/Weapons");
            
            // Sword
            CreateWeaponAsset("Iron Sword", 15f, 1.0f, 2f, 0,
                "Assets/_Project/ScriptableObjects/Weapons/IronSword.asset");
            
            // Dagger
            CreateWeaponAsset("Steel Dagger", 8f, 0.5f, 1.5f, 1,
                "Assets/_Project/ScriptableObjects/Weapons/SteelDagger.asset");
            
            // Hammer
            CreateWeaponAsset("War Hammer", 30f, 2.0f, 1.8f, 2,
                "Assets/_Project/ScriptableObjects/Weapons/WarHammer.asset");
            
            // Staff
            CreateWeaponAsset("Elemental Staff", 12f, 1.2f, 3f, 3,
                "Assets/_Project/ScriptableObjects/Weapons/ElementalStaff.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[EElemental] Weapon ScriptableObjects created!");
        }
        
        private static void CreateWeaponAsset(string name, float damage, float attackSpeed, 
            float range, int weaponType, string path)
        {
            var weaponDataType = System.Type.GetType("EElemental.Weapons.WeaponData, Assembly-CSharp");
            
            if (weaponDataType != null)
            {
                var asset = ScriptableObject.CreateInstance(weaponDataType);
                
                SetFieldValue(asset, weaponDataType, "weaponName", name);
                SetFieldValue(asset, weaponDataType, "baseDamage", damage);
                SetFieldValue(asset, weaponDataType, "attackSpeed", attackSpeed);
                SetFieldValue(asset, weaponDataType, "attackRange", range);
                SetFieldValue(asset, weaponDataType, "weaponType", weaponType);
                
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"Created: {path}");
            }
            else
            {
                Debug.LogWarning("WeaponData type not found. Please ensure the script is compiled.");
            }
        }
        
        private static void SetFieldValue(object obj, System.Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
        
        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
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
        }
        
        [MenuItem("Tools/EElemental/Create All ScriptableObjects")]
        public static void CreateAllScriptableObjects()
        {
            CreateElements();
            CreateEnemies();
            CreateWeapons();
            Debug.Log("[EElemental] All ScriptableObjects created!");
        }
#endif
    }
}
