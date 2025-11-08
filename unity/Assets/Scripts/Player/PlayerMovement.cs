using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 20f;

    [Header("Ground Checks")]
    [SerializeField] LayerMask groundLayers;
    [SerializeField] Transform[] groundChecks;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float jumpGracePeriod = 0.1f; // in seconds
    [SerializeField] float jumpBufferTime = 0.1f; // in seconds

    private Rigidbody2D rb;
    private float remainingGracePeriod = 0f;
    private float timeOnGround = 0f;
    private float moveInput = 0f;
    private bool jumpPressed = false;
    private KeyCode[] leftKeys;
    private KeyCode[] rightKeys;
    private KeyCode[] jumpKeys;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        ResetState();
    }

    void Update()
    {
        // move input
        moveInput = 0f;
        for (int i = 0; i < leftKeys.Length; i++)
        {
            if (Input.GetKey(leftKeys[i])) moveInput = -1f;
            if (Input.GetKey(rightKeys[i])) moveInput = 1f;
        }

        // jump input
        for (int i = 0; i < jumpKeys.Length; i++)
        {
            if (Input.GetKey(jumpKeys[i]) && CanJump())
            {
                jumpPressed = true; // flag jump
            }
        }
    }

    void FixedUpdate()
    {
        // move
        rb.AddForce(new Vector2(moveInput * moveSpeed * rb.drag, 0), ForceMode2D.Force);

        // update grace period and buffer timer
        if (IsGrounded())
        {
            timeOnGround += Time.fixedDeltaTime;
            remainingGracePeriod = jumpGracePeriod;
        }
        else
        {
            remainingGracePeriod -= Time.fixedDeltaTime;
            timeOnGround = 0f;
        }

        // jump
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
            remainingGracePeriod = 0f;
        }
    }

    public void SetKeybinds(KeyCode[] left, KeyCode[] right, KeyCode[] jump)
    {
        leftKeys = left;
        rightKeys = right;
        jumpKeys = jump;
    }

    bool CanJump()
    {
        bool isGrounded = IsGrounded();
        return (!isGrounded && remainingGracePeriod > 0f) || (isGrounded && timeOnGround > jumpBufferTime);
    }

    bool IsGrounded()
    {
        if (rb.velocity.y > 0) return false; // ignore if moving upwards
        foreach (Transform groundCheck in groundChecks)
        {
            if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayers))
            {
                return true;
            }
        }
        return false;
    }

    void ResetState()
    {
        remainingGracePeriod = 0f;
        timeOnGround = 0f;
        moveInput = 0f;
        jumpPressed = false;
    }
}
