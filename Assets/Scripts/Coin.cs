using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreAmount = 10;
    
    public float rotationSpeed = 100f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;

    private Vector3 startPos;

    private void Start()
    {
        float scaleFactor = (scoreAmount / 10f) * 20f;
        transform.localScale = Vector3.one * scaleFactor;

        float liftAmount = scaleFactor * 0.1f;
        transform.position += Vector3.up * liftAmount;

        startPos = transform.position;
    }

    private void Update()
    {
        // Rotate
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Float
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCoin();

            if (gameManager != null)
            {
                gameManager.AddScore(scoreAmount);
            }
            else
            {
            }
            Destroy(gameObject);
        }
    }
}
