using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ConvertTime : MonoBehaviour {
    public static DateTime ConvertUnixToTimeStamp(double timeStamp) {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timeStamp);
    }

    public static double ConvertTimeStampToUnix(DateTime dateTime) {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = dateTime - origin;
        return Math.Floor(diff.TotalSeconds);
    }
}
