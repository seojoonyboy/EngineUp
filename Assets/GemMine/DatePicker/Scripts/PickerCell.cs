using UnityEngine;
using System.Collections;

public class PickerCell : MonoBehaviour {

	DatePickerLayout parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void cellClicked() {
		parent = (DatePickerLayout) GetComponentInParent(typeof (DatePickerLayout));
		parent.cellClicked(this);
	}
}
