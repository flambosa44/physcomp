using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterDetect : MonoBehaviour
{
    public GameObject canvasGood;
    public AudioClip audioRightHome;
    public GameObject canvasBad;
    public AudioClip audioBegin;
    public AudioClip audioSettled;
    public AudioClip audioWrongHome;
    public AudioClip audioEnemyPresent;
    private Feedback Begin, RightHome, WrongHome, Settled, EnemyPresent;
    private List<Feedback> AllFeedback;
    public GameObject touchpool;
    private Water waterScript;
    private Water.IxGuides guide;
    private GameObject activeCanvas = null;
    private ESP32 bluetooth;
    public GameObject animalHome;
    public GameObject enemyFish;
    public float detectionAnimateSpeed;
    private Animator animalAnimator;
    private Animation animalAnimation;
    private AudioSource audioSource;
    public int vibrateValue;
    private bool nearHome, detached, nearEnemyHome, canvasRunning;
    private bool initiated = false;
    private Transform homeTrans, animalTrans;
    private Vector3 localPos, originalScale;
    private Quaternion localRot;
    public static float calmAnimSpeed = 0.75f;
    private static bool clownfishGhostEvent = false;
    private bool continueTrimodal = false;
    private float transitionRatio;
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
        //StartCoroutine(CanvasHandler(canvasBad, 5.0f));
        UpdateFeedback(Begin);
    }

    void Start()
    {
        transitionRatio = continueTrimodal ? 0.5f : 0.1f;
        waterScript = touchpool.GetComponentInChildren<Water>();
        guide = waterScript.guide;
        audioSource = this.gameObject.GetComponent<AudioSource>();
        animalAnimator = this.gameObject.GetComponent<Animator>();
        animalAnimation = this.gameObject.GetComponent<Animation>();
        bluetooth = GetComponentInChildren<ESP32>();
        nearHome = false;
        detached = false;
        animalHome = null;
        animalTrans = this.gameObject.transform;
        originalScale = animalTrans.localScale;
        Begin = new Feedback(canvasBad, audioBegin, "Try to match the " + this.gameObject.name + " with its symbiotic coral", true, 5.0f, audioSource);
        RightHome = new Feedback(canvasBad, audioRightHome, "Good job! This Sea Anemone is safe to use, now place the " + this.gameObject.name + " inside", true, 5.0f, audioSource);
        WrongHome = new Feedback(canvasBad, audioWrongHome, "Oh no! This isn't a Sea Anemone. Try to place the " + this.gameObject.name + " in a Sea Anemone.", true, 5.0f, audioSource);
        Settled = new Feedback(canvasGood, audioSettled, null, false, 10.0f, audioSource);
        EnemyPresent = new Feedback(canvasBad, audioEnemyPresent, "Oh no, the butterflyfish can't go here. The clownfish is protecting the sea anemone!", true, 5.0f, audioSource);
        AllFeedback = new List<Feedback>() { Begin, RightHome, WrongHome, Settled, EnemyPresent };

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{this.gameObject.name} buzz = {bluetooth.BuzzValue}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (detached || Global.AnimalNonColliders.Contains(other.gameObject.name))
            return;
        //waterScript.audioPaused = true;

        Water.Effect effect = waterScript.effectGOs[other.gameObject];


        bool success = Global.PossibleAnimalHomes.ContainsKey(other.gameObject.name);
        bluetooth.SetCollisionDetected(true, success ? 255 : 0);
        UpdateAnimation(success ? calmAnimSpeed : 2.0f);
        //PosTest.UpdateStatus("Enter Trigger");
        if (success)
        {
            //canvasGood.SetActive(true);
            nearHome = true;
            animalHome = Global.PossibleAnimalHomes[other.gameObject.name];
            homeTrans = animalHome.transform;
            UpdateFeedback(RightHome, effect.hasFired);
            StartCoroutine("SettleIn");
            //canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Good job! This Sea Anemone is safe to use, now place the " + this.gameObject.name + " inside";
        }
        else
        {
            GameObject enemyHome = enemyFish.GetComponent<EnterDetect>().animalHome;
            if (enemyHome != null && enemyHome.name == other.gameObject.name)
            {
                nearEnemyHome = true;
                enemyFish.GetComponent<EnterDetect>().Attack();
                UpdateFeedback(EnemyPresent, effect.hasFired);
                //canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Oh no, the butterflyfish can't go here. The clownfish is protecting the sea anemone!";
            }
            else
            {
                //Debug.Log("WRONG HOME = " + other.gameObject.name);
                UpdateFeedback(WrongHome, effect.hasFired);
                //canvasBad.GetComponentInChildren<TextMeshProUGUI>().text = "Oh no! This isn't a Sea Anemone. Try to place the " + this.gameObject.name + " in a Sea Anemone.";

            }
        }
        //StartCoroutine(CanvasHandler(canvasBad, 5.0f));
        if (!effect.hasFired)
            effect.hasFired = true;
    }

    public void Attack()
    {
        StartCoroutine("AttackCoroutine");
    }

    IEnumerator AttackCoroutine()
    {
        UpdateAnimation(4.0f);
        float moveSpeed = 0.5f, rotateSpeed = 100;
        EnterDetect enemyScript = enemyFish.GetComponent<EnterDetect>();
        PosTest.UpdateStatus("Starting Attack, NearEnemyHome = " + enemyScript.NearEnemyHome);
        Vector3 pos, enemyPos, originalPos, targetRot;
        Quaternion rot, originalRot;
        bool finishedMove, finishedRot, finishedReturn, finishedSettle = false, finished;
        while (enemyScript.NearEnemyHome)
        {
            pos = this.gameObject.transform.position;
            rot = this.gameObject.transform.rotation;
            enemyPos = enemyFish.transform.position;
            targetRot = (enemyPos - pos).normalized;

            PosTest.vOrigin = this.gameObject.transform.parent.transform.TransformPoint(localPos);
            PosTest.rOrigin = (this.gameObject.transform.parent.rotation * localRot).eulerAngles;
            PosTest.vClown = pos;
            PosTest.rClown = rot.eulerAngles;
            PosTest.vButt = enemyPos;
            PosTest.rButt = targetRot;
            PosTest.vDiff = targetRot;
            PosTest.rDiff = targetRot;

            if (Vector3.Distance(pos, enemyPos) > 0.01f)
                this.gameObject.transform.position = Vector3.MoveTowards(pos, enemyPos, moveSpeed * Time.deltaTime);
            //if (Vector3.Distance(rot.eulerAngles, targetRot) > 1.0f && Vector3.Distance(pos, enemyPos) > 0.1f)
            this.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rot.eulerAngles, targetRot, rotateSpeed * Time.deltaTime, 2.0f));
            yield return null;
        }

        UpdateAnimation(1.0f);
        do
        {
            originalPos = this.gameObject.transform.parent.transform.TransformPoint(localPos);
            originalRot = this.gameObject.transform.parent.rotation * localRot;
            pos = this.gameObject.transform.position;
            rot = this.gameObject.transform.rotation;
            finishedMove = Vector3.Distance(originalPos, pos) <= 0.01f;
            targetRot = (originalPos - pos).normalized;
            finishedRot = Vector3.Distance(targetRot, rot.eulerAngles) <= 1.0f || Vector3.Distance(originalPos, pos) <= 0.1f;
            finishedSettle = finishedMove && Vector3.Distance(originalRot.eulerAngles, rot.eulerAngles) <= 1f;

            PosTest.vOrigin = originalPos;
            PosTest.rOrigin = originalRot.eulerAngles;
            PosTest.vClown = pos;
            PosTest.rClown = rot.eulerAngles;
            PosTest.vButt = new Vector3();
            PosTest.rButt = targetRot;
            PosTest.vDiff = originalPos - pos;

            if (!finishedMove)
            {
                this.gameObject.transform.position = Vector3.MoveTowards(pos, originalPos, moveSpeed * Time.deltaTime);
                if (!finishedRot)
                    this.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rot.eulerAngles, targetRot, rotateSpeed * Time.deltaTime, 2.0f));
                PosTest.UpdateStatus("finishedMove = " + finishedMove + "\nfinishedRot = " + finishedRot);
            }
            else if (finishedMove && !finishedSettle)
            {
                PosTest.UpdateStatus("In Settle");
                //this.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(rot, originalRot, 10 * Time.deltaTime, 3f));
                //this.gameObject.transform.rotation = Quaternion.RotateTowards(this.gameObject.transform.rotation, (this.gameObject.transform.parent.rotation * localRot), 10 * Time.deltaTime);
                this.gameObject.transform.rotation = Quaternion.RotateTowards(rot, originalRot, 50 * Time.deltaTime);
                finishedSettle = Vector3.Distance(originalRot.eulerAngles, rot.eulerAngles) <= 1f;
                if (finishedSettle)
                    break;
            }
            ////else if (finishedMove)
            ////{
            ////    this.gameObject.transform.rotation = (this.gameObject.transform.parent.rotation * localRot);
            ////    finishedSettle = true;
            ////    break;
            ////}
                

            yield return null;
        }
        while (!finishedSettle);
        UpdateAnimation(transitionRatio);
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
        float maxDistance = deltaPos.magnitude, time = 0, currentDistance;
        float[] posTolerances = { 0.02f, 0.02f, 0.02f };
        float[] rotTolerances = { 3f, 3f, 3f };
        bool notDetached = true;
        GameObject effect = homeTrans.Find("Floating_Particles").gameObject;

        while (nearHome && notDetached)
        {
            //yield return new WaitForSecondsRealtime(3f);
            //if (!nearHome)
            //    break;

            float settleDistance = 0.07f, ratio, restDuration = 3.0f;
            animalPos = animalTrans.position;
            homePos = homeTrans.position;
            animalRot = animalTrans.rotation.eulerAngles;
            homeRot = homeTrans.rotation.eulerAngles;
            tempDeltaPos = animalPos - homePos;
            tempDeltaRot = animalRot - homeRot;
            currentDistance = tempDeltaPos.magnitude;

            switch(guide)
            {
                case Water.IxGuides.ghost:
                    //UpdateAnimation(currentDistance * calmAnimSpeed / maxDistance);
                    if (!clownfishGhostEvent && !waterScript.clownfishGhost.activeSelf)
                    {
                        waterScript.clownfishGhost.SetActive(true);
                        clownfishGhostEvent = true;
                    }
                    ratio = FishSwim.Lerp(0, maxDistance, transitionRatio, 1, currentDistance);
                    UpdateAnimation(calmAnimSpeed * ratio);
                    bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * ratio);
                    break;
                case Water.IxGuides.trimodal:
                    ratio = FishSwim.Lerp(0,maxDistance, transitionRatio, 1,currentDistance);
                    UpdateAnimation(calmAnimSpeed*ratio);
                    bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * ratio);
                    //Debug.Log($"Outside settle, dist = {currentDistance} | buzz = {bluetooth.BuzzValue}");
                    break;
                case Water.IxGuides.enviro:
                    break;
                default:
                    break;
            }
            
            while(currentDistance <= settleDistance)
            {
                if (!effect.activeSelf)
                    effect.SetActive(true);

                time += Time.deltaTime;
                if(time >= restDuration)
                {
                    Detach();
                    notDetached = false;
                    break;
                }
                else
                {
                    deltaPos = animalTrans.position - homeTrans.position;
                    deltaRot = animalTrans.rotation.eulerAngles - homeTrans.rotation.eulerAngles;

                    if (AreSimilar(deltaPos, tempDeltaPos, posTolerances) && AreSimilar(tempDeltaRot, deltaRot, rotTolerances))
                    {
                        switch (guide)
                        {
                            case Water.IxGuides.ghost:
                                //animalTrans.localScale = originalScale * FishSwim.Lerp(0, restDuration, 1, 1.5f, time);
                                Water.SetEffectAlpha(effect.GetComponent<ParticleSystem>(), FishSwim.Lerp(0, 3, 0.3f, 1, time));
                                if (continueTrimodal)
                                {
                                    ratio = FishSwim.Lerp(0, restDuration, transitionRatio, 0, time);
                                    UpdateAnimation(calmAnimSpeed * ratio < 0.1f ? 0.1f : calmAnimSpeed * ratio);
                                    bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * ratio);
                                }
                                break;
                            case Water.IxGuides.trimodal:
                                ratio = FishSwim.Lerp(0, restDuration, transitionRatio, 0, time);
                                UpdateAnimation(calmAnimSpeed * ratio < 0.1f ? 0.1f : calmAnimSpeed * ratio);
                                bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * ratio);
                                //Debug.Log($"Inside settle, dist = {currentDistance} | buzz = {bluetooth.BuzzValue}");
                                break;
                            case Water.IxGuides.enviro:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        tempDeltaPos = animalTrans.position - homeTrans.position;
                        tempDeltaRot = animalTrans.rotation.eulerAngles - homeTrans.rotation.eulerAngles;
                        time = 0;
                        switch (guide)
                        {
                            case Water.IxGuides.ghost:
                                //animalTrans.localScale = originalScale;
                                Water.SetEffectAlpha(effect.GetComponent<ParticleSystem>(), 0);
                                if(continueTrimodal)
                                {
                                    UpdateAnimation(calmAnimSpeed * transitionRatio);
                                    bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * transitionRatio);
                                }
                                break;
                            case Water.IxGuides.trimodal:
                                UpdateAnimation(calmAnimSpeed * transitionRatio);
                                bluetooth.BuzzValue = 255 - Mathf.RoundToInt(130.0f * transitionRatio);
                                break;
                            case Water.IxGuides.enviro:
                                break;
                            default:
                                break;
                        }
                    }
                }
                yield return null;
                currentDistance = (animalTrans.position - homeTrans.position).magnitude;
                if (!nearHome || currentDistance > settleDistance)
                {
                    switch (guide)
                    {
                        case Water.IxGuides.ghost:
                            animalTrans.localScale = originalScale;
                            effect.SetActive(false);
                            break;
                        case Water.IxGuides.trimodal:
                            break;
                        case Water.IxGuides.enviro:
                            break;
                        default:
                            break;
                    }
                    break;
                }
                    
            }
            float animSpeed = animalAnimator == null ? animalAnimation[animalAnimation.clip.name].speed : animalAnimator.speed;
            Debug.Log($"Distance: {currentDistance} | AnimSpeed: {animSpeed} | Buzz: {bluetooth.BuzzValue}");
            yield return null;
        }
        waterScript.audioPaused = false;
    }

    private bool AreSimilar(Vector3 vec1, Vector3 vec2, float[] tolerances)
    {
        return Mathf.Abs(vec1.x - vec2.x) <= tolerances[0]
            && Mathf.Abs(vec1.y - vec2.y) <= tolerances[1]
            && Mathf.Abs(vec1.z - vec2.z) <= tolerances[2];

    }

    private void Detach()
    {
        PosTest.rClown = this.gameObject.transform.eulerAngles;
        this.detached = true;
        GameObject vuforiaTarget = this.gameObject.transform.parent.gameObject;
        this.gameObject.transform.SetParent(touchpool.transform);
        //vuforiaTarget.SetActive(false);
        Global.PossibleAnimalHomes.Remove(animalHome.name);
        localPos = this.gameObject.transform.localPosition;
        localRot = this.gameObject.transform.localRotation;
        PosTest.UpdateStatus(this.gameObject.name + " Detached!");
        UpdateFeedback(Settled);
        waterScript.ChangeFish();
        //StartCoroutine(CanvasHandler(canvasGood, 5.0f));
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
        waterScript.audioPaused = false;
        Debug.Log($"Buzz: {bluetooth.BuzzValue}");
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
        //foreach (AudioSource source in audioSources)
        //    source.Stop();
        //audioSources[success ? 0 : 1].Play();
    }

    void UpdateFeedback(Feedback target, bool hasFired = false)
    {
        foreach (Feedback feedback in AllFeedback)
            feedback.Deactivate();
        target.Activate(this, hasFired);
    }

    void UpdateAnimation(float speed)
    {
        if (animalAnimator != null)
            animalAnimator.speed = speed;
        else if (animalAnimation != null)
            animalAnimation[animalAnimation.clip.name].speed = speed;
    }

    public class Feedback
    {
        public GameObject canvas;
        public TextMeshProUGUI textMesh;
        public AudioClip audioClip;
        public string text;
        public bool textUpdateable;
        public float lifeCycle;
        public AudioSource audio;

        public Feedback(GameObject canvas, AudioClip audioClip, string text, bool textUpdateable, float lifeCycle, AudioSource audio)
        {
            this.canvas = canvas;
            this.audioClip = audioClip;
            this.textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
            this.text = text;
            this.textUpdateable = textUpdateable;
            this.lifeCycle = lifeCycle;
            this.audio = audio;
        }

        public void Activate(EnterDetect enterDetect, bool hasFired)
        {
            if (textUpdateable && !string.IsNullOrEmpty(text))
                this.textMesh.text = text;
            //this.canvas.SetActive(true);

            if(!hasFired)
            {
                this.audio.clip = this.audioClip;
                if (this.audio.clip != null)
                    audio.Play();
            }

            //enterDetect.StartCoroutine(this.FeedbackLifecycle());

        }

        IEnumerator FeedbackLifecycle()
        {
            float time = 0f;
            while (time <= this.lifeCycle)
            {
                time += Time.deltaTime;
                yield return null;
            }
            this.Deactivate();
        }

        public void Deactivate()
        {
            //if (this.audio.isPlaying)
            //    this.audio.Stop();
            if(this.canvas.activeSelf)
                this.canvas.SetActive(false);
        }
    }

}
