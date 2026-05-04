using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
       transform.position += Vector3.right * 10f * Time.deltaTime;
    }
    
}