using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DPadController : NetworkBehaviour
{
    public Vector2 currDirection;

    private Vector2 origin = new Vector2();
    private float width;
    private float height;
    private int setup = 100;

    void Start()
    {
        //This will only work on mobile if the dpad is on the bottom-left of the screen
        var canvasScalar = GetComponentInParent<Canvas>().scaleFactor;
        width = GetComponent<RectTransform>().rect.width * canvasScalar;
        height  = GetComponent<RectTransform>().rect.height * canvasScalar;

        origin.x = width / 2;
        origin.y = height / 2;
    }

    void FixedUpdate()
    {
        var touch = Input.touches.Where(x => x.position.x <= width && x.position.y <= height).FirstOrDefault();

        if(touch.position.x != 0 && touch.position.y != 0)
        {
            float x = touch.position.x - origin.x;
            float y = touch.position.y - origin.y;

            if(Math.Abs(x) > Math.Abs(y))
            {
                y = 0.0f;
                x = x / Math.Abs(x);
            }
            else
            {
                x = 0.0f;
                y = y / Math.Abs(y);
            }

            currDirection.x = x;
            currDirection.y = y;
        }
        else
        {
            currDirection.x = 0.0f;
            currDirection.y = 0.0f;
        }
    }

}
