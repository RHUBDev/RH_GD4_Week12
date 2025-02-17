using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    public Vector2 inputDirection,lookDirection;
    Animator anim;
    private Vector3 touchStart, touchEnd;
    public Image dpad;
    public float dpadRadius = 50;
    private Touch theTouch;
    private int leftFingerID= -1;
    public float maxMoveSpeed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        //makes the character look down by default
        lookDirection = new Vector2(0, -1);

        dpadRadius = Screen.dpi / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //getting input from keyboard controls
        
        //calculateDesktopInputs();

        //calculateMobileInput();

        calculateCustomTouchInput();

        //sets up the animator
        animationSetup();

        //moves the player
        //transform.Translate(inputDirection * moveSpeed * Time.deltaTime);
    }


    void calculateDesktopInputs()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector2(x, y).normalized;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            attack();
        }

    }


    void animationSetup()
    {
        //checking if the player wants to move the character or not
        if (inputDirection.magnitude > 0.1f)
        {
            //changes look direction only when the player is moving, so that we remember the last direction the player was moving in
            lookDirection = inputDirection;

            //sets "isWalking" true. this triggers the walking blend tree
            anim.SetBool("isWalking", true);
            
            //moves the player
            transform.Translate(inputDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            // sets "isWalking" false. this triggers the idle blend tree
            anim.SetBool("isWalking", false);

        }

        //sets the values for input and lookdirection. this determines what animation to play in a blend tree
        anim.SetFloat("inputX", lookDirection.x);
        anim.SetFloat("inputY", lookDirection.y);
        anim.SetFloat("lookX", lookDirection.x);
        anim.SetFloat("lookY", lookDirection.y);
    }

    public void attack()
    {
        anim.SetTrigger("Attack");
    }

    void calculateMobileInput()
    {
        if(Input.GetMouseButton(0))
        {
            dpad.gameObject.SetActive(true);

            if(Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
            }

            touchEnd = Input.mousePosition;
            
            float x = touchEnd.x - touchStart.x;
            float y = touchEnd.y - touchStart.y;

            inputDirection = new Vector2(x, y).normalized;

            if((touchEnd - touchStart).magnitude > dpadRadius)
            {
                dpad.transform.position = touchStart + (touchEnd - touchStart).normalized * dpadRadius;
            }
            else
            {
                dpad.transform.position = touchEnd;
            }
        }
        else
        {
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
        }
    }

    void calculateTouchInput()
    {
        if(Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);
            dpad.gameObject.SetActive(true);

            if(theTouch.phase == TouchPhase.Began)
            {
                touchStart = theTouch.position;
            }
            else if(theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
            {
                touchEnd = theTouch.position;
            
            float x = touchEnd.x - touchStart.x;
            float y = touchEnd.y - touchStart.y;

            Vector2 inputDir = new Vector2(x, y).normalized;
            
            inputDirection = inputDir;

            if((touchEnd - touchStart).magnitude > dpadRadius)
            {
                dpad.transform.position = touchStart + (touchEnd - touchStart).normalized * dpadRadius;
            }
            else
            {
                dpad.transform.position = touchEnd;
            }
            }
        }
        else
        {
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
        }
    }

    void calculateCustomTouchInput()
    {
        if(Input.touchCount > 0)
        {
            //theTouch = Input.GetTouch(0);
            //dpad.gameObject.SetActive(true);
            
            for(int i = 0; i < Input.touchCount; i++)
            {
                if(Input.GetTouch(i).position.x < (Screen.width/2))
                {
                    if(leftFingerID == -1 &&  Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        Debug.Log("0");
                        theTouch = Input.GetTouch(i);
                        leftFingerID = Input.GetTouch(i).fingerId;
                        dpad.gameObject.SetActive(true);
                    }
                    //DoLeftTouch();

                    if(Input.GetTouch(i).phase == TouchPhase.Moved && Input.GetTouch(i).fingerId == leftFingerID)
                    {
                       theTouch = Input.GetTouch(i);
                       Debug.Log("1");
                      // DoLeftTouch();
                    }
                    if(Input.GetTouch(i).phase == TouchPhase.Ended && Input.GetTouch(i).fingerId == leftFingerID)
                    {
                        theTouch = Input.GetTouch(i);
                        Debug.Log("1.5");
                        leftFingerID = -1;
                        inputDirection = Vector2.zero;
                    }
                    DoLeftTouch();
                }
            }
        }
        else
        {
            
            Debug.Log("1.7");
            leftFingerID = -1;
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
        }
    }

    void DoLeftTouch()
    {
        if(theTouch.phase == TouchPhase.Began)
        {
            Debug.Log("2");
            touchStart = theTouch.position;
            dpad.transform.position = touchStart;
        }
        if(theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
        {
            
            Debug.Log("3");
            touchEnd = theTouch.position;
            
            //float x = touchEnd.x - touchStart.x;
            //float y = touchEnd.y - touchStart.y;

            //Vector2 inputDir = new Vector2(x, y).normalized;
            
            //inputDirection = inputDir;
            
            if((touchEnd - touchStart).magnitude > dpadRadius)
            {
                dpad.transform.position = touchStart + ((touchEnd - touchStart).normalized * dpadRadius);
            }
            else
            {
                dpad.transform.position = touchEnd;
            }
            float x = dpad.transform.position.x - touchStart.x;
            float y = dpad.transform.position.y - touchStart.y;

             Vector2 inputDir = new Vector2(x,y);

             inputDirection =  (inputDir / dpadRadius) * maxMoveSpeed;
             Debug.Log("inputDir = " + inputDir + ", inputDirection " + inputDirection);
        }
    }
}
