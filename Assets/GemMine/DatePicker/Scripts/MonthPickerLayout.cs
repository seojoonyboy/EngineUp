using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Globalization;

public class MonthPickerLayout : DatePickerLayout {

	public override void setupCells(DateTime now) {
		// get all child objects
		getCells ();
		// calculate the starting year which is dividable by 16
		CultureInfo culture = CultureInfo.CurrentCulture;
		int i = 0;
		foreach (Button go in CalendarCells) {
			go.transform.FindChild ("Text").GetComponent<Text> ().text = culture.DateTimeFormat.AbbreviatedMonthNames[i].ToString ();
			go.transform.FindChild ("Text").GetComponent<Text> ().color = fontActiveColor;
			if (i+1 == now.Month)
				go.transform.GetComponent<Image> ().sprite = currentEntryImg;
			else
				go.transform.GetComponent<Image> ().sprite = actualEntryImg;

			i++;
		}
	}

	public override void cellClicked(PickerCell cell) {
		// set the new date
		calendar.SelectedDate = new DateTime(
			calendar.SelectedDate.Year,
			cell.transform.GetSiblingIndex() + 1,
			Mathf.Clamp(calendar.SelectedDate.Day, 1, DateTime.DaysInMonth(calendar.SelectedDate.Year,cell.transform.GetSiblingIndex() + 1)));
		// zoom out
		calendar.ZoomOut ();
	}

	public override string getInfo(DateTime selected)
	{
		return selected.Year.ToString();
	}
}
