using UnityEngine;
using System.Collections.Generic;

public class MapLine : MonoBehaviour {
    public void drawLine(float[] lat, float[] lon) {
        List<Vector2> list = new List<Vector2>();

        for(int i=0; i<lat.Length-1;i++) {
            int cnt = 0;
            if(cnt == 2) {
                float midLon = Mathf.Abs(lon[i - 2] + lon[i]) / 2f;
                float midLat = Mathf.Abs(lat[i - 2] + lat[i]) / 2f;

                Vector2 loc = new Vector2(midLon, midLat);
                list.Add(loc);
                cnt = 0;
            }
            cnt++;
        }
        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }
}