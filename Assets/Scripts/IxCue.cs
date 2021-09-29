using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IxCue : MonoBehaviour
{
    private float delay;
    private float proximityTrigger;
    private ParticleSystem particleSys;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(float delay, float proximityTrigger)
    {
        this.delay = delay;
        this.proximityTrigger = proximityTrigger;


    }

}
