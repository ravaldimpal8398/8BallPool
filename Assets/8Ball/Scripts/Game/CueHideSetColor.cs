using UnityEngine;
using System.Collections;


public class CueHideSetColor : MonoBehaviour {

    void Start() {

        Color color = Camera.main.backgroundColor;
        color.a = 1;
        GetComponent<SpriteRenderer>().color = color;
    }

}
