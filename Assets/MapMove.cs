using UnityEngine;
using System.Collections;

public class MapMove : MonoBehaviour {
	bool dragging=false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(- Input.GetAxis("Mouse X"), - Input.GetAxis("Mouse Y"),0);
	}

}
