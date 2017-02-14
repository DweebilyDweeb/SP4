using UnityEngine;
using System.Collections;

public class WagonMoveForward : MonoBehaviour {
    float movementSpeed;
    float z;
    // Use this for initialization
    void Start () {
        movementSpeed = 0.2f;
        z = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        z += Time.deltaTime * movementSpeed;
        gameObject.GetComponent<Transform>().Translate(0, 0, z);
        if (z > 0.4f)
        {
            gameObject.GetComponent<Transform>().localPosition = new Vector3(1.313234f, 0.02144068f, -2.48f);
            z = 0.0f;
        }
    }
}
