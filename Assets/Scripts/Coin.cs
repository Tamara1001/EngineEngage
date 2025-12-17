using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreAmount = 10;

    private void Start()
    {
        // Scale based on score: 10 -> 1.0, 5 -> 0.5, 20 -> 2.0
        float scaleFactor = scoreAmount / 10f;
        transform.localScale = Vector3.one * scaleFactor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin: Player touched coin.");
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(scoreAmount);
                Debug.Log($"Coin: Added {scoreAmount} score.");
            }
            else
            {
                Debug.LogError("Coin: GameManager not found!");
            }
            Destroy(gameObject);
        }
    }
}
