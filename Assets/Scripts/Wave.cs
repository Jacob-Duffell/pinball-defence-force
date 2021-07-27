using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    private GameObject wavePattern;
	private List<GameObject> enemies = new List<GameObject>();
	private float enemySpeed;
    private int enemyMaxHealth;
    private int enemyCount;

	public GameObject WavePattern { get => wavePattern; set => wavePattern = value; }
	public List<GameObject> Enemies { get => enemies; set => enemies = value; }
	public float EnemySpeed { get => enemySpeed; set => enemySpeed = value; }
	public int EnemyMaxHealth { get => enemyMaxHealth; set => enemyMaxHealth = value; }
	public int EnemyCount { get => enemyCount; set => enemyCount = value; }
}
