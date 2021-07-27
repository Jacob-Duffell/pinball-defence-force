using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject creditsPanel;
	[SerializeField] private Image blackoutImage;
	[SerializeField] private AudioMixer mixer;

	private AudioSource menuMusic;
	private AudioSource buttonClickSFX;

	private void Start()
	{
		SaveDataHandler.Load();

		InitialiseAudio();

		mainMenuPanel.SetActive(true);
		creditsPanel.SetActive(false);
		blackoutImage.gameObject.SetActive(true);

		AudioSource[] _audioSources = GetComponents<AudioSource>();

		menuMusic = _audioSources[0];
		buttonClickSFX = _audioSources[1];

		menuMusic.Play();

		StartCoroutine(SceneLoader.FadeFromBlack(blackoutImage, 1));
	}

	/// <summary>
	/// Initialises the audio mixer volumes for the Main Menu scene.
	/// </summary>
	private void InitialiseAudio()
	{
		if (GameHandler.VolumeMusic == 0)
		{
			mixer.SetFloat("Music", -80);
		}
		else
		{
			mixer.SetFloat("Music", Mathf.Log10(GameHandler.VolumeMusic) * 20);
		}

		if (GameHandler.VolumeSFX == 0)
		{
			mixer.SetFloat("SFX", -80);
		}
		else
		{
			mixer.SetFloat("SFX", Mathf.Log10(GameHandler.VolumeMusic) * 20);
		}
	}

	/// <summary>
	/// Called when the "Start Game" button is pressed. Loads the Hub Menu scene.
	/// </summary>
	public void StartGame()
	{
		menuMusic.Stop();
		buttonClickSFX.Play();

		StartCoroutine(SceneLoader.FadeToBlack(blackoutImage, 1, SceneLoader.Scene.HubMenuScene));
	}

	/// <summary>
	/// Called when the "Credits" button is pressed. Opens the credits menu.
	/// </summary>
	public void OpenCreditsMenu()
	{
		buttonClickSFX.Play();

		mainMenuPanel.SetActive(false);
		creditsPanel.SetActive(true);
	}

	/// <summary>
	/// Called when the "Back" button is pressed. Returns to the main panel of the Main Menu scene.
	/// </summary>
	public void CloseCreditsMenu()
	{
		buttonClickSFX.Play();

		creditsPanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}
}
