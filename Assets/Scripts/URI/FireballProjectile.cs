using System;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private object fireballPrefab;

    public float speed = 10f;

void Update()
{
    transform.Translate(Vector2.right * speed * Time.deltaTime);
}
    private object Instantiate(object fireballPrefab, Vector3 spawnPos, Quaternion rotation)
    {
        throw new NotImplementedException();
    }
}