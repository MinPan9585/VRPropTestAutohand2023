using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    float lerpTime = 0;
    float scaleMultiplier;
    float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        scaleMultiplier = Random.Range(0.75f, 1.25f);
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpTime < 0.9f)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * scaleMultiplier, lerpTime + speed * Time.deltaTime);
            lerpTime = lerpTime + speed * Time.deltaTime;
        }
    }
}
