using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GameControllerScript : MonoBehaviour {

    private Image imageClock1;
    private Image imageClock2;

    private Animator messageBubble;
    private Text messageBubbleText;

    private int currentImage = 1;

    public float playerTime;

    public GameObject cueController;
    private CueController cueControllerScript;
    public GameObject shotPowerObject;
    private ShotPowerScript shotPowerScript;

    private float messageTime = 0;
   
    private bool timeSoundsStarted = false;

    int loopCount = 0;

    private float waitingOpponentTime = 0;
   
    void Start() {
      
        shotPowerScript = shotPowerObject.GetComponent<ShotPowerScript>();
        cueControllerScript = cueController.GetComponent<CueController>();
        playerTime = GameManager.Instance.playerTime;
        imageClock1 = GameObject.Find("AvatarClock1").GetComponent<Image>();
        imageClock2 = GameObject.Find("AvatarClock2").GetComponent<Image>();

      
       




        playerTime = playerTime * Time.timeScale;


     

        if (!GameManager.Instance.roomOwner)
            currentImage = 2;
    }

   
    void Update() {
        if (!GameManager.Instance.stopTimer) {
            updateClock();
        }
    }


    private void updateClock() {
        float minus;
        if (currentImage == 1) {
            playerTime = GameManager.Instance.playerTime;
            if (GameManager.Instance.offlineMode)
                playerTime = GameManager.Instance.playerTime + GameManager.Instance.cueTime;
            minus = 1.0f / playerTime * Time.deltaTime;

            imageClock1.fillAmount -= minus;

            if (imageClock1.fillAmount < 0.25f && !timeSoundsStarted) {
             
                timeSoundsStarted = true;
            }

            if (imageClock1.fillAmount == 0) {
           
                GameManager.Instance.stopTimer = true;
                shotPowerScript.resetCue();
              
                
                    GameManager.Instance.wasFault = true;
                    GameManager.Instance.cueController.setTurnOffline(true);
               


                GameManager.Instance.cueController.ShotPowerIndicator.deactivate();
                GameManager.Instance.cueController.ShotPowerIndicator.resetCue();

                GameManager.Instance.cueController.cueSpinObject.GetComponent<SpinController>().hideController();

                GameManager.Instance.cueController.whiteBallLimits.SetActive(false);
                GameManager.Instance.ballHand.SetActive(false);

              

                if (!GameManager.Instance.offlineMode) {
                    cueControllerScript.setOpponentTurn();
                }

            }

        } else {
          
            playerTime = GameManager.Instance.playerTime;
            if (GameManager.Instance.offlineMode)
             
            minus = 1.0f / playerTime * Time.deltaTime;
        

            if (GameManager.Instance.offlineMode && imageClock2.fillAmount < 0.25f && !timeSoundsStarted) {
               
                timeSoundsStarted = true;
            }

            if (imageClock2.fillAmount == 0) {
                GameManager.Instance.stopTimer = true;

            

                if (GameManager.Instance.offlineMode) {
                    GameManager.Instance.wasFault = true;
                    GameManager.Instance.cueController.setTurnOffline(true);
                }
            }
        }

    }

    public void showMessage(string message) {


    

        float timeDiff = Time.time - messageTime;

    

        if (timeDiff > 6) {
            messageBubbleText.text = message;
          
            messageTime = Time.time;
        } else {
            Debug.Log("Show message with delay");
            StartCoroutine(showMessageWithDelay(message, (6.0f - timeDiff) / 1.0f));
        }
    }

   

    IEnumerator showMessageWithDelay(string message, float delayTime) {
        yield return new WaitForSeconds(delayTime);

       
        messageTime = Time.time;

    }

    public IEnumerator updateMessageBubbleText() {
        yield return new WaitForSeconds(1.0f * 2);
        waitingOpponentTime -= 1;
        if (!GameManager.Instance.opponentDisconnected) {
          
        }
        if (waitingOpponentTime > 0 && !GameManager.Instance.opponentActive && !GameManager.Instance.opponentDisconnected) {
            StartCoroutine(updateMessageBubbleText());
        }
    }

    

    public void resetTimers(int currentTimer, bool showMessageBool) {

       
        timeSoundsStarted = false;
        imageClock1.fillAmount = 1;
        imageClock2.fillAmount = 1;

        this.currentImage = currentTimer;

        if (GameManager.Instance.offlineMode) {
            if (showMessageBool) {

           
            }

        } else {
            if (currentTimer == 1 && showMessageBool) {
                showMessage("It's your turn");
            }
        }




      

        GameManager.Instance.stopTimer = false;
    }


}
