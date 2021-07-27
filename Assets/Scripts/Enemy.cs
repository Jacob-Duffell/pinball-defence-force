using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float impactForce;
    [SerializeField] private float speed;
    [SerializeField] private int targetWaypoint;
    [SerializeField] private GameObject waypoints;

    private Animator animator;
    private float currentHealth;
    private int maxHealth;
    private float score;
    private Slider healthBar;

	void Start()
    {
        animator = GetComponent<Animator>();
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        waypoints = GameObject.FindGameObjectWithTag("WavePattern");
    }

	void Update()
    {
        healthBar.value = (float)currentHealth / maxHealth;

        // Move enemy towards next waypoint.
        if (waypoints == null)
		{
            waypoints = GameObject.FindGameObjectWithTag("WavePattern");
		}
        else
		{
            if (Vector3.Distance(transform.position, waypoints.transform.GetChild(targetWaypoint).transform.position) <= 0.001f)
            {
                targetWaypoint++;

                if (targetWaypoint >= waypoints.transform.childCount)
                {
                    targetWaypoint = 0;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, waypoints.transform.GetChild(targetWaypoint).transform.position, speed * Time.deltaTime);
        }
    }

	private void OnCollisionEnter2D(Collision2D _collision)
	{
        // Damage or kill the enemy, and bounce the player off the enemy.
		if (_collision.gameObject.tag == "Player")
		{
            _collision.rigidbody.AddForce(-_collision.GetContact(0).normal * impactForce, ForceMode2D.Impulse);

            currentHealth -= GameHandler.DamageMultiplier;

            if (currentHealth <= 0)
            {
                GameController.Current.EnemyDeathSFX.Play();

                StartCoroutine("EnemyDeath", 0.75f);
            }
            else
			{
                GameController.Current.HitEnemySFX.Play();
                GameController.Current.AddToScore(Score / 2);
			}
        }
	}

    /// <summary>
    /// Increments user score and kills enemy when death animation is completed.
    /// </summary>
    /// <param name="_waitTime"></param>
    /// <returns></returns>
    private IEnumerator EnemyDeath(float _waitTime)
    {
        speed = 0;
        animator.SetBool("IsDead", true);

        yield return new WaitForSeconds(_waitTime);

        GameController.Current.AddToScore(score);
        GetComponent<PooledObject>().RecycleSelf();
    }

    public float Speed { get => speed; set => speed = value; }
	public GameObject Waypoints { get => waypoints; set => waypoints = value; }
	public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Score { get => score; set => score = value; }
    public int TargetWaypoint { get => targetWaypoint; set => targetWaypoint = value; }
}
