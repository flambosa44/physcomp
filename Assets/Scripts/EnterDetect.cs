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
    private Animation animalAnimation;
    private AudioSource[] audioSources;
    public int vibrateValue;
    private bool nearHome, detached;
    private Transform homeTrans, animalTrans;
    // Start is called before the first frame update
    void Start()
    {
        audioSources = this.gameObject.GetComponents<AudioSource>();
        animalAnimator = this.gameObject.GetComponent<Animator>();
        animalAnimation = this.gameObject.GetComponent<Animation>();
        bluetooth = GetComponentInChildren<ESP32>();
        nearHome = false;
        detached = false;
        homeTrans = animalHome.transform;
        animalTrans = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (detached || Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        bool success = other.gameObject.name == animalHome.name;
        bluetooth.GetComponent<ESP32>().SetCollisionDetected(true, success ? 255 : 0);
        UpdateAnimation(success ? 0.5f : 2.0f);
        PlayAudio(other.gameObject.name == animalHome.name);
        PosTest.UpdateStatus("Enter Trigger");
        if (success)
        {
            //canvasGood.SetActive(true);
            nearHome = true;
            StartCoroutine("SettleIn");
        }
        else
        {
            //canvasBad.SetActive(true);
        }
    }

    IEnumerator SettleIn()
    {
        Vector3 animalPos = animalTrans.position;
        Vector3 animalRot = animalTrans.rotation.eulerAngles;
        Vector3 homePos = homeTrans.position;
        Vector3 homeRot = homeTrans.rotation.eulerAngles;
        Vector3 deltaPos = animalPos - homePos;
        Vector3 deltaRot = animalRot - homeRot;
        Vector3 tempDeltaPos, tempDeltaRot;
        float[] posTolerances = { 0.07f, 0.07f, 0.07f };
        float[] rotTolerances = { 3f, 3f, 3f };
        while (nearHome)
        {
            yield return new WaitForSecondsRealtime(3f);
            if (!nearHome)
                break;
            animalPos = animalTrans.position;
            homePos = homeTrans.position;
            animalRot = animalTrans.rotation.eulerAngles;
            homeRot = homeTrans.rotation.eulerAngles;
            tempDeltaPos = animalPos - homePos;
            tempDeltaRot = animalRot - homeRot;
            PosTest.UpdateVectors(deltaPos, deltaRot);
            if (AreSimilar(deltaPos, tempDeltaPos, posTolerances) && AreSimilar(tempDeltaRot, deltaRot, rotTolerances))
                Detach();

            else
            {
                deltaPos = animalPos - homePos;
                deltaRot = animalRot - homeRot;
            }
            yield return null;
        }
    }

    private bool AreSimilar(Vector3 vec1, Vector3 vec2, float[] tolerances)
    {
        return Mathf.Abs(vec1.x - vec2.x) <= tolerances[0]
            && Mathf.Abs(vec1.y - vec2.y) <= tolerances[1]
            && Mathf.Abs(vec1.z - vec2.z) <= tolerances[2];

    }

    private void Detach()
    {
        this.detached = true;
        this.gameObject.transform.SetParent(null);
        PosTest.UpdateStatus("Clownfish Detached!");
    }

    void OnTriggerExit(Collider other)
    {

        if (detached || Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        PosTest.UpdateStatus("Exit Trigger");
        bluetooth.GetComponent<ESP32>().SetCollisionDetected(false, vibrateValue);
        //canvasGood.SetActive(false);
        //canvasBad.SetActive(false);
        UpdateAnimation(1.0f);
        if (nearHome)
            nearHome = false;
    }

    void PlayAudio(bool success)
    {
        foreach (AudioSource source in audioSources)
            source.Stop();
        audioSources[success ? 0 : 1].Play();
    }

    void UpdateAnimation(float speed)
    {
        if (animalAnimator != null)
            animalAnimator.speed = speed;
        else if (animalAnimation != null)
            animalAnimation[animalAnimation.clip.name].speed = speed;
    }
}
