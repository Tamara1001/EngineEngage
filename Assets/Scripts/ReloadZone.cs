using UnityEngine;

public class ReloadZone : MonoBehaviour
{
    [SerializeField] Transform player;
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("ReloadZone");
            collision.gameObject.GetComponent<PlayerManager>().ReloadZone();
        }
    }
}
