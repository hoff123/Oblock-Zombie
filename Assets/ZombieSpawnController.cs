using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 2;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; // Delay between each zombie in a wave

    public int currentWave = 0;
    public float waveCooldown = 10.0f; // Time in seconds between waves

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;

    public TextMeshProUGUI currentWaveUI;
    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;
        currentZombiesAlive = new List<Enemy>(); // Initialize the list

        GlobalReferences.Instance.waveNumber = currentWave;

        StartNextWave();
    }

    private void StartNextWave()
    {

        currentZombiesAlive.Clear();

        currentWave++;

        GlobalReferences.Instance.waveNumber = currentWave;

        currentWaveUI.text = "Wave: " + currentWave.ToString();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            // Generate a random offset with a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the Zombie
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            // Get enemy script
            Enemy enemyScript = zombie.GetComponent<Enemy>();

            // Track this zombie
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        // Get all dead zombies
        List<Enemy> zombiesToRemove = new List<Enemy>();
        foreach (Enemy zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        // Remove all dead zombies
        foreach (Enemy zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        // Start cooldown if all zombies are dead
        if (currentZombiesAlive.Count == 0 && !inCooldown)
        {
            // Start the cooldown for the next wave
            StartCoroutine(WaveCooldown());
        }

        // Run the cooldown counter
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
            if (cooldownCounter <= 0)
            {
                cooldownCounter = 0;
                inCooldown = false;
                waveOverUI.gameObject.SetActive(false);
                currentZombiesPerWave *= 2;
                StartNextWave();
            }
        }
        else
        {
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = Mathf.Ceil(cooldownCounter).ToString();
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);
    }
}
