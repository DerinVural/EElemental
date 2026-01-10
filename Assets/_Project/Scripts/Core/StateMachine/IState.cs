namespace EElemental.Core
{
    /// <summary>
    /// Base interface for all states in the state machine.
    /// Provides lifecycle methods for state management.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called when entering this state.
        /// </summary>
        void Enter();
        
        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        void Execute();
        
        /// <summary>
        /// Called every fixed update while in this state.
        /// Use for physics-related updates.
        /// </summary>
        void FixedExecute();
        
        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void Exit();
    }
}
