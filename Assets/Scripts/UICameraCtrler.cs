using UnityEngine;
using System.Collections;

public class UICameraCtrler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera _camera = gameObject.GetComponent<Camera>();
        _camera.orthographicSize = (Screen.height * 720f / 1280f) / Screen.width;
        Debug.Log(_camera.orthographicSize);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
