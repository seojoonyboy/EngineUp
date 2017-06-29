using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
using UnityEngine.UI;

public class DayPickerLayout : DatePickerLayout  {


	public override void setupCells(DateTime now) {

		DateTime firstOfMonth = new DateTime (now.Year, now.Month, 1);
		DateTime firstEntry = firstOfMonth.AddDays (-(int)firstOfMonth.DayOfWeek);
	
		// get all child objects
		getCells ();
		// calculate the starting year which is dividable by 16
		//CultureInfo culture = CultureInfo.CurrentCulture;

		int i = 0;
		foreach (Button go in CalendarCells) {
			go.transform.FindChild ("Text").GetComponent<Text> ().text = firstEntry.Day.ToString ();
			if (firstEntry == now) {
				go.transform.FindChild ("Text").GetComponent<Text> ().color = fontActiveColor;
				go.transform.GetComponent<Image> ().sprite = currentEntryImg;
			} else if (firstEntry.Month != now.Month) {
				go.transform.FindChild ("Text").GetComponent<Text> ().color = fontDeactiveColor;
				go.transform.GetComponent<Image> ().sprite = otherEntryImg;
			} else {
				go.transform.FindChild ("Text").GetComponent<Text> ().color = fontActiveColor;
				go.transform.GetComponent<Image> ().sprite = actualEntryImg;
			}
			firstEntry = firstEntry.AddDays (1);
			i++;
		}
	}



	public override void cellClicked(PickerCell cell) {
		int clickedIndex = cell.transform.GetSiblingIndex ();
		int firstDay = (int) new DateTime(calendar.SelectedDate.Year,calendar.SelectedDate.Month, 1).DayOfWeek;
		int numberOfDays = DateTime.DaysInMonth(calendar.SelectedDate.Year,calendar.SelectedDate.Month);
		int numberOfDaysLastMonth = 0;
        
		if (calendar.SelectedDate.Month == 1)
			numberOfDaysLastMonth = DateTime.DaysInMonth(calendar.SelectedDate.Year - 1, 12);
		else
			numberOfDaysLastMonth = DateTime.DaysInMonth(calendar.SelectedDate.Year, calendar.SelectedDate.Month - 1);

		// clicked in the last month
		if (clickedIndex < firstDay) {
			Debug.Log ("In den letzten MOnat geklickt");
			Debug.Log ("clicked index: " + clickedIndex);
			Debug.Log ("first day: " + firstDay);
			calendar.SelectedDate = new DateTime (
				calendar.SelectedDate.AddMonths(-1).Year,
				calendar.SelectedDate.AddMonths (-1).Month,
				clickedIndex + numberOfDaysLastMonth - firstDay + 1);
		} 
		// clicked in the next month
		else if (clickedIndex > firstDay + numberOfDays - 1) {
			calendar.SelectedDate = new DateTime (
				calendar.SelectedDate.AddMonths(1).Year,
				calendar.SelectedDate.AddMonths (1).Month,
				clickedIndex - numberOfDays - firstDay + 1);
		} 
		// clicked in the current month
		else {
			calendar.SelectedDate = new DateTime (
				calendar.SelectedDate.Year,
				calendar.SelectedDate.Month,
				clickedIndex - firstDay + 1);
		}
		calendar.ZoomOut ();

        GameObject.Find("MyInfoPanel").GetComponent<StatViewController>().calenderSelEnd(calendar.SelectedDate.ToString());
        Destroy(gameObject.transform.parent.parent.parent.parent.gameObject);
    }



	public override string getInfo(DateTime selected)
	{
		return selected.ToString("MMMM", CultureInfo.CurrentCulture) + " " + selected.Year;
	}

}
