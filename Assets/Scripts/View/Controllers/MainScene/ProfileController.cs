using UnityEngine;
using System.Collections;

public class ProfileController : MonoBehaviour {
    Animation anim;
    public GameObject[] arrows;

    public bool
        isOver,
        isDown;
    void Start() {
        anim = GetComponent<Animation>();
        isOver = true;
        isDown = false;
    }

    public void Scroll() {
        if (isOver) {
            isOver = false;
            if (isDown) {
                anim.Play("Up");
            }
            else {
                anim.Play("Down");
            }
        }
    }

    public void scrollDownDone() {
        isOver = true;
        isDown = true;
        arrows[0].transform.localRotation = new Quaternion(0, 180, 0, 1);
        arrows[1].transform.localRotation = new Quaternion(0, 180, 0, 1);
    }

    public void scrollUpDone() {
        isOver = true;
        isDown = false;
    }
}
