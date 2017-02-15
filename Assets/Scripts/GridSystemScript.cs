﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using GridID = System.Int32;

public class GridSystemScript : MonoBehaviour
{
	private class SearchNode
	{
		public GridID gridID;
		public int gCost; //From the start to here.
		public int hCost; //From here to the end.
		public SearchNode parent;

		public SearchNode(GridID _gridID, int _gCost = 0, int _hCost = 0)
		{
			gridID = _gridID;
			gCost = _gCost;
			hCost = _hCost;
			SearchNode parent = null;
		}

		public int GetFCost()
		{
			return gCost + hCost;
		}
	}

	public Vector3 originPosition;			// Position of the start

	public GridScript gridPrefab;			// Prefab of the grid we're using.

	public TextAsset csvFile;				// Reference of CSV file

	public int initNumRows, initNumCols;
	private int numRows, numColumns;		// Number of rows & columns
	private GameObject[] grids;				// Dynamic array of grids[numRows * numCols]

	// Use this for initialization
	void Start ()
	{
		numRows = 0;
		numColumns = 0;

		if (gridPrefab == null) //If there's no prefab, don't do anything.
		{
			return;
		}

		if (csvFile == null) //Load the CSV file by default.
		{
			LoadGrid();
		}
		else
		{
			LoadCSV();
		}

		SetGridsNeighbours();
	}

	// Update is called once per frame
	void Update()
	{
	}

	private int ComputeID(int _row, int _column)
	{
		return _row * numColumns + _column;
	}

	private void LoadGrid()
	{
		numRows = Mathf.Max(0, initNumRows);
		numColumns = Mathf.Max(0, initNumCols);
		if (numRows == 0 || numColumns == 0)
		{
			return;
		}

		grids = new GameObject[numRows * numColumns];
		for (int row = 0; row < numRows; ++row)
		{
			for (int column = 0; column < numColumns; ++column)
			{
				GridID id = ComputeID(row, column);
				grids [id] = GameObject.Instantiate (gridPrefab.gameObject);
				Vector3 gridScale = gridPrefab.gameObject.GetComponent<Transform> ().localScale;
				grids [id].GetComponent<Transform> ().position = new Vector3 (column * gridScale.x + originPosition.x, originPosition.y, row * gridScale.z + originPosition.z);
				grids [id].GetComponent<GridScript>().SetID (id);
			}
		}
	}

	// Loads CSV file
	private void LoadCSV()
	{
		char lineSeparator = '\n';
		char fieldSeparator = ',';

		//Load the CSV file into a string.
		string[] lines = csvFile.text.Split(lineSeparator);

		//Get the number of rows & columns.
		numRows = lines.Length;
		numColumns = 0;
		foreach (string record in lines)
		{
			if (record.Length > numColumns)
			{
				numColumns = record.Length;
			}
		}

		//Create our grid.
		grids = new GameObject[numRows * numColumns];

		int row = 0;
        foreach(string record in lines)
        {
			int column = 0;
            string[] fields = record.Split(fieldSeparator);
            foreach(string field in fields)
            {
				if (field.Equals("1"))
				{
					GridID id = ComputeID(row, column);
					grids [id] = GameObject.Instantiate (gridPrefab.gameObject);
					Vector3 gridScale = gridPrefab.gameObject.GetComponent<Transform> ().localScale;
					grids [id].GetComponent<Transform> ().position = new Vector3 (column * gridScale.x + originPosition.x, originPosition.y, row * gridScale.z + originPosition.z);
					grids [id].GetComponent<GridScript>().SetID (id);
				}
				++column;
            }
			++row;
        }
	}

	private void SetGridsNeighbours()
	{
		for (int row = 0; row < numRows; ++row)
		{
			for (int column = 0; column < numColumns; ++column)
			{
				GridID id = row * numColumns + column;
				if (grids [id] == null)
				{
					continue;
				}

				GridID previousRowID = -1;
				GridID nextRowID = -1;
				GridID previousColumnID = -1;
				GridID nextColumnID = -1;

				{
					GameObject previousRowNeighbour = GetGrid (row - 1, column);
					if (previousRowNeighbour != null)
					{
						previousRowID = previousRowNeighbour.GetComponent<GridScript> ().GetID ();
					}
				}
				{
					GameObject nextRowNeighbour = GetGrid (row + 1, column);
					if (nextRowNeighbour != null)
					{
						nextRowID = nextRowNeighbour.GetComponent<GridScript> ().GetID ();
					}
				}
				{
					GameObject previousColumnNeighbour = GetGrid (row, column - 1);
					if (previousColumnNeighbour != null)
					{
						previousColumnID = previousColumnNeighbour.GetComponent<GridScript> ().GetID ();
					}
				}
				{
					GameObject nextColumnNeighbour = GetGrid (row, column + 1);
					if (nextColumnNeighbour != null)
					{
						nextColumnID = nextColumnNeighbour.GetComponent<GridScript> ().GetID ();
					}
				}

				grids [id].GetComponent<GridScript> ().SetNeighbourIDs (previousRowID, nextRowID, previousColumnID, nextColumnID);
			}
		}
	}

	// Get the tile from its ID
	public GameObject GetGrid(GridID _id)
	{
		if (_id < 0 || _id >= numRows * numColumns)
		{
			return null;
		}

		return grids[_id];
	}

	public GameObject GetGrid(int _row, int _column)
	{
		if (_row < 0 || _row >= numRows)
		{
			return null;
		}
		if (_column < 0 || _column >= numColumns)
		{
			return null;
		}

		return grids[_row * numColumns + _column];
	}

	private int ComputeHCost(GridID _startID, GridID _endID)
	{
		int startColumn = _startID % numColumns;
		int startRow = (_startID - startColumn) / numColumns; //Actually no need to minus first.

		int endColumn = _endID & numColumns;
		int endRow = (_endID - endColumn) / numColumns; //Actually no need to minus first.

		return Mathf.Abs(endRow - startRow) + Mathf.Abs (endColumn - startColumn);
	}

	private bool InList(List<SearchNode> _list, GridID _gridID)
	{
		for (int i = 0; i < _list.Count; ++i)
		{
			if (_list [i].gridID == _gridID)
			{
				return true;
			}
		}

		return false;
	}

	// Pathfinding search function from start to end
	public List<GridID> Search(GridID _startID, GridID _endID)
	{
		if (GetGrid (_startID) == null || GetGrid(_endID) == null) //Check if the path is even remotely possible.
		{
			return null;
		}

		if (GetGrid (_startID).GetComponent<GridScript> ().wall != null || GetGrid (_endID).GetComponent<GridScript> ().wall != null) //I can't go through walls. What am I, a ghost?
		{
			return null;
		}

		List<GridID> result = new List<GridID>();
		List<SearchNode> openList = new List<SearchNode>();
		List<SearchNode> closedList = new List<SearchNode>();

		openList.Add(new SearchNode(_startID, 0, ComputeHCost(_startID, _endID)));

		//Look through our Open List.
		while (openList.Count != 0)
		{
			//Find the cheapest node.
			SearchNode cheapestNode = openList [0];
			for (int i = 1; i < openList.Count; ++i)
			{
				if (openList [i].GetFCost() < cheapestNode.GetFCost())
				{
					cheapestNode = openList [i];
				}
			}
				
			//Look through our neighbours and get the cheapest one.
			List<GridID> neighbourIDs = GetGrid(cheapestNode.gridID).GetComponent<GridScript>().GetNeighbourIDs();
			for (int j = 0; j < neighbourIDs.Count; ++j)
			{
				int neighbourID = neighbourIDs[j];

				if (neighbourID < 0) //No neighbour there.
				{
					continue;
				}

				if (GetGrid (neighbourID).GetComponent<GridScript> ().wall != null) //The neighbour has a wall. No go.
				{
					continue;
				}

				if (InList (closedList, neighbourID)) //If it is in the closed list, move on.
				{
					continue;
				}

				SearchNode neighbourNode = null;

				for (int k = 0; k < openList.Count; ++k) //Check if it is in the open list.
				{
					if (openList[k].gridID == neighbourID)
					{
						neighbourNode = openList [k];
						break;
					}
				}

				int movementCost = 1;
				if (neighbourNode != null) //If it is in the Open List
				{
					int gCost = cheapestNode.gCost + movementCost;
					if (gCost < neighbourNode.gCost)
					{
						neighbourNode.gCost = gCost;
						neighbourNode.parent = cheapestNode;
					}
				}
				else //Make a new node.
				{
					neighbourNode = new SearchNode (neighbourID, movementCost + cheapestNode.gCost, ComputeHCost (neighbourID, _endID));
					neighbourNode.parent = cheapestNode;
					openList.Add (neighbourNode);
				}

				if (neighbourNode.gridID == _endID) //Have we found the one we're looking for? (As in the node, not the love of our lives.)
				{
					SearchNode currentNode = neighbourNode;
					while (currentNode != null)
					{
						result.Insert (0, currentNode.gridID);
						currentNode = currentNode.parent;
					}

					return result; //Return our path.
				}
			}

			openList.Remove (cheapestNode);
			closedList.Add (cheapestNode);
		}

		return null; //Paiseh, no path found.
	}
}