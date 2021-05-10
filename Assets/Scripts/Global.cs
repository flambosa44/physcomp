using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> animalNonColliders;
    private static List<string> animalNonCollidersNames;

    void Start()
    {
        animalNonCollidersNames = new List<string>();
        foreach (GameObject nonCollider in animalNonColliders)
            animalNonCollidersNames.Add(nonCollider.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<string> AnimalNonColliders
    {
        get { return animalNonCollidersNames; }
    }
}
