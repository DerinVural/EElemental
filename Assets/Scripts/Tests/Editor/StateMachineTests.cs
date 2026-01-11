using NUnit.Framework;
using EElemental.Core;

namespace EElemental.Tests.Editor
{
    /// <summary>
    /// State Machine sistem testleri
    /// </summary>
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine _stateMachine;
        private MockState _idleState;
        private MockState _runState;
        private MockState _attackState;
        
        [SetUp]
        public void Setup()
        {
            _stateMachine = new StateMachine();
            _idleState = new MockState("Idle");
            _runState = new MockState("Run");
            _attackState = new MockState("Attack");
        }
        
        [TearDown]
        public void TearDown()
        {
            _stateMachine = null;
            _idleState = null;
            _runState = null;
            _attackState = null;
        }
        
        #region Initialization Tests
        
        [Test]
        public void StateMachine_InitialState_IsNull()
        {
            Assert.IsNull(_stateMachine.CurrentState);
        }
        
        [Test]
        public void StateMachine_Initialize_SetsCurrentState()
        {
            _stateMachine.Initialize(_idleState);
            
            Assert.AreEqual(_idleState, _stateMachine.CurrentState);
        }
        
        [Test]
        public void StateMachine_Initialize_CallsEnterOnFirstState()
        {
            _stateMachine.Initialize(_idleState);
            
            Assert.IsTrue(_idleState.WasEnterCalled);
        }
        
        [Test]
        public void StateMachine_Initialize_DoesNotCallExitOnFirstState()
        {
            _stateMachine.Initialize(_idleState);
            
            Assert.IsFalse(_idleState.WasExitCalled);
        }
        
        #endregion
        
        #region Transition Tests
        
        [Test]
        public void StateMachine_ChangeState_UpdatesCurrentState()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.ChangeState(_runState);
            
            Assert.AreEqual(_runState, _stateMachine.CurrentState);
        }
        
        [Test]
        public void StateMachine_ChangeState_CallsExitOnPreviousState()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.ChangeState(_runState);
            
            Assert.IsTrue(_idleState.WasExitCalled);
        }
        
        [Test]
        public void StateMachine_ChangeState_CallsEnterOnNewState()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.ChangeState(_runState);
            
            Assert.IsTrue(_runState.WasEnterCalled);
        }
        
        [Test]
        public void StateMachine_ChangeState_ToSameState_DoesNotReEnter()
        {
            _stateMachine.Initialize(_idleState);
            _idleState.ResetFlags();
            
            _stateMachine.ChangeState(_idleState);
            
            // Aynı state'e geçişte Enter/Exit çağrılmamalı
            Assert.IsFalse(_idleState.WasEnterCalled);
            Assert.IsFalse(_idleState.WasExitCalled);
        }
        
        [Test]
        public void StateMachine_ChangeState_MultipleTransitions_Correct()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.ChangeState(_runState);
            _stateMachine.ChangeState(_attackState);
            
            Assert.AreEqual(_attackState, _stateMachine.CurrentState);
            Assert.IsTrue(_idleState.WasExitCalled);
            Assert.IsTrue(_runState.WasExitCalled);
            Assert.IsTrue(_attackState.WasEnterCalled);
        }
        
        #endregion
        
        #region Update Tests
        
        [Test]
        public void StateMachine_Update_CallsUpdateOnCurrentState()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.Update();
            
            Assert.IsTrue(_idleState.WasUpdateCalled);
        }
        
        [Test]
        public void StateMachine_Update_WithNoState_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _stateMachine.Update());
        }
        
        [Test]
        public void StateMachine_FixedUpdate_CallsFixedUpdateOnCurrentState()
        {
            _stateMachine.Initialize(_idleState);
            _stateMachine.FixedUpdate();
            
            Assert.IsTrue(_idleState.WasFixedUpdateCalled);
        }
        
        #endregion
        
        #region Edge Cases
        
        [Test]
        public void StateMachine_ChangeState_WithNullState_DoesNotThrow()
        {
            _stateMachine.Initialize(_idleState);
            
            Assert.DoesNotThrow(() => _stateMachine.ChangeState(null));
        }
        
        [Test]
        public void StateMachine_Initialize_WithNullState_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _stateMachine.Initialize(null));
        }
        
        #endregion
    }
    
    /// <summary>
    /// Test için Mock State implementasyonu
    /// </summary>
    public class MockState : IState
    {
        public string Name { get; private set; }
        public bool WasEnterCalled { get; private set; }
        public bool WasExitCalled { get; private set; }
        public bool WasUpdateCalled { get; private set; }
        public bool WasFixedUpdateCalled { get; private set; }
        
        public MockState(string name)
        {
            Name = name;
        }
        
        public void Enter()
        {
            WasEnterCalled = true;
        }
        
        public void Exit()
        {
            WasExitCalled = true;
        }
        
        public void Update()
        {
            WasUpdateCalled = true;
        }
        
        public void FixedUpdate()
        {
            WasFixedUpdateCalled = true;
        }
        
        public void ResetFlags()
        {
            WasEnterCalled = false;
            WasExitCalled = false;
            WasUpdateCalled = false;
            WasFixedUpdateCalled = false;
        }
    }
}
