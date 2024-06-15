using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator animator;

    private Vector2 _input;
    private bool _isMoving;
    [SerializeField] private bool _isGrounded;
    private bool _isFacingRight = true;


    [SerializeField] private float verticalErrorRate;
    [SerializeField] private Sprite moveUp;
    [SerializeField] private Sprite moveDown;

    private CameraFollowObject cameraFollowObjectScript;
    [SerializeField] private GameObject cameraFollowGameObject;

    private float _fallSpeedYDampingChangeThreshold;
    
    [Header("Jump")]
    [SerializeField] private float jumpTime = 0.35f;
    private float jumpTimeCounter;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private Transform _feetPos;
    private bool _isJumping;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        cameraFollowObjectScript = cameraFollowGameObject.GetComponent<CameraFollowObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    private void FixedUpdate()
    {
        Move();

        if (_rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (_rb.velocity.y >= 0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    private void Update()
    {
        Jump();
    }

    private void Jump()
    {
        _isGrounded = Physics2D.OverlapCircle(_feetPos.position, _groundCheckRadius, _groundLayer);
        
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.velocity = Vector2.up * _jumpForce;
            jumpTimeCounter = jumpTime;
            _isJumping = true;
        }
        
        if (Input.GetKey(KeyCode.Space) && _isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                _rb.velocity = Vector2.up * _jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isJumping = false;
        }
    }

    private void Move()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        _input = _input.normalized;

        _rb.velocity = new Vector2(_input.x * _speed, _rb.velocity.y);


        _isMoving = _input.x != 0;

        if (_isMoving)
        {
            animator.SetBool("IsMove", true);
            TurnCheck();
        }
        else
        {
            animator.SetBool("IsMove", false);
        }

        if(_rb.velocity.y > verticalErrorRate)
        {
            animator.enabled = false;
            _spriteRenderer.sprite = moveUp;
        }
        else if(_rb.velocity.y <- verticalErrorRate)
        {
            animator.enabled = false;
            _spriteRenderer.sprite = moveDown;
        }
        else
        {
            animator.enabled = true;
        }
    }

    private void TurnCheck()
    {
        if (_input.x > 0 && !_isFacingRight)
        {
            Turn();
        }
        else if (_input.x < 0 && _isFacingRight)
        {
            Turn();
        }
    }
    private void Turn()
    {
        if (_isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = !_isFacingRight;
            cameraFollowObjectScript.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = !_isFacingRight;
            cameraFollowObjectScript.CallTurn();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGrounded = true;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGrounded = false;
    }

    public bool GetIsFacingDirection()
    {
        return _isFacingRight;
    }
}
