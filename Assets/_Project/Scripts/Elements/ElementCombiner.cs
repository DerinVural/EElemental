using UnityEngine;
using EElemental.Core;

namespace EElemental.Elements
{
    /// <summary>
    /// Handles element combination logic at runtime.
    /// </summary>
    public class ElementCombiner : Singleton<ElementCombiner>
    {
        [SerializeField] private ElementDatabase _database;
        
        public ElementDatabase Database => _database;
        
        // Events
        public static event System.Action<ElementType, ElementType, ElementData> OnElementsCombined;
        
        /// <summary>
        /// Try to combine two elements and return the result.
        /// </summary>
        public ElementData Combine(ElementType a, ElementType b)
        {
            if (_database == null)
            {
                Debug.LogError("[ElementCombiner] No ElementDatabase assigned!");
                return null;
            }
            
            var result = _database.TryCombine(a, b);
            
            if (result != null)
            {
                OnElementsCombined?.Invoke(a, b, result);
                Debug.Log($"[ElementCombiner] Combined {a} + {b} = {result.elementName}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Combine using ElementData objects.
        /// </summary>
        public ElementData Combine(ElementData a, ElementData b)
        {
            if (a == null || b == null) return null;
            return Combine(a.type, b.type);
        }
        
        /// <summary>
        /// Get element data by type.
        /// </summary>
        public ElementData GetElement(ElementType type)
        {
            return _database?.GetElement(type);
        }
        
        /// <summary>
        /// Check if combination is possible.
        /// </summary>
        public bool CanCombine(ElementType a, ElementType b)
        {
            return _database?.CanCombine(a, b) ?? false;
        }
    }
}
