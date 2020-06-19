using UnityEngine;
using System.Collections;

public class CallPocketButtonController : MonoBehaviour {

    public GameObject calledPocket;
    private GameObject[] callPockets;
    public int potNumber;
    // Use this for initialization
    void Start() {
        callPockets = GameObject.FindGameObjectsWithTag("callPocket");
    }

    // Update is called once per frame
    void Update() {

    }

   
    void OnMouseDown() {
        Debug.Log("Called " + potNumber + " pocket");
        GameManager.Instance.calledPocketID = potNumber;
        GameManager.Instance.calledPocket = true;
        calledPocket.SetActive(true);
        for (int i = 0; i < callPockets.Length; i++) {
            callPockets[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            callPockets[i].SetActive(false);
        }
      
    }
}
