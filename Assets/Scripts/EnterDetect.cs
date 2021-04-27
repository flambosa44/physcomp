using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDetect : MonoBehaviour
{
    public GameObject canvas;
    public GameObject bluetooth;
    public GameObject clownfish;
    public float detectionAnimateSpeed;
    private Animator clownfishAnimator;
    private AudioSource audioSource;
    public int vibrateValue;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        clownfishAnimator = clownfish.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == clownfish.name)
        {
            bluetooth.GetComponent<ESP32_Hub>().SetCollisionDetected(true, vibrateValue);
            canvas.SetActive(true);
            clownfishAnimator.speed = detectionAnimateSpeed;
            audioSource.Play();
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == clownfish.name)
        {
            bluetooth.GetComponent<ESP32_Hub>().SetCollisionDetected(false, vibrateValue);
            canvas.SetActive(false);
            clownfishAnimator.speed = 1.0f;
        }
    }
}
