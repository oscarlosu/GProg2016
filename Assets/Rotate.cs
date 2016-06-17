using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public float angle = 90;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, angle * Time.deltaTime);
	}
}
