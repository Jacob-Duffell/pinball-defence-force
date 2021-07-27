using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneLoader
{
    public enum Scene
	{
		MainMenuScene = 0,
		HubMenuScene = 1,
		GameplayScene = 2
	}

    /// <summary>
    /// Loads the specified game scene.
    /// </summary>
    /// <param name="_scene"></param>
	public static void Load(Scene _scene)
	{
		SceneManager.LoadScene(_scene.ToString());
	}

    /// <summary>
    /// Fades the current scene to black and calls for a specified scene to be loaded.
    /// </summary>
    /// <param name="_blackoutImage"></param>
    /// <param name="_speed"></param>
    /// <param name="_scene"></param>
    /// <returns></returns>
    public static IEnumerator FadeToBlack(Image _blackoutImage, int _speed, Scene _scene)
    {
        Color _imageColour = _blackoutImage.color;
        float _fadeAmount;

        while (_blackoutImage.color.a < 1)
        {
            _fadeAmount = _imageColour.a + (_speed * Time.deltaTime);

            _imageColour.a = _fadeAmount;
            _blackoutImage.color = _imageColour;

            yield return null;
        }

        Load(_scene);
    }

    /// <summary>
    /// Fades a scene from black. Usually called at the start of a scene.
    /// </summary>
    /// <param name="_blackoutImage"></param>
    /// <param name="_speed"></param>
    /// <returns></returns>
    public static IEnumerator FadeFromBlack(Image _blackoutImage, int _speed)
    {
        Color _imageColour = _blackoutImage.color;
        float _fadeAmount;

        while (_blackoutImage.color.a > 0)
        {
            _fadeAmount = _imageColour.a - (_speed * Time.deltaTime);

            _imageColour.a = _fadeAmount;
            _blackoutImage.color = _imageColour;

            yield return null;
        }
    }
}
