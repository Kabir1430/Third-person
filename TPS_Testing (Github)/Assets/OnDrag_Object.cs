using Cinemachine;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // For detecting input in UI elements
using UnityEngine.UI; // For detecting input in UI elements
using UnityEngine.UIElements;

public class OnDrag_Object: MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler

{ 
    //[Header("Touch Inputs")]              //this your script 
    [SerializeField]
      public Transform Cinemachine_Track;
        public float Vertical_Limit = 60f; // Example value, adjust as needed

        public float currentX;
        public float currentY;   

        public float sensitivity;
//    public CinemachinePOV POV;         // UI slider for horizontal speed

    public TextMeshProUGUI Horizontal;

    public TextMeshProUGUI Vertical;

    public TextMeshProUGUI Sensitvity;
    [SerializeField]

    public static Vector2 TouchDist;
    private void Start()
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.delta.magnitude > 0.01f)  // You can adjust the threshold for minimal movement
        {
            TouchDist.x = eventData.delta.x;
            TouchDist.y = eventData.delta.y;
          //  Debug.Log("TouchDist.x " + TouchDist.x + " TouchDist.y " + TouchDist.y);

            Rotate_Camera();
        }
        else
            Debug.Log("Stationary");

     }

    void Rotate_Camera()

    {
        UI();
        currentX += TouchDist.x * sensitivity * Time.deltaTime;
        currentY -= TouchDist.y * sensitivity * Time.deltaTime;

        currentX = Mathf.Repeat(currentX, 360f);
        currentY = Mathf.Clamp(currentY, -Vertical_Limit, Vertical_Limit);
    

    // Apply the calculated rotation to the camera
    Quaternion rotation = Quaternion.Euler(currentY, currentX, 0.0f);
    Cinemachine_Track.rotation = rotation;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        TouchDist = Vector2.zero;

      
    }
    void UI()

    {
        Sensitvity.text = sensitivity.ToString();
        Horizontal.text = currentX.ToString();
        
        Vertical.text = currentY.ToString();
    }
    public void Sensitivity(float Sensor)
    {
        sensitivity = Sensor;
    }
}