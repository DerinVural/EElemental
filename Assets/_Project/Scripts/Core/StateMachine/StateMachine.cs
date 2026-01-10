using UnityEngine;

namespace EElemental.Core
{
    /// <summary>
    /// Generic state machine implementation.
    /// Manages state transitions and lifecycle.
    /// </summary>
    public class StateMachine
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }
        
        private bool _isTransitioning;
        
        /// <summary>
        /// Initialize the state machine with a starting state.
        /// </summary>
        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            CurrentState?.Enter();
        }
        
        /// <summary>
        /// Transition to a new state.
        /// </summary>
        public void TransitionTo(IState newState)
        {
            if (newState == null || _isTransitioning) return;
            if (newState == CurrentState) return;
            
            _isTransitioning = true;
            
            PreviousState = CurrentState;
            CurrentState?.Exit();
            
            CurrentState = newState;
            CurrentState?.Enter();
            
            _isTransitioning = false;
        }
        
        /// <summary>
        /// Update the current state. Call this in Update().
        /// </summary>
        public void Execute()
        {
            CurrentState?.Execute();
        }
        
        /// <summary>
        /// Fixed update the current state. Call this in FixedUpdate().
        /// </summary>
        public void FixedExecute()
        {
            CurrentState?.FixedExecute();
        }
        
        /// <summary>
        /// Return to the previous state.
        /// </summary>
        public void ReturnToPreviousState()
        {
            if (PreviousState != null)
            {
                TransitionTo(PreviousState);
            }
        }
    }
}
