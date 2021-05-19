using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PosTest : MonoBehaviour
{
    public GameObject fish;
    private Transform fishTrans, poolTrans;
    private TextMeshProUGUI XrOrigin, YrOrigin, ZrOrigin, XrClown, YrClown, ZrClown, XrButt, YrButt, ZrButt, XrDiff, YrDiff, ZrDiff;
    private TextMeshProUGUI XvOrigin, YvOrigin, ZvOrigin, XvClown, YvClown, ZvClown, XvButt, YvButt, ZvButt, XvDiff, YvDiff, ZvDiff, status;
    private static string statusText = "N/A";
    public static Vector3 vOrigin = new Vector3(), vClown = new Vector3(), vButt = new Vector3(), vDiff = new Vector3();
    public static Vector3 rOrigin = new Vector3(), rClown = new Vector3(), rButt = new Vector3(), rDiff = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI[] textMeshes = this.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI textMesh in textMeshes)
        {
            if (textMesh.gameObject.name == "Xrot")
                XrOrigin = textMesh;
            else if (textMesh.gameObject.name == "Yrot")
                YrOrigin = textMesh;
            else if (textMesh.gameObject.name == "Zrot")
                ZrOrigin = textMesh;
            else if (textMesh.gameObject.name == "Xrot (1)")
                XrClown = textMesh;
            else if (textMesh.gameObject.name == "Yrot (1)")
                YrClown = textMesh;
            else if (textMesh.gameObject.name == "Zrot (1)")
                ZrClown = textMesh;
            else if (textMesh.gameObject.name == "Xrot (2)")
                XrButt = textMesh;
            else if (textMesh.gameObject.name == "Yrot (2)")
                YrButt = textMesh;
            else if (textMesh.gameObject.name == "Zrot (2)")
                ZrButt = textMesh;
            else if (textMesh.gameObject.name == "Xrot (3)")
                XrDiff = textMesh;
            else if (textMesh.gameObject.name == "Yrot (3)")
                YrDiff = textMesh;
            else if (textMesh.gameObject.name == "Zrot (3)")
                ZrDiff = textMesh;
            else if (textMesh.gameObject.name == "Xvec")
                XvOrigin = textMesh;
            else if (textMesh.gameObject.name == "Yvec")
                YvOrigin = textMesh;
            else if (textMesh.gameObject.name == "Zvec")
                ZvOrigin = textMesh;
            else if (textMesh.gameObject.name == "Xvec (1)")
                XvClown = textMesh;
            else if (textMesh.gameObject.name == "Yvec (1)")
                YvClown = textMesh;
            else if (textMesh.gameObject.name == "Zvec (1)")
                ZvClown = textMesh;
            else if (textMesh.gameObject.name == "Xvec (2)")
                XvButt = textMesh;
            else if (textMesh.gameObject.name == "Yvec (2)")
                YvButt = textMesh;
            else if (textMesh.gameObject.name == "Zvec (2)")
                ZvButt = textMesh;
            else if (textMesh.gameObject.name == "Xvec (3)")
                XvDiff = textMesh;
            else if (textMesh.gameObject.name == "Yvec (3)")
                YvDiff = textMesh;
            else if (textMesh.gameObject.name == "Zvec (3)")
                ZvDiff = textMesh;
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

        XvOrigin.text = vOrigin.x.ToString();
        YvOrigin.text = vOrigin.y.ToString();
        ZvOrigin.text = vOrigin.z.ToString();
        XvClown.text = vClown.x.ToString();
        YvClown.text = vClown.y.ToString();
        ZvClown.text = vClown.z.ToString();
        XvButt.text = vButt.x.ToString();
        YvButt.text = vButt.y.ToString();
        ZvButt.text = vButt.z.ToString();
        XvDiff.text = vDiff.x.ToString();
        YvDiff.text = vDiff.y.ToString();
        ZvDiff.text = vDiff.z.ToString();
        XrOrigin.text = rOrigin.x.ToString();
        YrOrigin.text = rOrigin.y.ToString();
        ZrOrigin.text = rOrigin.z.ToString();
        XrClown.text = rClown.x.ToString();
        YrClown.text = rClown.y.ToString();
        ZrClown.text = rClown.z.ToString();
        XrButt.text = rButt.x.ToString();
        YrButt.text = rButt.y.ToString();
        ZrButt.text = rButt.z.ToString();
        XrDiff.text = rDiff.x.ToString();
        YrDiff.text = rDiff.y.ToString();
        ZrDiff.text = rDiff.z.ToString();
        status.text = statusText;
    }

    public static void UpdateStatus(string message)
    {
        statusText = message;
    }
}
