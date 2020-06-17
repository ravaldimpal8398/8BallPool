using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using UnityEngine.SceneManagement;


public class InitMenuScript : MonoBehaviour {

    public GameObject menuCanvas;
    public GameObject tablesCanvas;
    public GameObject gameTitle;
 



    /*public void backToMenuFromTableSelect() {
        tablesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        gameTitle.SetActive(true);
    }*/

 

    public void playOffline() {
        GameManager.Instance.tableNumber = 0;
        GameManager.Instance.offlineMode = true;
        GameManager.Instance.roomOwner = true;
        SceneManager.LoadScene("GameScene");
    }

  

  

 
   /* public void setTableNumber() {
        GameManager.Instance.tableNumber = Int32.Parse(GameObject.Find("TextTableNumber").GetComponent<Text>().text);
    }*/


  

}
