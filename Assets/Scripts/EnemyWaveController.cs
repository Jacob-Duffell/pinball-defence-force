using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour
{
    [SerializeField] private List<GameObject> wavePatterns;
    [SerializeField] private List<string> enemyTypes;

    private int currentWaveNumber;
    private ObjectPooler objectPooler;
    private Wave currentWave;

	private void Start()
	{
        objectPooler = GetComponent<ObjectPooler>();
        currentWaveNumber = 1;
        currentWave = CreateNewEnemyWave();
        BeginNewWave();
	}

	private void Update()
	{
        if (CheckAllEnemiesKilled())
		{
            Destroy(GameObject.FindGameObjectWithTag("WavePattern"));
            currentWaveNumber++;
            currentWave = CreateNewEnemyWave();
            BeginNewWave();
		}
	}

    /// <summary>
    /// Initialises enemies and waypoint pattern for new wave.
    /// </summary>
	private void BeginNewWave()
	{
        GameObject _currentWaypointPattern = Instantiate(currentWave.WavePattern);

        for (int i = 0; i < currentWave.EnemyCount; i++)
		{
            currentWave.Enemies.Add(objectPooler.GetPooledObject(enemyTypes[Random.Range(0, enemyTypes.Count)]));

            currentWave.Enemies[i].GetComponent<Enemy>().transform.position = currentWave.WavePattern.transform.GetChild(i).transform.position;
            currentWave.Enemies[i].GetComponent<Enemy>().MaxHealth = currentWave.EnemyMaxHealth;
            currentWave.Enemies[i].GetComponent<Enemy>().CurrentHealth = currentWave.EnemyMaxHealth;
            currentWave.Enemies[i].GetComponent<Enemy>().Score = currentWaveNumber * 10 * GameHandler.PointsMultiplier;
            currentWave.Enemies[i].GetComponent<Enemy>().Speed = currentWave.EnemySpeed / 3f;
            currentWave.Enemies[i].GetComponent<Enemy>().Waypoints = _currentWaypointPattern;
            currentWave.Enemies[i].GetComponent<Enemy>().TargetWaypoint = i;
            currentWave.Enemies[i].SetActive(true);
        }
	}

    /// <summary>
    /// Creates a new Wave object to be used when initialising the new wave.
    /// </summary>
    /// <returns></returns>
	private Wave CreateNewEnemyWave()
	{
        Wave _newWave = new Wave();

        _newWave.WavePattern = wavePatterns[Random.Range(0, wavePatterns.Count)];
        _newWave.EnemySpeed = currentWaveNumber;
        _newWave.EnemyMaxHealth = currentWaveNumber;
        _newWave.EnemyCount = _newWave.WavePattern.transform.childCount;

        return _newWave;
	}

    /// <summary>
    /// Checks if all enemies in the current wave have been killed.
    /// </summary>
    /// <returns></returns>
    private bool CheckAllEnemiesKilled()
	{
        bool _enemyInactive = true;

        for (int i = 0; i < currentWave.EnemyCount; i++)
        {
            if (currentWave.Enemies[i].activeSelf)
            {
                _enemyInactive = false;
            }
        }

        return _enemyInactive;
    }

    public int CurrentWaveNumber { get => currentWaveNumber; set => currentWaveNumber = value; }
}
