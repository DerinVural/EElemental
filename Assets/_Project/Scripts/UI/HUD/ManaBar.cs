using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Mana bar UI component that displays player's current/max mana.
    /// Updates via GameEvents for loose coupling.
    /// </summary>
    public class ManaBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _manaText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Visual Settings")]
        [SerializeField] private Color _fullManaColor = new Color(0.2f, 0.5f, 1f); // Blue
        [SerializeField] private Color _lowManaColor = new Color(0.5f, 0.2f, 0.8f); // Purple
        [SerializeField] [Range(0f, 1f)] private float _lowManaThreshold = 0.2f;
        [SerializeField] private bool _smoothTransition = true;
        [SerializeField] private float _transitionSpeed = 8f;

        [Header("Regen Visual Feedback")]
        [SerializeField] private bool _showRegenGlow = true;
        [SerializeField] private Image _regenGlowImage;
        [SerializeField] private float _glowFadeSpeed = 2f;

        private float _currentFillAmount;
        private float _targetFillAmount;
        private float _glowAlpha;
        private int _previousMana;

        private void OnEnable()
        {
            // Subscribe to mana change events
            GameEvents.OnPlayerManaChanged += UpdateManaBar;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            GameEvents.OnPlayerManaChanged -= UpdateManaBar;
        }

        private void Start()
        {
            // Initialize with full mana
            _currentFillAmount = 1f;
            _targetFillAmount = 1f;
            _previousMana = 0;
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

            // Fade out regen glow
            if (_showRegenGlow && _regenGlowImage != null)
            {
                _glowAlpha = Mathf.Lerp(_glowAlpha, 0f, Time.deltaTime * _glowFadeSpeed);
                Color glowColor = _regenGlowImage.color;
                glowColor.a = _glowAlpha;
                _regenGlowImage.color = glowColor;
            }
        }

        /// <summary>
        /// Update mana bar when player mana changes.
        /// Called via GameEvents.OnPlayerManaChanged.
        /// </summary>
        private void UpdateManaBar(int currentMana, int maxMana)
        {
            if (maxMana <= 0)
            {
                Debug.LogWarning("[ManaBar] Max mana is zero or negative!");
                return;
            }

            _targetFillAmount = (float)currentMana / maxMana;

            if (!_smoothTransition)
            {
                _currentFillAmount = _targetFillAmount;
                UpdateVisuals();
            }

            // Update text
            if (_manaText != null)
            {
                _manaText.text = $"{currentMana} / {maxMana}";
            }

            // Show glow on mana regen
            if (_showRegenGlow && currentMana > _previousMana)
            {
                _glowAlpha = 1f;
            }

            _previousMana = currentMana;
        }

        /// <summary>
        /// Update visual elements (fill, color).
        /// </summary>
        private void UpdateVisuals()
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = _currentFillAmount;

                // Interpolate color based on mana percentage
                float colorT = _currentFillAmount / _lowManaThreshold;
                _fillImage.color = Color.Lerp(_lowManaColor, _fullManaColor, Mathf.Clamp01(colorT));
            }
        }

        #region Public Methods

        /// <summary>
        /// Show the mana bar.
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
        /// Hide the mana bar.
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
        /// Manually set mana (useful for testing).
        /// </summary>
        public void SetMana(int current, int max)
        {
            UpdateManaBar(current, max);
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure valid threshold
            _lowManaThreshold = Mathf.Clamp01(_lowManaThreshold);

            // Update visuals in editor
            if (_fillImage != null && !Application.isPlaying)
            {
                _fillImage.fillAmount = 1f;
                _fillImage.color = _fullManaColor;
            }
        }
#endif

        #endregion
    }
}
