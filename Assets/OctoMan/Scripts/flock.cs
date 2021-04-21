using UnityEngine;
using System.Collections;

public class flock : MonoBehaviour {

	globalFlock GBF;

	[HideInInspector]public float speed;//swimming speed
	public float rotationSpeed = 5.0f;//rotation speed
	public float minSpeed = 0.8f;//minimum swimming speed
	public float maxSpeed = 1.5f;//maximum swimming speed
	[Tooltip("Swim Speed Modifier: if you resize the fish you might want to increase this multiplier, it doesn't affect the animation speed")]
	public float swimSpeedModifier = 1f; //if you resize the fish you might want to increase this multiplier, it doesn't affect the animation speed
	Vector3 averageHeading;//head towards the average Heading of the current Fish Group
	Vector3 averagePosition;//Heading to the average Position of the current Fish Group

	public Vector3 distanceOffset;
	[Range(1f,30)]public float neighborDistance = 3.0f;//max distance to a neighbor - Higher number more schooling
	[Range(0.1f,10)]public float avoidDistance = 2f;//max distance to avoid other fishes
	[HideInInspector]public Vector3 newGoalPos;
	[HideInInspector]public Vector3 updatePos;
	[HideInInspector]public bool turning = false;

	void Start () 
	{
		speed = Random.Range(minSpeed, maxSpeed);//set a random speed
		this.GetComponent<Animator>().speed = speed;//set the animator speed to swimming speed
		GBF = GameObject.Find("FishManager").GetComponent<globalFlock>();
		updatePos = GBF.WaterCube.transform.position;
		newGoalPos = GBF.UpdateGoalPos();
	}

//if hitting a collider and not turning - do a turn
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Glas")
		{
			if (!turning)
			{
				//recalculate newGoalPos
				newGoalPos = updatePos;
			}
			turning = true;
		}
	}
//reset turning
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Glas")
		{
			turning = false;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (turning)
		{
			Vector3 direction = newGoalPos - transform.position;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
			speed = Random.Range(minSpeed, maxSpeed);
			this.GetComponent<Animator>().speed = speed;
		}
		else
		{
			if (Random.Range(0, 10) < 1)
			{
				ApplyRules();
			}
		}
		transform.Translate(0, 0, Time.deltaTime * (speed*swimSpeedModifier));
	}

	void ApplyRules()
	{
		GameObject[] gos;//
		gos = globalFlock.allFish;

		Vector3 vcentre = Vector3.zero; // the center of the group
		Vector3 vavoid = Vector3.zero; // avoid anybody who is near the center of the group
		float gSpeed = 0.1f;//group speed

		Vector3 goalPos = globalFlock.goalPos;

		float dist;//to calculate the distance to the neigbors

		int groupSize = 0;//the group size
		foreach (GameObject go in gos)//loop throuph all fishes
		{
			if (go != this.gameObject)//make sure you are not asking for yourself
			{
				dist = Vector3.Distance(go.transform.position, this.transform.position+distanceOffset);//calculate the distance to the fish currently lopped through
				if (dist <= neighborDistance)//checks if it's a neighbor
				{
					vcentre += go.transform.position;// add the centre of the fish
					groupSize++;// increase the groupsize by 1

					if (dist < avoidDistance)//depending on the modelsize the number(avoidDistance) needs to be bigger or smaller
					{
						vavoid = vavoid + (this.transform.position+distanceOffset - go.transform.position);//recalculate the vavoid
					}

					//find the average speed of the group by adding up the speed
					flock anotherFlock = go.GetComponent<flock>();
					gSpeed = gSpeed + anotherFlock.speed;
				}
			}
		}

		if (groupSize > 0)//if the fish is in a group
		{
			vcentre = vcentre / groupSize + (goalPos - (this.transform.position+distanceOffset));//avarage center of the group
			speed = gSpeed / groupSize;//average speed of the group
			this.GetComponent<Animator>().speed = speed;//set animator speed

			Vector3 direction = (vcentre + vavoid) - transform.position+distanceOffset;// average group direction
			if (direction != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color32(255,0,0,20);
		Gizmos.DrawSphere(transform.position+distanceOffset, avoidDistance/2);

		Gizmos.color = new Color32(0,255,0,20);
		Gizmos.DrawSphere(transform.position+distanceOffset, neighborDistance/2);

	}
}
