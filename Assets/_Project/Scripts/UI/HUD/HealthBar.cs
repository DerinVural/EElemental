using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Health bar UI component that displays player's current/max health.
    /// Updates via GameEvents for loose coupling.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Visual Settings")]
        [SerializeField] private Color _healthyColor = Color.green;
        [SerializeField] private Color _lowHealthColor = Color.red;
        [SerializeField] [Range(0f, 1f)] private float _lowHealthThreshold = 0.3f;
        [SerializeField] private bool _smoothTransition = true;
        [SerializeField] private float _transitionSpeed = 5f;

        [Header("Animation")]
        [SerializeField] private bool _pulseOnLowHealth = true;
        [SerializeField] private float _pulseSpeed = 2f;
        [SerializeField] private float _pulseAmount = 0.2f;

        private float _currentFillAmount;
        private float _targetFillAmount;
        private bool _isLowHealth;

        private void OnEnable()
        {
            // Subscribe to health change events
            GameEvents.OnPlayerHealthChanged += UpdateHealthBar;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            GameEvents.OnPlayerHealthChanged -= UpdateHealthBar;
        }

        private void Start()
        {
            // Initialize with full health
            _currentFillAmount = 1f;
            _targetFillAmount = 1f;
            UpdateVisuals();
        }

        private void Update()
        {
            // Smooth fill transition
            if (_smoothTransition && Mathf.Abs(_currentFillAmount - _targetFillAmount) > 0.001f)
            {
                _currentFillAmount = Mathf.Lerp(_currentFillAmount, _targetFillAmount, Time.deltaTime * _transitionSpeed);
                UpdateVisuals();
            }

            // Pulse effect on low health
            if (_pulseOnLowHealth && _isLowHealth)
            {
                float pulse = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmount;
                transform.localScale = Vector3.one * pulse;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Update health bar when player health changes.
        /// Called via GameEvents.OnPlayerHealthChanged.
        /// </summary>
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (maxHealth <= 0)
            {
                Debug.LogWarning("[HealthBar] Max health is zero or negative!");
                return;
            }

            _targetFillAmount = currentHealth / maxHealth;

            if (!_smoothTransition)
            {
                _currentFillAmount = _targetFillAmount;
                UpdateVisuals();
            }

            // Update text
            if (_healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            }

            // Check low health state
            _isLowHealth = _targetFillAmount <= _lowHealthThreshold;
        }

        /// <summary>
        /// Update visual elements (fill, color).
        /// </summary>
        private void UpdateVisuals()
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = _currentFillAmount;

                // Interpolate color based on health percentage
                _fillImage.color = Color.Lerp(_lowHealthColor, _healthyColor, _currentFillAmount / _lowHealthThreshold);
            }
        }

        #region Public Methods

        /// <summary>
        /// Show the health bar.
        /// </summary>
        public void Show()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hide the health bar.
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Manually set health (useful for testing).
        /// </summary>
        public void SetHealth(float current, float max)
        {
            UpdateHealthBar(current, max);
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure valid threshold
            _lowHealthThreshold = Mathf.Clamp01(_lowHealthThreshold);

            // Update visuals in editor
            if (_fillImage != null && !Application.isPlaying)
            {
                _fillImage.fillAmount = 1f;
                _fillImage.color = _healthyColor;
            }
        }
#endif

        #endregion
    }
}
