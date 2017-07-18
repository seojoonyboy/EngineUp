using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DatePickerLayout : MonoBehaviour {

	// hiw many rows and columns has the grid
	public int columns;
	public int rows;
	public int horizSpacing;
	public int vertSpacing;

	// the grid consists of buttons
	public GameObject button;

	// the color scheme
	public Sprite otherEntryImg;
	public Sprite currentEntryImg;
	public Sprite actualEntryImg;

	public Color fontActiveColor;
	public Color fontDeactiveColor;

	public Calendar calendar;

	// array to store the found buttons
	protected Button[] CalendarCells;



	//
	// public int getNumberOfCells()
	//
	// Get the current number of cells the picker has
	//

	public int getNumberOfCells() {
		return columns * rows;
	}

	//
	// public void getCells()
	//
	// get all cells and populate the array
	//

	public void getCells() {
		if (CalendarCells == null || CalendarCells.Length == 0)
			CalendarCells = GetComponentsInChildren<Button> () ;
	}


	//
	// public void createCells(string name)
	//
	// create the buttons and give them names
	//

	public void createCells(string name) {
		RectTransform mainPanelRect = gameObject.GetComponent<RectTransform> ();	
		GridLayoutGroup grid = gameObject.GetComponent<GridLayoutGroup> ();

		// create the elements
		for (int i = 0; i < columns * rows; i++) {
			GameObject go = Instantiate (button,Vector3.one,Quaternion.identity) as GameObject;
			go.transform.SetParent (this.transform, false);
			go.transform.localScale = Vector3.one;
			go.name = name;
		}

		// set the grid layout
		grid.cellSize = new Vector2(mainPanelRect.rect.width / columns - horizSpacing, mainPanelRect.rect.height / rows - vertSpacing);	
		grid.spacing = new Vector2 (horizSpacing, vertSpacing);
	}


	public virtual void setupCells(DateTime now) {
	}

	public virtual void cellClicked(PickerCell cell) {
	}

	public virtual string getInfo(DateTime selected) {
		return "";
	}

	public void setSprites (Sprite current, Sprite actual, Sprite other) { 
		currentEntryImg = current;
		actualEntryImg = actual;
		otherEntryImg = other;
	}

	public void setFontColors (Color active, Color deactive) {
		fontActiveColor = active;
		fontDeactiveColor = deactive;
	}

	public void setSpacing (int hSpacing, int vSpacing, Color spacingColor) {
		horizSpacing = hSpacing;
		vertSpacing = vSpacing;
		GetComponent<Image> ().color = spacingColor;
	}
}
