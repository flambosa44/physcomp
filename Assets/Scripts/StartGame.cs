using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGame : MonoBehaviour
{
    public GameObject numHintsGO, hint1GO, hint2GO, hint3GO, guidesGO, discreteToggle, proxToggle, audioToggle;
    private TMP_Dropdown numHints, hint1, hint2, hint3, guides;
    private TMP_Dropdown[] hintDropdowns;
    private GameObject[] dropdownGOs;
    public static List<Water.IxHints> selectedHints;
    public static Water.IxGuides selectedGuide;
    public static bool isDiscrete, isProximity, useAudio, isSet = false;
    // Start is called before the first frame update
    void Start()
    {
        dropdownGOs = new GameObject[] { hint1GO, hint2GO, hint3GO };
        numHints = numHintsGO.GetComponent<TMP_Dropdown>();
        hint1 = hint1GO.GetComponent<TMP_Dropdown>();
        hint2 = hint2GO.GetComponent<TMP_Dropdown>();
        hint3 = hint3GO.GetComponent<TMP_Dropdown>();
        guides = guidesGO.GetComponent<TMP_Dropdown>();

        List<string> hints = new List<string>(Enum.GetNames(typeof(Water.IxHints)));
        hintDropdowns = new TMP_Dropdown[] { hint1, hint2, hint3 };
        foreach (TMP_Dropdown hintDropdown in hintDropdowns)
        {
            hintDropdown.ClearOptions();
            hintDropdown.AddOptions(hints);
        }
        guides.ClearOptions();
        guides.AddOptions(new List<string>(Enum.GetNames(typeof(Water.IxGuides))));
        numHints.onValueChanged.AddListener(delegate { numHintsChanged(numHints); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void numHintsChanged(TMP_Dropdown change)
    {
        GameObject temp;
        int max = change.value;
        for(int i = 0; i < dropdownGOs.Length; i++)
        {
            temp = dropdownGOs[i];
            if (i <= max && !temp.activeSelf)
                temp.SetActive(true);
            else if (i > max)
                temp.SetActive(false);
        }
    }

    public void StartPrototype()
    {
        selectedHints = new List<Water.IxHints>();
        foreach(TMP_Dropdown hintDropdown in hintDropdowns)
        {
            Water.IxHints hint;
            if (hintDropdown.gameObject.activeSelf && Enum.TryParse<Water.IxHints>(hintDropdown.options[hintDropdown.value].text, out hint)
                    && !selectedHints.Contains(hint))
                selectedHints.Add(hint);

        }
        Enum.TryParse<Water.IxGuides>(guides.options[guides.value].text, out selectedGuide);
        isDiscrete = discreteToggle.GetComponent<Toggle>().isOn;
        isProximity = proxToggle.GetComponent<Toggle>().isOn;
        useAudio = audioToggle.GetComponent<Toggle>().isOn;
        isSet = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
