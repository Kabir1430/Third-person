using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller_POV : MonoBehaviour
{


  [Header("Movement")] 
    public FixedJoystick joystick;
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public Animator Anim;
    public float smoothingSpeed = 0.1f;  // Speed of smoothing for both axes   
    public float AnimSpeed ;   // Target horizontal movement (input-based)

    [Header("Look")] 
    public float smoothSpeed= 0f;
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
    public  float horizontal;
    public float vertical;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

      
    }

    void Update()
    {

        //  Look();

        float horizontal = /*Input.GetAxis("Horizontal");*/joystick.Horizontal;
        float vertical = /*Input.GetAxis("Vertical");/*/joystick.Vertical;
        Vector3 move = new Vector3(horizontal, 0f, vertical);
        move = move.x * Camera.transform.right.normalized + move.z * Camera.transform.forward.normalized;
        move = move.normalized * moveSpeed * Time.deltaTime;

        move.y = 0f;
        rb.MovePosition(transform.position + move);






        if (move.magnitude > 0f)
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(move);

            // Smoothly rotate to the target direction using Quaternion.Lerp
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
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

   /* public void Player_Sensor(float Value)
    {
      Touch.sensitivity= Value;

             Sensitivity.text = Touch.sensitivity.ToString();
    }
    */
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
      //  Sensitivity.text = Touch.sensitivity.ToString();
    }
    void BlendTree(float x, float y)
    {
        // currentMoveY = Mathf.SmoothDamp(currentMoveY, y, ref velocityY, smoothingSpeed);
       // float speed = new Vector2(x, y).sqrMagnitude;
        //speed = Mathf.Clamp(speed, 0, 10);
        // Set the parameters in the Animator for each axis independently, without extra scaling or delta time.

        //Anim.SetFloat("x",x, AnimSpeed,Time.deltaTime);  // Horizontal movement blend
        //    Anim.SetFloat("y",y, AnimSpeed ,Time.deltaTime);  // Vertical movement blend
     //   Anim.SetFloat("y", speed, AnimSpeed, Time.deltaTime);  // Vertical movement blend
                                                               // Debug.Log( y);

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
