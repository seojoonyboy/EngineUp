using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using GemMine.EasyEasing;

public class Calendar : MonoBehaviour {

	public DatePickerLayout dayPanel;
	public DatePickerLayout monthPanel;
	public DatePickerLayout yearPanel;
	public DatePickerLayout weekdayPanel;

	public PickerTweens year;
	public PickerTweens month;
	public PickerTweens day;
	public PickerTweens completePanel;

	public GameObject NavPanel;
	public GameObject TopPanel;

	public Text NavPanelInfoText;
	public Text SelectedDateText;

	public Font calendarFont;
	public bool startCalOpen;

	public Sprite actualMonthImg;
	public Sprite otherMonthImg;
	public Sprite currentMonthImg;
	public Sprite navBarImg;
	public Color fontActiveColor;
	public Color fontDeactiveColor;
	public Color spacingColor;

	public int horizSpacing, vertSpacing;

	public bool zoomAnimation;
	public bool hideAnimation;

	DateTime selectedDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);

	public enum ActivePanel {
		year,
		month,
		day,
	};

	ActivePanel activePanel;

	public DateTime SelectedDate {
		get {return selectedDate;}
		set { selectedDate = value; 
			setupCells ();}
	}

	void setupCells() {
		yearPanel.setupCells (selectedDate);
		monthPanel.setupCells (selectedDate);
		dayPanel.setupCells (selectedDate);
		weekdayPanel.setupCells (selectedDate);
	}

	// Use this for initialization
	void Start () {
		if (!startCalOpen)
			completePanel.startShrunk ();

		activePanel = ActivePanel.year;
		setFont ();
		setTheme ();

		yearPanel.createCells ("Year");
		monthPanel.createCells ("Month");
		dayPanel.createCells ("Day");
		weekdayPanel.createCells ("WeekDay");
		setupCells ();

		setNavPanel();

		setSelectedDateText ();
	}


	public void setFont() {
		Text[] texts = GetComponentsInChildren<Text>();
		foreach (Text text in texts) {
			text.font = calendarFont;
		}
	}

	public void setTheme() {
		dayPanel.setSprites (currentMonthImg, actualMonthImg, otherMonthImg);
		monthPanel.setSprites (currentMonthImg, actualMonthImg, otherMonthImg);
		yearPanel.setSprites (currentMonthImg, actualMonthImg, otherMonthImg);
		weekdayPanel.setSprites(currentMonthImg, actualMonthImg, otherMonthImg);
		dayPanel.setFontColors (fontActiveColor, fontDeactiveColor);
		monthPanel.setFontColors (fontActiveColor, fontDeactiveColor);
		yearPanel.setFontColors (fontActiveColor, fontDeactiveColor);
		weekdayPanel.setFontColors (fontActiveColor, fontDeactiveColor);
		dayPanel.setSpacing(horizSpacing, vertSpacing, spacingColor);
		monthPanel.setSpacing(horizSpacing, vertSpacing, spacingColor);
		yearPanel.setSpacing(horizSpacing, vertSpacing, spacingColor);
	}

	// Update is called once per frame
	void Update () {
		
	}


	public void Shrink() {
		if (completePanel.canShrink ())
			completePanel.ShrinkIn (hideAnimation);
		else
			completePanel.ShrinkOut (hideAnimation);
	}



	public void ZoomOut() {
		if (!completePanel.canShrink ())
			return;
		
		switch (activePanel) {
		case ActivePanel.year:
			year.ZoomOut (zoomAnimation);
			activePanel++;			
			break;
		case ActivePanel.month:
			month.ZoomOut (zoomAnimation);
			activePanel++;
			break;
		case ActivePanel.day:
			break;
		}
		setupCells ();
		setSelectedDateText ();
		setNavPanel ();
	}



	public void ZoomIn() {
		if (!completePanel.canShrink ())
			return;

		switch (activePanel) {
		case ActivePanel.year:
			break;
		case ActivePanel.month:
			year.ZoomIn (zoomAnimation);
			activePanel--;
			break;
		case ActivePanel.day:
			month.ZoomIn (zoomAnimation);
			activePanel--;
			break;
		}
		setupCells ();
		setNavPanel ();
		setSelectedDateText ();
	}


	void setNavPanel() {
		switch (activePanel) {
		case ActivePanel.year:
			NavPanelInfoText.text = yearPanel.getInfo (selectedDate);
			break;
		case ActivePanel.month:
			NavPanelInfoText.text = monthPanel.getInfo (selectedDate);
			break;
		case ActivePanel.day:
			NavPanelInfoText.text = dayPanel.getInfo (selectedDate);
			break;
		}
		NavPanelInfoText.color = fontActiveColor;
		//NavPanel.GetComponent<Image> ().sprite = navBarImg;
		//NavPanel.GetComponent<Image> ().color = Color.white;
		//NavPanel.transform.FindChild ("ButtonBack").FindChild ("Image").GetComponent<Image> ().color = fontActiveColor;
		//NavPanel.transform.FindChild ("ButtonForward").FindChild ("Image").GetComponent<Image> ().color = fontActiveColor;
	}
		

	void setSelectedDateText() {
		SelectedDateText.text = selectedDate.ToString("D");
		SelectedDateText.color = fontActiveColor;
	}



	public void previousButtonClicked() {
		changeDate (-1);
	}
		
	public void nextButtonClicked() {
		changeDate (1);
	}

	public void changeDate(int direction) {
		switch (activePanel) {
		case ActivePanel.day:
			selectedDate = new DateTime (
				selectedDate.AddMonths(direction).Year,
				selectedDate.AddMonths(direction).Month,
				selectedDate.AddMonths(direction).Day);
			break;
		case ActivePanel.month:
			selectedDate = new DateTime (
				selectedDate.AddYears(direction).Year,
				selectedDate.AddYears(direction).Month,
				selectedDate.AddYears(direction).Day);
			break;
		case ActivePanel.year:
			// anzahl der zeilen und spalten ermitteln
			// und den Multiplikator bstimmern
			int numberOfCells = yearPanel.getNumberOfCells();
			selectedDate = new DateTime (
				selectedDate.AddYears(direction*numberOfCells).Year,
				selectedDate.AddYears(direction*numberOfCells).Month,
				selectedDate.AddYears(direction*numberOfCells).Day);
			break;
		}
		setupCells ();
		setNavPanel ();
		setSelectedDateText ();
	}
}
