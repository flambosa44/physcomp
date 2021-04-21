using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollDetect : MonoBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name == "ClownFish")
        canvas.SetActive(true);
        bluetooth.GetComponent<ESP32_Hub>().SetCollisionDetected(true);
    }
}
