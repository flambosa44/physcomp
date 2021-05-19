using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterDetect : MonoBehaviour
{
    public GameObject canvasGood;
    public GameObject canvasBad;
    private GameObject activeCanvas = null;
    private ESP32 bluetooth;
    public GameObject animalHome;
    public GameObject enemyFish;
    public float detectionAnimateSpeed;
    private Animator animalAnimator;
    private Animation animalAnimation;
    private AudioSource[] audioSources;
    public int vibrateValue;
    private bool nearHome, detached, nearEnemyHome, canvasRunning;
    private bool initiated = false;
    private Transform homeTrans, animalTrans;
    private Vector3 localPos, localRot;
    // Start is called before the first frame update

    public bool NearEnemyHome
    {
        get { return this.nearEnemyHome; }
        set { this.nearEnemyHome = value; }
    }

    public bool Iniitiated
    {
        get { return this.initiated; }
        set { this.initiated = value; }
    }

    public void Initiate()
    {
        this.initiated = true;
        Start();
        bluetooth.SetCollisionDetected(true, vibrateValue);
        canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Try to match the " + this.gameObject.name + " with its symbiotic coral";
        StartCoroutine(CanvasHandler(canvasBad, 5.0f));
    }

    void Start()
    {
        audioSources = this.gameObject.GetComponents<AudioSource>();
        animalAnimator = this.gameObject.GetComponent<Animator>();
        animalAnimation = this.gameObject.GetComponent<Animation>();
        bluetooth = GetComponentInChildren<ESP32>();
        nearHome = false;
        detached = false;
        animalHome = null;
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
        bool success = Global.PossibleAnimalHomes.ContainsKey(other.gameObject.name);
        bluetooth.SetCollisionDetected(true, success ? 255 : 0);
        UpdateAnimation(success ? 0.5f : 2.0f);
        PlayAudio(success);
        //PosTest.UpdateStatus("Enter Trigger");
        if (success)
        {
            //canvasGood.SetActive(true);
            nearHome = true;
            animalHome = Global.PossibleAnimalHomes[other.gameObject.name];
            homeTrans = animalHome.transform;
            StartCoroutine("SettleIn");
            canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Good job! This Sea Anemone is safe to use, now place the " + this.gameObject.name + " inside";
        }
        else
        {
            GameObject enemyHome = enemyFish.GetComponent<EnterDetect>().animalHome;
            if (enemyHome != null && enemyHome.name == other.gameObject.name)
            {
                nearEnemyHome = true;
                enemyFish.GetComponent<EnterDetect>().Attack();
                canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Oh no, the butterflyfish can't go here. The clownfish is protecting the sea anemone!";
            }
            else
            {
                canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Oh no! This isn't a Sea Anemone. Try to place the " + this.gameObject.name + " in a Sea Anemone.";
                
            }
        }
        StartCoroutine(CanvasHandler(canvasBad, 5.0f));
    }

    public void Attack()
    {
        StartCoroutine("AttackCoroutine");
    }

    IEnumerator AttackCoroutine()
    {
        UpdateAnimation(2.0f);
        float speed = 0.1f;
        EnterDetect enemyScript = enemyFish.GetComponent<EnterDetect>();
        PosTest.UpdateStatus("Starting Attack, NearEnemyHome = " + enemyScript.NearEnemyHome);
        Vector3 pos, rot, enemyPos, targetRot;
        Vector3 originalPos, originalRot;
        bool finishedMove, finishedRot, finished;
        while (enemyScript.NearEnemyHome)
        {
            pos = this.gameObject.transform.position;
            rot = this.gameObject.transform.rotation.eulerAngles;
            enemyPos = enemyFish.transform.position;
            targetRot = enemyPos - pos;

            PosTest.vOrigin = this.gameObject.transform.parent.transform.TransformPoint(localPos);
            PosTest.rOrigin = this.gameObject.transform.parent.transform.TransformDirection(localRot);
            PosTest.vClown = pos;
            PosTest.rClown = rot;
            PosTest.vButt = enemyPos;
            PosTest.rButt = targetRot;
            PosTest.vDiff = targetRot;

            if (Vector3.Distance(pos, enemyPos) > 0.001f)
                this.gameObject.transform.position = Vector3.MoveTowards(pos, enemyPos, speed * Time.deltaTime);
            if (Vector3.Distance(rot, targetRot) > 0.001f)
                this.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rot, targetRot, speed * Time.deltaTime, 0.0f));
            yield return null;
        }

        UpdateAnimation(1.0f);
        do
        {
            originalPos = this.gameObject.transform.parent.transform.TransformPoint(localPos);
            originalRot = this.gameObject.transform.parent.transform.TransformDirection(localRot);
            pos = this.gameObject.transform.position;
            rot = this.gameObject.transform.rotation.eulerAngles;
            finishedMove = Vector3.Distance(originalPos, pos) <= 0.001f;
            finishedRot = Vector3.Distance(originalRot, rot) <= 0.001f;
            finished = finishedMove && finishedRot;

            PosTest.vOrigin = originalPos;
            PosTest.rOrigin = originalRot;
            PosTest.vClown = pos;
            PosTest.rClown = rot;
            PosTest.vButt = new Vector3();
            PosTest.rButt = new Vector3();
            PosTest.vDiff = originalPos - pos;

            if (!finishedMove)
                this.gameObject.transform.position = Vector3.MoveTowards(pos, originalPos, speed * Time.deltaTime);
            //if(!finishedRot)
            //    this.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rot, originalRot, speed * Time.deltaTime, 0.0f));
            yield return null;
        }
        while (!finishedMove);
        UpdateAnimation(0.5f);
        PosTest.UpdateStatus(this.gameObject.name + " Finished Attack");
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

            if (AreSimilar(deltaPos, tempDeltaPos, posTolerances) && AreSimilar(tempDeltaRot, deltaRot, rotTolerances))
            {
                Detach();
                break;
            }

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
        GameObject vuforiaTarget = this.gameObject.transform.parent.gameObject;
        this.gameObject.transform.SetParent(animalHome.transform);
        //vuforiaTarget.SetActive(false);
        Global.PossibleAnimalHomes.Remove(animalHome.name);
        localPos = this.gameObject.transform.localPosition;
        localRot = this.gameObject.transform.localRotation.eulerAngles;
        PosTest.UpdateStatus(this.gameObject.name + " Detached!");
        StartCoroutine(CanvasHandler(canvasGood, 20.0f));
    }

    void OnTriggerExit(Collider other)
    {

        if (detached || Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        //PosTest.UpdateStatus("Exit Trigger");
        bluetooth.SetCollisionDetected(false, vibrateValue);
        //canvasGood.SetActive(false);
        //canvasBad.SetActive(false);
        UpdateAnimation(1.0f);
        nearEnemyHome = false;
        if (nearHome)
        {
            nearHome = false;
            animalHome = null;
            homeTrans = null;
        }
    }

    IEnumerator CanvasHandler(GameObject activeCanvas, float canvasLifespan)
    {
        while (canvasRunning)
            yield return null;
        StartCoroutine(CanvasCoroutine(activeCanvas,canvasLifespan));
    }

    IEnumerator CanvasCoroutine(GameObject activeCanvas, float canvasLifespan)
    {
        canvasRunning = true;
        activeCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(canvasLifespan);
        activeCanvas.SetActive(false);
        canvasRunning = false;
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
