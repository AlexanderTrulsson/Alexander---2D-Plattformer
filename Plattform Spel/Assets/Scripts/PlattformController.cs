using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float riseDistance = 5f;
    public float riseTime = 2f;
    public float delayBeforeRise = 0.5f;  // Delay before starting the rise

    public void RisePlatform()
    {
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        // Wait for 0.5 seconds before beginning the movement
        yield return new WaitForSeconds(delayBeforeRise);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * riseDistance;
        float elapsedTime = 0f;

        while (elapsedTime < riseTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / riseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }
}

