using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MapLine : MonoBehaviour {

    void Start() {
        //drawLine();
    }

    public void drawLine(string[] lat, string[] lon) {
        List<Vector2> list = new List<Vector2>();

        for(int i=0; i<lat.Length-1;i++) {
            //Debug.Log("LAT : " + float.Parse(lat[i]));
            //Debug.Log("LON : " + float.Parse(lon[i]));
            Vector2 loc = new Vector2((float.Parse(lon[i])),float.Parse(lat[i]));
            list.Add(loc);
        }
        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }

    public void drawLine() {
        List<Vector2> list = new List<Vector2>();

        list.Add(new Vector2(127.74180f,37.87936f));
        list.Add(new Vector2(127.74672f,37.88152f));
        list.Add(new Vector2(127.74802f,37.88231f));

        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list,Color.red,3f));
    }
}