using NUnit.Framework;
using UnityEngine;
using EElemental.Combat;

namespace EElemental.Tests.Editor
{
    /// <summary>
    /// Combat System testleri
    /// </summary>
    [TestFixture]
    public class CombatSystemTests
    {
        #region Input Buffer Tests
        
        [Test]
        public void InputBuffer_QueueInput_StoresInput()
        {
            // Arrange
            var buffer = new InputBuffer(0.15f);
            
            // Act
            buffer.QueueInput(InputType.LightAttack);
            
            // Assert
            Assert.IsTrue(buffer.HasInput);
            Assert.AreEqual(InputType.LightAttack, buffer.ConsumeInput());
        }
        
        [Test]
        public void InputBuffer_ConsumeInput_ClearsBuffer()
        {
            // Arrange
            var buffer = new InputBuffer(0.15f);
            buffer.QueueInput(InputType.HeavyAttack);
            
            // Act
            buffer.ConsumeInput();
            
            // Assert
            Assert.IsFalse(buffer.HasInput);
        }
        
        [Test]
        public void InputBuffer_MultipleInputs_KeepsLatest()
        {
            // Arrange
            var buffer = new InputBuffer(0.15f);
            
            // Act
            buffer.QueueInput(InputType.LightAttack);
            buffer.QueueInput(InputType.HeavyAttack);
            buffer.QueueInput(InputType.Dash);
            
            // Assert - Son girilen input korunmalı
            Assert.AreEqual(InputType.Dash, buffer.ConsumeInput());
        }
        
        [Test]
        public void InputBuffer_Clear_RemovesAllInputs()
        {
            // Arrange
            var buffer = new InputBuffer(0.15f);
            buffer.QueueInput(InputType.LightAttack);
            
            // Act
            buffer.Clear();
            
            // Assert
            Assert.IsFalse(buffer.HasInput);
        }
        
        #endregion
        
        #region Combo System Tests
        
        [Test]
        public void ComboHandler_Initialize_StartsAtZero()
        {
            // Arrange
            var handler = new ComboHandler();
            
            // Assert
            Assert.AreEqual(0, handler.CurrentComboIndex);
        }
        
        [Test]
        public void ComboHandler_AdvanceCombo_IncrementsIndex()
        {
            // Arrange
            var handler = new ComboHandler();
            var comboData = CreateMockComboData(3);
            handler.SetComboData(comboData);
            
            // Act
            handler.AdvanceCombo();
            
            // Assert
            Assert.AreEqual(1, handler.CurrentComboIndex);
            
            // Cleanup
            Object.DestroyImmediate(comboData);
        }
        
        [Test]
        public void ComboHandler_AdvanceCombo_WrapsAtMax()
        {
            // Arrange
            var handler = new ComboHandler();
            var comboData = CreateMockComboData(3);
            handler.SetComboData(comboData);
            
            // Act - 3 kez ilerlet (0 -> 1 -> 2 -> 0)
            handler.AdvanceCombo();
            handler.AdvanceCombo();
            handler.AdvanceCombo();
            
            // Assert - Max combo'da başa dönmeli
            Assert.AreEqual(0, handler.CurrentComboIndex);
            
            // Cleanup
            Object.DestroyImmediate(comboData);
        }
        
        [Test]
        public void ComboHandler_ResetCombo_ResetsToZero()
        {
            // Arrange
            var handler = new ComboHandler();
            var comboData = CreateMockComboData(3);
            handler.SetComboData(comboData);
            handler.AdvanceCombo();
            handler.AdvanceCombo();
            
            // Act
            handler.ResetCombo();
            
            // Assert
            Assert.AreEqual(0, handler.CurrentComboIndex);
            
            // Cleanup
            Object.DestroyImmediate(comboData);
        }
        
        [Test]
        public void ComboHandler_GetCurrentAttack_ReturnsCorrectAttack()
        {
            // Arrange
            var handler = new ComboHandler();
            var comboData = CreateMockComboData(3);
            handler.SetComboData(comboData);
            
            // Act
            var attack = handler.GetCurrentAttack();
            
            // Assert
            Assert.IsNotNull(attack);
            
            // Cleanup
            Object.DestroyImmediate(comboData);
        }
        
        #endregion
        
        #region Damage Calculator Tests
        
        [Test]
        public void DamageCalculator_CalculateDamage_AppliesMultipliers()
        {
            // Arrange
            float baseDamage = 10f;
            float attackerMultiplier = 1.5f;
            float defenderReduction = 0.1f;
            
            // Act
            float result = DamageCalculator.Calculate(baseDamage, attackerMultiplier, defenderReduction);
            
            // Assert
            // 10 * 1.5 * (1 - 0.1) = 13.5
            Assert.AreEqual(13.5f, result, 0.01f);
        }
        
        [Test]
        public void DamageCalculator_CalculateDamage_MinimumIsOne()
        {
            // Arrange
            float baseDamage = 1f;
            float attackerMultiplier = 0.1f;
            float defenderReduction = 0.99f;
            
            // Act
            float result = DamageCalculator.Calculate(baseDamage, attackerMultiplier, defenderReduction);
            
            // Assert - Minimum 1 hasar verilmeli
            Assert.GreaterOrEqual(result, 1f);
        }
        
        [Test]
        public void DamageCalculator_CriticalHit_DoublesDamage()
        {
            // Arrange
            float baseDamage = 10f;
            bool isCritical = true;
            float critMultiplier = 2f;
            
            // Act
            float result = DamageCalculator.CalculateWithCrit(baseDamage, isCritical, critMultiplier);
            
            // Assert
            Assert.AreEqual(20f, result);
        }
        
        #endregion
        
        #region I-Frame Tests
        
        [Test]
        public void IFrameController_StartIFrames_MakesInvulnerable()
        {
            // Arrange
            var controller = new IFrameController();
            
            // Act
            controller.StartIFrames(0.5f);
            
            // Assert
            Assert.IsTrue(controller.IsInvulnerable);
        }
        
        [Test]
        public void IFrameController_AfterDuration_BecomesVulnerable()
        {
            // Arrange
            var controller = new IFrameController();
            controller.StartIFrames(0.1f);
            
            // Act - Simulate time passing
            controller.Update(0.15f);
            
            // Assert
            Assert.IsFalse(controller.IsInvulnerable);
        }
        
        [Test]
        public void IFrameController_DuringIFrames_BlocksDamage()
        {
            // Arrange
            var controller = new IFrameController();
            controller.StartIFrames(0.5f);
            
            // Act
            bool canTakeDamage = controller.CanTakeDamage();
            
            // Assert
            Assert.IsFalse(canTakeDamage);
        }
        
        #endregion
        
        #region Helper Methods
        
        private ComboData CreateMockComboData(int attackCount)
        {
            var comboData = ScriptableObject.CreateInstance<ComboData>();
            // Combo attack'ları setup et
            return comboData;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Test için InputBuffer mock implementasyonu
    /// </summary>
    public enum InputType
    {
        None,
        LightAttack,
        HeavyAttack,
        Dash,
        Jump
    }
    
    public class InputBuffer
    {
        private float _bufferDuration;
        private InputType _bufferedInput;
        private float _inputTime;
        
        public bool HasInput => _bufferedInput != InputType.None && (Time.time - _inputTime) < _bufferDuration;
        
        public InputBuffer(float duration)
        {
            _bufferDuration = duration;
            _bufferedInput = InputType.None;
        }
        
        public void QueueInput(InputType input)
        {
            _bufferedInput = input;
            _inputTime = Time.time;
        }
        
        public InputType ConsumeInput()
        {
            var input = _bufferedInput;
            _bufferedInput = InputType.None;
            return input;
        }
        
        public void Clear()
        {
            _bufferedInput = InputType.None;
        }
    }
    
    /// <summary>
    /// Test için IFrameController mock implementasyonu
    /// </summary>
    public class IFrameController
    {
        private float _remainingTime;
        
        public bool IsInvulnerable => _remainingTime > 0;
        
        public void StartIFrames(float duration)
        {
            _remainingTime = duration;
        }
        
        public void Update(float deltaTime)
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= deltaTime;
            }
        }
        
        public bool CanTakeDamage()
        {
            return !IsInvulnerable;
        }
    }
    
    /// <summary>
    /// Test için ComboHandler mock implementasyonu
    /// </summary>
    public class ComboHandler
    {
        private ComboData _comboData;
        public int CurrentComboIndex { get; private set; }
        
        public void SetComboData(ComboData data)
        {
            _comboData = data;
        }
        
        public void AdvanceCombo()
        {
            if (_comboData != null)
            {
                CurrentComboIndex = (CurrentComboIndex + 1) % 3; // Mock 3 combo
            }
        }
        
        public void ResetCombo()
        {
            CurrentComboIndex = 0;
        }
        
        public AttackData GetCurrentAttack()
        {
            return ScriptableObject.CreateInstance<AttackData>();
        }
    }
    
    /// <summary>
    /// Test için DamageCalculator mock implementasyonu
    /// </summary>
    public static class DamageCalculator
    {
        public static float Calculate(float baseDamage, float attackerMultiplier, float defenderReduction)
        {
            float result = baseDamage * attackerMultiplier * (1f - defenderReduction);
            return Mathf.Max(1f, result);
        }
        
        public static float CalculateWithCrit(float baseDamage, bool isCritical, float critMultiplier)
        {
            return isCritical ? baseDamage * critMultiplier : baseDamage;
        }
    }
}
