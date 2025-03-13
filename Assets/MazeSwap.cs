using UnityEngine;

public class MazeSwap : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to swap between
    private GameObject player; // The player game object
    private BoxCollider boxCollider; // The BoxCollider of the current prefab
    private bool hasSwapped = false; // Flag to ensure the swap happens only once
    private bool playerInside = true; // Flag to track if player is inside the collider
    private float cooldownTime = 2.0f; // Cooldown period to prevent constant swapping
    private float lastSwapTime = 0f; // Timestamp of the last swap

    void Start()
    {
        // Initialize the BoxCollider and player variables
        boxCollider = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Check if the player variable is assigned
        if (player == null)
        {
            Debug.LogError("Player GameObject is not assigned in the Inspector");
        }

        // Check if the prefabs array is not empty
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("Prefabs array is empty or not assigned");
        }
    }

    void Update()
    {
        // Ensure the player variable is assigned and prefabs array is not empty before proceeding
        if (player != null && prefabs != null && prefabs.Length > 0)
        {
            // Check if the player is outside the bounds of the collider and hasn't swapped yet
            if (!boxCollider.bounds.Contains(player.transform.position) && playerInside)
            {
                if (Time.time - lastSwapTime > cooldownTime)
                {
                    SwapPrefab();
                    hasSwapped = true; // Set the flag to true after the swap
                    playerInside = false; // Mark the player as outside the collider
                    lastSwapTime = Time.time; // Update the last swap time
                }
            }
        }
    }

    void SwapPrefab()
    {
        // Select a random prefab from the array
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject newPrefab = prefabs[randomIndex];

        // Instantiate the new prefab at the same position and rotation
        Instantiate(newPrefab, transform.position, transform.rotation);

        // Destroy the current prefab
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Reset the swap flag when the player re-enters the collider
        if (other.gameObject == player)
        {
            hasSwapped = false;
            playerInside = true; // Mark the player as inside the collider
        }
    }
}
