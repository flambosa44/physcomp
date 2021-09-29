using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public List<IxHints> hints;
    public IxGuides guide;
    public bool isDiscrete = false, isProximity = false, useAudio = false;
    public bool audioPaused = false;
    public GameObject arCamera, clownfishGhost;
    public float proximityTrigger;
    private AudioSource audio;
    private List<GameObject> effectGOs = new List<GameObject>();
    //private List<IxCue> effects = new List<IxCue>();
    // Start is called before the first frame update
    void Start()
    {
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
            foreach(Transform methodTrans in assetTrans)
            {
                name = methodTrans.gameObject.name.ToLower();
                found = false;
                foreach(IxHints hint in hints)
                {
                    if(name.StartsWith(hint.ToString()) && isDiscrete == name.EndsWith("discrete"))
                    {
                        found = true;
                        effectGOs.Add(methodTrans.gameObject);
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
            GameObject closestCoral = null;
            float smallestDistance = 10000;
            float distance;

            foreach(GameObject obj in effectGOs)
            {
                distance = Vector3.Distance(arCamera.transform.position, obj.transform.position);
                if(distance < smallestDistance)
                {
                    closestCoral = obj;
                    smallestDistance = distance;
                }
                if (isProximity)
                {
                    obj.SetActive(distance <= proximityTrigger);
                    float alpha = FishSwim.Lerp(0, proximityTrigger, 1, 0.1f, distance);
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
            if (useAudio && !audioPaused)
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
}
