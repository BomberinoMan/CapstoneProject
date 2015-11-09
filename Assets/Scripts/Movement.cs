using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float Speed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	void Update ()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (ver == 0.0f && hor != 0.0f)
            rb.position = new Vector3(
                rb.position.x + Input.GetAxisRaw("Horizontal") * Speed,
                RoundAxis(0.3f, 0.7f, rb.position.y),
                0.0f);
        else if(hor == 0.0f && ver != 0.0f)
            rb.position = new Vector3(
                RoundAxis(0.3f, 0.7f, rb.position.x),
                rb.position.y + Input.GetAxisRaw("Vertical") * Speed,
                0.0f);
    }

    private float RoundAxis(float roundDownDecimal, float roundUpDecimal, float num)
    {
        float remainder = num % 1;

        if (remainder <= roundDownDecimal)
            return (int)num + remainder/2.0f;
        else if (remainder >= roundUpDecimal)
            return (int)num + 1.0f - ((1.0f-remainder)/2.0f);

        return num;
    }
}
