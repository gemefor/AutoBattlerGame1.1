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

    [Header("Upgrade Data")]
    [SerializeField] private UpgradeData speedUpgradeData;
    [SerializeField] private UpgradeData damageUpgradeData;
    [SerializeField] private UpgradeData moveSpeedUpgradeData;
    [SerializeField] private float healAmount = 30f;

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
    [SerializeField] private bool isMainMenu = false;

    private bool isGameOver = false;
    private bool isPaused = false;
    private AutoWeapon cachedWeapon;

    private void Awake()
    {
        if (isMainMenu)
        {
            ShowMainMenu();
        }
        else
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthBar;

        if (playerController != null)
            playerController.OnExperienceChanged += UpdateLevelText;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.AddListener(OnUpgradeDamageClicked);

        if (speedButton != null)
            speedButton.onClick.AddListener(SelectSpeedUpgrade);

        if (healButton != null)
            healButton.onClick.AddListener(SelectHealUpgrade);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(ContinueGame);

        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
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

        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(StartGame);

        if (continueGameButton != null)
            continueGameButton.onClick.RemoveListener(ContinueGame);

        if (quitGameButton != null)
            quitGameButton.onClick.RemoveListener(QuitGame);
    }

    private void Update()
    {
        if (isMainMenu) return;
        if (isGameOver) return;

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

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("Main Menu Shown");
    }

    private void StartGame()
    {
        Debug.Log($"Starting game, loading scene: {gameSceneName}");

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        SceneManager.LoadScene(gameSceneName);
        isMainMenu = false;
        Time.timeScale = 1f;
    }

    private void ContinueGame()
    {
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool HasSaveGame()
    {
        return false;
    }

    private void LoadGame()
    {
        Debug.Log("Loading saved game...");
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
            // Создаем UpgradeData для скорости прямо в коде
            UpgradeData speedUpgrade = ScriptableObject.CreateInstance<UpgradeData>();
            speedUpgrade.upgradeType = UpgradeType.MoveSpeed;
            speedUpgrade.value = 1.5f; // На сколько увеличить скорость
            playerController.ApplyUpgrade(speedUpgrade);
        }
        CloseLevelUpMenu();
    }


    private void SelectHealUpgrade()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log($"Healed for {healAmount} HP");
        }
        CloseLevelUpMenu();
    }

    private void OnUpgradeDamageClicked()
    {
        AutoWeapon weapon = FindFirstObjectByType<AutoWeapon>();
        if (weapon != null)
        {
            // Создаем UpgradeData для урона прямо в коде
            UpgradeData damageUpgrade = ScriptableObject.CreateInstance<UpgradeData>();
            damageUpgrade.upgradeType = UpgradeType.Damage;
            damageUpgrade.value = 10f; // На сколько увеличить урон
            weapon.ApplyUpgrade(damageUpgrade);
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

    private void OnResumeButtonClicked()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
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
        cachedWeapon = null;

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
        cachedWeapon = null;
        isMainMenu = true;

        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Helper Methods

    private AutoWeapon GetWeapon()
    {
        if (cachedWeapon == null)
        {
            cachedWeapon = FindFirstObjectByType<AutoWeapon>();
            if (cachedWeapon == null)
            {
                Debug.LogWarning("AutoWeapon not found on scene!");
            }
        }
        return cachedWeapon;
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