using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{
    private void Awake()
	{
		if (!FB.IsInitialized)
		{
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			FB.ActivateApp();
			Debug.Log("Facebook SDK Initialised");
		}
	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			Debug.Log("Facebook SDK Initialised");
		}
		else
		{
			Debug.Log("Failed to initialise the Facebook SDK");
		}
	}

	private void OnHideUnity(bool _isGameShown)
	{
		if (!_isGameShown)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}

	private void AuthCallback(ILoginResult _result)
	{
		if (FB.IsLoggedIn)
		{
			AccessToken _accessToken = AccessToken.CurrentAccessToken;

			Debug.Log(_accessToken.UserId);

			foreach (string _permission in _accessToken.Permissions)
			{
				Debug.Log(_permission);
			}

			gameObject.GetComponent<HubMenuController>().OnConnectedToFacebook();
		}
		else
		{
			Debug.Log("User cancelled login.");
		}
	}

	public void OnFacebookLoginButtonPressed()
	{
		List<string> _permissions = new List<string>() { "public_profile", "email", "user_friends" };

		FB.LogInWithReadPermissions(_permissions, AuthCallback);
	}
}
