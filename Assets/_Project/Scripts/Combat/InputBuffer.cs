using System.Collections.Generic;
using UnityEngine;

namespace EElemental.Combat
{
    /// <summary>
    /// Buffers player inputs for responsive combat.
    /// Allows inputs to be registered slightly before they can be executed.
    /// </summary>
    public class InputBuffer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _bufferFrames = 6; // ~100ms at 60fps
        
        private Queue<BufferedInput> _inputQueue = new Queue<BufferedInput>();
        
        /// <summary>
        /// Buffer an input for later consumption.
        /// </summary>
        public void BufferInput(AttackInput input)
        {
            _inputQueue.Enqueue(new BufferedInput
            {
                input = input,
                frameStamp = Time.frameCount
            });
        }
        
        /// <summary>
        /// Try to consume a buffered input.
        /// Returns null if no valid buffered input exists.
        /// </summary>
        public AttackInput? ConsumeBuffer()
        {
            // Remove stale inputs
            while (_inputQueue.Count > 0)
            {
                var buffered = _inputQueue.Peek();
                
                if (Time.frameCount - buffered.frameStamp > _bufferFrames)
                {
                    _inputQueue.Dequeue();
                    continue;
                }
                
                return _inputQueue.Dequeue().input;
            }
            
            return null;
        }
        
        /// <summary>
        /// Check if there's a valid buffered input without consuming it.
        /// </summary>
        public bool HasBufferedInput()
        {
            CleanStaleInputs();
            return _inputQueue.Count > 0;
        }
        
        /// <summary>
        /// Peek at the next buffered input without consuming it.
        /// </summary>
        public AttackInput? PeekBuffer()
        {
            CleanStaleInputs();
            if (_inputQueue.Count > 0)
            {
                return _inputQueue.Peek().input;
            }
            return null;
        }
        
        /// <summary>
        /// Clear all buffered inputs.
        /// </summary>
        public void Clear()
        {
            _inputQueue.Clear();
        }
        
        private void CleanStaleInputs()
        {
            while (_inputQueue.Count > 0 && 
                   Time.frameCount - _inputQueue.Peek().frameStamp > _bufferFrames)
            {
                _inputQueue.Dequeue();
            }
        }
        
        private struct BufferedInput
        {
            public AttackInput input;
            public int frameStamp;
        }
    }
}
