using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public AudioClip[] jumpSound;
    public float speed = 1;
    public float jumpForce = 7;
    public float jumpTime = 0.5f;
    public float jumpMultiplayer = 1;
    public float rayOffset;
    public LayerMask ground;
    AudioSource audioSound;
    Rigidbody2D rb;
    float dashTime = 1.5f;
    float dashSpeed = 35;
    float dashCooldown;
    float jumpTimeCooldown;
    float hz;
    float _speed;
    bool onGround;
    bool hold = false;
    private void Start()
    {
        audioSound = Camera.main.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        jumpTimeCooldown = jumpTime;
    }
    void Update()
    {
        hz = Input.GetAxisRaw("Horizontal");
        rb.velocity = new(hz * speed, rb.velocity.y);
        dashCooldown -= Time.deltaTime;
        //if (hz != 0)
        //{
        //    if (Physics2D.Raycast(transform.position, Vector2.right, rayOffset, ground) && hz > 0) hz = 0;
        //    else if (Physics2D.Raycast(transform.position, Vector2.left, rayOffset, ground) && hz < 0) hz = 0;
        //}
        if (hold && !onGround && jumpTimeCooldown > 0)
        {
            jumpTimeCooldown -= Time.deltaTime;
            rb.velocity = new(rb.velocity.x, jumpForce * (1 + jumpTime - jumpTimeCooldown) * jumpMultiplayer);
        }
        onGround = Physics2D.Raycast(transform.position, Vector2.down, rayOffset, ground);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position,Vector2.down*rayOffset);
    }
    public void GetInput(InputAction.CallbackContext _context)
    {
        if (_context.started && onGround)
        {
            audioSound.clip = jumpSound[Random.Range(0, jumpSound.Length)];
            audioSound.Play();
            rb.velocity = new(rb.velocity.x, jumpForce);
            jumpTimeCooldown = jumpTime;
        }
        else if (_context.canceled) jumpTimeCooldown = 0;
        hold = _context.performed;
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && dashCooldown < 0 && !GetComponent<Weapon>().laser)
        {
            Debug.Log("DASHED");
            dashCooldown = dashTime;
            _speed = speed;
            speed = dashSpeed;
            Invoke("DashStop", 0.18f);
        }
    }
    void DashStop()
    {
        speed = _speed;
    }
}
