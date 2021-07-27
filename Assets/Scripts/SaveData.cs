using System;

[System.Serializable] public class SaveData
{
	private DateTime savedTime;
	private double savedTimeRemaining;
	private float damageMultiplier;
	private float pointsMultiplier;
	private int currentMoney;
	private int currentLives;
	private int currentPremiumMoney;
	private int highScore;
	private int highestLevel;
	private int maxLives;
	private int maxPinballs;
	private int totalDeaths;
	private int totalKills;
	private int totalMoney;

	private int lifeUpgradeLevel;
	private int pointsMultiplierUpgradeLevel;
	private int damageMultiplierUpgradeLevel;
	private int pinballMultiplierUpgradeLevel;

	private float volumeMusic;
	private float volumeSFX;

	private bool facebookConnected;

	public DateTime SavedTime { get => savedTime; set => savedTime = value; }
	public double SavedTimeRemaining { get => savedTimeRemaining; set => savedTimeRemaining = value; }
	public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }
	public float PointsMultiplier { get => pointsMultiplier; set => pointsMultiplier = value; }
	public int CurrentMoney { get => currentMoney; set => currentMoney = value; }
	public int CurrentLives { get => currentLives; set => currentLives = value; }
	public int CurrentPremiumMoney { get => currentPremiumMoney; set => currentPremiumMoney = value; }
	public int HighScore { get => highScore; set => highScore = value; }
	public int HighestLevel { get => highestLevel; set => highestLevel = value; }
	public int MaxLives { get => maxLives; set => maxLives = value; }
	public int MaxPinballs { get => maxPinballs; set => maxPinballs = value; }
	public int TotalDeaths { get => totalDeaths; set => totalDeaths = value; }
	public int TotalKills { get => totalKills; set => totalKills = value; }
	public int TotalMoney { get => totalMoney; set => totalMoney = value; }
	public int LifeUpgradeLevel { get => lifeUpgradeLevel; set => lifeUpgradeLevel = value; }
	public int PointsMultiplierUpgradeLevel { get => pointsMultiplierUpgradeLevel; set => pointsMultiplierUpgradeLevel = value; }
	public int DamageMultiplierUpgradeLevel { get => damageMultiplierUpgradeLevel; set => damageMultiplierUpgradeLevel = value; }
	public int PinballMultiplierUpgradeLevel { get => pinballMultiplierUpgradeLevel; set => pinballMultiplierUpgradeLevel = value; }
	public float VolumeMusic { get => volumeMusic; set => volumeMusic = value; }
	public float VolumeSFX { get => volumeSFX; set => volumeSFX = value; }
	public bool FacebookConnected { get => facebookConnected; set => facebookConnected = value; }
}
