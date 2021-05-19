using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CustomTrackingHandler : DefaultTrackableEventHandler
{
    // Start is called before the first frame update
    public GameObject child;
    private EnterDetect childScript;

    protected override void Start()
    {
        base.Start();
        childScript = child.GetComponent<EnterDetect>();
    }
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        if (!childScript.Iniitiated)
            childScript.Initiate();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
    }

}
