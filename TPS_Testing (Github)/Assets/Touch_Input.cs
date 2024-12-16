using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // For detecting input in UI elements

public class Touch_Input : MonoBehaviour
{
        public float sensitivity;
 /*      [Header("Look")]
      //  public Touch touch;
        public Transform Cinemachine_Track;

        public float Vertical_Limit = 60f; // Example value, adjust as needed

        // Variables to store the accumulated rotation angles
     //   private float currentRotationX = 0f;

      //  private float currentRotationY = 0f;

        public GameObject targetImageObject; // Assign your image GameObject here

        // private bool isTouching = false; // Flag to check if touch is active

        public  Vector2 lastTouchPosition;
        public float currentX;
        public float currentY;   
        public int activeTouchId;

        public bool rotatingCamera;

        public Player_Controller playeScript;
        [Header("UI")]

        public TextMeshProUGUI Vector;
        public TextMeshProUGUI CurrentX;
        public TextMeshProUGUI CurrentY;

        void LateUpdate()
        {
           CameraRotation();
            UI();
            // If not touching, stop updating the rotation
        }


        void UI() {
            Vector.text = "Last Touch Position: x = " + lastTouchPosition.x + ", y = " + lastTouchPosition.y;


            CurrentX.text = currentX.ToString();
            CurrentY.text = currentY.ToString();
        }
           private void CameraRotation()
        {
            // Check for touch input

            foreach (UnityEngine.Touch touch in UnityEngine.Input.touches)
            {

                if (IsTouchOverUI(touch.position))
                {
                    sensitivity = 6;
                    Debug.Log("Touching image");
                    switch (touch.phase)
                    {

                        case UnityEngine.TouchPhase.Began:
                            // Check if touch is on the right half of the screen for rotating camera
                            if (touch.position.x >= Screen.width / 2)
                            {
                                if (!rotatingCamera)
                                {
                                    rotatingCamera = true;
                                    activeTouchId = touch.fingerId;
                                    lastTouchPosition = touch.position;
                                }
                            }
                            break;
                        case UnityEngine.TouchPhase.Moved:
                            // Check if this touch is the active one for camera rotation
                            if (rotatingCamera && touch.fingerId == activeTouchId)
                            {
                                Vector2 delta = touch.position - lastTouchPosition;
                                currentX += delta.x * sensitivity * Time.deltaTime;
                                currentY -= delta.y * sensitivity * Time.deltaTime;
                                // Wrap around rotation angles
                                currentX = Mathf.Repeat(currentX, 360f);
                                currentY = Mathf.Clamp(currentY, -Vertical_Limit, Vertical_Limit); // Clamp vertical rotation
                                lastTouchPosition = touch.position;
                            }
                            break;
                        case UnityEngine.TouchPhase.Ended:
                            // Check if the ended touch was the active one for camera rotation
                            if (touch.fingerId == activeTouchId)
                            {
                                activeTouchId = -1;
                                rotatingCamera = false;


                            }
                            break;
                    }
                }
                else
                {
                    Debug.Log("Not Touching image");
                    sensitivity = 0;
                }
            }

            // Check for mouse input
            if (!rotatingCamera && UnityEngine.Input.GetMouseButton(0))
            {
                float mouseX = UnityEngine.Input.GetAxis("Mouse X");
                float mouseY = UnityEngine.Input.GetAxis("Mouse Y");

                currentX += mouseX * sensitivity * Time.deltaTime;
                currentY -= mouseY * sensitivity * Time.deltaTime;

                currentX = Mathf.Repeat(currentX, 360f);
                currentY = Mathf.Clamp(currentY, -Vertical_Limit, Vertical_Limit);
            }

            // Apply the calculated rotation to the camera
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0.0f);
            Cinemachine_Track.rotation = rotation;
        }
        // Function to check if touch is over the UI Image (or other UI element)
        private bool IsTouchOverUI(Vector2 screenPosition)
        {
            // Create PointerEventData using the current event system
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            // Create a list to store all the raycast results
            var raycastResults = new System.Collections.Generic.List<RaycastResult>();

            // Perform a raycast against the UI elements
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            // Check if the raycast hit the target image object (or any other UI element)
            foreach (var result in raycastResults)
            {
                if (result.gameObject == targetImageObject)
                {
                    return true; // If the raycast hit the target object, return true
                }
            }

            return false; // If no raycast hit the target image, return false
        }

    }  


        */
    [Header("Touch Inputs")]              //this your script 
    [SerializeField]
    public float touchSensitivity = 10f;


    public CinemachineVirtualCamera Cinemachine;  

    public CinemachinePOV POV;  
    [SerializeField]

    public static Vector2 TouchDist;
    private void Start()
    {
        POV = Cinemachine.GetCinemachineComponent<CinemachinePOV>();
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }
    public void OnDrag(PointerEventData eventData)
    {
        TouchDist.x = eventData.delta.x;
        TouchDist.y = eventData.delta.y;
       
    }


    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TouchDist = Vector2.zero;

      
    }
    private float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {

            case "Mouse X":

                if (Input.touchCount > 0)
                {
                    return TouchDist.x / touchSensitivity;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    return TouchDist.y / touchSensitivity;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
                break;
        }

        return 0f;

}
    }