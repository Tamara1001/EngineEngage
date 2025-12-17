using System.Collections;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float speed = 5f;

    public float distanceBetweenStations = 45f;

    public void MoveToNextStation()
    {
        Vector3 targetPosition = transform.position + new Vector3(distanceBetweenStations, 0, 0);
        StartCoroutine(MoveRoutine(targetPosition));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos; // Ensure exact finish
    }
}
