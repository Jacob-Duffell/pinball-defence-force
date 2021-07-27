using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeID
{
	PointsMultiplier = 0,
	DamageUpgrade = 1,
	ExtraPinballs = 2,
	ExtraLives = 3
};

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade", order = 1)]
public class Upgrade : ScriptableObject
{
	[SerializeField] private UpgradeID upgradeID;
    [SerializeField] private string upgradeName;
    [SerializeField] private string upgradeDescription;
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private int upgradePriceMoney;
    [SerializeField] private int upgradePricePremium;
	[SerializeField] private int upgradePriceMultiplier;

	public string UpgradeName { get => upgradeName; set => upgradeName = value; }
	public string UpgradeDescription { get => upgradeDescription; set => upgradeDescription = value; }
	public Sprite UpgradeIcon { get => upgradeIcon; set => upgradeIcon = value; }
	public int UpgradePriceMoney { get => upgradePriceMoney; set => upgradePriceMoney = value; }
	public int UpgradePricePremium { get => upgradePricePremium; set => upgradePricePremium = value; }
	public UpgradeID UpgradeID { get => upgradeID; set => upgradeID = value; }
	public int UpgradePriceMultiplier { get => upgradePriceMultiplier; set => upgradePriceMultiplier = value; }
}