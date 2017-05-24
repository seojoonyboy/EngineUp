using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public GameObject[] 
        effectSound,
        music;

    public void playEffect(int index) {
        effectSound[index].GetComponent<AudioSource>().Play();
    }

    public void playMusic() {
        
    }
}
