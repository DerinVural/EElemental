using UnityEngine;
using UnityEditor;

namespace EElemental.Editor
{
    /// <summary>
    /// Quick access menu for all EElemental setup tools.
    /// </summary>
    public static class EElementalMenu
    {
#if UNITY_EDITOR
        [MenuItem("Tools/EElemental/=== FULL SETUP (Do This First!) ===", priority = 0)]
        public static void FullSetup()
        {
            Debug.Log("==============================================");
            Debug.Log("[EElemental] Starting full project setup...");
            Debug.Log("==============================================");
            
            // Step 1: Create folder structure
            TestSceneSetup.CreateSOFolders();
            
            // Step 2: Show layer setup info
            TestSceneSetup.SetupLayersAndTags();
            
            // Step 3: Create ScriptableObjects
            ScriptableObjectCreator.CreateAllScriptableObjects();
            
            // Step 4: Create Prefabs
            PrefabCreator.CreateAllPrefabs();
            
            Debug.Log("");
            Debug.Log("==============================================");
            Debug.Log("[EElemental] Full setup complete!");
            Debug.Log("==============================================");
            Debug.Log("");
            Debug.Log("NEXT STEPS:");
            Debug.Log("1. Go to Edit → Project Settings → Tags and Layers");
            Debug.Log("2. Add the layers shown above (Ground, Player, Enemy, etc.)");
            Debug.Log("3. Add the sorting layers shown above");
            Debug.Log("4. Open Tools → EElemental → Setup Test Scene");
            Debug.Log("5. Press Play and test!");
        }
        
        [MenuItem("Tools/EElemental/--- Setup ---", priority = 10)]
        public static void SetupSeparator() { }
        
        [MenuItem("Tools/EElemental/--- Prefabs ---", priority = 30)]
        public static void PrefabSeparator() { }
        
        [MenuItem("Tools/EElemental/--- ScriptableObjects ---", priority = 50)]
        public static void SOSeparator() { }
        
        [MenuItem("Tools/EElemental/Open Documentation", priority = 100)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/DerinVural/EElemental");
        }
        
        [MenuItem("Tools/EElemental/About EElemental", priority = 101)]
        public static void About()
        {
            EditorUtility.DisplayDialog("About EElemental",
                "EElemental v1.0\n\n" +
                "A 2D side-scroller rogue-like game with elemental powers.\n\n" +
                "Inspired by Dead Cells.\n\n" +
                "Built with Unity 2022.3 LTS\n\n" +
                "GitHub: github.com/DerinVural/EElemental",
                "OK");
        }
#endif
    }
}
