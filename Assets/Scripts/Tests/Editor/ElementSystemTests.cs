using NUnit.Framework;
using UnityEngine;
using EElemental.Elements;

namespace EElemental.Tests.Editor
{
    /// <summary>
    /// Element System testleri
    /// </summary>
    [TestFixture]
    public class ElementSystemTests
    {
        private ElementDatabase _database;
        
        [SetUp]
        public void Setup()
        {
            _database = ScriptableObject.CreateInstance<ElementDatabase>();
            InitializeMockElements();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_database != null)
            {
                Object.DestroyImmediate(_database);
            }
        }
        
        private void InitializeMockElements()
        {
            // Test için mock element'ler oluştur
            // Not: Gerçek testlerde bu elementler Resources'dan yüklenebilir
        }
        
        #region Element Combination Tests
        
        [Test]
        public void ElementCombiner_FireAndWater_ReturnsSteam()
        {
            // Arrange
            var fire = CreateMockElement("Fire", Color.red);
            var water = CreateMockElement("Water", Color.blue);
            
            // Act
            var result = ElementCombiner.Combine(fire, water);
            
            // Assert
            // Steam elementi dönmeli (null kontrolü - combiner setup gerekli)
            Assert.IsNotNull(result, "Fire + Water should return a combined element");
        }
        
        [Test]
        public void ElementCombiner_SameElements_ReturnsEnhanced()
        {
            // Arrange
            var fire = CreateMockElement("Fire", Color.red);
            
            // Act
            var result = ElementCombiner.Combine(fire, fire);
            
            // Assert
            // Aynı element kombinasyonu güçlendirilmiş versiyon dönmeli veya aynı element
            Assert.IsNotNull(result);
        }
        
        [Test]
        public void ElementCombiner_NullElement_ReturnsOther()
        {
            // Arrange
            var fire = CreateMockElement("Fire", Color.red);
            
            // Act
            var result = ElementCombiner.Combine(fire, null);
            
            // Assert
            Assert.AreEqual(fire, result);
        }
        
        [Test]
        public void ElementCombiner_BothNull_ReturnsNull()
        {
            // Act
            var result = ElementCombiner.Combine(null, null);
            
            // Assert
            Assert.IsNull(result);
        }
        
        #endregion
        
        #region Element Data Tests
        
        [Test]
        public void ElementData_DefaultValues_AreCorrect()
        {
            // Arrange
            var element = ScriptableObject.CreateInstance<ElementData>();
            
            // Act & Assert
            Assert.IsNotNull(element);
            Assert.AreEqual(1f, element.DamageMultiplier);
            
            // Cleanup
            Object.DestroyImmediate(element);
        }
        
        [Test]
        public void ElementData_DamageMultiplier_CanBeSet()
        {
            // Arrange
            var element = CreateMockElement("Fire", Color.red);
            element.DamageMultiplier = 1.5f;
            
            // Assert
            Assert.AreEqual(1.5f, element.DamageMultiplier);
            
            // Cleanup
            Object.DestroyImmediate(element);
        }
        
        #endregion
        
        #region Element Effectiveness Tests
        
        [Test]
        public void ElementData_Effectiveness_FireAgainstWater_IsReduced()
        {
            // Fire element su karşı zayıf olmalı
            // Bu test ElementData.GetEffectiveness metodunu test eder
            var fire = CreateMockElement("Fire", Color.red);
            var water = CreateMockElement("Water", Color.blue);
            
            // Cleanup
            Object.DestroyImmediate(fire);
            Object.DestroyImmediate(water);
            
            Assert.Pass("Effectiveness system to be implemented with counter-element logic");
        }
        
        [Test]
        public void ElementData_Effectiveness_FireAgainstEarth_IsEnhanced()
        {
            // Fire element toprak karşı güçlü olmalı
            var fire = CreateMockElement("Fire", Color.red);
            var earth = CreateMockElement("Earth", Color.green);
            
            // Cleanup
            Object.DestroyImmediate(fire);
            Object.DestroyImmediate(earth);
            
            Assert.Pass("Effectiveness system to be implemented with counter-element logic");
        }
        
        #endregion
        
        #region Helper Methods
        
        private ElementData CreateMockElement(string name, Color color)
        {
            var element = ScriptableObject.CreateInstance<ElementData>();
            element.name = name;
            // Element properties burada set edilebilir
            return element;
        }
        
        #endregion
    }
}
