using System.Collections.Generic;
using UnityEngine;

namespace EElemental.Elements
{
    /// <summary>
    /// Database containing all elements in the game.
    /// Used for lookups and element combinations.
    /// </summary>
    [CreateAssetMenu(fileName = "ElementDatabase", menuName = "EElemental/Element Database")]
    public class ElementDatabase : ScriptableObject
    {
        [Header("Base Elements")]
        public ElementData fireElement;
        public ElementData waterElement;
        public ElementData earthElement;
        public ElementData airElement;
        
        [Header("Combined Elements")]
        public ElementData steamElement;    // Fire + Water
        public ElementData magmaElement;    // Fire + Earth
        public ElementData infernoElement;  // Fire + Air
        public ElementData mudElement;      // Water + Earth
        public ElementData iceElement;      // Water + Air
        public ElementData sandElement;     // Earth + Air
        
        [Header("All Elements (Auto-populated)")]
        public List<ElementData> allElements = new List<ElementData>();
        
        private Dictionary<ElementType, ElementData> _elementLookup;
        private Dictionary<(ElementType, ElementType), ElementData> _combinationLookup;
        
        private void OnEnable()
        {
            BuildLookupTables();
        }
        
        public void BuildLookupTables()
        {
            // Build element type lookup
            _elementLookup = new Dictionary<ElementType, ElementData>();
            
            foreach (var element in allElements)
            {
                if (element != null && !_elementLookup.ContainsKey(element.type))
                {
                    _elementLookup[element.type] = element;
                }
            }
            
            // Add base elements if not in list
            AddToLookup(fireElement);
            AddToLookup(waterElement);
            AddToLookup(earthElement);
            AddToLookup(airElement);
            AddToLookup(steamElement);
            AddToLookup(magmaElement);
            AddToLookup(infernoElement);
            AddToLookup(mudElement);
            AddToLookup(iceElement);
            AddToLookup(sandElement);
            
            // Build combination lookup
            _combinationLookup = new Dictionary<(ElementType, ElementType), ElementData>();
            
            foreach (var element in _elementLookup.Values)
            {
                if (element.combinations == null) continue;
                
                foreach (var combo in element.combinations)
                {
                    var key1 = (element.type, combo.combinesWith);
                    var key2 = (combo.combinesWith, element.type);
                    
                    if (!_combinationLookup.ContainsKey(key1))
                        _combinationLookup[key1] = combo.result;
                    if (!_combinationLookup.ContainsKey(key2))
                        _combinationLookup[key2] = combo.result;
                }
            }
        }
        
        private void AddToLookup(ElementData element)
        {
            if (element != null && !_elementLookup.ContainsKey(element.type))
            {
                _elementLookup[element.type] = element;
            }
        }
        
        /// <summary>
        /// Get element data by type.
        /// </summary>
        public ElementData GetElement(ElementType type)
        {
            if (_elementLookup == null) BuildLookupTables();
            return _elementLookup.TryGetValue(type, out var element) ? element : null;
        }
        
        /// <summary>
        /// Try to combine two elements.
        /// </summary>
        public ElementData TryCombine(ElementType a, ElementType b)
        {
            if (_combinationLookup == null) BuildLookupTables();
            
            if (a == b) return null; // Can't combine same element
            
            return _combinationLookup.TryGetValue((a, b), out var result) ? result : null;
        }
        
        /// <summary>
        /// Check if two elements can be combined.
        /// </summary>
        public bool CanCombine(ElementType a, ElementType b)
        {
            return TryCombine(a, b) != null;
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Auto-populate All Elements")]
        private void AutoPopulateElements()
        {
            allElements.Clear();
            
            if (fireElement != null) allElements.Add(fireElement);
            if (waterElement != null) allElements.Add(waterElement);
            if (earthElement != null) allElements.Add(earthElement);
            if (airElement != null) allElements.Add(airElement);
            if (steamElement != null) allElements.Add(steamElement);
            if (magmaElement != null) allElements.Add(magmaElement);
            if (infernoElement != null) allElements.Add(infernoElement);
            if (mudElement != null) allElements.Add(mudElement);
            if (iceElement != null) allElements.Add(iceElement);
            if (sandElement != null) allElements.Add(sandElement);
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}
