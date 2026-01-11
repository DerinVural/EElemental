using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Pause menu controller.
    /// Handles pause/resume, settings, and return to main menu.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _quitButton;

        [Header("Settings Buttons")]
        [SerializeField] private Button _backButton;

        [Header("Input")]
        [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

        [Header("Scene Names")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        private bool _isPaused;

        private void Start()
        {
            // Setup button listeners
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResumeClicked);

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);

            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            // Hide pause menu initially
            HidePauseMenu();
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(OnResumeClicked);

            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);

            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);

            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void Update()
        {
            // Toggle pause with key
            if (Input.GetKeyDown(_pauseKey))
            {
                TogglePause();
            }
        }

        #region Pause/Resume

        /// <summary>
        /// Toggle pause state.
        /// </summary>
        public void TogglePause()
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;

            ShowPauseMenu();

            // Trigger event
            GameEvents.TriggerGamePaused();
            GameEvents.TriggerGameStateChanged(GameState.Paused);

            Debug.Log("[PauseMenu] Game paused.");
        }

        /// <summary>
        /// Resume the game.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;

            HidePauseMenu();

            // Trigger event
            GameEvents.TriggerGameResumed();
            GameEvents.TriggerGameStateChanged(GameState.Playing);

            Debug.Log("[PauseMenu] Game resumed.");
        }

        #endregion

        #region Button Handlers

        private void OnResumeClicked()
        {
            Resume();
        }

        private void OnSettingsClicked()
        {
            ShowSettingsPanel();
        }

        private void OnMainMenuClicked()
        {
            // Resume time before changing scenes
            Time.timeScale = 1f;

            // Clear events
            GameEvents.ClearAllEvents();

            // End current run
            GameEvents.TriggerRunEnded(false);

            // Load main menu
            SceneManager.LoadScene(_mainMenuSceneName);

            Debug.Log("[PauseMenu] Returning to main menu.");
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnBackClicked()
        {
            ShowPausePanel();
        }

        #endregion

        #region Panel Management

        private void ShowPauseMenu()
        {
            if (_pausePanel != null)
                _pausePanel.SetActive(true);

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

        private void HidePauseMenu()
        {
            if (_pausePanel != null)
                _pausePanel.SetActive(false);

            if (_settingsPanel != null)
                _settingsPanel.SetActive(false);

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

        private void ShowPausePanel()
        {
            if (_pausePanel != null) _pausePanel.SetActive(true);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);
        }

        private void ShowSettingsPanel()
        {
            if (_pausePanel != null) _pausePanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(true);
        }

        #endregion

        #region Public Properties

        public bool IsPaused => _isPaused;

        #endregion
    }
}
