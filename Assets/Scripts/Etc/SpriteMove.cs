using UnityEngine;
using System.Collections;

public class SpriteMove : MonoBehaviour {
    public GameObject[] sprites;
    public Transform[] targets;

    void Start() {
        //StartCoroutine("move");
    }

    IEnumerator move() {
        while(true) {
            foreach(GameObject sprite in sprites) {
                Vector2 newPos;
                if(sprite.transform.position.x <= targets[0].transform.position.x) {
                    newPos = new Vector2(targets[1].transform.position.x, sprite.transform.position.y);
                }
                else {
                    newPos = new Vector2(sprite.transform.position.x - 0.1f, sprite.transform.position.y);                    
                }
                sprite.transform.position = newPos;
            }
            yield return new WaitForSeconds(0.5f);
            
        }
    }
}
