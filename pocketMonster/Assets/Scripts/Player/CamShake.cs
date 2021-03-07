using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude, float delay)
    {
        Vector3 originalPos = transform.position;

        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(ActualShake(duration, magnitude, originalPos));
    }

    public IEnumerator ActualShake(float duration, float magnitude, Vector3 pos)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(pos.x + x, (pos.y + y), pos.z + z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.position = pos;
    }
}