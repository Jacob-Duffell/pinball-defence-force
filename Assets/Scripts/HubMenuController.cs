using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class HubMenuController : MonoBehaviour
{
    const int FIFTEEN_MINUTES = 900;

    [Header("Menu Functionality")]
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Image blackoutImage;
    [SerializeField] private RectTransform menuContainer;

    [Header("Hub Menu Panels")]
    [SerializeField] private GameObject hubMenuPanel;
    [SerializeField] private GameObject refillLivesPanel;

    [Header("Hub Menu References")]
    [SerializeField] private TMP_Text highestLevelText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text livesTimerText;
    [SerializeField] private List<TMP_Text> moneyTextGroup = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> premiumTextGroup = new List<TMP_Text>();

    [Header("Refill Lives Panel References")]
    [SerializeField] private TMP_Text currentLivesText;
    [SerializeField] private TMP_Text maxLivesText;
    [SerializeField] private TMP_Text refillLivesPriceText;

    [Header("Shop References")]
    [SerializeField] private GameObject itemUpgradePanel;
    [SerializeField] private GameObject upgradeShopPrefab;
    [SerializeField] private List<Upgrade> upgrades = new List<Upgrade>();
    [SerializeField] private Transform shopUpgradeContainer;

    [Header("Settings References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject facebookConnectedPanel;
    [SerializeField] private Button facebookConnectedButton;
    [SerializeField] private TMP_Text facebookConnectedText;

    [Header("Audio References")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private AudioSource hubMenuMusic;
    private AudioSource buttonClickSFX;
    private AudioSource refillLivesSFX;
    private AudioSource insufficientFundsSFX;
    private AudioSource purchaseUpgradeSFX;
    private AudioSource startNewGameSFX;

    private Coroutine moveToMenuCoroutine;
    private Vector3[] menuPositions;

    private double currentTimeRemaining;
    private Upgrade selectedUpgrade;
    private int selectedUpgradeLevel;
    private int refillLivesPrice;

	private void Start()
    {
        SaveDataHandler.Load();

        blackoutImage.gameObject.SetActive(true);
        CalculateLivesTimerOnStart();
        InitialiseShopMenu();

        InitialiseMenuTransitions();

        hubMenuPanel.SetActive(true);
        refillLivesPanel.SetActive(false);
        itemUpgradePanel.SetActive(false);
        settingsPanel.SetActive(true);
        facebookConnectedPanel.SetActive(false);

        highestLevelText.text = GameHandler.HighestLevel.ToString();
        highScoreText.text = GameHandler.HighScore.ToString();
        livesText.text = GameHandler.CurrentLives.ToString();

        UpdateMoneyText();
        UpdatePremiumText();

        if (GameHandler.FacebookConnected)
		{
            facebookConnectedText.text = "CONNECTED!";
            facebookConnectedButton.enabled = false;
        }

        musicSlider.value = GameHandler.VolumeMusic;
        sfxSlider.value = GameHandler.VolumeSFX;

        SetMusicVolume();
        SetSFXVolume();

        AudioSource[] _audioSources = GetComponents<AudioSource>();

        hubMenuMusic = _audioSources[0];
        buttonClickSFX = _audioSources[1];
        refillLivesSFX = _audioSources[2];
        insufficientFundsSFX = _audioSources[3];
        purchaseUpgradeSFX = _audioSources[4];
        startNewGameSFX = _audioSources[5];

        StartCoroutine(SceneLoader.FadeFromBlack(blackoutImage, 1));

        hubMenuMusic.Play();
    }

    private void Update()
    {
        UpdateLivesTimer();

        livesText.text = GameHandler.CurrentLives.ToString();
        livesTimerText.text = TimeSpan.FromSeconds((int)currentTimeRemaining).ToString(@"m\m\:s\s", System.Globalization.CultureInfo.InvariantCulture) + " Until Full";
    }

    /// <summary>
    /// Calculates how much time is remaining until the user reaches max lives.
    /// </summary>
    private void CalculateLivesTimerOnStart()
    {
        if (GameHandler.CurrentLives == GameHandler.MaxLives)
        {
            currentTimeRemaining = 0;
        }
        else
        {
            TimeSpan _timeDifference = new TimeSpan(DateTime.Now.Ticks - GameHandler.SavedTime.Ticks);

            currentTimeRemaining = GameHandler.SavedTimeRemaining - _timeDifference.TotalSeconds;
        }
    }

    /// <summary>
    /// Called when user loads the gameplay scene. Removes a life and updates the lives timer.
    /// </summary>
    private void CalculateLivesTimerOnExit()
    {
        GameHandler.CurrentLives--;
        GameHandler.SavedTime = DateTime.Now;
        GameHandler.SavedTimeRemaining = currentTimeRemaining + FIFTEEN_MINUTES;

        currentTimeRemaining = GameHandler.SavedTimeRemaining;
    }

    /// <summary>
    /// Finds the positions of the menu transition points.
    /// </summary>
    private void InitialiseMenuTransitions()
    {
        Vector3 _halfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        menuPositions = new Vector3[menuContainer.childCount];

        for (int i = 0; i < menuPositions.Length; i++)
        {
            menuPositions[i] = menuContainer.GetChild(i).position - _halfScreen;
        }
    }

    /// <summary>
    /// Fills the shop menu with all specified upgrades.
    /// </summary>
    private void InitialiseShopMenu()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade _currentUpgrade = upgrades[i];
            GameObject _upgradeItem = Instantiate(upgradeShopPrefab, shopUpgradeContainer);

            _upgradeItem.transform.GetChild(0).GetComponent<TMP_Text>().text = _currentUpgrade.UpgradeName;
            _upgradeItem.transform.GetChild(1).GetComponent<TMP_Text>().text = _currentUpgrade.UpgradeDescription;
            _upgradeItem.transform.GetChild(2).GetComponent<Image>().sprite = _currentUpgrade.UpgradeIcon;

            switch (_currentUpgrade.UpgradeID)
            {
                case UpgradeID.DamageUpgrade:
                    _upgradeItem.transform.GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.DamageMultiplierUpgradeLevel;
                    break;
                case UpgradeID.ExtraLives:
                    _upgradeItem.transform.GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.LifeUpgradeLevel;
                    break;
                case UpgradeID.ExtraPinballs:
                    _upgradeItem.transform.GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.PinballMultiplierUpgradeLevel;
                    break;
                case UpgradeID.PointsMultiplier:
                    _upgradeItem.transform.GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.PointsMultiplierUpgradeLevel;
                    break;
                default:
                    break;
            }

            _upgradeItem.GetComponent<Button>().onClick.AddListener(() => OnUpgradeItemButtonClick(_currentUpgrade));
        }
    }

    /// <summary>
    /// Sets the bottom panel of the shop menu to show the details of the selected upgrade.
    /// </summary>
    /// <param name="_upgrade"></param>
    private void OnUpgradeItemButtonClick(Upgrade _upgrade)
    {
        buttonClickSFX.Play();

        selectedUpgrade = _upgrade;

        switch (_upgrade.UpgradeID)
        {
            case UpgradeID.DamageUpgrade:
                selectedUpgradeLevel = GameHandler.DamageMultiplierUpgradeLevel;
                break;
            case UpgradeID.ExtraLives:
                selectedUpgradeLevel = GameHandler.LifeUpgradeLevel;
                break;
            case UpgradeID.ExtraPinballs:
                selectedUpgradeLevel = GameHandler.PinballMultiplierUpgradeLevel;
                break;
            case UpgradeID.PointsMultiplier:
                selectedUpgradeLevel = GameHandler.PointsMultiplierUpgradeLevel;
                break;
            default:
                break;
        }

        itemUpgradePanel.SetActive(true);

        if (selectedUpgradeLevel == 0)
        {
            itemUpgradePanel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = _upgrade.UpgradePricePremium.ToString();
            itemUpgradePanel.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = _upgrade.UpgradePriceMoney.ToString();
        }
        else
        {
            itemUpgradePanel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = (_upgrade.UpgradePricePremium * (selectedUpgradeLevel * _upgrade.UpgradePriceMultiplier)).ToString();
            itemUpgradePanel.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = (_upgrade.UpgradePriceMoney * (selectedUpgradeLevel * _upgrade.UpgradePriceMultiplier)).ToString();
        }

        switch (_upgrade.UpgradeID)
        {
            case UpgradeID.DamageUpgrade:
                itemUpgradePanel.transform.GetChild(3).GetComponent<TMP_Text>().text = "x" + GameHandler.DamageMultiplier + " Damage";
                break;
            case UpgradeID.ExtraLives:
                itemUpgradePanel.transform.GetChild(3).GetComponent<TMP_Text>().text = "Max " + GameHandler.MaxLives + " Lives";
                break;
            case UpgradeID.ExtraPinballs:
                itemUpgradePanel.transform.GetChild(3).GetComponent<TMP_Text>().text = "Max " + GameHandler.MaxPinballs + " Pinballs";
                break;
            case UpgradeID.PointsMultiplier:
                itemUpgradePanel.transform.GetChild(3).GetComponent<TMP_Text>().text = "x" + GameHandler.PointsMultiplier + " Points";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Applies upgrade functionality to the user's save data and increments the upgrade level.
    /// </summary>
    /// <param name="_purchaseType"></param>
    private void PurchaseUpgrade(string _purchaseType)
    {
        switch (selectedUpgrade.UpgradeID)
        {
            case UpgradeID.DamageUpgrade:
                GameHandler.DamageMultiplier += 0.1f;
                GameHandler.DamageMultiplierUpgradeLevel++;

                TriggerUpgradeAnalyticsEvent(selectedUpgrade, GameHandler.DamageMultiplierUpgradeLevel, _purchaseType);
                break;
            case UpgradeID.ExtraLives:
                GameHandler.MaxLives += 1;
                GameHandler.LifeUpgradeLevel++;

                GameHandler.SavedTimeRemaining = currentTimeRemaining + FIFTEEN_MINUTES;
                currentTimeRemaining = GameHandler.SavedTimeRemaining;

                TriggerUpgradeAnalyticsEvent(selectedUpgrade, GameHandler.LifeUpgradeLevel, _purchaseType);
                break;
            case UpgradeID.ExtraPinballs:
                GameHandler.MaxPinballs += 1;
                GameHandler.PinballMultiplierUpgradeLevel++;

                TriggerUpgradeAnalyticsEvent(selectedUpgrade, GameHandler.PinballMultiplierUpgradeLevel, _purchaseType);
                break;
            case UpgradeID.PointsMultiplier:
                GameHandler.PointsMultiplier += 0.1f;
                GameHandler.PointsMultiplierUpgradeLevel++;

                TriggerUpgradeAnalyticsEvent(selectedUpgrade, GameHandler.PointsMultiplierUpgradeLevel, _purchaseType);
                break;
            default:
                break;
        }

        GameHandler.SavedTime = DateTime.Now;

        SaveDataHandler.Save();

        OnUpgradeItemButtonClick(selectedUpgrade);
        UpdateShopItems();
    }

    /// <summary>
    /// Triggers analytics for the upgrade that is purchased.
    /// </summary>
    /// <param name="_upgradeType"></param>
    /// <param name="_upgradeLevel"></param>
    /// <param name="_purchaseType"></param>
    private void TriggerUpgradeAnalyticsEvent(Upgrade _upgradeType, int _upgradeLevel, string _purchaseType)
    {
        AnalyticsResult _analyticsResult = Analytics.CustomEvent(
            "UpgradePurchased",
            new Dictionary<string, object>
            {
                {"UpgradeType", _upgradeType.name},
                {"UpgradeLevel", _upgradeLevel },
                {"PurchaseType", _purchaseType }
            });

        Debug.Log("Analytics Result: " + _analyticsResult);
    }

    /// <summary>
    /// Updates the current time remaining until max lives is reached. Also handles giving the player lives when certain points in the timer are reached.
    /// </summary>
    private void UpdateLivesTimer()
    {
        if (currentTimeRemaining > 0)
        {
            currentTimeRemaining -= Time.deltaTime;
        }
        else if (currentTimeRemaining <= 0)
        {
            currentTimeRemaining = 0;

            GameHandler.CurrentLives = GameHandler.MaxLives;
        }
        else
        {
            double _totalTimeCountdown = GameHandler.MaxLives * FIFTEEN_MINUTES;

            GameHandler.CurrentLives = (int)((_totalTimeCountdown - currentTimeRemaining) / FIFTEEN_MINUTES);
        }
    }

    /// <summary>
    /// Updates the money text for all objects in the money group.
    /// </summary>
    private void UpdateMoneyText()
	{
        for (int i = 0; i < moneyTextGroup.Count; i++)
		{
            moneyTextGroup[i].text = GameHandler.CurrentMoney.ToString();
        }
	}

    /// <summary>
    /// Updates the premium currency text for all objects in the premium currency group.
    /// </summary>
    public void UpdatePremiumText()
	{
        for (int i = 0; i < premiumTextGroup.Count; i++)
		{
            premiumTextGroup[i].text = GameHandler.CurrentPremiumMoney.ToString();
        }
	}

    /// <summary>
    /// Refreshes the shop items within the shop menu.
    /// </summary>
    private void UpdateShopItems()
	{
        for (int i = 0; i < upgrades.Count; i++)
		{
            switch (upgrades[i].UpgradeID)
            {
                case UpgradeID.DamageUpgrade:
                    shopUpgradeContainer.transform.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.DamageMultiplierUpgradeLevel;
                    break;
                case UpgradeID.ExtraLives:
                    shopUpgradeContainer.transform.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.LifeUpgradeLevel;
                    break;
                case UpgradeID.ExtraPinballs:
                    shopUpgradeContainer.transform.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.PinballMultiplierUpgradeLevel;
                    break;
                case UpgradeID.PointsMultiplier:
                    shopUpgradeContainer.transform.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "LVL " + GameHandler.PointsMultiplierUpgradeLevel;
                    break;
                default:
                    break;
            }
		}
	}

    /// <summary>
    /// Opens up Facebook reward menu when the user connects their Facebook account.
    /// </summary>
    public void OnConnectedToFacebook()
	{
        settingsPanel.SetActive(false);
        facebookConnectedPanel.SetActive(true);
	}

	#region Button Functions

    /// <summary>
    /// Closes the "Facebook Connected" menu and restores the normal "Settings" menu.
    /// Rewards the user with their bonus Premium money.
    /// </summary>
    public void CloseFacebookConnectedMenu()
	{
        refillLivesSFX.Play();

        GameHandler.FacebookConnected = true;

        GameHandler.CurrentPremiumMoney += 100;

        facebookConnectedPanel.SetActive(false);
        settingsPanel.SetActive(true);

        facebookConnectedButton.enabled = false;

        UpdatePremiumText();

        facebookConnectedText.text = "CONNECTED!";

        GameHandler.SavedTime = DateTime.Now;
        SaveDataHandler.Save();
	}

    /// <summary>
    /// Closes the "Refill Lives" menu and restores the normal "Hub" menu.
    /// </summary>
	public void CloseRefillLivesMenu()
    {
        buttonClickSFX.Play();

        refillLivesPanel.SetActive(false);
        hubMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Calls for the user's data to be deleted, then transitions to the Main Menu scene.
    /// </summary>
    public void DeleteSaveData()
	{
        hubMenuMusic.Stop();
        buttonClickSFX.Play();

        SaveDataHandler.DeleteSaveData();
        StartCoroutine(SceneLoader.FadeToBlack(blackoutImage, 1, SceneLoader.Scene.MainMenuScene));
	}

    /// <summary>
    /// Specifies the menu to be move towards and triggers the coroutine to achieve the menu transition.
    /// </summary>
    /// <param name="_id"></param>
    public void MoveMenu(int _id)
    {
        buttonClickSFX.Play();

        itemUpgradePanel.SetActive(false);

        if (moveToMenuCoroutine != null)
        {
            StopCoroutine(moveToMenuCoroutine);
        }

        moveToMenuCoroutine = StartCoroutine(MoveToMenu(-menuPositions[_id]));
    }

    /// <summary>
    /// Hides the "Hub" menu and opens the "Refill Lives" menu if the user does not currently have max lives.
    /// </summary>
    public void OpenRefillLivesMenu()
    {
        buttonClickSFX.Play();

        if (GameHandler.CurrentLives < GameHandler.MaxLives)
		{
            hubMenuPanel.SetActive(false);
            refillLivesPanel.SetActive(true);

            refillLivesPrice = (GameHandler.MaxLives - GameHandler.CurrentLives) * 5;

            currentLivesText.text = GameHandler.CurrentLives.ToString();
            maxLivesText.text = GameHandler.MaxLives.ToString();
            refillLivesPriceText.text = refillLivesPrice.ToString();
        }
    }

    /// <summary>
    /// Purchases the selected upgrade with Premium money, provided the user has enough Premium money to complete the purchase.
    /// </summary>
    public void PurchaseWithPremium()
	{
        if (selectedUpgradeLevel == 0)
		{
            if (selectedUpgrade.UpgradePricePremium <= GameHandler.CurrentPremiumMoney)
            {
                purchaseUpgradeSFX.Play();

                GameHandler.CurrentPremiumMoney -= selectedUpgrade.UpgradePricePremium;
                PurchaseUpgrade("Premium");
            }
            else
			{
                insufficientFundsSFX.Play();
			}
        }
        else
		{
            int _upgradePricePremium = selectedUpgrade.UpgradePricePremium * (selectedUpgradeLevel * selectedUpgrade.UpgradePriceMultiplier);

            if (_upgradePricePremium <= GameHandler.CurrentPremiumMoney)
            {
                purchaseUpgradeSFX.Play();

                GameHandler.CurrentPremiumMoney -= _upgradePricePremium;
                PurchaseUpgrade("Premium");
            }
            else
			{
                insufficientFundsSFX.Play();
			}
        }

        UpdatePremiumText();
	}

    /// <summary>
    /// Purchases the selected upgrade with the standard money, provided the user has enough standard money to complete the purchase.
    /// </summary>
    public void PurchaseWithMoney()
	{
        if (selectedUpgradeLevel == 0)
		{
            if (selectedUpgrade.UpgradePriceMoney <= GameHandler.CurrentMoney)
            {
                purchaseUpgradeSFX.Play();

                GameHandler.CurrentMoney -= selectedUpgrade.UpgradePriceMoney;
                PurchaseUpgrade("Standard");
            }
            else
			{
                insufficientFundsSFX.Play();
			}
        }
        else
		{
            int _upgradePriceMoney = selectedUpgrade.UpgradePriceMoney * (selectedUpgradeLevel * selectedUpgrade.UpgradePriceMultiplier);

            if (_upgradePriceMoney <= GameHandler.CurrentMoney)
            {
                purchaseUpgradeSFX.Play();

                GameHandler.CurrentMoney -= _upgradePriceMoney;
                PurchaseUpgrade("Standard");
            }
            else
			{
                insufficientFundsSFX.Play();
			}
        }

        UpdateMoneyText();
	}

    /// <summary>
    /// Transitions the user to the Main Menu scene.
    /// </summary>
    public void QuitGame()
	{
        hubMenuMusic.Stop();
        buttonClickSFX.Play();

        GameHandler.SavedTime = DateTime.Now;
        SaveDataHandler.Save();

        StartCoroutine(SceneLoader.FadeToBlack(blackoutImage, 1, SceneLoader.Scene.MainMenuScene));
	}

    /// <summary>
    /// Sets the user's lives to max, provided the user has enough Premium money.
    /// </summary>
    public void RefillLives()
	{
        if (GameHandler.CurrentPremiumMoney >= refillLivesPrice)
		{
            refillLivesSFX.Play();

            GameHandler.CurrentPremiumMoney -= refillLivesPrice;

            UpdatePremiumText();

            GameHandler.CurrentLives = GameHandler.MaxLives;
            currentTimeRemaining = 0;

            GameHandler.SavedTimeRemaining = currentTimeRemaining;

            SaveDataHandler.Save();

            CloseRefillLivesMenu();
		}
        else
		{
            insufficientFundsSFX.Play();
		}
	}

    /// <summary>
    /// Sets the volume ofthe Music mixer group.
    /// </summary>
    public void SetMusicVolume()
	{
        GameHandler.VolumeMusic = musicSlider.value;

        if (GameHandler.VolumeMusic == 0)
		{
            mixer.SetFloat("Music", -80);
		}
        else
		{
            mixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        }
	}

    /// <summary>
    /// Sets the volume of the SFX mixer group.
    /// </summary>
    public void SetSFXVolume()
	{
        GameHandler.VolumeSFX = sfxSlider.value;

        if (GameHandler.VolumeSFX == 0)
		{
            mixer.SetFloat("SFX", -80);
		}
        else
		{
            mixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
		}
    }

    /// <summary>
    /// If the user has enough lives, the game transitions from the Hub Menu scene to the Gameplay scene.
    /// </summary>
    public void StartGameplayScene()
	{
        if (GameHandler.CurrentLives > 0)
		{
            hubMenuMusic.Stop();
            startNewGameSFX.Play();

            CalculateLivesTimerOnExit();

            SaveDataHandler.Save();

            StartCoroutine(SceneLoader.FadeToBlack(blackoutImage, 1, SceneLoader.Scene.GameplayScene));
        }
        else
		{
            insufficientFundsSFX.Play();
		}
	}

	#endregion

    /// <summary>
    /// Smoothly animates the transition between the current menu and the selected menu.
    /// </summary>
    /// <param name="_desiredPosition"></param>
    /// <returns></returns>
	private IEnumerator MoveToMenu(Vector3 _desiredPosition)
    {
		while (((Vector3)menuContainer.anchoredPosition).ToString() != _desiredPosition.ToString())
		{
			menuContainer.anchoredPosition = Vector3.Lerp(menuContainer.anchoredPosition, _desiredPosition, smoothSpeed);

			yield return null;
		}
	}

    public AudioSource RefillLivesSFX { get => refillLivesSFX; set => refillLivesSFX = value; }
}