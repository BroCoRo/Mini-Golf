using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //deltaTime helps actions happen smoothly
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime); //Rotate around a single plane
    }
}