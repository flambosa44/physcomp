using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDetect : MonoBehaviour
{
    public GameObject canvasGood;
    public GameObject canvasBad;
    private ESP32 bluetooth;
    public GameObject animalHome;
    public float detectionAnimateSpeed;
    private Animator animalAnimator;
    private AudioSource[] audioSources;
    public int vibrateValue;
    // Start is called before the first frame update
    void Start()
    {
        audioSources = this.gameObject.GetComponents<AudioSource>();
        animalAnimator = this.gameObject.GetComponent<Animator>();
        bluetooth = GetComponentInChildren<ESP32>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        bool success = other.gameObject.name == animalHome.name;
        bluetooth.GetComponent<ESP32>().SetCollisionDetected(true, success ? 255 : 0);
        animalAnimator.speed = success ? 0.5f : 2.0f;
        PlayAudio(other.gameObject.name == animalHome.name);
        if(success)
            canvasGood.SetActive(true);
        else
            canvasBad.SetActive(true);

    }

    void OnTriggerExit(Collider other)
    {
        if (Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        bluetooth.GetComponent<ESP32>().SetCollisionDetected(false, vibrateValue);
        canvasGood.SetActive(false);
        canvasBad.SetActive(false);
        animalAnimator.speed = 1.0f;
    }

    void PlayAudio(bool success)
    {
        foreach (AudioSource source in audioSources)
            source.Stop();
        audioSources[success ? 0 : 1].Play();
    }
}
