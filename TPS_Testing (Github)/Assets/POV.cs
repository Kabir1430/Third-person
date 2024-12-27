using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

public class POV : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Touch Inputs")]
    [SerializeField]
    public float touchSensitivity = 10f;

    private Vector2 initialTouchPosition;
    private Vector2 lastTouchPosition;  // Track last touch position to detect movement
    public static Vector2 TouchDist; // Track touch movement

    public CinemachineVirtualCamera Cinemachine;  // Reference to Cinemachine camera
    private CinemachinePOV pov_input;  // Cinemachine POV component

    public TextMeshProUGUI Horizontal;  // UI for Horizontal speed display
    public TextMeshProUGUI Vertical;    // UI for Vertical speed display

    private bool isDragging = false;  // Flag to check if dragging is happening
    private bool isRotating = false;  // Flag to check if the camera should rotate

    private float stopThreshold = 0.1f; // Threshold for stopping rotation after dragging
    private float lastTouchDeltaX = 0f;  // Track delta X from last drag to detect stopping
    private float lastTouchDeltaY = 0f;  // Track delta Y from last drag to detect stopping

    private void Start()
    {
        // Get CinemachinePOV component from the virtual camera
        pov_input = Cinemachine.GetCinemachineComponent<CinemachinePOV>();
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }

    // Handle drag movement for touch input
    public void OnDrag(PointerEventData eventData)
    {
        // Calculate the delta between the current and last touch position
        Vector2 deltaPosition = eventData.position - lastTouchPosition;

        // Only rotate the camera if the movement is greater than the threshold (4 pixels here)
        if (Mathf.Abs(deltaPosition.x) > 4 || Mathf.Abs(deltaPosition.y) > 4)
        {
            TouchDist.x = deltaPosition.x;
            TouchDist.y = deltaPosition.y;
            isDragging = true;  // Set the dragging flag to true (camera will rotate)
            isRotating = true;  // Allow rotation
        }
        else
        {
            // If movement is less than the threshold, stop the rotation
            isRotating = false;
            TouchDist = Vector2.zero;  // Reset the TouchDist to prevent rotation
        }

        // Update last touch position for the next drag
        lastTouchPosition = eventData.position;

        // Check if the touch has stopped moving (small movement detected after dragging)
        if (Mathf.Abs(deltaPosition.x) < stopThreshold && Mathf.Abs(deltaPosition.y) < stopThreshold)
        {
            // Stop any small movement that still causes rotation
            isRotating = false;
            TouchDist = Vector2.zero;
        }

        // Debug the current touch distances
        Debug.Log("TouchDist.x " + TouchDist.x + " TouchDist.y " + TouchDist.y);
    }

    // Capture the initial touch position when the user presses down
    public void OnPointerDown(PointerEventData eventData)
    {
        initialTouchPosition = eventData.position;  // Set the initial touch position
        lastTouchPosition = initialTouchPosition;   // Initialize last touch position
        isDragging = false;  // Reset dragging status
        isRotating = false;  // Ensure camera doesn't rotate initially
        Debug.Log("Pointer Down at: " + initialTouchPosition);
    }

    // Reset TouchDist when the user releases the touch
    public void OnPointerUp(PointerEventData eventData)
    {
        TouchDist = Vector2.zero;
        isDragging = false;  // Reset dragging status
        isRotating = false;  // Stop camera rotation when the user stops dragging
        Debug.Log("Pointer Up - TouchDist reset");
    }

    // Delegate function to handle axis input
    private float HandleAxisInputDelegate(string axisName)
    {
        UI();  // Update UI for the speeds

        // Handle both "Mouse X" and "Mouse Y" axis
        switch (axisName)
        {
            case "Mouse X":
                if (Input.touchCount > 0 && isRotating)  // Ensure touch movement is active
                {
                    // Use TouchDist for touch input
                    return TouchDist.x / touchSensitivity;  // Adjust for sensitivity
                }
                else
                {
                    return 0f;  // No movement, don't rotate
                }

            case "Mouse Y":
                if (Input.touchCount > 0 && isRotating)  // Ensure touch movement is active
                {
                    // Use TouchDist for touch input
                    return TouchDist.y / touchSensitivity;  // Adjust for sensitivity
                }
                else
                {
                    return 0f;  // No movement, don't rotate
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognized.");
                break;
        }

        return 0f;
    }
    
    // Update the UI with current max speeds
    void UI()
    {
        Vertical.text = pov_input.m_VerticalAxis.m_MaxSpeed.ToString();
        Horizontal.text = pov_input.m_HorizontalAxis.m_MaxSpeed.ToString();
    }

    // Update vertical speed based on slider value
    public void UpdateVerticalSpeed(float value)
    {
        pov_input.m_VerticalAxis.m_MaxSpeed = value;
    }

    // Update horizontal speed based on slider value
    public void UpdateHorizontalSpeed(float X)
    {
        pov_input.m_HorizontalAxis.m_MaxSpeed = X * 2;  // Multiplying to make the speed faster
        Horizontal.text = pov_input.m_HorizontalAxis.m_MaxSpeed.ToString();
    }
}
