using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Globalization;

public class WeekDayLayout : DatePickerLayout {


	public override void setupCells(DateTime now) {
		// get all child objects
		getCells ();

		CultureInfo culture = CultureInfo.CurrentCulture;
		int i = 0;
		foreach (Button go in CalendarCells) {
			go.transform.FindChild ("Text").GetComponent<Text> ().text = culture.DateTimeFormat.AbbreviatedDayNames[i].ToString ();
			go.transform.FindChild ("Text").GetComponent<Text> ().fontSize = 22;
			go.transform.FindChild ("Text").GetComponent<Text> ().color = fontActiveColor;
			go.transform.GetComponent<Image> ().sprite = actualEntryImg;
			i++;
		}
	}
	
}
