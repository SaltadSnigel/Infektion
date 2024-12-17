using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Reference to the player prefab
    public GameObject playerPrefab;

    // Number of players to spawn
    public int numberOfPlayers = 5;

    // Define the spawn area (X: -9 to 9, Y: -5 to 5)
    public Vector2 spawnAreaMin = new Vector2(-12, -12);
    public Vector2 spawnAreaMax = new Vector2(12, 12);

    void Start()
    {
        // Call the method to spawn the players when the game starts
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Random position within the spawn area
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

            // Create the spawn position
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Instantiate a player at the random spawn position
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
