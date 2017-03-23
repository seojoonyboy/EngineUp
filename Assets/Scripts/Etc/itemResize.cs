using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemResize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UIWidget containerSprite = gameObject.transform.parent.GetComponent<UIWidget>();
        //Debug.Log(containerSprite.width);
        int size = containerSprite.width / (gameObject.GetComponent<UITable>().columns + 1);
        Debug.Log(size);
        //gameObject.GetComponent<UISprite>().SetDimensions()
        UISprite[] sprites = gameObject.GetComponentsInChildren<UISprite>();
        foreach(UISprite sprite in sprites) {
            sprite.SetDimensions(size, size);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
