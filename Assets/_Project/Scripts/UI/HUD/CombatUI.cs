using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Combat UI displays combo counter, damage numbers, and critical hit feedback.
    /// Provides visual feedback for combat actions.
    /// </summary>
    public class CombatUI : MonoBehaviour
    {
        [Header("Combo Counter")]
        [SerializeField] private GameObject _comboPanel;
        [SerializeField] private TextMeshProUGUI _comboCountText;
        [SerializeField] private TextMeshProUGUI _comboLabelText;
        [SerializeField] private CanvasGroup _comboCanvasGroup;
        [SerializeField] private float _comboFadeDuration = 2f;

        [Header("Damage Numbers")]
        [SerializeField] private GameObject _damageNumberPrefab;
        [SerializeField] private Transform _damageNumberContainer;
        [SerializeField] private Color _normalDamageColor = Color.white;
        [SerializeField] private Color _criticalDamageColor = Color.yellow;

        [Header("Critical Hit Flash")]
        [SerializeField] private Image _critFlashImage;
        [SerializeField] private float _critFlashDuration = 0.2f;

        [Header("Animation")]
        [SerializeField] private float _comboScalePunch = 1.3f;
        [SerializeField] private float _comboScaleSpeed = 8f;

        private int _currentCombo;
        private float _comboFadeTimer;
        private float _targetComboScale = 1f;
        private float _currentComboScale = 1f;
        private float _critFlashTimer;

        private Queue<GameObject> _damageNumberPool = new Queue<GameObject>();
        private const int PoolSize = 20;

        private void OnEnable()
        {
            // Subscribe to combat events
            GameEvents.OnComboCompleted += OnComboUpdate;
            GameEvents.OnComboReset += OnComboReset;
            GameEvents.OnAttackHit += OnAttackHit;
            GameEvents.OnCriticalHit += OnCriticalHit;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            GameEvents.OnComboCompleted -= OnComboUpdate;
            GameEvents.OnComboReset -= OnComboReset;
            GameEvents.OnAttackHit -= OnAttackHit;
            GameEvents.OnCriticalHit -= OnCriticalHit;
        }

        private void Start()
        {
            // Initialize damage number pool
            if (_damageNumberPrefab != null && _damageNumberContainer != null)
            {
                for (int i = 0; i < PoolSize; i++)
                {
                    GameObject obj = Instantiate(_damageNumberPrefab, _damageNumberContainer);
                    obj.SetActive(false);
                    _damageNumberPool.Enqueue(obj);
                }
            }

            // Hide combo panel initially
            if (_comboPanel != null)
            {
                _comboPanel.SetActive(false);
            }

            // Hide crit flash
            if (_critFlashImage != null)
            {
                _critFlashImage.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Handle combo fade out
            if (_comboFadeTimer > 0)
            {
                _comboFadeTimer -= Time.deltaTime;

                if (_comboCanvasGroup != null)
                {
                    _comboCanvasGroup.alpha = Mathf.Clamp01(_comboFadeTimer / _comboFadeDuration);
                }

                if (_comboFadeTimer <= 0)
                {
                    _comboPanel?.SetActive(false);
                }
            }

            // Handle combo scale animation
            if (Mathf.Abs(_currentComboScale - _targetComboScale) > 0.01f)
            {
                _currentComboScale = Mathf.Lerp(_currentComboScale, _targetComboScale, Time.deltaTime * _comboScaleSpeed);

                if (_comboCountText != null)
                {
                    _comboCountText.transform.localScale = Vector3.one * _currentComboScale;
                }
            }

            // Handle crit flash
            if (_critFlashTimer > 0)
            {
                _critFlashTimer -= Time.deltaTime;

                if (_critFlashImage != null)
                {
                    float alpha = _critFlashTimer / _critFlashDuration;
                    Color flashColor = _critFlashImage.color;
                    flashColor.a = alpha;
                    _critFlashImage.color = flashColor;

                    if (_critFlashTimer <= 0)
                    {
                        _critFlashImage.gameObject.SetActive(false);
                    }
                }
            }
        }

        #region Event Handlers

        /// <summary>
        /// Called when combo count updates.
        /// </summary>
        private void OnComboUpdate(int comboCount)
        {
            _currentCombo = comboCount;

            // Show combo panel
            if (_comboPanel != null)
            {
                _comboPanel.SetActive(true);
            }

            // Update text
            if (_comboCountText != null)
            {
                _comboCountText.text = comboCount.ToString();
            }

            if (_comboLabelText != null)
            {
                _comboLabelText.text = "COMBO!";
            }

            // Reset fade timer
            _comboFadeTimer = _comboFadeDuration;

            if (_comboCanvasGroup != null)
            {
                _comboCanvasGroup.alpha = 1f;
            }

            // Punch scale animation
            _targetComboScale = _comboScalePunch;
            StartCoroutine(ResetComboScale());
        }

        /// <summary>
        /// Called when combo is reset/broken.
        /// </summary>
        private void OnComboReset()
        {
            _currentCombo = 0;
            _comboFadeTimer = 0.5f; // Quick fade out
        }

        /// <summary>
        /// Called when an attack lands.
        /// </summary>
        private void OnAttackHit(DamageInfo damageInfo, GameObject target)
        {
            // Spawn damage number
            if (target != null)
            {
                Vector3 worldPos = target.transform.position + Vector3.up;
                SpawnDamageNumber(damageInfo.Amount, worldPos, damageInfo.IsCritical);
            }
        }

        /// <summary>
        /// Called when a critical hit occurs.
        /// </summary>
        private void OnCriticalHit(float damage, GameObject target)
        {
            // Show crit flash
            if (_critFlashImage != null)
            {
                _critFlashImage.gameObject.SetActive(true);
                _critFlashTimer = _critFlashDuration;
            }
        }

        #endregion

        #region Damage Numbers

        /// <summary>
        /// Spawn a floating damage number at world position.
        /// </summary>
        private void SpawnDamageNumber(float damage, Vector3 worldPos, bool isCritical)
        {
            if (_damageNumberPrefab == null || _damageNumberPool.Count == 0)
                return;

            // Get from pool
            GameObject damageObj = _damageNumberPool.Dequeue();
            damageObj.SetActive(true);

            // Position (convert world to screen space)
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            damageObj.transform.position = screenPos;

            // Set text and color
            TextMeshProUGUI damageText = damageObj.GetComponent<TextMeshProUGUI>();
            if (damageText != null)
            {
                damageText.text = Mathf.CeilToInt(damage).ToString();
                damageText.color = isCritical ? _criticalDamageColor : _normalDamageColor;
                damageText.fontSize = isCritical ? 48 : 36;
            }

            // Animate (handled by DamageNumber component if exists)
            DamageNumber damageNumber = damageObj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Play(() =>
                {
                    // Return to pool
                    damageObj.SetActive(false);
                    _damageNumberPool.Enqueue(damageObj);
                });
            }
        }

        #endregion

        #region Coroutines

        private System.Collections.IEnumerator ResetComboScale()
        {
            yield return new WaitForSeconds(0.1f);
            _targetComboScale = 1f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually reset combo (for testing).
        /// </summary>
        public void ResetCombo()
        {
            OnComboReset();
        }

        #endregion
    }

    /// <summary>
    /// Helper component for damage number animation.
    /// Attach to damage number prefab.
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 1f;
        [SerializeField] private float _riseSpeed = 50f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private TextMeshProUGUI _text;
        private float _timer;
        private System.Action _onComplete;
        private Vector3 _startPos;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void Play(System.Action onComplete)
        {
            _timer = 0f;
            _onComplete = onComplete;
            _startPos = transform.position;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _lifetime)
            {
                _onComplete?.Invoke();
                return;
            }

            // Rise up
            Vector3 pos = _startPos + Vector3.up * (_riseSpeed * _timer);
            transform.position = pos;

            // Fade out
            if (_text != null)
            {
                float alpha = _fadeCurve.Evaluate(_timer / _lifetime);
                Color color = _text.color;
                color.a = alpha;
                _text.color = color;
            }
        }
    }
}
