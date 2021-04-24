using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDetect : MonoBehaviour
{
    public GameObject canvas;
    public GameObject bluetooth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ClownFish")
        {
            canvas.SetActive(true);
            bluetooth.GetComponent<ESP32_Hub>().SetCollisionDetected(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ClownFish")
        {
            canvas.SetActive(false);
            bluetooth.GetComponent<ESP32_Hub>().SetCollisionDetected(false);
        }
    }
}
