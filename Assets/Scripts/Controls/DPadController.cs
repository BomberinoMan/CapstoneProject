using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DPadController : MonoBehaviour
{
    public Vector2 currDirection;

    private Vector2 _origin = new Vector2();
    private float _width;
    private float _height;

    void Start()
    {
        //This will only work on mobile if the dpad is on the bottom-left of the screen
        var canvasScalar = GameObject.Find("TouchControlOverlay").GetComponent<Canvas>().scaleFactor;
        _width = GetComponent<RectTransform>().rect.width * canvasScalar;
        _height = GetComponent<RectTransform>().rect.height * canvasScalar;

        _origin.x = _width / 2;
        _origin.y = _height / 2;
    }

    void FixedUpdate()
    {
        var touch = Input.touches.Where(x => x.position.x <= _width && x.position.y <= _height).FirstOrDefault();

        if (touch.position.x != 0 && touch.position.y != 0)
        {
            float x = touch.position.x - _origin.x;
            float y = touch.position.y - _origin.y;

            if (Math.Abs(x) > Math.Abs(y))
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
