using UnityEngine;
using System.Collections.Generic;

public class MapLine : MonoBehaviour {
    public void drawLine(string[] lat, string[] lon) {
        List<Vector2> list = new List<Vector2>();

        for(int i=0; i<lat.Length-1;i++) {
            Vector2 loc = new Vector2((float.Parse(lon[i])),float.Parse(lat[i]));
            list.Add(loc);
        }
        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }
}