using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public List<IxHints> hints;
    public IxGuides guide;
    public bool isDiscrete = false, isProximity = false, useAudio = false;
    public bool audioPaused = false;
    public GameObject clownFish, butterflyFish, clownfishGhost, ghostCoral;
    private GameObject currentFish;
    public float proximityTrigger;
    private AudioSource audio;
    public Dictionary<GameObject, Effect> effectGOs = new Dictionary<GameObject, Effect>();
    //private List<IxCue> effects = new List<IxCue>();
    // Start is called before the first frame update
    void Start()
    {
        currentFish = clownFish;
        audio = this.gameObject.GetComponent<AudioSource>();
        hints = StartGame.isSet ? StartGame.selectedHints : hints;
        guide = StartGame.isSet ? StartGame.selectedGuide : guide;
        isDiscrete = StartGame.isSet ? StartGame.isDiscrete : isDiscrete;
        isProximity = StartGame.isSet ? StartGame.isProximity : isProximity;
        useAudio = StartGame.isSet ? StartGame.useAudio : useAudio;
        string name;
        bool found;
        foreach(Transform assetTrans in this.transform)
        {
            Destroy(assetTrans.gameObject.GetComponent<MeshRenderer>());
            if (assetTrans.gameObject.name == ghostCoral.name)
                continue;
            foreach(Transform methodTrans in assetTrans)
            {
                name = methodTrans.gameObject.name.ToLower();
                found = false;
                foreach(IxHints hint in hints)
                {
                    if(name.StartsWith(hint.ToString()) && isDiscrete == name.EndsWith("discrete"))
                    {
                        found = true;
                        effectGOs.Add(assetTrans.gameObject, new Effect(methodTrans.gameObject));
                        //effects.Add(methodTrans.gameObject.GetComponent<IxCue>());
                        //effects[effects.Count - 1].Init(loopDelay, proximityTrigger);
                        break;
                    }

                }
                if(!isProximity)
                    methodTrans.gameObject.SetActive(found);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isProximity || (useAudio && !audioPaused))
        {
            GameObject closestCoral = null, obj;
            float smallestDistance = 10000;
            float distance;

            foreach(Effect effect in effectGOs.Values)
            {
                obj = effect.effectGO;
                if(effect.hasFired)
                {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                    continue;
                }

                if (!currentFish.activeSelf)
                {
                    smallestDistance = 10000;
                    break;
                }
                    
                distance = Vector3.Distance(currentFish.transform.position, obj.transform.position);
                if(distance < smallestDistance)
                {
                    closestCoral = obj;
                    smallestDistance = distance;
                }
                if (isProximity)
                {
                    obj.SetActive(distance <= proximityTrigger);
                    float alpha = FishSwim.Lerp(0.2f, proximityTrigger, 1, 0.3f, distance);
                    ParticleSystem pSys = obj.GetComponent<ParticleSystem>();
                    var main = pSys.main;
                    var colOverLife = pSys.colorOverLifetime;
                    var grad = new Gradient();
                    main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, alpha);
                    //pSys.colorOverLifetime.color.gradient.alphaKeys = new GradientAlphaKey[]{ new GradientAlphaKey(alpha,1)};
                    grad.SetKeys(colOverLife.color.gradient.colorKeys, new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1) });
                    colOverLife.color = grad;
                    //for(int i = 0; i < colOverLife.color.gradient.alphaKeys.Length; i++)
                    //{
                    //    Debug.Log("AK" + i + " = " + colOverLife.color.gradient.alphaKeys[i].alpha);
                    //}
                    
                }
                //else
                //    obj.GetComponent<ParticleSystem>().colorOverLifetime.color 

                
            }
            if (useAudio && !audioPaused && smallestDistance < 10000)
            {
                float volume = 1 / Mathf.Exp(8*smallestDistance);
                audio.volume = volume;
                if (!audio.isPlaying)
                    audio.Play();
            }
            else
            {
                if (audio.isPlaying)
                    audio.Stop();
            }
            //Debug.Log("Distance = " + smallestDistance);
        }

    }



    public enum IxHints
    {
        bubbles = 0,
        weeds,
        floating
    }

    public enum IxGuides
    {
        ghost = 0,
        trimodal,
        enviro
    }

    public class Effect
    {
        public GameObject effectGO;
        public bool hasFired;

        public Effect(GameObject effectGO)
        {
            this.hasFired = false;
            this.effectGO = effectGO;
        }
    }

    public void ActivateAnemone()
    {
        string name;
        bool found;
        foreach (Transform methodTrans in ghostCoral.transform)
        {
            name = methodTrans.gameObject.name.ToLower();
            found = false;
            foreach (IxHints hint in hints)
            {
                if (name.StartsWith(hint.ToString()) && isDiscrete == name.EndsWith("discrete"))
                {
                    found = true;
                    effectGOs.Add(ghostCoral, new Effect(methodTrans.gameObject));
                    //effects.Add(methodTrans.gameObject.GetComponent<IxCue>());
                    //effects[effects.Count - 1].Init(loopDelay, proximityTrigger);
                    break;
                }

            }
            if (!isProximity)
                methodTrans.gameObject.SetActive(found);
        }
        Global.AnimalNonColliders.Remove(ghostCoral.name);
        Global.PossibleAnimalHomes.Add(ghostCoral.name, ghostCoral);
    }

    public static void SetEffectAlpha(ParticleSystem pSys, float alpha)
    {
        var main = pSys.main;
        var colOverLife = pSys.colorOverLifetime;
        var grad = new Gradient();
        main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, alpha);
        //pSys.colorOverLifetime.color.gradient.alphaKeys = new GradientAlphaKey[]{ new GradientAlphaKey(alpha,1)};
        grad.SetKeys(colOverLife.color.gradient.colorKeys, new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1) });
        colOverLife.color = grad;
    }

    public void ChangeFish()
    {
        currentFish = butterflyFish;
        Debug.Log($"{currentFish.name} activeSelf = {currentFish.activeSelf}");
    }
}
