using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    public float speed = -360;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
