using UnityEngine;
using UnityEngine.SceneManagement;

namespace EElemental.Core
{
    /// <summary>
    /// Main game manager. Handles game state, pause, and scene transitions.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver,
            Victory
        }
        
        [Header("Game State")]
        [SerializeField] private GameState _currentState = GameState.MainMenu;
        
        public GameState CurrentState => _currentState;
        public bool IsPaused => _currentState == GameState.Paused;
        public bool IsPlaying => _currentState == GameState.Playing;
        
        // Events
        public static event System.Action<GameState> OnGameStateChanged;
        public static event System.Action OnGamePaused;
        public static event System.Action OnGameResumed;
        public static event System.Action OnRunStarted;
        public static event System.Action<bool> OnRunEnded; // true = victory
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        private void Start()
        {
            SetState(GameState.MainMenu);
        }
        
        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;
            
            GameState previousState = _currentState;
            _currentState = newState;
            
            // Handle time scale
            Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;
            
            // Fire events
            OnGameStateChanged?.Invoke(newState);
            
            switch (newState)
            {
                case GameState.Paused:
                    OnGamePaused?.Invoke();
                    break;
                case GameState.Playing:
                    if (previousState == GameState.Paused)
                        OnGameResumed?.Invoke();
                    break;
            }
        }
        
        public void StartNewRun()
        {
            SetState(GameState.Playing);
            OnRunStarted?.Invoke();
        }
        
        public void EndRun(bool victory)
        {
            SetState(victory ? GameState.Victory : GameState.GameOver);
            OnRunEnded?.Invoke(victory);
        }
        
        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }
        
        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }
        
        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
                PauseGame();
            else if (_currentState == GameState.Paused)
                ResumeGame();
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
