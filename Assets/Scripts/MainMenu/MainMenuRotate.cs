using UnityEngine;
using System.Collections;

public class MainMenuRotate : MonoBehaviour {

    private float rotatespeed;
	
    // Use this for initialization
	void Start () {
        rotatespeed = 10.0f;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0.0f, rotatespeed * Time.deltaTime, 0.0f);
    }
}