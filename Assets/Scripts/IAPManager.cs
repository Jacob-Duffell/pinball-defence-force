using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
	private const string premium10 = "com.DefaultCompany.PinballDefenceForce.premium10";
	private const string premium50 = "com.DefaultCompany.PinballDefenceForce.premium50";
	private const string premium100 = "com.DefaultCompany.PinballDefenceForce.premium100";
	private const string premium500 = "com.DefaultCompany.PinballDefenceForce.premium500";
	private const string premium1000 = "com.DefaultCompany.PinballDefenceForce.premium1000";
	private const string premium10000 = "com.DefaultCompany.PinballDefenceForce.premium10000";

	/// <summary>
	/// Increases user's Premium currency count based on the item they purchased.
	/// </summary>
	/// <param name="_product"></param>
	public void OnPurchaseComplete(Product _product)
	{
		gameObject.GetComponent<HubMenuController>().RefillLivesSFX.Play();

		switch (_product.definition.id)
		{
			case premium10:
				GameHandler.CurrentPremiumMoney += 10;
				break;
			case premium50:
				GameHandler.CurrentPremiumMoney += 50;
				break;
			case premium100:
				GameHandler.CurrentPremiumMoney += 100;
				break;
			case premium500:
				GameHandler.CurrentPremiumMoney += 500;
				break;
			case premium1000:
				GameHandler.CurrentPremiumMoney += 1000;
				break;
			case premium10000:
				GameHandler.CurrentPremiumMoney += 10000;
				break;
			default:
				break;
		}

		gameObject.GetComponent<HubMenuController>().UpdatePremiumText();
		GameHandler.SavedTime = System.DateTime.Now;
		SaveDataHandler.Save();
	}

	/// <summary>
	/// Triggered if a purchase errors out or the user cancels.
	/// </summary>
	/// <param name="_product"></param>
	/// <param name="_reason"></param>
	public void OnPurchaseFailed(Product _product, PurchaseFailureReason _reason)
	{
		Debug.Log("Purchase of " + _product.definition.id + " failed due to " + _reason);
	}
}
