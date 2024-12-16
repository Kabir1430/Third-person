using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.UIElements;

public class Touch_Mobile : MonoBehaviour
{
    [Header("Direction")] // Start is called before the first frame update
    public TextMeshProUGUI DirectionText;
      public TextMeshProUGUI D_Position_Start_Text;

      public TextMeshProUGUI D_Position_End_Text;
    private Touch theTouch;

    private Vector2 touchStartPosition, touchEndPosition;
      private string direction;

    [Header("MultiDirection")] // Start is called before the first frame update
    private string multiTouchInfo;
    public TextMeshProUGUI multiTouchInfoDisplay;
    private int maxTapCount = 0;
    // Update is called once per frame
    void Update()
    {    
     Direction();
    }
    void MultiDirection()
    {
        multiTouchInfo = string.Format("Max tap count: {0}\n", maxTapCount);

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                theTouch = Input.GetTouch(i);

                multiTouchInfo+=string.Format("Touch {0}-Position {1}-Tap Count: {2}-Finger ID: {3}\nRadius: {4} ({5}%)\n",
			i, theTouch.position, theTouch.tapCount, theTouch.fingerId, theTouch.radius,
			((theTouch.radius / (theTouch.radius + theTouch.radiusVariance)) * 100f).ToString("F1"));

            if (theTouch.tapCount > maxTapCount)
            {
                maxTapCount = theTouch.tapCount;
            }
        }
    }

    multiTouchInfoDisplay.text = multiTouchInfo;

    }
    void Direction()

    {
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStartPosition = theTouch.position;
            }

            else if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
            {
                touchEndPosition = theTouch.position;

                float x = touchEndPosition.x - touchStartPosition.x;
                float y = touchEndPosition.y - touchStartPosition.y;

                if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0)
                {
                    direction = "Tapped";
                }

                else if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    direction = x > 0 ? "Right" : "Left";
                }

                else
                {
                    direction = y > 0 ? "Up" : "Down";
                }
            }
        }
        D_Position_Start_Text.text = touchStartPosition.ToString();
        D_Position_End_Text.text = touchEndPosition.ToString();
        DirectionText.text = direction;



    }


}