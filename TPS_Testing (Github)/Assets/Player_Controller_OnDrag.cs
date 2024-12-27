using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller_POV_OnDrag : MonoBehaviour
{


  [Header("Movement")] 

    public FixedJoystick joystick;
    public float moveSpeed = 5f;
    public Animator Anim;
    Vector3 move;

    public CharacterController controller;
    public bool isGrounded;

    public bool Standing;
    public Animator Crosshair;
    public float smoothingSpeed = 0.1f;  // Speed of smoothing for both axes   
    public float AnimSpeed ;   // Target horizontal movement (input-based)
    public float gravity;   // Target horizontal movement (input-based)
    public   Vector3 playerVelocity;
    [Header("Look")] 
    public float Player_smoothSpeed= 0f;
    public float Camera_smoothSpeed= 0f;
     public Camera Camera;
   // public Transform Object;

    [Header("Fps")]
    public float timeBetweenFrames;
    public int frameCount;
    public float fps;  // The calculated FPS
    public float updateInterval = 0.5f; // How o
    public TextMeshProUGUI FPS;
 
    [Header("UI")]
    public TextMeshProUGUI Player_Rot;
    public TextMeshProUGUI Time_Text;
    public TextMeshProUGUI Player_Speed;
    public OnDrag_Object OnDrag_Object;
    [Header("Added By Aedan")]
    [SerializeField] Transform chest;
    [SerializeField] Transform cam_pivot;

    //public Vector3 moveDir;

    void Start()
    {
        cam_pivot.SetParent(null);
    }


     void FixedUpdate()
    {
        Follow();
     Charater_Move();
        
    }

    private void Follow()
    {
        // Smoothing the cam_pivot position based on chest position
        Vector3 targetPosition = chest.position;
        cam_pivot.position = Vector3.Lerp(cam_pivot.position, targetPosition, Time.deltaTime * Camera_smoothSpeed);
    }


    private void Charater_Move()        
    {
        isGrounded=controller.isGrounded;
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        if(move!=Vector3.zero)
        {
            Standing = false;
            BlendTree(horizontal, vertical);
            Anim.SetBool("Stand", Standing);
        }
        else

        {
            Standing=true;

           // OnDrag_Object.Rotate_Camera();
            Anim.SetBool("Stand", Standing);
        }

      move = new Vector3(horizontal, 0f, vertical);
       
        move = move.x * Camera.transform.right.normalized + move.z * Camera.transform.forward.normalized;
        move = move.normalized * moveSpeed * 2 * Time.deltaTime;

        move.y = 0f;
        controller.Move(move * Time.deltaTime *moveSpeed);
        Crosshair.SetBool("walk", false);
        
        if(move!=Vector3.zero)
        {
         Crosshair.SetBool("walk", true);
        }
        Quaternion targetRotation = Quaternion.Euler(0,Camera.transform.eulerAngles.y,0);  // Find the target rotation based on movement direction
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Player_smoothSpeed * Time.deltaTime);  // Smoothly rotate the character


        if (!isGrounded)
        {
            playerVelocity.y += gravity * Time.deltaTime;

        }
        else
        {
            playerVelocity.y = 0;
        }
        
        controller.Move(playerVelocity * Time.deltaTime);
        Update_ui();
        Fps();

    }




    public void Player_Rot_UI(float Value)
    {

        Player_smoothSpeed = Value;
        Player_Rot.text = Player_smoothSpeed.ToString();
    }
    public void Player_Speed_UI(float Value)
    {
        moveSpeed = Value;
        Player_Speed.text = moveSpeed.ToString();
    }

    public void Time_Delta(float Value)
    {
        Time.timeScale = Value;

        Time_Text.text= Time.timeScale.ToString();
    }
    void Update_ui()
    {
        Player_Rot.text = Player_smoothSpeed.ToString();
        Player_Speed.text = moveSpeed.ToString();
        Time_Text.text = Time.timeScale.ToString();

        OnDrag_Object.Sensitvity.text = OnDrag_Object.sensitivity.ToString();
    }
    public void BlendTree(float x, float y)
    {
        Anim.SetFloat("x", x,AnimSpeed,Time.deltaTime);          // Update Speed in Animator
        Anim.SetFloat("y", y,AnimSpeed,Time.deltaTime);  // Update Direction in Animator
    }
    void Animation_Bool()
    {

        if (joystick.Horizontal != 0|| joystick.Vertical  != 0)
        {
            Anim.SetBool("Run", true);  // Vertical movement blend

        }
        else
        {
            Anim.SetBool("Run", false);
        }
    }

     void Fps()
    {

        Application.targetFrameRate = 60;
        // Count frames and accumulate time
        timeBetweenFrames += Time.deltaTime;
        frameCount++;
          
        // Update the FPS value every updateInterval seconds
        if (timeBetweenFrames >= updateInterval)
        {
            fps = frameCount / timeBetweenFrames; // Calculate FPS
            timeBetweenFrames = 0;  // Reset the counter
            frameCount = 0;         // Reset the frame counter

            // Optionally, you can display the FPS on the screen
            FPS.text = fps.ToString("F0");
        //    Debug.Log("FPS: " + fps);
        }
    }
}
