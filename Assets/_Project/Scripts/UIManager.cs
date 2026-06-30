using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Level Up Menu Panel")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Button speedButton;
    [SerializeField] private Button healButton;
    [SerializeField] private Button upgradeDamageButton;

    [Header("Pause Menu Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Game Over Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private string gameSceneName = "Game";

    [Header("Settings")]
    [SerializeField] private bool isMainMenu = false; // Отмечаем, если это главное меню

    private bool isGameOver = false;
    private bool isPaused = false;

    private void Awake()
    {
        // Если это главное меню - показываем его, скрываем всё остальное
        if (isMainMenu)
        {
            ShowMainMenu();
        }
        else
        {
            // Скрываем панель паузы при старте игры
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);

            // Скрываем главное меню если оно есть
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Подписываемся на события здоровья и опыта игрока
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthBar;

        if (playerController != null)
            playerController.OnExperienceChanged += UpdateLevelText;

        // Подписываемся на клики по кнопкам улучшений
        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.AddListener(OnUpgradeDamageClicked);

        if (speedButton != null)
            speedButton.onClick.AddListener(SelectSpeedUpgrade);

        if (healButton != null)
            healButton.onClick.AddListener(SelectHealUpgrade);

        // Подписываем кнопки меню паузы
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        // Подписываем кнопки главного меню
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(ContinueGame);

        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        // Отписываемся при уничтожении/выключении объекта
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;

        if (playerController != null)
            playerController.OnExperienceChanged -= UpdateLevelText;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.RemoveListener(OnUpgradeDamageClicked);

        if (speedButton != null)
            speedButton.onClick.RemoveListener(SelectSpeedUpgrade);

        if (healButton != null)
            healButton.onClick.RemoveListener(SelectHealUpgrade);

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);

        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        // Отписываем кнопки главного меню
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(StartGame);

        if (continueGameButton != null)
            continueGameButton.onClick.RemoveListener(ContinueGame);

        if (quitGameButton != null)
            quitGameButton.onClick.RemoveListener(QuitGame);
    }

    private void Update()
    {
        // Если это главное меню - не обрабатываем паузу
        if (isMainMenu) return;

        if (isGameOver) return;

        // Проверяем, не открыто ли меню улучшений
        if (levelUpPanel != null && levelUpPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    #region Main Menu Methods

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        // Скрываем все остальные панели
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("Main Menu Shown");
    }

    private void StartGame()
    {
        Debug.Log($"Starting game, loading scene: {gameSceneName}");

        // Скрываем главное меню
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        // Загружаем игровую сцену
        SceneManager.LoadScene("GameScene");

        // Сбрасываем флаг главного меню
        isMainMenu = false;

        Time.timeScale = 1f;
    }

    private void ContinueGame()
    {
        // Проверяем, есть ли сохраненная игра
        if (HasSaveGame())
        {
            Debug.Log("Loading saved game...");
            LoadGame();
        }
        else
        {
            Debug.Log("No saved game found, starting new game...");
            StartGame();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        // Если в редакторе - останавливаем игру
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // В собранной игре - закрываем приложение
            Application.Quit();
#endif
    }

    // Проверка наличия сохранения
    private bool HasSaveGame()
    {
        // Здесь можно добавить проверку сохранений
        // Например: return PlayerPrefs.HasKey("SaveGame");
        return false; // Пока возвращаем false
    }

    private void LoadGame()
    {
        // Здесь логика загрузки сохранения
        Debug.Log("Loading saved game...");
        // Загружаем сцену с сохраненным прогрессом
        SceneManager.LoadScene(gameSceneName);
    }

    #endregion

    #region Health & Level Updates

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    private void UpdateLevelText(int currentLvl, int currentExp, int expToNextLvl)
    {
        if (levelText != null)
        {
            levelText.text = $"LVL: {currentLvl} ({currentExp}/{expToNextLvl})";
        }

        if (currentLvl > 1 && currentExp == 0)
        {
            OpenLevelUpMenu();
        }
    }

    #endregion

    #region Level Up Menu

    private void OpenLevelUpMenu()
    {
        if (levelUpPanel != null && !isPaused && !isGameOver)
        {
            levelUpPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void SelectSpeedUpgrade()
    {
        if (playerController != null)
        {
            playerController.UpgradeSpeed(1.2f);
        }
        CloseLevelUpMenu();
    }

    private void SelectHealUpgrade()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(30f);
        }
        CloseLevelUpMenu();
    }

    private void OnUpgradeDamageClicked()
    {
        AutoWeapon weapon = FindFirstObjectByType<AutoWeapon>();
        if (weapon != null)
        {
            weapon.UpgradeDamage(10f);
        }
        else
        {
            Debug.LogWarning("AutoWeapon not found on scene!");
        }
        CloseLevelUpMenu();
    }

    private void CloseLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    #endregion

    #region Pause Menu

    public void TogglePause()
    {
        // Не включаем паузу, если игра закончена или открыто меню улучшений
        if (isGameOver) return;
        if (levelUpPanel != null && levelUpPanel.activeSelf) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // Отдельный метод для кнопки Resume
    private void OnResumeButtonClicked()
    {
        // Проверяем, что игра действительно на паузе
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            // Если по какой-то причине кнопка нажата, а паузы нет - просто скрываем панель
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
        }
    }

    private void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("Game Paused");
        }
    }

    private void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("Game Resumed");
        }
    }

    #endregion

    #region Game Over

    public void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;

        // Скрываем все панели
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Navigation

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;

        // Устанавливаем флаг главного меню
        isMainMenu = true;

        // Загружаем сцену главного меню
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Public Methods

    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void SetMainMenuMode(bool isMainMenu)
    {
        this.isMainMenu = isMainMenu;

        if (isMainMenu)
        {
            ShowMainMenu();
        }
    }

    #endregion
}