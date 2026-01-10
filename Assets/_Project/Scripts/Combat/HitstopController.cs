using UnityEngine;
using System.Collections;

namespace EElemental.Combat
{
    /// <summary>
    /// Handles hitstop (freeze frames) and screen shake for impact feel.
    /// </summary>
    public class HitstopController : MonoBehaviour
    {
        [Header("Hitstop Settings")]
        [SerializeField] private bool _affectTimeScale = true;
        [SerializeField] private float _hitstopTimeScale = 0.05f;
        
        [Header("Screen Shake")]
        [SerializeField] private float _defaultShakeIntensity = 0.1f;
        [SerializeField] private float _shakeDuration = 0.1f;
        [SerializeField] private Camera _targetCamera;
        
        private Coroutine _hitstopCoroutine;
        private Coroutine _shakeCoroutine;
        private Vector3 _originalCamPosition;
        
        // Static instance for easy access
        public static HitstopController Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            
            if (_targetCamera == null)
                _targetCamera = Camera.main;
            
            if (_targetCamera != null)
                _originalCamPosition = _targetCamera.transform.localPosition;
        }
        
        /// <summary>
        /// Trigger hitstop effect.
        /// </summary>
        public void DoHitstop(int frames, float shakeIntensity = -1f)
        {
            if (frames <= 0) return;
            
            float duration = frames / 60f; // Convert frames to seconds at 60fps
            
            if (_hitstopCoroutine != null)
                StopCoroutine(_hitstopCoroutine);
            
            _hitstopCoroutine = StartCoroutine(HitstopRoutine(duration));
            
            // Screen shake
            if (shakeIntensity < 0) shakeIntensity = _defaultShakeIntensity;
            if (shakeIntensity > 0)
            {
                DoScreenShake(shakeIntensity, _shakeDuration);
            }
        }
        
        private IEnumerator HitstopRoutine(float duration)
        {
            if (_affectTimeScale)
            {
                float originalTimeScale = Time.timeScale;
                Time.timeScale = _hitstopTimeScale;
                
                // Wait in real time
                yield return new WaitForSecondsRealtime(duration);
                
                Time.timeScale = originalTimeScale;
            }
            else
            {
                yield return new WaitForSecondsRealtime(duration);
            }
            
            _hitstopCoroutine = null;
        }
        
        /// <summary>
        /// Trigger screen shake.
        /// </summary>
        public void DoScreenShake(float intensity, float duration)
        {
            if (_targetCamera == null) return;
            
            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);
            
            _shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }
        
        private IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;
                
                _targetCamera.transform.localPosition = _originalCamPosition + new Vector3(x, y, 0);
                
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            _targetCamera.transform.localPosition = _originalCamPosition;
            _shakeCoroutine = null;
        }
        
        /// <summary>
        /// Stop all effects immediately.
        /// </summary>
        public void StopAllEffects()
        {
            if (_hitstopCoroutine != null)
            {
                StopCoroutine(_hitstopCoroutine);
                Time.timeScale = 1f;
            }
            
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                if (_targetCamera != null)
                    _targetCamera.transform.localPosition = _originalCamPosition;
            }
        }
        
        private void OnDestroy()
        {
            StopAllEffects();
        }
    }
}
