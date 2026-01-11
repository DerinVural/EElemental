using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Death screen UI for rogue-like game.
    /// Displays run statistics and allows player to retry or return to main menu.
    /// </summary>
    public class DeathScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _deathPanel;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _runStatsText;
        [SerializeField] private TextMeshProUGUI _seedText;

        [Header("Buttons")]
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _newRunButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("Animation")]
        [SerializeField] private float _fadeInDuration = 2f;
        [SerializeField] private float _delayBeforeShow = 1f;

        [Header("Scene Names")]
        [SerializeField] private string _gameplaySceneName = "Gameplay";
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        private bool _isShowing;
        private float _fadeTimer;
        private int _currentSeed;

        private void OnEnable()
        {
            // Subscribe to death event
            GameEvents.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            // Unsubscribe
            GameEvents.OnPlayerDeath -= OnPlayerDeath;
        }

        private void Start()
        {
            // Setup button listeners
            if (_retryButton != null)
                _retryButton.onClick.AddListener(OnRetryClicked);

            if (_newRunButton != null)
                _newRunButton.onClick.AddListener(OnNewRunClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            // Hide death screen initially
            HideDeathScreen();
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (_retryButton != null)
                _retryButton.onClick.RemoveListener(OnRetryClicked);

            if (_newRunButton != null)
                _newRunButton.onClick.RemoveListener(OnNewRunClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }

        private void Update()
        {
            // Handle fade in animation
            if (_isShowing && _canvasGroup != null)
            {
                _fadeTimer += Time.unscaledDeltaTime;

                if (_fadeTimer >= _delayBeforeShow)
                {
                    float fadeProgress = (_fadeTimer - _delayBeforeShow) / _fadeInDuration;
                    _canvasGroup.alpha = Mathf.Clamp01(fadeProgress);

                    if (fadeProgress >= 1f)
                    {
                        _isShowing = false;
                    }
                }
            }
        }

        #region Event Handlers

        /// <summary>
        /// Called when player dies.
        /// </summary>
        private void OnPlayerDeath()
        {
            // Show death screen after delay
            Invoke(nameof(ShowDeathScreen), _delayBeforeShow);

            // Trigger game state change
            GameEvents.TriggerGameStateChanged(GameState.GameOver);

            // End current run
            GameEvents.TriggerRunEnded(false);

            Debug.Log("[DeathScreen] Player died. Showing death screen.");
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Retry with same seed.
        /// </summary>
        private void OnRetryClicked()
        {
            Debug.Log($"[DeathScreen] Retrying with seed: {_currentSeed}");

            // Get current seed
            if (PlayerPrefs.HasKey("CurrentSeed"))
            {
                _currentSeed = PlayerPrefs.GetInt("CurrentSeed");
            }

            // Clear events
            GameEvents.ClearAllEvents();

            // Start new run with same seed
            GameEvents.TriggerRunStarted(_currentSeed);

            // Reload scene
            Time.timeScale = 1f;
            SceneManager.LoadScene(_gameplaySceneName);
        }

        /// <summary>
        /// Start new run with new seed.
        /// </summary>
        private void OnNewRunClicked()
        {
            Debug.Log("[DeathScreen] Starting new run with new seed.");

            // Generate new seed
            int newSeed = Random.Range(int.MinValue, int.MaxValue);
            PlayerPrefs.SetInt("CurrentSeed", newSeed);
            PlayerPrefs.Save();

            // Clear events
            GameEvents.ClearAllEvents();

            // Start new run
            GameEvents.TriggerRunStarted(newSeed);

            // Reload scene
            Time.timeScale = 1f;
            SceneManager.LoadScene(_gameplaySceneName);
        }

        /// <summary>
        /// Return to main menu.
        /// </summary>
        private void OnMainMenuClicked()
        {
            Debug.Log("[DeathScreen] Returning to main menu.");

            // Clear events
            GameEvents.ClearAllEvents();

            // Load main menu
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        #endregion

        #region Display Management

        private void ShowDeathScreen()
        {
            if (_deathPanel != null)
                _deathPanel.SetActive(true);

            // Get current seed
            if (PlayerPrefs.HasKey("CurrentSeed"))
            {
                _currentSeed = PlayerPrefs.GetInt("CurrentSeed");
            }

            // Update texts
            UpdateTexts();

            // Start fade in
            _isShowing = true;
            _fadeTimer = 0f;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void HideDeathScreen()
        {
            if (_deathPanel != null)
                _deathPanel.SetActive(false);

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

        private void UpdateTexts()
        {
            // Title
            if (_titleText != null)
            {
                _titleText.text = "YOU DIED";
            }

            // Run stats (TODO: Collect actual stats during run)
            if (_runStatsText != null)
            {
                string stats = BuildRunStatsText();
                _runStatsText.text = stats;
            }

            // Seed
            if (_seedText != null)
            {
                _seedText.text = $"SEED: {_currentSeed}";
            }
        }

        private string BuildRunStatsText()
        {
            // TODO: Implement proper stat tracking
            // For now, return placeholder text

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("RUN STATISTICS");
            sb.AppendLine();
            sb.AppendLine($"Rooms Cleared: {PlayerPrefs.GetInt("RoomsCleared", 0)}");
            sb.AppendLine($"Enemies Defeated: {PlayerPrefs.GetInt("EnemiesDefeated", 0)}");
            sb.AppendLine($"Time Survived: {FormatTime(PlayerPrefs.GetFloat("TimeSurvived", 0f))}");
            sb.AppendLine();
            sb.AppendLine("Better luck next time!");

            return sb.ToString();
        }

        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually show death screen (for testing).
        /// </summary>
        public void Show()
        {
            ShowDeathScreen();
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        [ContextMenu("Test Death Screen")]
        private void TestDeathScreen()
        {
            // Set fake stats
            PlayerPrefs.SetInt("RoomsCleared", 5);
            PlayerPrefs.SetInt("EnemiesDefeated", 23);
            PlayerPrefs.SetFloat("TimeSurvived", 245.5f);
            PlayerPrefs.SetInt("CurrentSeed", 12345);

            ShowDeathScreen();
        }
#endif

        #endregion
    }
}
