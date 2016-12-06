using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MapLine : MonoBehaviour {

    public void drawLine(string[] lat, string[] lon) {
        List<Vector2> list = new List<Vector2>();

        for(int i=0; i<lat.Length-1;i++) {
            Debug.Log("LAT : " + float.Parse(lat[i]));
            Debug.Log("LON : " + float.Parse(lon[i]));
            Vector2 loc = new Vector2((float.Parse(lat[i])),float.Parse(lon[i]));
            //list.Add(loc);
            list.Add(new Vector2((float)37.879183,(float)127.741478));
            list.Add(new Vector2((float)37.880902,(float)127.745491));
            list.Add(new Vector2((float)37.882248,(float)127.748270));
        }
        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }
}