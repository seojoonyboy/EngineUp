using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class YearPickerLayout : DatePickerLayout {

	int currentYear;
	int startYear;


	//
	// public int getStartYear()
	//
	// return the starting year for the year picker
	//

	public int getStartYear() {
		return startYear;
	}



	public override void setupCells(DateTime now) {
		// get all child objects
		getCells ();
		// calculate the starting year which is dividable by 16
		startYear = ((int)(now.Year / getNumberOfCells())) * getNumberOfCells();
		currentYear = startYear;
		foreach (Button go in CalendarCells) {
			go.transform.FindChild ("Text").GetComponent<Text> ().text = currentYear.ToString ();
			go.transform.FindChild ("Text").GetComponent<Text> ().color = fontActiveColor;
			if (currentYear == now.Year)
				go.transform.GetComponent<Image> ().sprite = currentEntryImg;
			else
				go.transform.GetComponent<Image> ().sprite = actualEntryImg;
			currentYear++;
		}
	}


	public override void cellClicked(PickerCell cell) {
		// set the new date
		calendar.SelectedDate = new DateTime(
			cell.transform.GetSiblingIndex() + startYear,
			calendar.SelectedDate.Month,
			calendar.SelectedDate.Day);
		// zoom out
		calendar.ZoomOut ();
	}


	public override string getInfo(DateTime selected)
	{
		return startYear + " - " + (currentYear - 1);
	}
}
