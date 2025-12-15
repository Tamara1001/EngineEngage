using UnityEngine;

public class MyWeapon : MonoBehaviour
{
    [SerializeField] private GameObject SFX_Flash;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float bulletSpeed = 500f;
    [SerializeField] float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject newVFX = Instantiate(SFX_Flash, spawnPoint.position, spawnPoint.rotation);
            GameObject newBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
            bulletRigidbody.AddForce(spawnPoint.forward * bulletSpeed);
            nextFireTime = Time.time + 1f / fireRate;
            Destroy(newVFX, 0.3f);
            Destroy(newBullet, 2f);
        }
    }
}
