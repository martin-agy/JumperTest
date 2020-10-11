using UnityEngine;

public class Moving : MonoBehaviour
{
    public float Jump = 5f;
    public float RunSpeed = 1;
    public float StrafeSpeed = 1;
    public float Gravity = 10f;
    public GameObject[] Blocks;

    CharacterController characterController;
    Camera headCamera;
    Vector3 velocity = Vector3.zero;
    float bodyAngle = 0f; // Turning body left/right
    float headAngle = 0f; // Turning head up/down
    int blockIndex = 0;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        headCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }


        // Find rotation from mouse 
        bodyAngle += 10f * Input.GetAxis("Mouse X");
        headAngle -= 10f * Input.GetAxis("Mouse Y");

        // Make sure max 80 degrees up/down
        headAngle = Mathf.Clamp(headAngle, -80f, 80f);

        // Turn the body left/right
        transform.rotation = Quaternion.Euler(0f, bodyAngle, 0f);
        // Turn the head camera up/down
        headCamera.transform.localRotation = Quaternion.Euler(headAngle, 0f, 0f);

        // Velocity up/down (jumping and falling)
        if (characterController.isGrounded)
        {
            // If grounded: Jump if space is pressed
            if (Input.GetKeyDown(KeyCode.Space))
                velocity.y = Jump;
        }
        else
        {
            // If flying: gravitate 
            velocity.y -= Gravity * Time.deltaTime;
        }

        // Desired walking velocity
        float vx = 0;
        float vz = 0;
        if (Input.GetKey(KeyCode.D))
            vx += StrafeSpeed;
        if (Input.GetKey(KeyCode.A))
            vx -= StrafeSpeed;
        if (Input.GetKey(KeyCode.W))
            vz += RunSpeed;
        if (Input.GetKey(KeyCode.S))
            vz -= RunSpeed;
        // Use Lerp to smooth the motion
        velocity.x = Mathf.Lerp(velocity.x, vx, 10f * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, vz, 10f * Time.deltaTime);

        // Get the vector in world coordinates and move player
        Vector3 worldVelocity = transform.TransformVector(velocity);
        characterController.Move(worldVelocity * Time.deltaTime);

        // Look through button 1 to 9 to see if clicked
        // If there are as many blocks: use it
        for (int i = 1; i <= 9; i++)
            if (Input.GetKey(i.ToString()))
                if (Blocks.Length >= i)
                    blockIndex = i - 1;

        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        // If we have a block straight in front of the camera
        if (Physics.Raycast(headCamera.transform.position, headCamera.transform.forward, out hit, 100, layerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Create a block according to hit point and normal
                if (Input.GetKey(KeyCode.LeftControl))
                    Instantiate<GameObject>(Blocks[blockIndex], hit.point + 0.5f * hit.normal, Quaternion.LookRotation(hit.normal, Vector3.up));
                // Create a block assuming the hit is a block
                else
                    Instantiate<GameObject>(Blocks[blockIndex], hit.transform.position + hit.normal, hit.transform.rotation);
            }


            // Destroy the hit block
            if (Input.GetMouseButtonDown(1))
                Destroy(hit.transform.gameObject);
        }
    }
}
