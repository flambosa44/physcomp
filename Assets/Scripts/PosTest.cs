using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PosTest : MonoBehaviour
{
    public GameObject fish;
    private Transform fishTrans, poolTrans;
    private TextMeshProUGUI Xrot, Yrot, Zrot, Xvec, Yvec, Zvec, Mvec, status;
    private static string statusText = "N/A";
    private static Vector3 deltaPos = new Vector3(), deltaRot = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI[] textMeshes = this.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI textMesh in textMeshes)
        {
            if (textMesh.gameObject.name == "Xrot")
                Xrot = textMesh;
            else if (textMesh.gameObject.name == "Yrot")
                Yrot = textMesh;
            else if (textMesh.gameObject.name == "Zrot")
                Zrot = textMesh;
            else if (textMesh.gameObject.name == "Xvec")
                Xvec = textMesh;
            else if (textMesh.gameObject.name == "Yvec")
                Yvec = textMesh;
            else if (textMesh.gameObject.name == "Zvec")
                Zvec = textMesh;
            else if (textMesh.gameObject.name == "Mvec")
                Mvec = textMesh;
            else if (textMesh.gameObject.name == "Status")
                status = textMesh;
        }
        //fishTrans = fish.transform;
        //poolTrans = fish.GetComponent<EnterDetect>().animalHome.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 animalPos = fishTrans.position;
        //Vector3 animalRot = fishTrans.rotation.eulerAngles;
        //Vector3 homePos = poolTrans.position;
        //Vector3 homeRot = poolTrans.rotation.eulerAngles;
        //Vector3 deltaPos = animalPos - homePos;
        //Vector3 deltaRot = animalRot - homeRot;

        Xrot.text = "X: " + deltaRot.x;
        Yrot.text = "Y: " + deltaRot.y;
        Zrot.text = "Z: " + deltaRot.z;
        Xvec.text = "X: " + deltaPos.x;
        Yvec.text = "Y: " + deltaPos.y;
        Zvec.text = "Z: " + deltaPos.z;
        Mvec.text = "M: " + deltaPos.magnitude;
        status.text = statusText;
    }

    public static void UpdateVectors(Vector3 extDeltaPos, Vector3 extDeltaRot)
    {
        deltaPos = extDeltaPos;
        deltaRot = extDeltaRot;
    }

    public static void UpdateStatus(string message)
    {
        statusText = message;
    }
}
