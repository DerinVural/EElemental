using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using EElemental.Core;

namespace EElemental.UI
{
    /// <summary>
    /// Main menu UI controller.
    /// Handles new game, continue, settings, and quit functionality.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _creditsPanel;

        [Header("Buttons")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _quitButton;

        [Header("Settings Buttons")]
        [SerializeField] private Button _backButton;

        [Header("Seed Input")]
        [SerializeField] private TMP_InputField _seedInputField;
        [SerializeField] private Toggle _customSeedToggle;

        [Header("Game Settings")]
        [SerializeField] private string _gameplaySceneName = "Gameplay";

        private bool _useCustomSeed;
        private int _customSeed;

        private void Start()
        {
            // Setup button listeners
            if (_newGameButton != null)
                _newGameButton.onClick.AddListener(OnNewGameClicked);

            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
                // Disable if no save exists
                _continueButton.interactable = HasSaveData();
            }

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);

            if (_creditsButton != null)
                _creditsButton.onClick.AddListener(OnCreditsClicked);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);

            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            if (_customSeedToggle != null)
                _customSeedToggle.onValueChanged.AddListener(OnCustomSeedToggled);

            if (_seedInputField != null)
                _seedInputField.onEndEdit.AddListener(OnSeedInputChanged);

            // Show main panel
            ShowMainPanel();

            // Trigger game state
            GameEvents.TriggerGameStateChanged(GameState.MainMenu);
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (_newGameButton != null)
                _newGameButton.onClick.RemoveListener(OnNewGameClicked);

            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnContinueClicked);

            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);

            if (_creditsButton != null)
                _creditsButton.onClick.RemoveListener(OnCreditsClicked);

            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);

            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);

            if (_customSeedToggle != null)
                _customSeedToggle.onValueChanged.RemoveListener(OnCustomSeedToggled);

            if (_seedInputField != null)
                _seedInputField.onEndEdit.RemoveListener(OnSeedInputChanged);
        }

        #region Button Handlers

        private void OnNewGameClicked()
        {
            Debug.Log("[MainMenu] Starting new game...");

            // Determine seed
            int? seed = _useCustomSeed ? _customSeed : (int?)null;

            // Clear save data
            ClearSaveData();

            // Start new run
            StartNewGame(seed);
        }

        private void OnContinueClicked()
        {
            Debug.Log("[MainMenu] Continuing game...");

            // Load save data and continue
            LoadSaveData();

            // Load gameplay scene
            SceneManager.LoadScene(_gameplaySceneName);
        }

        private void OnSettingsClicked()
        {
            ShowSettingsPanel();
        }

        private void OnCreditsClicked()
        {
            ShowCreditsPanel();
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
            ShowMainPanel();
        }

        private void OnCustomSeedToggled(bool isOn)
        {
            _useCustomSeed = isOn;

            if (_seedInputField != null)
            {
                _seedInputField.interactable = isOn;
            }
        }

        private void OnSeedInputChanged(string input)
        {
            if (int.TryParse(input, out int seed))
            {
                _customSeed = seed;
            }
            else
            {
                Debug.LogWarning("[MainMenu] Invalid seed input. Using random seed.");
                _useCustomSeed = false;

                if (_customSeedToggle != null)
                    _customSeedToggle.isOn = false;
            }
        }

        #endregion

        #region Panel Management

        private void ShowMainPanel()
        {
            if (_mainPanel != null) _mainPanel.SetActive(true);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);
            if (_creditsPanel != null) _creditsPanel.SetActive(false);
        }

        private void ShowSettingsPanel()
        {
            if (_mainPanel != null) _mainPanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(true);
            if (_creditsPanel != null) _creditsPanel.SetActive(false);
        }

        private void ShowCreditsPanel()
        {
            if (_mainPanel != null) _mainPanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);
            if (_creditsPanel != null) _creditsPanel.SetActive(true);
        }

        #endregion

        #region Game Management

        private void StartNewGame(int? seed)
        {
            // Clear events from previous sessions
            GameEvents.ClearAllEvents();

            // Trigger run started event
            int actualSeed = seed ?? GenerateRandomSeed();
            GameEvents.TriggerRunStarted(actualSeed);

            // Store seed for GameManager
            PlayerPrefs.SetInt("CurrentSeed", actualSeed);
            PlayerPrefs.Save();

            // Load gameplay scene
            SceneManager.LoadScene(_gameplaySceneName);
        }

        private int GenerateRandomSeed()
        {
            return Random.Range(int.MinValue, int.MaxValue);
        }

        #endregion

        #region Save/Load

        private bool HasSaveData()
        {
            // Check if save data exists
            return PlayerPrefs.HasKey("SaveExists");
        }

        private void LoadSaveData()
        {
            // TODO: Implement proper save/load system
            // For now, just load with saved seed
            if (PlayerPrefs.HasKey("CurrentSeed"))
            {
                int seed = PlayerPrefs.GetInt("CurrentSeed");
                GameEvents.TriggerRunStarted(seed);
            }
        }

        private void ClearSaveData()
        {
            PlayerPrefs.DeleteKey("SaveExists");
            PlayerPrefs.DeleteKey("CurrentSeed");
            PlayerPrefs.Save();
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        [ContextMenu("Test New Game")]
        private void TestNewGame()
        {
            OnNewGameClicked();
        }

        [ContextMenu("Create Fake Save")]
        private void CreateFakeSave()
        {
            PlayerPrefs.SetInt("SaveExists", 1);
            PlayerPrefs.SetInt("CurrentSeed", 12345);
            PlayerPrefs.Save();

            if (_continueButton != null)
                _continueButton.interactable = true;

            Debug.Log("[MainMenu] Fake save created!");
        }
#endif

        #endregion
    }
}
