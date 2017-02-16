using UnityEngine;
using System.Collections;

public class WagonMoveForward : MonoBehaviour {
    float movementSpeed;
    float z;
    Vector3 startPosition;
    // Use this for initialization
    void Start () {
        movementSpeed = 20.0f;
        startPosition = gameObject.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        print("Framerate: " + 1.0f / Time.deltaTime);
        gameObject.GetComponent<Transform>().Translate(0, 0, Time.deltaTime * movementSpeed);
        if (gameObject.GetComponent<Transform>().position.z > 32.0f)
        {
            gameObject.GetComponent<Transform>().position = startPosition;
        }
    }
}
