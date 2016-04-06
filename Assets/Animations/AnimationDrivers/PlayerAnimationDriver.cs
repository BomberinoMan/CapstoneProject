using UnityEngine;
using UnityEngine.Networking;

public static class Direction
{
    public static int Up = 1;
    public static int Right = 2;
    public static int Down = 3;
    public static int Left = 4;
}

public class PlayerAnimationDriver : NetworkBehaviour
{
    [SyncVar]
    private Animator _animator;
    private bool _isLocalPlayer;
    private DPadController _dPad;
    private PlayerControllerComponent _player;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _player = gameObject.GetComponentInParent<PlayerControllerComponent>();
        _isLocalPlayer = _player.isLocalPlayer;
        _dPad = gameObject.GetComponentInParent<PlayerControllerComponent>().dPad;
    }

    void FixedUpdate()
    {
        if (!_isLocalPlayer)
            return;

        float hor = _dPad.currDirection.x;
        float vert = _dPad.currDirection.y;

        if (hor == 0 && vert == 0)
        {
            hor = Input.GetAxisRaw("Horizontal");
            vert = Input.GetAxisRaw("Vertical");
        }

        if (_player.reverseMovement)
        {
            hor *= -1.0f;
            vert *= -1.0f;
        }

        if (vert != 0.0)
        {
            _animator.SetFloat("Speed", Mathf.Abs(vert));

            if (vert > 0.0)
                _animator.SetInteger("Direction", Direction.Up);
            else if (vert < 0.0)
                _animator.SetInteger("Direction", Direction.Down);
        }
        else if (hor != 0.0)
        {
            _animator.SetFloat("Speed", Mathf.Abs(hor));

            if (hor > 0.0)
                _animator.SetInteger("Direction", Direction.Right);
            else if (hor < 0.0)
                _animator.SetInteger("Direction", Direction.Left);
        }
        else
            _animator.SetFloat("Speed", 0.0f);
    }

    public Vector2 GetDirection()
    {
        switch (_animator.GetInteger("Direction"))
        {
            case 1:
                return new Vector2(0.0f, 1.0f); // Up
            case 2:
                return new Vector2(1.0f, 0.0f); // Right
            case 3:
                return new Vector2(0.0f, -1.0f); // Down
            case 4:
                return new Vector2(-1.0f, 0.0f); // Left
            default:
                return new Vector2(1.0f, 0.0f); // Right
        }
    }
}
