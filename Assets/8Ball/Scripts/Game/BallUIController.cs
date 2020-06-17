using UnityEngine;
using System.Collections;

public class BallUIController : MonoBehaviour {

    // Use this for initialization
    private PotedBallsGUIController cont;
    void Start() {
        GameManager.Instance.tableNumber = 0;
        GameManager.Instance.offlineMode = true;
        GameManager.Instance.roomOwner = true;
        cont = GameObject.Find("PotedBallsGUI").GetComponent<PotedBallsGUIController>();
    }

    
    
    public void moveOthersAfterPot() {
        cont.moveOtherBalls();
    }


}
