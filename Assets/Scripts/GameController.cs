using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameController : MonoBehaviour
{
    private enum GameState
    {
        Play = 0,
        Pause = 1,
        GameOver = 2
    }

    private static GameController current;

    [SerializeField] private GameObject background;
    [SerializeField] private List<Sprite> backgrounds;
    [SerializeField] private Image blackoutImage;

    [SerializeField] private HingeJoint2D leftFlipper;
    [SerializeField] private HingeJoint2D rightFlipper;

    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    private EnemyWaveController enemyWaveController;
    private GameObject player;
	private GameState currentState;
    private int continuePremiumCost;
    private int gameOverCount = 0;
    private int lives;
    private float score = 0;
    private TMP_Text gameOverHighScoreText;
    private TMP_Text gameOverScoreText;
    private TMP_Text gameOverMoneyEarnedText;
    private TMP_Text gameOverPremiumText;

    private GameObject gameOverContinueWithAdsPanel;
    private GameObject gameOverContinueWithoutAdsPanel;

    private AudioSource gameplayMusic;
    private AudioSource buttonClickSFX;
    private AudioSource flipperSFX;
    private AudioSource levelCompleteSFX;
    private AudioSource insufficientFundsSFX;
    private AudioSource revivePlayerSFX;
    private AudioSource hitEnemySFX;
    private AudioSource enemyDeathSFX;
    private AudioSource loseLifeSFX;

    private event Action<float> OnAddToScore;
	private event Action OnPlayerDeath;

    private void Start()
    {
        SaveDataHandler.Load();

        lives = GameHandler.MaxPinballs;

        current = this;
        currentState = GameState.Play;
        Time.timeScale = 1;

        enemyWaveController = gameObject.GetComponent<EnemyWaveController>();

        blackoutImage.gameObject.SetActive(true);
        StartCoroutine(SceneLoader.FadeFromBlack(blackoutImage, 1));

        player = GameObject.FindGameObjectWithTag("Player");
        gameOverHighScoreText = gameOverPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        gameOverScoreText = gameOverPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        gameOverMoneyEarnedText = gameOverPanel.transform.GetChild(3).GetComponent<TMP_Text>();
        gameOverPremiumText = gameOverPanel.transform.GetChild(6).GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        gameOverContinueWithAdsPanel = gameOverPanel.transform.GetChild(4).GetChild(0).gameObject;
        gameOverContinueWithoutAdsPanel = gameOverPanel.transform.GetChild(4).GetChild(1).gameObject;

        OnAddToScore += IncrementScore;
        OnPlayerDeath += LoseLife;

        SetBackground();
        UpdateLivesText();

        AudioSource[] _audioSources = GetComponents<AudioSource>();

        gameplayMusic = _audioSources[0];
        buttonClickSFX = _audioSources[1];
        flipperSFX = _audioSources[2];
        levelCompleteSFX = _audioSources[3];
        insufficientFundsSFX = _audioSources[4];
        revivePlayerSFX = _audioSources[5];
        hitEnemySFX = _audioSources[6];
        enemyDeathSFX = _audioSources[7];
        loseLifeSFX = _audioSources[8];

        gameplayMusic.Play();
}

    private void Update()
    {
        if (currentState == GameState.Play)
		{
            GetFlipperTouchInput();
        }
    }

	private void OnDestroy()
	{
        OnAddToScore -= IncrementScore;
        OnPlayerDeath -= LoseLife;
	}

	/// <summary>
	/// Handles the touch input for the pinball flippers.
	/// </summary>
	private void GetFlipperTouchInput()
	{
        for (int i = 0; i < Input.touchCount; i++)
		{
            if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
                flipperSFX.Play();
            }

            if (Input.touches[i].position.x <= Screen.width / 2)
            {
                leftFlipper.useMotor = true;
            }
            else if (Input.touches[i].position.x >= Screen.width / 2)
            {
                rightFlipper.useMotor = true;
            }

            if (Input.GetTouch(i).phase == TouchPhase.Ended && Input.GetTouch(i).position.x <= Screen.width / 2)
			{
                leftFlipper.useMotor = false;
			}
            else if (Input.GetTouch(i).phase == TouchPhase.Ended && Input.GetTouch(i).position.x >= Screen.width / 2)
            {
                rightFlipper.useMotor = false;
            }
        }
    }

    /// <summary>
    /// Updates the current score, as well as the associated UI text.
    /// </summary>
    /// <param name="_scoreToAdd"></param>
    private void IncrementScore(float _scoreToAdd)
	{
        score += _scoreToAdd;
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
	}

    /// <summary>
    /// Decreases the player's life counter. Triggers the game over state if the player's lives reaches 0.
    /// </summary>
    private void LoseLife()
	{
        player.transform.position = new Vector3(0.02f, 4.769f, 0f);
        lives--;

        UpdateLivesText();

        if (lives <= 0)
		{
            gameplayMusic.Pause();
            levelCompleteSFX.Play();

            currentState = GameState.GameOver;

            gameOverCount++;
            continuePremiumCost = (int) Math.Pow(5, gameOverCount); // Exponentially increases the cost of reviving the player.

            SetGameOverText();
            SetGameOverContinuePanel();

            Time.timeScale = 0;
            gameOverPanel.SetActive(true);
		}
        else
		{
            loseLifeSFX.Play();
		}
	}

    /// <summary>
    /// Chooses a random background when the game starts.
    /// </summary>
    private void SetBackground()
	{
        Sprite randomBackground = backgrounds[UnityEngine.Random.Range(0, 2)];

        background.GetComponent<SpriteRenderer>().sprite = randomBackground;
	}

    /// <summary>
    /// Decides whether the rewarded ad button will be visible based on if it is the user's first game over in the current gameplay session.
    /// </summary>
    private void SetGameOverContinuePanel()
	{
        if (gameOverCount == 1)
		{
            gameOverContinueWithoutAdsPanel.SetActive(false);
            gameOverContinueWithAdsPanel.SetActive(true);

            gameOverContinueWithAdsPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = continuePremiumCost.ToString();
		}
        else if (gameOverCount > 1)
		{
            gameOverContinueWithoutAdsPanel.SetActive(true);
            gameOverContinueWithAdsPanel.SetActive(false);

            gameOverContinueWithoutAdsPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = continuePremiumCost.ToString();
        }
	}

    /// <summary>
    /// Updates the relevant text on the Game Over menu.
    /// </summary>
    private void SetGameOverText()
	{
        if (score > GameHandler.HighScore)
        {
            GameHandler.HighScore = Mathf.FloorToInt(score);
        }

        gameOverHighScoreText.text = "High Score: " + GameHandler.HighScore;
        gameOverScoreText.text = "Score: " + Mathf.FloorToInt(score);
        gameOverMoneyEarnedText.text = "Money Earned: " + (Mathf.FloorToInt(score) / 10);
        gameOverPremiumText.text = GameHandler.CurrentPremiumMoney.ToString();
    }

    /// <summary>
    /// Triggers an analytics event whenever a player reaches the Game Over state.
    /// </summary>
    private void TriggerGameOverAnalyticsEvent()
	{
        AnalyticsResult _analyticsResult = Analytics.CustomEvent(
            "PlayerRevived",
            new Dictionary<string, object>
            {
                {"GameOverCountCurrentSession", gameOverCount},
                {"CurrentScore", score},
                {"HighScore", GameHandler.HighScore},
                {"CurrentWave", enemyWaveController.CurrentWaveNumber},
                {"HighestWave", GameHandler.HighestLevel},
                {"TotalEnemiesDefeatedAllSessions", GameHandler.TotalKills},
                {"TotalMoneyEarnedAllSessions", GameHandler.TotalMoney}
            });

        Debug.Log("Analytics Result: " + _analyticsResult);
    }

    /// <summary>
    /// Triggers an analytics event whenever a player chooses to revive themselves.
    /// </summary>
    /// <param name="_reviveType"></param>
    private void TriggerReviveAnalyticsEvent(string _reviveType)
	{
        AnalyticsResult _analyticsResult = Analytics.CustomEvent(
            "PlayerRevived",
            new Dictionary<string, object>
            {
                {"ReviveType", _reviveType},
                {"GameOverCount", gameOverCount},
                {"PremiumCost", continuePremiumCost},
                {"CurrentWave",  enemyWaveController.CurrentWaveNumber},
                {"CurrentScore", score},
                {"HighScore", GameHandler.HighScore}
            });

        Debug.Log("Analytics Result: " + _analyticsResult);
    }

    /// <summary>
    /// Updates the lives text to reflect the current lives remaining.
    /// </summary>
    private void UpdateLivesText()
	{
        livesText.text = "Lives: " + lives;
	}

    /// <summary>
    /// Invokes the increment score event.
    /// </summary>
    /// <param name="_scoreToAdd"></param>
    public void AddToScore(float _scoreToAdd)
	{
        OnAddToScore?.Invoke(_scoreToAdd);
	}

    /// <summary>
    /// Invokes the player death event.
    /// </summary>
    public void PlayerDeath()
	{
        if (OnPlayerDeath != null)
		{
            OnPlayerDeath?.Invoke();
		}
	}

	#region Button Press Functions

    /// <summary>
    /// If the player has enough premium money, they are revived with an single extra life.
    /// </summary>
    public void ContinueWithPremium()
	{
        if (GameHandler.CurrentPremiumMoney >= continuePremiumCost)
		{
            revivePlayerSFX.Play();
            gameplayMusic.UnPause();

            GameHandler.CurrentPremiumMoney -= continuePremiumCost;

            currentState = GameState.Play;
            gameOverPanel.SetActive(false);
            lives = 1;
            UpdateLivesText();
            Time.timeScale = 1;

            TriggerReviveAnalyticsEvent("Premium");
		}
        else
		{
            insufficientFundsSFX.Play();
		}
	}

    /// <summary>
    /// The player is revived after they watch a rewarded advertisement.
    /// </summary>
    public void ContinueWithRewardedAd()
	{
        revivePlayerSFX.Play();
        gameplayMusic.UnPause();

        currentState = GameState.Play;
        gameOverPanel.SetActive(false);
        lives = 1;
        UpdateLivesText();
        Time.timeScale = 1;

        TriggerReviveAnalyticsEvent("RewardedAd");
    }

    /// <summary>
    /// Saves the game and transitions the user to the Hub Menu scene.
    /// </summary>
    public void OnMenuButtonPressed()
	{
        buttonClickSFX.Play();

        Time.timeScale = 1;

        if (enemyWaveController.CurrentWaveNumber > GameHandler.HighestLevel)
		{
            GameHandler.HighestLevel = enemyWaveController.CurrentWaveNumber;
		}

        GameHandler.CurrentMoney += Mathf.FloorToInt(score / 10);
        GameHandler.TotalMoney += Mathf.FloorToInt(score / 10);

        TriggerGameOverAnalyticsEvent();

        SaveDataHandler.Save();
        StartCoroutine(SceneLoader.FadeToBlack(blackoutImage, 1, SceneLoader.Scene.HubMenuScene));
	}

    /// <summary>
    /// Pauses the game and opens the pause menu.
    /// </summary>
    public void OnPauseButtonPressed()
	{
        if (currentState != GameState.GameOver)
		{
            buttonClickSFX.Play();
            gameplayMusic.Pause();

            currentState = GameState.Pause;
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// Unpauses the game and closes the pause menu.
    /// </summary>
    public void OnResumeButtonPressed()
	{
        buttonClickSFX.Play();
        gameplayMusic.UnPause();

        currentState = GameState.Play;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion

    public static GameController Current { get => current; set => current = value; }
	public AudioSource HitEnemySFX { get => hitEnemySFX; set => hitEnemySFX = value; }
	public AudioSource EnemyDeathSFX { get => enemyDeathSFX; set => enemyDeathSFX = value; }
}
