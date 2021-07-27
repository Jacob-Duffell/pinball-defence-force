using System;

public static class GameHandler
{
	private static DateTime savedTime;
	private static double savedTimeRemaining;
	private static float damageMultiplier;
	private static float pointsMultiplier;
	private static int currentMoney;
	private static int currentLives;
	private static int currentPremiumMoney;
	private static int highScore;
	private static int highestLevel;
	private static int maxLives;
	private static int maxPinballs;
	private static int totalDeaths;
	private static int totalKills;
	private static int totalMoney;

	private static int lifeUpgradeLevel;
	private static int pointsMultiplierUpgradeLevel;
	private static int damageMultiplierUpgradeLevel;
	private static int pinballMultiplierUpgradeLevel;

	private static float volumeMusic;
	private static float volumeSFX;

	private static bool facebookConnected;

	public static DateTime SavedTime { get => savedTime; set => savedTime = value; }
	public static double SavedTimeRemaining { get => savedTimeRemaining; set => savedTimeRemaining = value; }
	public static float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }
	public static float PointsMultiplier { get => pointsMultiplier; set => pointsMultiplier = value; }
	public static int CurrentMoney { get => currentMoney; set => currentMoney = value; }
	public static int CurrentLives { get => currentLives; set => currentLives = value; }
	public static int CurrentPremiumMoney { get => currentPremiumMoney; set => currentPremiumMoney = value; }
	public static int HighScore { get => highScore; set => highScore = value; }
	public static int HighestLevel { get => highestLevel; set => highestLevel = value; }
	public static int MaxLives { get => maxLives; set => maxLives = value; }
	public static int TotalDeaths { get => totalDeaths; set => totalDeaths = value; }
	public static int TotalKills { get => totalKills; set => totalKills = value; }
	public static int TotalMoney { get => totalMoney; set => totalMoney = value; }
	public static int MaxPinballs { get => maxPinballs; set => maxPinballs = value; }
	public static int LifeUpgradeLevel { get => lifeUpgradeLevel; set => lifeUpgradeLevel = value; }
	public static int PointsMultiplierUpgradeLevel { get => pointsMultiplierUpgradeLevel; set => pointsMultiplierUpgradeLevel = value; }
	public static int DamageMultiplierUpgradeLevel { get => damageMultiplierUpgradeLevel; set => damageMultiplierUpgradeLevel = value; }
	public static int PinballMultiplierUpgradeLevel { get => pinballMultiplierUpgradeLevel; set => pinballMultiplierUpgradeLevel = value; }
	public static float VolumeMusic { get => volumeMusic; set => volumeMusic = value; }
	public static float VolumeSFX { get => volumeSFX; set => volumeSFX = value; }
	public static bool FacebookConnected { get => facebookConnected; set => facebookConnected = value; }
}
