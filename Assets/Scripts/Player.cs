using UnityEngine;

public class Player : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D _collision)
	{
		// Player loses a life if they collide with the spikes at the bottom of the screen.
		if (_collision.tag == "Spikes")
		{
			GameController.Current.PlayerDeath();
		}
	}
}
