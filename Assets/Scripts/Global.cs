using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> animalNonColliders;
    public List<GameObject> candidateAnimalHomes;
    private static Dictionary<string, GameObject> dictCandidateAnimalHomes;
    private static List<string> animalNonCollidersNames;

    void Start()
    {
        animalNonCollidersNames = new List<string>();
        foreach (GameObject nonCollider in animalNonColliders)
            animalNonCollidersNames.Add(nonCollider.name);

        dictCandidateAnimalHomes = new Dictionary<string, GameObject>();
        foreach (GameObject animalHome in candidateAnimalHomes)
            dictCandidateAnimalHomes.Add(animalHome.name, animalHome);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<string> AnimalNonColliders
    {
        get { return animalNonCollidersNames; }
    }

    public static Dictionary<string, GameObject> PossibleAnimalHomes
    {
        get { return dictCandidateAnimalHomes; }
    }

}
