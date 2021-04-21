using UnityEngine;
using System.Collections;

/* This script instantiates the fishes and updates the goal position randomly
 * 
 * 
 * 
*/
public class globalFlock : MonoBehaviour {

	public GameObject[] fishPrefabs;
	//public static int tankSize = 2;
	public GameObject WaterCube;//the water itself
	private BoxCollider Water;//get the size of the water
	public static Vector3 fishTank;//save the size of the collider here

	static int numFish = 30;//number of fish
	public static GameObject[] allFish = new GameObject[numFish];

	public static Vector3 goalPos = Vector3.zero;

	void Start () 
	{
		Water = WaterCube.GetComponent<BoxCollider>();
		fishTank = Water.bounds.size-new Vector3(1,1,1);//decrease the bounds by 1, so fishes wont be in the glass
		Water.enabled = false;//disable collider because of weird effects could happen
	
		//instantiate all fishes
		for (int i = 0; i < numFish; i++)
		{
			Vector3 pos = UpdateGoalPos();
			allFish[i] = (GameObject)Instantiate(fishPrefabs[Random.Range(0,fishPrefabs.Length)], pos, Quaternion.identity);
		}
	}

	void Update () 
	{
		if (Random.Range(0, 10000) < 50)//with a chance of 50 in 10000 update fish goal
		{
			goalPos = UpdateGoalPos();
		}
	}

	public Vector3 UpdateGoalPos()
	{
		goalPos = new Vector3(Random.Range(WaterCube.transform.position.x-fishTank.x/2,WaterCube.transform.position.x+fishTank.x/2), 
			Random.Range(WaterCube.transform.position.y-fishTank.y/2,WaterCube.transform.position.y+fishTank.y/2), 
			Random.Range(WaterCube.transform.position.z-fishTank.z/2,WaterCube.transform.position.z+fishTank.z/2));
		
		return goalPos;
	}
}
