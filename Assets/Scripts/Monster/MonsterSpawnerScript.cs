using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GridID = System.Int32;

public class MonsterSpawnerScript : MonoBehaviour {

	public GridSystemScript gridSystem;
	public GridID _startID;
	public GridID _endID;
	public List<GridID> path;

	public GameObject goblinPrefab;
	float spawnTimer;
	int numMonsters;

	void Awake()
	{
		spawnTimer = 0.0f;
		numMonsters = 0;
	}

	// Use this for initialization
	void Start ()
	{
		GeneratePath ();
		//SpawnGoblin ();
	}
	
	// Update is called once per frame
	void Update ()
	{	
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0.0f && numMonsters < 10)
		{
			SpawnGoblin ();
			spawnTimer = 2.5f;
			++numMonsters;
		}
	}

	public void GeneratePath()
	{
		if (gridSystem != null)
		{
			path = gridSystem.Search (_startID, _endID);
		}
	}

	public void SpawnGoblin()
	{
		GameObject goblin = GameObject.Instantiate (goblinPrefab);
		goblin.GetComponent<MonsterScript> ().spawner = this;
		goblin.GetComponent<MonsterScript> ().gridSystem = gridSystem;
		goblin.GetComponent<MonsterScript> ().path = path;

		if (path != null && path.Count > 0)
		{
			goblin.GetComponent<Transform> ().position = gridSystem.GetGrid(path[0]).GetComponent<Transform>().position + new Vector3(0, 0.5f, 0);
		}
	}
}