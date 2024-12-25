using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // For detecting input in UI elements
using UnityEngine.UI; // For detecting input in UI elements

public class POV: MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler  

{
    [Header("Touch Inputs")]              //this your script 
    [SerializeField]
    public float touchSensitivity = 10f;


    public CinemachineVirtualCamera Cinemachine;  

    public CinemachinePOV pov_input;         // UI slider for horizontal speed

    public TextMeshProUGUI Horizontal;

    public TextMeshProUGUI Vertical;
    [SerializeField]

    public static Vector2 TouchDist;
    private void Start()
    {
        pov_input = Cinemachine.GetCinemachineComponent<CinemachinePOV>();
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }
    public void OnDrag(PointerEventData eventData)
    {
        TouchDist.x = eventData.delta.x;
        TouchDist.y = eventData.delta.y;

      //. Debug.Log("TouchDist.x " + TouchDist.x +" TouchDist.y "+ TouchDist.y);
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

        UI();
                     
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

   
    public void UpdateVerticalSpeed(float value)
    {
        // Get the current horizontal speed to ensure vertical speed is always smaller

        pov_input.m_VerticalAxis.m_MaxSpeed = value;
  
    }

    // Update horizontal speed based on slider value
    public void UpdateHorizontalSpeed(float X)
    {
      
        pov_input.m_HorizontalAxis.m_MaxSpeed = X * 2;
        Horizontal.text =  pov_input.m_HorizontalAxis.m_MaxSpeed .ToString();
  
    }
    void UI()
    {
        // Update the UI text with the current max speeds
        Vertical.text = pov_input.m_VerticalAxis.m_MaxSpeed.ToString();
        Horizontal.text = pov_input.m_HorizontalAxis.m_MaxSpeed.ToString();

    }
}