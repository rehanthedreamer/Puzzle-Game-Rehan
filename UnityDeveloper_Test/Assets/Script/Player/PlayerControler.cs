using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static event Action<string> updateCubeCount;
    public static event Action showGameCompleted;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform body;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;

    [Header("Gravity Rotation System")]
    [SerializeField] GameObject holObj;
    [SerializeField] Transform holoParent;
    public float gravityStrength = 20f;

    [Header("On ground")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public bool isGrounded;
    public float fallTime = 0f;
    public float maxFallTime = 0.5f; 

    private Rigidbody rb;
    public Vector3 gravityDirection = Vector3.down;

    private Vector3 pendingRotation;
    private bool isPreviewing = false;
    int cubeCollectedCount;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;  
    }

    void Update()
    {
        if(!TimerManager.instance.GetIsTimeRunning())return;

        MovePlayer();
        HandleJump();
        RotateHolo();
        CheckGround();
        CheckFreeFall();
      
    }

    void FixedUpdate()
    {
          ApplyDirectionalGravity();
    }

// Player move and rotation controler
    void MovePlayer()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(KeyCode.A)) h = 1f;
        if (Input.GetKey(KeyCode.D)) h = -1f;

        if (Input.GetKey(KeyCode.W)) v = -1f;
        if (Input.GetKey(KeyCode.S)) v = 1f;

        Vector3 input = new Vector3(h, 0, v).normalized;

        animator.SetBool("Running", input.magnitude > 0);

        if (input.magnitude > 0)
        {
        Vector3 moveDir = body.transform.right * input.x + body.transform.forward * input.z;
        rb.MovePosition(rb.position - moveDir * moveSpeed * Time.deltaTime);
        
        // if (moveDir.magnitude > 0)
        // {
        //     Quaternion targetRot = Quaternion.LookRotation(moveDir, transform.up);
        //     body.rotation = Quaternion.Slerp(body.rotation, targetRot, Time.deltaTime * 10f);
        // }
        }
        MouseYawRotation();
    }

void MouseYawRotation()
{
    Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    Vector2 mousePos = Input.mousePosition;
    float mouseX = (mousePos.x - screenCenter.x) / screenCenter.x; // -1 to 1

    if (Mathf.Abs(mouseX) < .3f) return;
     float currentAngle = 0f;
    float relativeAngle = -mouseX * 90f;
    float targetAngle = 0f;

 
        currentAngle = transform.eulerAngles.y;
        targetAngle = currentAngle + relativeAngle;
        Quaternion targetRot = Quaternion.Euler(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);

}
   

// handle player jump from space
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

// check of player is on ground 
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 1.1f);
    }

// Holo controller 
  void RotateHolo()
{
    bool holoActive = Input.GetKey(KeyCode.LeftArrow) || 
                     Input.GetKey(KeyCode.RightArrow) || 
                     Input.GetKey(KeyCode.UpArrow) || 
                     Input.GetKey(KeyCode.DownArrow);

    holObj.SetActive(holoActive);

    if (holoActive)
    {
        isPreviewing = true;

        // Preview rotations (same as before)
        if (Input.GetKey(KeyCode.LeftArrow))
            pendingRotation = new Vector3(0, 0, 90);
        else if (Input.GetKey(KeyCode.RightArrow))
            pendingRotation = new Vector3(0, 0, -90);
        else if (Input.GetKey(KeyCode.UpArrow))
            pendingRotation = new Vector3(90, 0, 0);
        else if (Input.GetKey(KeyCode.DownArrow))
            pendingRotation = new Vector3(-90, 0, 0);

        holoParent.localRotation = Quaternion.Euler(pendingRotation);
    }

    // APPLY: Rotate to make CURRENT opposite-up the new down
    if (!holoActive && isPreviewing && Input.GetKeyDown(KeyCode.Return))
    {
        isPreviewing = false;
        
        // Calculate rotation that aligns CURRENT transform.up to OPPOSITE direction
        Vector3 currentUp = transform.up;
        Vector3 targetUp = -currentUp;  // Opposite of current up = new down
        
        // Preview rotation direction applied to current orientation
        Quaternion previewRot = Quaternion.Euler(pendingRotation);
        Vector3 newUp = (previewRot * transform.rotation * Vector3.up).normalized;
        
        // Rotation from current up → new up direction
        Quaternion rotationToTarget = Quaternion.FromToRotation(currentUp, newUp);
        transform.rotation = rotationToTarget * transform.rotation;
        
        // Update gravity to new down direction
        gravityDirection = -transform.up;
        rb.linearVelocity = Vector3.zero;
        holObj.SetActive(false);
    }
}


    void ApplyDirectionalGravity()
    {
        
        rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Point"))
        {
            cubeCollectedCount++;
            updateCubeCount?.Invoke(cubeCollectedCount.ToString());
            collision.collider.gameObject.SetActive(false);
            if(cubeCollectedCount>4)
            showGameCompleted?.Invoke();
        }
    }

// check player on ground by raycast 
    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            gravityDirection.normalized,
            groundCheckDistance,
            groundLayer
        );
    }

// check if player is free falling
    void CheckFreeFall()
    {
            if (!isGrounded)
            {
                fallTime += Time.deltaTime;

                if (fallTime >= maxFallTime)
                {
                TimerManager.instance.StopTimer();
                }
            }
            else
            {
                fallTime = 0f; 
            }
    }
    

}
