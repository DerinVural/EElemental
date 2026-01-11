using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EElemental.Core;
using EElemental.Elements;

namespace EElemental.UI
{
    /// <summary>
    /// Displays currently active element and element switching UI.
    /// Shows element icons, colors, and visual feedback.
    /// </summary>
    public class ElementUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _elementIcon;
        [SerializeField] private TextMeshProUGUI _elementNameText;
        [SerializeField] private Image _elementGlow;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Element Icons")]
        [SerializeField] private Sprite _fireIcon;
        [SerializeField] private Sprite _waterIcon;
        [SerializeField] private Sprite _earthIcon;
        [SerializeField] private Sprite _airIcon;
        [SerializeField] private Sprite _noneIcon;

        [Header("Element Colors")]
        [SerializeField] private Color _fireColor = new Color(1f, 0.3f, 0f);
        [SerializeField] private Color _waterColor = new Color(0.2f, 0.5f, 1f);
        [SerializeField] private Color _earthColor = new Color(0.6f, 0.4f, 0.2f);
        [SerializeField] private Color _airColor = new Color(0.8f, 1f, 0.9f);
        [SerializeField] private Color _noneColor = Color.white;

        [Header("Animation")]
        [SerializeField] private bool _animateOnChange = true;
        [SerializeField] private float _switchAnimDuration = 0.3f;
        [SerializeField] private float _pulseSpeed = 3f;
        [SerializeField] private float _pulseAmount = 0.1f;

        private ElementType _currentElement = ElementType.None;
        private float _animationTimer;
        private bool _isAnimating;

        private void OnEnable()
        {
            // Subscribe to element change events
            GameEvents.OnPlayerElementChanged += UpdateElement;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            GameEvents.OnPlayerElementChanged -= UpdateElement;
        }

        private void Start()
        {
            // Initialize with no element
            UpdateElementVisuals(ElementType.None);
        }

        private void Update()
        {
            // Handle switch animation
            if (_animateOnChange && _isAnimating)
            {
                _animationTimer += Time.deltaTime;

                if (_animationTimer >= _switchAnimDuration)
                {
                    _isAnimating = false;
                    transform.localScale = Vector3.one;
                }
                else
                {
                    // Pop in/out animation
                    float t = _animationTimer / _switchAnimDuration;
                    float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
                    transform.localScale = Vector3.one * scale;
                }
            }

            // Gentle pulse effect
            if (_elementGlow != null && _currentElement != ElementType.None)
            {
                float pulse = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmount;
                Color glowColor = _elementGlow.color;
                glowColor.a = pulse * 0.5f;
                _elementGlow.color = glowColor;
            }
        }

        /// <summary>
        /// Update active element display.
        /// Called via GameEvents.OnPlayerElementChanged.
        /// </summary>
        private void UpdateElement(ElementType newElement)
        {
            if (_currentElement == newElement) return;

            _currentElement = newElement;
            UpdateElementVisuals(newElement);

            // Start animation
            if (_animateOnChange)
            {
                _isAnimating = true;
                _animationTimer = 0f;
            }
        }

        /// <summary>
        /// Update visual elements based on active element type.
        /// </summary>
        private void UpdateElementVisuals(ElementType element)
        {
            // Update icon
            if (_elementIcon != null)
            {
                _elementIcon.sprite = GetElementIcon(element);
                _elementIcon.color = GetElementColor(element);
            }

            // Update glow
            if (_elementGlow != null)
            {
                _elementGlow.color = GetElementColor(element);
            }

            // Update text
            if (_elementNameText != null)
            {
                _elementNameText.text = GetElementName(element);
                _elementNameText.color = GetElementColor(element);
            }
        }

        #region Element Data Helpers

        private Sprite GetElementIcon(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => _fireIcon,
                ElementType.Water => _waterIcon,
                ElementType.Earth => _earthIcon,
                ElementType.Air => _airIcon,
                _ => _noneIcon
            };
        }

        private Color GetElementColor(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => _fireColor,
                ElementType.Water => _waterColor,
                ElementType.Earth => _earthColor,
                ElementType.Air => _airColor,
                _ => _noneColor
            };
        }

        private string GetElementName(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => "FIRE",
                ElementType.Water => "WATER",
                ElementType.Earth => "EARTH",
                ElementType.Air => "AIR",
                ElementType.Steam => "STEAM",
                ElementType.Lava => "LAVA",
                ElementType.Lightning => "LIGHTNING",
                ElementType.Ice => "ICE",
                ElementType.Mud => "MUD",
                ElementType.Dust => "DUST",
                _ => "NONE"
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the element UI.
        /// </summary>
        public void Show()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hide the element UI.
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Manually set element (useful for testing).
        /// </summary>
        public void SetElement(ElementType element)
        {
            UpdateElement(element);
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Update visuals in editor
            if (_elementIcon != null && !Application.isPlaying)
            {
                _elementIcon.sprite = _noneIcon;
                _elementIcon.color = _noneColor;
            }
        }
#endif

        #endregion
    }
}
