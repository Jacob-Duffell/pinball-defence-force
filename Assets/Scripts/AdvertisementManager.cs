using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementManager : MonoBehaviour, IUnityAdsListener
{
    private const string placement = "rewardedVideo";

	private void Start()
	{
		Advertisement.AddListener(this);
		Advertisement.Initialize("4089591", true);
	}

	public void ShowAd()
	{
		Advertisement.Show(placement);
	}

	public void OnUnityAdsDidFinish(string _placementId, ShowResult _showResult)
	{
		GameController.Current.ContinueWithRewardedAd();
	}

	public void OnUnityAdsDidError(string _message)
	{
		// No code needed here.
	}

	public void OnUnityAdsDidStart(string _placementId)
	{
		// No code needed here.
	}

	public void OnUnityAdsReady(string _placementId)
	{
		// No code needed here.
	}
}
