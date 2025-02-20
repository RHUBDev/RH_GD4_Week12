using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    public Vector2 inputDirection,lookDirection;
    Animator anim;
    private Vector3 touchStart, touchEnd;
    public Image dpad;
    public Image dpadBack;
    public float dpadRadius = 50;
    private Touch theTouch;
    private int leftFingerID= -1;
    public float maxMoveSpeed = 1.5f;
    public TMP_Text pickupMessage;
    public TMP_Text healthMessage;
    public TMP_Text manaMessage;
    private float health = 50;
    private float maxHealth = 100;
    private float mana = 50;
    private float maxMana = 100;
    public Rigidbody2D rig;
    public float rigMoveSpeed = 0.01f;
    PlayerInput _playerInput;
    InputAction moveAction;
    InputAction attackAction;
    private float hitForce = 5f;
    [SerializeField] GameObject[] colls;
    [SerializeField] GameObject attackButton;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

#if UNITY_STANDALONE || UNITY_EDITOR
        attackButton.SetActive(false);
#endif
        //makes the character look down by default
        lookDirection = new Vector2(0, -1);

        dpadRadius = Screen.dpi / 2;

        _playerInput = GetComponent<PlayerInput>();
        moveAction = _playerInput.actions.FindAction("Move");
        attackAction = _playerInput.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        //getting input from keyboard controls
        
        #if UNITY_STANDALONE || UNITY_EDITOR
        calculateDesktopInputs();
        //calculateCustomTouchInput();
        //calculateMobileInput();
        #else
        calculateCustomTouchInput();
        #endif

        //moves the player
        //moved to animationSetup()
        //transform.Translate(inputDirection * moveSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {   
        //sets up the animator
        animationSetup();
    }

    public void Pickup(potionInfo potion)
    {
        pickupMessage.text = potion.message;
        
        health += potion.addHealth;
        mana += potion.addMana;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        if(mana > maxMana)
        {
            mana = maxMana;
        }
        healthMessage.text = "Health: "+ health;
        manaMessage.text = "Mana: "+ mana;
    }

    void calculateDesktopInputs()
    {
        //float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");

        //Vector2 inputDir = new Vector2(x, y).normalized;

        Vector2 inputDir = moveAction.ReadValue<Vector2>();

        inputDirection = inputDir * maxMoveSpeed;

        //if(Input.GetKeyDown(KeyCode.Space))
        if(attackAction.WasPressedThisFrame())
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
            //transform.Translate(inputDirection * moveSpeed * Time.deltaTime);
            rig.MovePosition(rig.position + inputDirection * moveSpeed * Time.fixedDeltaTime);
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

        StartCoroutine(DoAttack());
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

    /*void calculateTouchInput()
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
    }*/

    /*void calculateCustomTouchInput()
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
                        theTouch = Input.GetTouch(i);
                        leftFingerID = Input.GetTouch(i).fingerId;
                        dpad.gameObject.SetActive(true);
                        dpadBack.gameObject.SetActive(true);
                    }
                    //DoLeftTouch();

                    if(Input.GetTouch(i).phase == TouchPhase.Moved && Input.GetTouch(i).fingerId == leftFingerID)
                    {
                       theTouch = Input.GetTouch(i);
                      // DoLeftTouch();
                    }
                    if(Input.GetTouch(i).phase == TouchPhase.Ended && Input.GetTouch(i).fingerId == leftFingerID)
                    {
                        theTouch = Input.GetTouch(i);
                        leftFingerID = -1;
                        inputDirection = Vector2.zero;
                    }
                    DoLeftTouch();
                }
            }
        }
        else
        {
            leftFingerID = -1;
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
            dpadBack.gameObject.SetActive(false);
        }
    }*/

   /* void DoLeftTouch()
    {
        if(theTouch.phase == TouchPhase.Began)
        {
            touchStart = theTouch.position;
            dpad.transform.position = touchStart;
            dpadBack.transform.position = touchStart;
        }
        if(theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
        {
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
        }
    }*/

    IEnumerator ResetPickupText()
    {
        yield return new WaitForSeconds(2);
        pickupMessage.text = "";
    }

    public void TakeDamage(Vector3 dir)
    {
        if (health > 0)
        {
            health -= 20;
            healthMessage.text = "Health: " + health;
            rig.AddForce(dir * hitForce, ForceMode2D.Impulse);

            if (health <= 0)
            {
                health = 0;
                healthMessage.text = "Health: " + health;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    IEnumerator DoAttack()
    {
        yield return new WaitForEndOfFrame();

        GameObject coll = null;

        AnimatorClipInfo[] animInfo = anim.GetCurrentAnimatorClipInfo(0);
        Debug.Log("clip name = " + animInfo[0].clip.name);

        if (animInfo[0].clip.name == "Attack_Right")
        {
            coll = colls[0];
        }
        else if (animInfo[0].clip.name == "Attack_Left")
        {
            coll = colls[1];
        }
        else if (animInfo[0].clip.name == "Attack_Up")
        {
            coll = colls[2];
        }
        else if (animInfo[0].clip.name == "Attack_Down")
        {
            coll = colls[3];
        }

        if (coll != null)
        {
            coll.SetActive(true);
            yield return new WaitForSeconds(1);
            coll.SetActive(false);
        }
    }
}
