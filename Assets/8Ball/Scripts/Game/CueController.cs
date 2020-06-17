using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CueController : MonoBehaviour
{

    public GameObject cue;
    public GameObject posDetector;

    public UnityEngine.UI.Text text;

    private Rigidbody myRigidbody;
    public bool isServer;
    private LineRenderer lineRenderer;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;
    public GameObject ShotPowerInd;
    public ShotPowerScript ShotPowerIndicator;
    public GameObject targetLine;
    private GameObject circle;
    private RaycastHit hitInfo = new RaycastHit();
    private Ray ray = new Ray();
    public LayerMask layerMask;
    private Vector3 endPosition;
    private float radius;
    private bool displayTouched = false;
    private bool isFirstTouch = false;
    public int steps = 0;
    public bool shouldShot = false;
    private bool firstShotDone = false;
    public int fixedCounter = 0;
    public bool startShoot = false;
    public bool dataSent = false;
    private bool canCount;
    public bool ballsInMovement = false;
    public bool canRun = false;
    private ArrayList multiplayerDataArray = new ArrayList();
    private bool startMovement;
    private int counterFixPositionMulti = 0;
    private Vector3 multiFirstShotPower;
    private Vector3 multiFirstShotDirection;
    private Vector3 circleShotPos = Vector3.zero;
    private Vector3 initShotPos = Vector3.zero;
    private bool firstShot = true;
  
    private Vector3 shotDirection;
    private bool ballCollideFirst;
    public Vector3 trickShotAdd = Vector3.zero;
    public bool spinShowed;
    public GameObject cueSpinObject;
    public bool opponentShotStart = false;
    public bool opponentBallsStoped = false;
    public bool shotMyTurnDone = false;
    private bool raisedSixEvent = false;
    public GameObject gameControllerObject;
    private GameControllerScript gameControllerScript;
    private bool movingWhiteBall = false;
    public GameObject whiteBallLimits;
    public GameObject ballHand;
    public GameObject youWonMessage;
    public PotedBallsGUIController potedBallsGUI;
    public GameObject whiteBallTrigger;
    public GameObject[] callPocketsButtons;
    public GameObject[] calledPockets;
    private bool canShowControllers = true;
    public GameObject prizeText;
    private AudioSource[] audioSources;
    public GameObject audioController;
    public GameObject invitiationDialog;
    public GameObject wrongBall;
    public Sprite[] cueTextures;
    public GameObject chatButton;
    public bool opponentResumed = false;
    private Vector3 touchMousePos;
    void Start()
    {



        if(GameManager.Instance.roomOwner) {
            changeCueImage(GameManager.Instance.cueIndex);
        } else {
            changeCueImage(GameManager.Instance.opponentCueIndex);
        }

        audioSources = GetComponents<AudioSource>();

        GameManager.Instance.iWon = false;
        GameManager.Instance.iLost = false;


       

        potedBallsGUI = GameObject.Find("PotedBallsGUI").GetComponent<PotedBallsGUIController>();
        GameManager.Instance.ballHand = ballHand;
        GameManager.Instance.cueController = this;
        ShotPowerIndicator = ShotPowerInd.GetComponent<ShotPowerScript>();
        gameControllerScript = gameControllerObject.GetComponent<GameControllerScript>();
        GameManager.Instance.gameControllerScript = gameControllerScript;
        circle = GameObject.Find("Circle");
        targetLine = GameObject.Find("TargetLine");
        lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
        lineRenderer2 = GameObject.Find("Line2").GetComponent<LineRenderer>();
        lineRenderer3 = GameObject.Find("Line3").GetComponent<LineRenderer>();


        
        if (GameManager.Instance.tableNumber > 6)
        {
            GameManager.Instance.showTargetLines = false;
        }
        else
        {
            GameManager.Instance.showTargetLines = true;
        }


       
        if (GameManager.Instance.tableNumber > 1)
        {
            GameManager.Instance.callPocketBlack = true;
        }
        else
        {
            GameManager.Instance.callPocketBlack = false;
        }


       
        if (GameManager.Instance.tableNumber > 3)
        {
            GameManager.Instance.callPocketAll = true;
        }
        else
        {
            GameManager.Instance.callPocketAll = false;
        }

        if (!GameManager.Instance.showTargetLines)
        {
            lineRenderer2.enabled = false;
            lineRenderer3.enabled = false;
        }

        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;


        myRigidbody = GetComponent<Rigidbody>();

      
        Vector3 cueInitialPos = transform.position;
        cueInitialPos.x = cueInitialPos.x;
        cueInitialPos.z = cue.transform.position.z;
        cue.transform.position = cueInitialPos;

        isServer = false;




        if (GameManager.Instance.roomOwner)
        {
            ballHand.SetActive(true);
            GameManager.Instance.hasCueInHand = true;
            isServer = true;
            opponentBallsStoped = true;
           
        }

        canCount = false;  

    }


    public void changeCueImage(int index) {
        cue.GetComponent<SpriteRenderer>().sprite = cueTextures[index];
    }

  
    public void removeOnEventCall()
    {
     
    }


    void Update()
    {

        

        if (!spinShowed && canCheckAnotherCueRotation)
            StartCoroutine(rotateCue());
        

        

        if(!isServer) {
            drawTargetLines();
        }



        if (Input.GetMouseButtonDown(0))
        {
            steps = 0;
            isFirstTouch = true;
            displayTouched = true;
            touchMousePos = Vector3.zero;
        }


        if (Input.GetMouseButtonUp(0))
        {
            
            canRun = true;
            displayTouched = false;
        }


    }

    void FixedUpdate()
    {
        checkFirstCollision();
        shotOpponentTurn();
        shotMyTurn();

    }

    private void checkFirstCollision()
    {
       

        if (ballCollideFirst && gameObject.layer == 11)
        {
            RaycastHit hitInfo;


            if (Physics.SphereCast(initShotPos, radius, shotDirection, out hitInfo, Vector3.Distance(initShotPos, transform.position), layerMask.value))
            {

                if (!hitInfo.transform.tag.Equals(transform.tag))
                {

                    if (!hitInfo.collider)
                        return;

                    if (hitInfo.collider.isTrigger)
                    {
                       

                    }

                    if (!hitInfo.collider.isTrigger)
                    {
                      

                        Vector3 fixedPos = circleShotPos;
                        fixedPos.z = transform.position.z;
                        myRigidbody.transform.position = fixedPos;
                        gameObject.layer = 8;
                       
                    }

                }
            }
        }
    }

    public void callPocket()
    {
       
        for (int i = 0; i < callPocketsButtons.Length; i++)
        {
            callPocketsButtons[i].SetActive(true);
            callPocketsButtons[i].GetComponent<Animator>().Play("CallPocketShowAnimation");
        }
    }

    public void hidePocketButtons()
    {
        for (int i = 0; i < callPocketsButtons.Length; i++)
        {
            callPocketsButtons[i].SetActive(false);
        }
    }

    private void shotMyTurn()
    {

       

        if (GameManager.Instance.opponentActive && !GameManager.Instance.stopTimer && shouldShot && steps > 0 && isServer)
        {
            shotMyTurnDone = true;
          

            for (int i = 0; i < GameManager.Instance.balls.Length; i++)
            {
                GameManager.Instance.balls[i].GetComponent<Rigidbody>().maxAngularVelocity = 150;
            }

         

            startShoot = false;

            opponentBallsStoped = false;

            myRigidbody.velocity = Vector3.zero;
            Vector3 dir = transform.position - posDetector.transform.position;
            dir = dir.normalized;
            dir.z = 0;

            Vector3 trickShot = transform.position;
            trickShot = trickShot + trickShotAdd;
           

            float multipleBy = 0.03f + (GameManager.Instance.cuePower + 1) * 0.001f;



            Vector3 shotPower = dir * steps * multipleBy;

            if (ballCollideFirst)
                gameObject.layer = 11;

            initShotPos = transform.position;
            circleShotPos = circle.transform.position;
            shotDirection = dir;


            float velSum = Mathf.Abs(shotPower.x) + Mathf.Abs(shotPower.y) + Mathf.Abs(shotPower.z);
            audioSources[audioSources.Length - 1].volume = velSum / 1.8f;
            audioSources[audioSources.Length - 1].Play();

           

            

            myRigidbody.AddForceAtPosition(shotPower, trickShot, ForceMode.Impulse);
            ballsInMovement = true;


            GameManager.Instance.hasCueInHand = false;



            byte evCode = 0;
            Vector3[] content = new Vector3[] { shotPower, trickShot };
        

            targetLine.SetActive(false);

            GameManager.Instance.stopTimer = true;

           
            dataSent = true;
            shouldShot = false;
        }
    }


    private void shotOpponentTurn()
    {

        
        if (startMovement)
        {
            if (!firstShotDone)
            {
              
                firstShotDone = true;
                float velSum = Mathf.Abs(multiFirstShotPower.x) + Mathf.Abs(multiFirstShotPower.y) + Mathf.Abs(multiFirstShotPower.z);
                audioSources[audioSources.Length - 1].volume = velSum / 1.8f;
                audioSources[audioSources.Length - 1].Play();
               
            }
            else
            {
                
                if (fixedCounter == 1)
                {


                    if (multiplayerDataArray.Count > 0)
                    {


                        Vector3[] balls = (Vector3[])multiplayerDataArray[0];
                        multiplayerDataArray.RemoveAt(0);
                        GameObject spawnBalls = GameObject.Find("SpawnBalls");
                        SpawnBallsScript script = spawnBalls.GetComponent<SpawnBallsScript>();
                        for (int i = 0; i <= balls.Length - 3; i += 3)
                        {

                            if (!isServer /*|| shotMyTurnDone == false*/)
                            {
                                float dist = Vector3.Distance(balls[i], script.balls[i / 3].transform.position);

                                script.balls[i / 3].GetComponent<Rigidbody>().velocity = balls[i + 1];
                                script.balls[i / 3].GetComponent<Rigidbody>().angularVelocity = balls[i + 2];
                                script.balls[i / 3].transform.position = balls[i];
                            }

                        }

                    }
                    else
                    {

                        startMovement = false;

                    }


                }

                if (canCount)
                    fixedCounter++;
                if (fixedCounter > 15)
                    fixedCounter = 0;
            }
        }
    }


  
 
    List<String> collides = new List<String>();
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag.Contains("Ball"))
            collides.Add(other.tag);
    }


    public void checkShot() {
        
        if (GameManager.Instance.iWon)
            {

                if(!GameManager.Instance.wasFault) {
                    HideAllControllers();
                    GameManager.Instance.audioSources[3].Play();
                    youWonMessage.SetActive(true);
                    youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
                   
                } else {
                    HideAllControllers();
                    GameManager.Instance.audioSources[3].Play();
                    youWonMessage.SetActive(true);
                  
                }
                
            }
            else if (GameManager.Instance.iLost)
            {
                HideAllControllers();
                GameManager.Instance.audioSources[3].Play();
                youWonMessage.SetActive(true);
              
            }
            else
            {
                bool changedWhitePos = false;




                if (!GameManager.Instance.ballsStriked && !GameManager.Instance.validPot && GameManager.Instance.ballTouchBeforeStrike.Count < 4)
                {
                    GameManager.Instance.wasFault = true;
                    
                    

                   
                    GameManager.Instance.ballsStriked = true;
                
                    GameManager.Instance.ballTouchBeforeStrike.Clear();




                }
                else
                {
                    
                    GameManager.Instance.ballsStriked = true;
               

                    

                    if (!GameManager.Instance.firstBallTouched)
                    {
                        GameManager.Instance.wasFault = true;
                      
                    }

                    if (GameManager.Instance.firstBallTouched && GameManager.Instance.ballTouchedBand == 0)
                    {
                        GameManager.Instance.wasFault = true;
                     

                    }

                    if (!GameManager.Instance.validPot)
                    {
                        GameManager.Instance.wasFault = true;
                        if (GameManager.Instance.faultMessage.Length == 0)
                            GameManager.Instance.faultMessage = "";
                    }

                  
                }






                opponentBallsStoped = true;
                if(GameManager.Instance.offlineMode) {

                } else {
                    if (GameManager.Instance.wasFault)
                    {
                        if (GameManager.Instance.faultMessage.Length > 0)
                            GameManager.Instance.audioSources[2].Play();
                        setOpponentTurn();
                    }
                    else
                    {
                        StartCoroutine(setMyTurn(false));
                    }
                }

                
            }
    }

    private void getNewWhiteBallPosition(bool center)
    {
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().maxAngularVelocity = 150;

      

        bool collides = true;


        Vector3 newPos = new Vector3(-0f, -0.69f, -0.24f);


        if (!center)
            newPos = new Vector3(-2.9f, -0.69f, -0.24f);


        while (collides)
        {
            collides = false;
            for (int i = 1; i < GameManager.Instance.balls.Length; i++)
            {
                if (Vector2.Distance(GameManager.Instance.balls[i].transform.position, newPos) < 0.38f)
                {
                    collides = true;
                    break;
                }
            }

            if (!collides)
            {
                GameManager.Instance.whiteBall.GetComponent<Rigidbody>().transform.position = newPos;
                GameManager.Instance.whiteBall.GetComponent<LockZPosition>().ballActive = true;
                GameManager.Instance.whiteBall.SetActive(true);
                Vector3 newBallPos1 = GameManager.Instance.whiteBall.transform.position;
                newBallPos1.z = ballHand.transform.position.z;
                ballHand.transform.position = newBallPos1;
            }
            else
            {
                newPos.x -= 0.1f;
            }
        }

        GameManager.Instance.whiteBall.GetComponent<Rigidbody>().transform.position = newPos;
        GameManager.Instance.whiteBall.GetComponent<LockZPosition>().CancelInvoke();
    }



    void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Ball"))
            collides.Remove(other.tag);
    }




    void OnMouseDown()
    {
        if (GameManager.Instance.hasCueInHand)
        {
            ballHand.SetActive(false);
            HideAllControllers();
            movingWhiteBall = true;
            if (!GameManager.Instance.ballsStriked)
            {
                whiteBallLimits.SetActive(true);
             
            }
            else
            {
               
            }
        }
    }

    Vector3 lastCorrectPosition;
    void OnMouseDrag()
    {
       
        if (GameManager.Instance.hasCueInHand && isServer)
        {
            bool canMove = true;
            Vector3 collPos = Vector3.zero;
            float distance = 0.0f;

            Debug.Log(Vector2.Distance(GameManager.Instance.balls[1].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            for (int i = 1; i < GameManager.Instance.balls.Length; i++)
            {
                if (Vector2.Distance(GameManager.Instance.balls[i].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.38f)
                {
                    canMove = false;
                    collPos = GameManager.Instance.balls[i].transform.position;
                    distance = Vector2.Distance(GameManager.Instance.balls[i].transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;
                }
            }

            if (canMove)
            {

                float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
                Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));


                float newX = transform.position.x;
                float newY = transform.position.y;


                if (GameManager.Instance.ballsStriked)
                {
                    if (pos_move.x > -5.165136f && pos_move.x < 5.202874f)
                    {
                        newX = pos_move.x;
                    }
                    else
                    {
                        if (pos_move.x < -5.165136f)
                        {
                            newX = -5.165136f;
                        }
                        else
                        {
                            newX = 5.202874f;
                        }
                    }
                }
                else
                {
                    if (pos_move.x > -5.165136f && pos_move.x < -2.735605f)
                    {
                        newX = pos_move.x;
                    }
                    else
                    {
                        if (pos_move.x < -5.165136f)
                        {
                            newX = -5.165136f;
                        }
                        else
                        {
                            newX = -2.735605f;
                        }
                    }
                }

                if (pos_move.y > -3.097622f && pos_move.y < 1.804678f)
                {
                    newY = pos_move.y;
                }
                else
                {
                    if (pos_move.y < -3.097622f)
                    {
                        newY = -3.097622f;
                    }
                    else
                    {
                        newY = 1.804678f;
                    }
                }

                lastCorrectPosition = transform.position;
                transform.position = new Vector3(newX, newY, transform.position.z);
               
            }
        }
    }

    void OnMouseUp()
    {
        if (GameManager.Instance.hasCueInHand)
        {

            ShowAllControllers();
            movingWhiteBall = false;

            whiteBallLimits.SetActive(false);
          
        }
    }

    private void hideCalledPockets()
    {
        for (int i = 0; i < calledPockets.Length; i++)
        {
            calledPockets[i].SetActive(false);
        }
    }


    public void setTurnOffline(bool showTurnMessage) {

        checkShot();

      

        hidePocketButtons();

        if(GameManager.Instance.wasFault) {
            if(GameManager.Instance.faultMessage.Length > 0)
                GameManager.Instance.gameControllerScript.showMessage(GameManager.Instance.faultMessage);
            Debug.Log("Fault: " + GameManager.Instance.faultMessage);
            if(GameManager.Instance.offlinePlayerTurn == 1) {
                if(GameManager.Instance.offlinePlayer1OwnSolid) {
                    GameManager.Instance.ownSolids = false;
                } else {
                    GameManager.Instance.ownSolids = true;
                }
                GameManager.Instance.offlinePlayerTurn = 2;
                if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) 
                    gameControllerScript.resetTimers(2, showTurnMessage);
                
            } else {
                if(GameManager.Instance.offlinePlayer1OwnSolid) {
                    GameManager.Instance.ownSolids = true;
                } else {
                    GameManager.Instance.ownSolids = false;
                }
                GameManager.Instance.offlinePlayerTurn = 1;
                if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) 
                    gameControllerScript.resetTimers(1, showTurnMessage);
                
            }

           

            if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
                ballHand.SetActive(true);

                GameManager.Instance.hasCueInHand = true;

                Vector3 newBallPos = GameManager.Instance.whiteBall.transform.position;
                newBallPos.z = ballHand.transform.position.z;
                ballHand.transform.position = newBallPos;
            }
           
        } else {
            if (!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
                gameControllerScript.resetTimers(GameManager.Instance.offlinePlayerTurn, false);
                Debug.Log("Turn: " + GameManager.Instance.offlinePlayerTurn);
            }
                
        }

        
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
        ballsInMovement = false;
        GameManager.Instance.firstBallTouched = false;
        GameManager.Instance.ballTouchedBand = 0;
        GameManager.Instance.wasFault = false;
        GameManager.Instance.faultMessage = "";
        GameManager.Instance.validPot = false;
        isServer = true;
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.calledPocket = true;
        canShowControllers = true;

        if(!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
            Invoke("ShowAllControllers", 0.25f);
            ShotPowerIndicator.anim.Play("MakeVisible");
        }
            
    }

    public IEnumerator setMyTurn(bool showTurnMessage)
    {

        Debug.Log("My turn");

        GameManager.Instance.audioSources[0].Play();

        changeCueImage(GameManager.Instance.cueIndex);

        hidePocketButtons();
        gameControllerScript.resetTimers(1, showTurnMessage);
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
        ballsInMovement = false;
        GameManager.Instance.firstBallTouched = false;
        GameManager.Instance.ballTouchedBand = 0;
        GameManager.Instance.wasFault = false;
        GameManager.Instance.validPot = false;
        isServer = true;
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.calledPocket = true;

        if (GameManager.Instance.callPocketAll)
        {
            GameManager.Instance.calledPocket = false;
            canShowControllers = false;
            callPocket();
        }


        if (GameManager.Instance.callPocketBlack)
        {
            if (GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7)
            {
                GameManager.Instance.calledPocket = false;
                canShowControllers = false;
                callPocket();
            }
            else if (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)
            {
                GameManager.Instance.calledPocket = false;
                canShowControllers = false;
                callPocket();
            }
        }


        while (GameManager.Instance.calledPocket == false)
        {
            Debug.Log("CALLED: " + GameManager.Instance.calledPocket);
            yield return new WaitForSeconds(0.2f);
        }

        canShowControllers = true;

        if(!GameManager.Instance.iWon && !GameManager.Instance.iLost) {
            Invoke("ShowAllControllers", 0.25f);
            ShotPowerIndicator.anim.Play("MakeVisible");
        }

    }

    public void setOpponentTurn()
    {
        int oppoCueIndex = GameManager.Instance.opponentCueIndex;
        if(GameManager.Instance.opponentCueIndex >= cueTextures.Length - 1) {
            oppoCueIndex = cueTextures.Length-1;
        }
        changeCueImage(oppoCueIndex);

        hidePocketButtons();
        canShowControllers = true;
        hideCalledPockets();
        GameManager.Instance.noTypesPotedSolid = false;
        GameManager.Instance.noTypesPotedStriped = false;
      
        cueSpinObject.GetComponent<SpinController>().resetPositions();
        ballsInMovement = false;
      
        cue.GetComponent<Renderer>().enabled = true;
        Invoke("ShowAllControllers", 0.25f);
        isServer = false;
        multiplayerDataArray.Clear();
        startMovement = false;
        ShotPowerIndicator.anim.Play("ShotPowerAnimation");
        gameControllerScript.resetTimers(2, true);
    }

    private void setCuePosition()
    {
        
    }

    private void ShowAllControllers()
    {

        if (canShowControllers)
        {
         
            targetLine.SetActive(true);
            cue.transform.position = new Vector3(GameManager.Instance.balls[0].transform.position.x, GameManager.Instance.balls[0].transform.position.y, cue.transform.position.z);
            cue.GetComponent<Renderer>().enabled = true;
        }

    }

    public void HideAllControllers()
    {
        targetLine.SetActive(false);
        cue.GetComponent<Renderer>().enabled = false;
    }

    public void stopTimer()
    {
        GameManager.Instance.stopTimer = true;
    }


    public void noMoreTime()
    {

    }

    private bool canCheckAnotherCueRotation = true;

    int aa;

    private IEnumerator rotateCue()
    {
        canCheckAnotherCueRotation = false;
      

        if (!movingWhiteBall && !ballsInMovement && isServer)
        {
            float ang = 0;

            Quaternion initRot = cue.transform.rotation;
            Quaternion rot = cue.transform.rotation;

            
            

            if (!ShotPowerIndicator.mouseDown && displayTouched & !isFirstTouch)
            {
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Mouse Y")))
                {
                    ang = Input.GetAxis("Mouse X");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > transform.position.y)
                    {
                        ang = -ang;
                    }
                }
                else
                {
                    ang = Input.GetAxis("Mouse Y");
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
                    {
                        ang = -ang;
                    }
                }

                float multAng = Vector2.Distance(touchMousePos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                multAng *= 300.0f;

                if(multAng < 1.5f) multAng = 1.5f;

            
                multAng = multAng * 0.05f;

                
                if(multAng > 6.0f) multAng = 6.0f;



                ang *=  multAng;

                touchMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                isFirstTouch = false;
            }



           


            yield return new WaitForSeconds(0.01f);

            if (displayTouched) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + ang);
                cue.transform.rotation = rot;


                if (isServer && initRot.eulerAngles != rot.eulerAngles) {
                  
                }
            }

            canCheckAnotherCueRotation = true;

            drawTargetLines();
        } else {
            canCheckAnotherCueRotation = true;
        }
    }

    private IEnumerator rotateCueInvoke(Quaternion rot, Quaternion initRot) {

        yield return new WaitForSeconds(0.1f);

        if (displayTouched) {

            cue.transform.rotation = rot;

            

            if (isServer && initRot.eulerAngles != rot.eulerAngles) {
              
            }
        } 

        yield return true;
    }

    // DRAW TARGET LINES
    private void drawTargetLines()
    {
        
        if (!ballsInMovement)
        {
            Vector3 dir = transform.position - posDetector.transform.position;
            dir.z = 0;
            dir = dir.normalized;

            Vector3 linePos = transform.position;
            linePos.z = -3;

            lineRenderer.SetPosition(0, linePos);

            ray.origin = transform.position;
            ray.direction = dir;

            if (Physics.SphereCast(ray, radius, out hitInfo, 10, layerMask))
            {

                endPosition = ray.origin + (ray.direction.normalized * hitInfo.distance);
                endPosition.z = -3;


                circle.transform.position = endPosition;
                Vector3 linePos1 = endPosition;
                linePos1.z = -3;
                lineRenderer.SetPosition(1, linePos1);


                if (hitInfo.transform.tag.Contains("Ball"))
                {
                    //my line rendarar
                    int ballNumber = System.Int32.Parse(hitInfo.transform.tag.Replace("Ball", ""));
                    if (GameManager.Instance.playersHaveTypes)
                    {
                        if (isServer)
                        {
                            if ((GameManager.Instance.ownSolids && ballNumber < 8) || (!GameManager.Instance.ownSolids && ballNumber > 8) || ballNumber == 8)
                            {
                                drawLine(linePos);
                            }
                            else
                            {
                                wrongBall.SetActive(true);
                                // all false
                                circle.GetComponent<LineRenderer>().enabled = false;
                                ballCollideFirst = false;
                                lineRenderer2.enabled = false;
                                lineRenderer3.enabled = false;
                            }
                        }
                        else
                        {
                            if ((!GameManager.Instance.ownSolids && ballNumber < 8) || (GameManager.Instance.ownSolids && ballNumber > 8) || ballNumber == 8)
                            {
                                drawLine(linePos);
                            }
                            else
                            {
                                wrongBall.SetActive(true);
                                circle.GetComponent<LineRenderer>().enabled = false;
                                ballCollideFirst = false;
                                lineRenderer2.enabled = false;
                                lineRenderer3.enabled = false;
                            }
                        }

                    }
                    else
                    {
                        drawLine(linePos);
                    }


                }
                else
                {
                    wrongBall.SetActive(false);
                    circle.GetComponent<LineRenderer>().enabled = true;
                    ballCollideFirst = false;
                    lineRenderer2.enabled = false;
                    lineRenderer3.enabled = false;
                }
            }
        }
    }

    public void drawLine(Vector3 linePos)
    {
        wrongBall.SetActive(false);
        circle.GetComponent<LineRenderer>().enabled = true;
        Vector3 hitBallPosition = hitInfo.transform.position;
        hitBallPosition.z = -3;
        lineRenderer3.SetPosition(0, hitBallPosition);

        Vector3 r2dir = hitBallPosition - endPosition;

        Ray r2 = new Ray(hitBallPosition, r2dir);

        Vector3 pos3 = r2.origin + (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
        pos3.z = -3;
        lineRenderer3.SetPosition(1, pos3);

        Vector3 l = (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
        l = Quaternion.Euler(0, 0, -90) * l + endPosition;
        l.z = -3;
        lineRenderer2.SetPosition(0, endPosition);
        float angle = 90.0f;
        float angleBeetwen = AngleBetweenThreePoints(linePos, endPosition, l);

        if (angleBeetwen < 90.0f || angleBeetwen > 270.0f)
        {
            l = (3 + GameManager.Instance.cueAim * 0.25f) * r2dir;
            l = Quaternion.Euler(0, 0, 90) * l + endPosition;
            l.z = -3;
        }

        lineRenderer2.SetPosition(1, l);
        ballCollideFirst = true;


        if (GameManager.Instance.showTargetLines)
        {
            lineRenderer2.enabled = true;
            lineRenderer3.enabled = true;
        }
    }

    public float AngleBetweenThreePoints(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float a = pointB.x - pointA.x;
        float b = pointB.y - pointA.y;
        float c = pointB.x - pointC.x;
        float d = pointB.y - pointC.y;

        float atanA = Mathf.Atan2(a, b) * Mathf.Rad2Deg;
        float atanB = Mathf.Atan2(c, d) * Mathf.Rad2Deg;

        float output = atanB - atanA;
        output = Mathf.Abs(output);



        return output;
    }
}
