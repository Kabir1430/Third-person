using Cinemachine;
using TMPro;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{


  [Header("Movement")] 
    public FixedJoystick joystick;
    public float moveSpeed = 5f;
    private Rigidbody rb;
    public Animator Anim;
    public float smoothingSpeed = 0.1f;  // Speed of smoothing for both axes
    private float currentMoveX = 0f;  // Smoothed horizontal movement
    private float currentMoveY = 0f;  // Smoothed vertical movement
    public float AnimSpeed ;   // Target horizontal movement (input-based)
    private float targetMoveY = 0f;   // Target vertical movement (input-based)
    private float velocityX = 0f;     // SmoothDamp velocity for X axis
    private float velocityY = 0f;
    [Header("Look")] 
    public Transform Cinemanchine_Track;
    public float smoothSpeed= 0f;   
    // Current rotation on X-axis (vertical)
 //   public float Vertical_Limit= 0f;    // Current rotation on X-axis (vertical)
 //   private float rotationX = 0f;    // Current rotation on X-axis (vertical)
 //   private float rotationY = 0f;
    public Camera Camera;


    [Header("Fps")]

    private float timeBetweenFrames;
    private int frameCount;
    public float fps;  // The calculated FPS
    public float updateInterval = 0.5f; // How o
    public TextMeshProUGUI FPS;
    [Header("UI")]

    public TextMeshProUGUI Player_Rot;

    public TextMeshProUGUI Time_Text;
    public TextMeshProUGUI Sensitivity;
    public TextMeshProUGUI Player_Speed;
    public Touch_Input Touch;

    
    public bool playerisMoving;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

      
    }

    void Update()
    {

     //  Look();
      
        float horizontal = /*Input.GetAxis("Horizontal");*/joystick.Horizontal;
       float vertical = /*Input.GetAxis("Vertical");/*/joystick.Vertical;
      // float horizontal = Input.GetAxis("Horizontal");

       // float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0f, vertical);
        move = move.x * Camera.transform.right.normalized + move.z * Camera.transform.forward.normalized;
        move = move.normalized * moveSpeed * Time.deltaTime;

        move.y = 0f;
        rb.MovePosition(transform.position + move);

        //        Quaternion target = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0);

        // Create a target rotation using the Y-axis of Cinemanchine_Track and keep the current X and Z rotations of the object





        if (move.magnitude > 0f)
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(move);

            // Smoothly rotate to the target direction using Quaternion.Lerp
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
        //  Quaternion targetRotation = Quaternion.Euler(0, Cinemanchine_Track.eulerAngles.y, 0);

        // Smoothly rotate the object's transform to the target rotation using Quaternion.Lerp
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        Update_ui();
        BlendTree(horizontal, vertical);

       //         Fps();
    }
   
    
    void Jump()    
    {
        if (Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            
           // rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }



    public void Player_Rot_UI(float Value)
    {
        smoothSpeed = Value;
        Player_Rot.text = smoothSpeed.ToString();
    }
    public void Player_Speed_UI(float Value)
    {
        moveSpeed = Value;
        Player_Speed.text = moveSpeed.ToString();
    }
    public void Player_Sensor(float Value)
    {
      Touch.sensitivity= Value;

        Sensitivity.text = Touch.sensitivity.ToString();
    }

    public void Time_Delta(float Value)
    {
        Time.timeScale = Value;

        Time_Text.text= Time.timeScale.ToString();
    }
    void Update_ui()
    {
        Player_Rot.text = smoothSpeed.ToString();
        Player_Speed.text = moveSpeed.ToString();
        Time_Text.text = Time.timeScale.ToString();
        Sensitivity.text = Touch.sensitivity.ToString();
    }
    void BlendTree(float x, float y)
    {
       // currentMoveY = Mathf.SmoothDamp(currentMoveY, y, ref velocityY, smoothingSpeed);

        // Set the parameters in the Animator for each axis independently, without extra scaling or delta time.
        Anim.SetFloat("x",x, AnimSpeed,Time.deltaTime);  // Horizontal movement blend
        Anim.SetFloat("y",y, AnimSpeed ,Time.deltaTime);  // Vertical movement blend
       // Debug.Log( y);
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
            Debug.Log("FPS: " + fps);
        }
    }
}
