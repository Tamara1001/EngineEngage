using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin: Player touched coin.");
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(10);
                Debug.Log("Coin: Added 10 score.");
            }
            else
            {
                Debug.LogError("Coin: GameManager not found!");
            }
            Destroy(gameObject);
        }
    }
}
