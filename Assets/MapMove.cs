using UnityEngine;
using System.Collections;


public class MapMove : MonoBehaviour {
	float _minx=0;
	float _maxx=0;
	float _miny=0;
	float _maxy=0;
	public Camera c;

	void Start () {
		float v = Camera.main.camera.orthographicSize;  
		float h = v * ((float)Screen.width / (float)Screen.height);

		_miny = v;
		_maxy= (GetComponent<MeshRenderer> ()).bounds.size.y -v;
		_maxx = -h -30.0f; //the map loader draws the mesh a little off center for some reason, hence the -30. TODO I guess.
		_minx= -(GetComponent<MeshRenderer> ()).bounds.size.x +h;
		Debug.Log (string.Format("v {0}, h {1}, minx {2}, maxx {3},miny {4}, maxy{5}",v,h,_minx,_maxx,_miny,_maxy) );
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 worldPoint=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 pos=(worldPoint-transform.position)/32.0f;
			Debug.Log ("x"+ Mathf.FloorToInt(pos.x)+" y "+Mathf.FloorToInt(pos.y)*-1); //not sure if it's smart to mix a "(0,0) in top left" with Unitys "(0,0) in bottom left" coordinate system..
		}

		transform.position = new Vector3(Mathf.Clamp(transform.position.x- Input.GetAxis("Mouse X")*2.0f,_minx,_maxx), Mathf.Clamp(transform.position.y- Input.GetAxis("Mouse Y")*2.0f,_miny,_maxy),10);
	}

}
