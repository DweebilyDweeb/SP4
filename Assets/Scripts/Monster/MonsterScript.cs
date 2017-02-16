using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GridID = System.Int32;

public class MonsterScript : MonoBehaviour
{
	public MonsterSpawnerScript spawner;
	public GridSystemScript gridSystem;

	public List<GridID> path;
	public int currentPathIndex;

	float maxSpeed; //How fast we can move per second.
	float rotationSpeed; //How fast we can rotate per second.

	void Awake()
	{
		currentPathIndex = 0;
		maxSpeed = 5.0f;
		rotationSpeed = 720.0f;
	}

	// Use this for initialization
	void Start ()
	{	
		GameObject destinationGrid = gridSystem.GetGrid (path[currentPathIndex]); //Where we wanna go.
		if (destinationGrid == null)
		{
			//print ("Error! Destination Grid is null!");
			SlowDown ();
			return;
		}

		Vector3 direction = destinationGrid.transform.position - gameObject.transform.position;
		float yRotation = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
		gameObject.transform.eulerAngles = new Vector3 (0, yRotation, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Check if we still have places to walk.
		if (path == null || currentPathIndex >= path.Count)
		{
			//print ("End of the road.");
			print (gameObject.GetComponent<Rigidbody>().angularVelocity);
			SlowDown ();
			return;
		}

		//Check if we've reached the tile we are trying to go to.
		//print ("Current Path Index: " + currentPathIndex);
		GameObject destinationGrid = gridSystem.GetGrid (path[currentPathIndex]); //Where we wanna go.
		if (destinationGrid == null)
		{
			//print ("Error! Destination Grid is null!");
			SlowDown ();
			return;
		}

		//if (OnGround ())
		{
			Vector3 directionToCurrentGrid = destinationGrid.transform.position - gameObject.transform.position; //Direction towards where we wanna be.

			float bias = 50.0f; //Finish this rotation is roughly 1/10 of a second.
			if (Vector3.Dot (directionToCurrentGrid, gameObject.transform.forward) < 0.0f)
			{
				bias = 100.0f; //Finish this rotation is roughly 1/90 of a second.
			}

			Vector3 direction = Vector3.Lerp(gameObject.transform.forward, directionToCurrentGrid, Time.deltaTime * bias);
			direction.y = 0.0f; //Make sure we don't rotate along the X-Axis.
			direction = direction.normalized;

			//If this is a large turn.
			Vector3 forward = gameObject.transform.forward;
			forward.y = 0.0f;
			forward = forward.normalized;
			if (Vector3.Dot(direction, forward) < 0.99f)
			{
				if (Vector3.Dot(direction, gameObject.transform.right) > 0.0f)
				{
					//print ("Turning Right");
				}
				else
				{
					//print ("Turning Left");
				}
			}

			gameObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

			gameObject.GetComponent<Rigidbody> ().AddRelativeForce (0, 0, 100); //Push ourselves forward.

			//Limit our speed.
			if (gameObject.GetComponent<Rigidbody> ().velocity.magnitude > maxSpeed)
			{
				gameObject.GetComponent<Rigidbody> ().velocity = gameObject.GetComponent<Rigidbody> ().velocity.normalized;
				gameObject.GetComponent<Rigidbody> ().velocity *= maxSpeed;
			}
		}

		float distanceToGrid = Vector3.Distance(destinationGrid.transform.position, gameObject.transform.position);
		if (distanceToGrid < 0.5f)
		{
			++currentPathIndex;
		}
	}

	void SlowDown()
	{
		//Slow down by 80% every second.
		gameObject.GetComponent<Rigidbody> ().velocity *= 0.2f * (1.0f - Time.deltaTime);
	}

	void OnCollisionStay(Collision _collision)
	{
	}

	public bool OnGround()
	{
		Vector3 offset = gameObject.GetComponent<CapsuleCollider> ().center * 0.15f;
		Debug.DrawRay(gameObject.transform.position + offset, -Vector3.up, Color.red);
		return Physics.Raycast (gameObject.transform.position + offset, -Vector3.up, gameObject.GetComponent<CapsuleCollider>().bounds.extents.y + 0.1f);
	}
}