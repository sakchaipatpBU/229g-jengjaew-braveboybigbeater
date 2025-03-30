using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject focalPoint;
    public float jumpForce;
    public int jumpMax;
    public int jumpCount;
    public float jumpAnimTime;
    public Player player;

    private Animator playerAnim;

    private InputAction moveAction;
    private InputAction jumpAction;
    public float rotateSpeed;
    private float moveSpeed;
    private Rigidbody rb;

    private bool isDead = false;
    private bool isMoving;
    private bool isAtking;
    private bool isJumpNormal;
    private bool isJumpFlip;

    private bool isGrounded;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveSpeed = GetComponent<Player>().moveSpeed;
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (isDead) return;
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float verticalInput = moveInput.y;
        float horizontalInput = moveInput.x;

        // คำนวณทิศทางการเคลื่อนที่
        Vector3 moveDirection = (focalPoint.transform.forward * verticalInput) + (transform.right * horizontalInput);
        moveDirection.Normalize();

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        if (moveAction.IsPressed())
        {
            isMoving = true;
            isAtking = false;
            SetPlayerControllerAnim();
        }
        else
        {
            isMoving = false;
            SetPlayerControllerAnim();
        }
        // ให้ตัวละครหันหน้าไปตามทิศทาง focalPoint
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(focalPoint.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        if (jumpAction.triggered && jumpCount < jumpMax)
        {
            if (isGrounded)
            {
                isGrounded = false;
                isJumpNormal = true;
                SetPlayerControllerAnim();
            }
            else
            {
                isJumpFlip = true;
                SetPlayerControllerAnim();
            }
            player.ResetCombo();
            PlayerJump();
            StartCoroutine(JumpAnim());

        }

    }
    private void LateUpdate()
    {
        focalPoint.transform.position = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void PlayerJump()
    {
        jumpCount++;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    IEnumerator JumpAnim()
    {
        yield return new WaitForSeconds(jumpAnimTime);
        isJumpNormal = false;
        isJumpFlip = false;
        SetPlayerControllerAnim();
    }

    void SetPlayerControllerAnim()
    {
        playerAnim.SetBool("isMoving", isMoving);
        playerAnim.SetBool("isAtking", isAtking);
        playerAnim.SetBool("isJumpNormal", isJumpNormal);
        playerAnim.SetBool("isJumpFlip", isJumpFlip);


    }

    public void PlayerDead()
    {
        isDead = true;

    }
}
