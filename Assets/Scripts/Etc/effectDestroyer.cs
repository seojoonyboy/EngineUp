using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectDestroyer : MonoBehaviour {
    public void destroy() {
        Destroy(gameObject);
    }

    void OnDisable() {
        Destroy(gameObject);
    }
}
