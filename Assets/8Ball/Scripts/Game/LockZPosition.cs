using UnityEngine;
using System.Collections;
using System;



public class LockZPosition : MonoBehaviour {


    private Rigidbody rigid;
    private bool poted = false;
    private float minVelocity = 0.005f;//0.002f;
    private float minAngularVelocity = 0.25f; //0.15f;
    public bool ballActive = true;
    public PhysicMaterial material;
    private PotedBallsGUIController potedBallsGUI;
    public GameObject youWonMessage;
    private bool ballMoved = false;
    private bool audioDisabled = false;
    
    public AudioSource[] audioSources;
    private bool ballInactivePlayedAudio = false;
    void Start() {
        audioSources = GetComponents<AudioSource>();
        rigid = GetComponent<Rigidbody>();

        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "tableLayer";
        potedBallsGUI = GameObject.Find("PotedBallsGUI").GetComponent<PotedBallsGUIController>();

    }

   
    bool ballInMovement = false;

    void Update() {
        if (ballActive && rigid.velocity.sqrMagnitude < minVelocity &&
            rigid.angularVelocity.magnitude < minAngularVelocity) {

            rigid.Sleep();
            ballInMovement = false;
        }

        if (!rigid.IsSleeping() && !rigid.velocity.Equals(Vector3.zero) && !rigid.angularVelocity.Equals(Vector3.zero))
            ballInMovement = true;

        if (ballMoved && !audioDisabled && rigid.velocity.sqrMagnitude < minVelocity &&
            rigid.angularVelocity.magnitude < minAngularVelocity) {
         
        }




    }


    private bool firstBallPot = false;

    void OnTriggerEnter(Collider other) {

        Debug.Log("TRIGGER: " + other.tag);

        if (!poted && other.tag.Contains("Pot")) {
            audioSources[1].Play();
            GameManager.Instance.ballTouchedBand++;

           
            if (!firstBallPot) {

                if (transform.tag.Equals("WhiteBall")) {
                   
                } else {
                    int ballNumber = System.Int32.Parse(transform.tag.Replace("Ball", ""));
                   
                    if (ballNumber == 8) {
                        
                        GameManager.Instance.iLost = true;
                    } else if (ballNumber < 8) {
                        firstBallPot = true;
                        GameManager.Instance.noTypesPotedSolid = true;
                    } else if (ballNumber > 8) {
                        firstBallPot = true;
                        GameManager.Instance.noTypesPotedStriped = true;
                    }
                }

            }


            Debug.Log("POT");

            if (transform.tag.Equals("WhiteBall")) {
                GameManager.Instance.wasFault = true;
               
                DisableWhiteBall();
            } else {
                int ballNumber = System.Int32.Parse(transform.tag.Replace("Ball", ""));
                if (GameManager.Instance.cueController.isServer) {




                    if (GameManager.Instance.playersHaveTypes) {

                        
                        if ((GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7) ||
                            (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)) {
                            if (ballNumber == 8) {
                                if (GameManager.Instance.callPocketBlack) {
                                    if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                        GameManager.Instance.iWon = true;
                                    } else {
                                        GameManager.Instance.iLost = true;
                                    }
                                }
                               
                                else {
                                    GameManager.Instance.iWon = true;
                                }
                            }
                        } else {


                            if (GameManager.Instance.ownSolids) {
                                if (ballNumber < 8) {
                                    if (GameManager.Instance.callPocketAll) {
                                        if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                            GameManager.Instance.validPotsCount++;
                                            GameManager.Instance.validPot = true;
                                        }
                                    } else {
                                        GameManager.Instance.validPotsCount++;
                                        GameManager.Instance.validPot = true;
                                    }
                                } else if (ballNumber == 8) {
                                    Debug.Log("Poted 8 ball - game over");
                                    GameManager.Instance.iLost = true;

                                }
                            } else {
                                if (ballNumber > 8) {
                                    if (GameManager.Instance.callPocketAll) {
                                        if (other.tag.Equals("Pot" + GameManager.Instance.calledPocketID)) {
                                            GameManager.Instance.validPotsCount++;
                                            GameManager.Instance.validPot = true;
                                        }

                                    } else {
                                        GameManager.Instance.validPotsCount++;
                                        GameManager.Instance.validPot = true;
                                    }
                                } else if (ballNumber == 8) {
                                    Debug.Log("Poted 8 ball - game over");
                                    GameManager.Instance.iLost = true;
                                }
                            }
                        }



                    } else {
                        if (ballNumber != 8) {
                            GameManager.Instance.validPotsCount++;
                            GameManager.Instance.validPot = true;
                        } else {
                            GameManager.Instance.iLost = true;
                        }
                    }


                }

                if (ballNumber < 8) {
                    GameManager.Instance.solidPoted++;
                } else if (ballNumber > 8) {
                    GameManager.Instance.stripedPoted++;
                }

                poted = true;
                DisableBall(transform.tag);
            }
         
        }
    }


    int ballNumber;
    void OnCollisionEnter(Collision collision) {

       

        if (transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
            Vector3 v = GetComponent<Rigidbody>().velocity;
            float velSum = Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
            audioSources[2].volume = velSum / 8.0f;
            audioSources[2].Play();
        }

        if (GameManager.Instance.firstBallTouched && transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
            GameManager.Instance.ballTouchedBand++;
        }

        if (collision.collider.tag.Contains("Ball") && transform.tag.Contains("Ball")) {

            if (ballActive) {
               
                if (audioSources.Length > 0) {
                    Vector3 v = GetComponent<Rigidbody>().velocity;

                    float velSum = Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);

                    Vector3 v2 = collision.gameObject.GetComponent<Rigidbody>().velocity;

                    float velSum2 = Mathf.Abs(v2.x) + Mathf.Abs(v2.y) + Mathf.Abs(v2.z);

                    if (velSum > velSum2) velSum = velSum2;

                    audioSources[0].volume = velSum / 10.0f;

                    audioSources[0].Play();

                }
            } else {
                if (!ballInactivePlayedAudio && audioSources.Length > 0) {
                    ballInactivePlayedAudio = true;
                    audioSources[0].volume = 0.6f;
                    audioSources[0].Play();
                }
            }



        }

        if (transform.tag.Equals("WhiteBall")) {
            if (collision.collider.tag.Contains("Ball") && !collision.collider.tag.Equals("WhiteBall") && GameManager.Instance.cueController.isServer) {
                
                if (!GameManager.Instance.firstBallTouched) {
                    GameManager.Instance.firstBallTouched = true;

                    ballNumber = System.Int32.Parse(collision.collider.tag.Replace("Ball", ""));

                    if (GameManager.Instance.playersHaveTypes) {
                        Debug.Log("Inside");

                       
                        if ((GameManager.Instance.ownSolids && GameManager.Instance.solidPoted >= 7) ||
                            (!GameManager.Instance.ownSolids && GameManager.Instance.stripedPoted >= 7)) {
                            if (ballNumber != 8) {
                                GameManager.Instance.wasFault = true;
                               
                            }
                        } else {
                            if (GameManager.Instance.ownSolids) {
                                if (ballNumber >= 8) {
                                  
                                    GameManager.Instance.wasFault = true;
                                   
                                }
                            } else {
                                if (ballNumber <= 8) {
                                   
                                    GameManager.Instance.wasFault = true;
                                  
                                }
                            }
                        }
                    }
                }
            }
        } else {
            if (transform.tag.Contains("Ball") && collision.collider.tag.Equals("bumper")) {
                if (!GameManager.Instance.ballTouchBeforeStrike.Contains(transform.tag))
                    GameManager.Instance.ballTouchBeforeStrike.Add(transform.tag);
            }
        }
    }

    private void DisableWhiteBall() {


        Debug.Log("Disable White ball");
       
        ballActive = false;
        rigid.constraints = RigidbodyConstraints.None;
        Vector3 vel = rigid.velocity;
      ;
        vel.z = 5.0f;
     
        rigid.velocity = vel;

        

        Invoke("deactiveWhite", 1.0f);
     

    }

    private void DisableBall(string ii) {



       

        int i = 0;

        try {
            i = int.Parse(ii.Replace("Ball", ""));
        } catch (System.Exception e) { }

        if (i > 0 && i != 8) {
            potedBallsGUI.hidePotedBall(i - 1);
        }

        GetComponent<SphereCollider>().material = material;
        ballActive = false;
       
        rigid.constraints = RigidbodyConstraints.None;
        Vector3 vel = rigid.velocity;
       
        vel.z = 5.0f;
       
        rigid.velocity = vel;
        Invoke("disableMeshRenderer", 1.0f);
        Invoke("showMessage", 3.5f);

    }



    public void showMessage() {



        float timeDiff = Time.time - GameManager.Instance.messageTime;

     


        if (timeDiff > 2) {
            movePosition();
            GameManager.Instance.messageTime = Time.time;
        } else {
          
            StartCoroutine(showMessageWithDelay((2.0f - timeDiff) / 1.0f));
        }
    }

    IEnumerator showMessageWithDelay(float delayTime) {
        yield return new WaitForSeconds(delayTime);



        movePosition();
        GameManager.Instance.messageTime = Time.time;

    }


    public void EnableBall() {

    }

    private void deactiveWhite() {
        gameObject.SetActive(false);
    }



    private void disableMeshRenderer() {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void movePosition() {
     
      

        rigid.Sleep();
        GetComponent<MeshRenderer>().enabled = true;
      
        rigid.transform.position = new Vector3(5.61f, 1.317f, 5.45f);
      
        Invoke("setBallMoved", 1.0f);
    }

    private void setBallMoved() {
        ballMoved = true;
    }


}
