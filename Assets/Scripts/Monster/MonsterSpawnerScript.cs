using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GridID = System.Int32;

public class MonsterSpawnerScript : MonoBehaviour {

	public GridSystemScript gridSystem;
	public GridID _startID;
	public GridID _endID;
	public List<GridID> path;

	public GameObject testMonsterPrefab;
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

		//Don't collide with other monsters.
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Monster"), LayerMask.NameToLayer("Monster"), true);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0.0f)
		{
			SpawnTestMonster ();
			spawnTimer = 1.0f;
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

	public void SpawnTestMonster()
	{
		GameObject goblin = GameObject.Instantiate (testMonsterPrefab);
		goblin.GetComponent<AIMovement> ().gridSystem = gridSystem;
		goblin.GetComponent<AIMovement> ().path = path;

		if (path != null && path.Count > 0)
		{
			goblin.GetComponent<Transform> ().position = gridSystem.GetGrid(path[0]).GetComponent<Transform>().position + new Vector3(0, 0.5f, 0);
		}
	}
}