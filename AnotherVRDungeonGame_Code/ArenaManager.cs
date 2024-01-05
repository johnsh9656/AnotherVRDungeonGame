using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    // rounds
    [SerializeField] int round = 0;
    [SerializeField] float timeBetweenRounds = 5;
    [SerializeField] GameObject roundCanvas;
    [SerializeField] private Image roundImage;
    [SerializeField] private Color roundImageColour;
    [SerializeField] private TMP_Text roundText;
    private bool roundUIActive;

    // score
    [SerializeField] int score = 0;
    [SerializeField] private TMP_Text scoreText;

    // spawn
    [SerializeField] List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] List<SpawnPoint> availableSpawns = new List<SpawnPoint>();
    [SerializeField] GameObject[] spawnFX;

    // enemies
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] List<EnemyHealth> enemiesRemaining = new List<EnemyHealth>();
    private int enemiesToSpawn;
    private int enemiesSpawned;
    private int roundEnemiesKilled;

    // pickups
    [SerializeField] GameObject[] pickups;
    [SerializeField] Transform[] pickupLocations;
    List<Transform> availablePickupLocations = new List<Transform>();
    [SerializeField] int maxPickups = 3;

    private void Start()
    {
        roundCanvas.SetActive(false);
        StartCoroutine(StartNextRoundDelayed());

        for (int i = 0; i < pickupLocations.Length; i++)
        {
            availablePickupLocations.Add(pickupLocations[i]);
        }
    }

    private void Update()
    {
        if (roundUIActive && roundImage.color != roundImageColour)
        {
            roundImage.color += new Color(0, 0, 0, 1 * Time.deltaTime);
            if (roundImage.color.a >= .65)
            {
                roundImage.color = roundImageColour;
                roundUIActive = false;
                Invoke(nameof(DisableRoundUI), 4f);
            }
        } 
    }

    private void DisableRoundUI()
    {
        roundCanvas.SetActive(false);
    }

    private void StartNextRound()
    {
        round++;

        // activate round UI
        roundText.text = "Round " + round;
        roundCanvas.SetActive(true);
        roundUIActive = true;
        roundImage.color = new Color(0, 0, 0, 0);

        // spawn enemies
        enemiesToSpawn = round + Random.Range(0, 8);
        if (enemiesToSpawn > spawnPoints.Count) enemiesToSpawn = spawnPoints.Count;
        else if (enemiesToSpawn < 3) enemiesToSpawn = 3;

        if (round <= 3 && enemiesToSpawn > 7) enemiesToSpawn = 7;

        // get available spawns
        availableSpawns.Clear();
        int limit;
        if (round <= 3) { limit = 7; }
        else limit = spawnPoints.Count;
        
        for (int i = 0; i < limit; i++)
        {
            availableSpawns.Add(spawnPoints[i]);
        }

        enemiesSpawned = 0;
        // spawn enemies
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            //float waitToSpawn = Random.Range(0, 5f);
            //Invoke(nameof(SpawnEnemy), waitToSpawn);
        }

        // spawn pickups
        int numPickups = (int)(round * Random.Range(0, .25f));
        if (numPickups > maxPickups) numPickups = maxPickups;

        for (int i = 0; i < numPickups; i++)
        {
            Transform location = availablePickupLocations[Random.Range(0, availablePickupLocations.Count)];
            GameObject newPickup = Instantiate
                (pickups[Random.Range(0, pickups.Length)], location.position, location.rotation, location);

            availablePickupLocations.Remove(location);
        }
    }

    private void SpawnEnemy()
    {
        // choose spawnpoint
        SpawnPoint chosenSpawn = availableSpawns[(Random.Range(0, availableSpawns.Count))];
        availableSpawns.Remove(chosenSpawn);

        // instantiate enemy at spawnpoint
        int enemyIndex = ChooseNextEnemy();
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], chosenSpawn.transform.position, chosenSpawn.transform.rotation);
        enemiesRemaining.Add(enemy.GetComponent<EnemyHealth>());
        enemiesSpawned++;
        // vfx
        GameObject fxPrefab = spawnFX[Random.Range(0, spawnFX.Length)];
        Vector3 pos = new Vector3
            (chosenSpawn.transform.position.x, 
            chosenSpawn.transform.position.y + 0.25f, 
            chosenSpawn.transform.position.z);
        GameObject fx = Instantiate(fxPrefab, chosenSpawn.transform.position, Quaternion.identity);
        Destroy(fx, 4);
    }

    private int ChooseNextEnemy()
    {
        int enemyIndex;

        if (round == 1) enemyIndex = 0;
        else if (round < 4) { enemyIndex = Random.Range(0, 2); }
        else if (round < 6) { enemyIndex = Random.Range(0, 3); }
        else { enemyIndex = Random.Range(0, 4); }

        return enemyIndex;
    }

    public void OnEnemyKilled(EnemyHealth enemy, int enemyScore)
    {
        score += enemyScore;
        enemiesRemaining.Remove(enemy);

        if (enemiesRemaining.Count == 0 && enemiesToSpawn == enemiesSpawned)
        {
            StartCoroutine(StartNextRoundDelayed());
        }
    }
    
    public void OnPickup(Transform pickupSpawn)
    {
        availablePickupLocations.Add(pickupSpawn);
    }

    private IEnumerator StartNextRoundDelayed()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        StartNextRound();
    }

    public int GetScore()
    {
        return score;
    }
}
