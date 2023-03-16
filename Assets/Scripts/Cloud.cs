using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    float lerpTime = 0;
    float scaleMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        scaleMultiplier = Random.Range(0.6f, 0.9f);
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpTime < 0.9f)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * scaleMultiplier, lerpTime + Time.deltaTime);
            lerpTime = lerpTime + Time.deltaTime;
        }
        
    }
}
