using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    float spawnTime = 0f;
    float cooldown = 0.2f;
    public GameObject cloudPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnTime)
        {
            Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            Instantiate(cloudPrefab, transform.position, Quaternion.Euler(rotation));
            spawnTime = Time.time + cooldown;
        }
    }
}
