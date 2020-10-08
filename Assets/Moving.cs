using UnityEngine;

public class Moving : MonoBehaviour
{
    public float Jump = 5f;
    public float RunSpeed = 1;
    public float StrafeSpeed = 1;
    public float Gravity = 10f;
    public GameObject Block;

    CharacterController characterController;
    Camera headCamera;
    Vector3 velocity = Vector3.zero;
    float ang = 0f; // Turning body left/right
    float azimuth = 0f; // Turning head up/down

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        headCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Find rotation from mouse 
        ang += 10f * Input.GetAxis("Mouse X");
        azimuth -= 10f * Input.GetAxis("Mouse Y");

        // Make sure max 80 degrees up/down
        azimuth = Mathf.Clamp(azimuth, -80f, 80f);

        // Turn the body left/right
        transform.rotation = Quaternion.Euler(0f, ang, 0f);
        // Turn the head camera up/down
        headCamera.transform.localRotation = Quaternion.Euler(azimuth, 0f, 0f);

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
        velocity.x = Mathf.Lerp(velocity.x, vx, 50f * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, vz, 50f * Time.deltaTime);

        // Get the vector in world coordinates and move player
        Vector3 worldVelocity = transform.TransformVector(velocity);
        characterController.Move(worldVelocity * Time.deltaTime);

        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        // If we have a block straight in front of the camera
        if (Physics.Raycast(headCamera.transform.position, headCamera.transform.forward, out hit, 10, layerMask))
        {
            // Create a block assuming the hit is a block
            if (Input.GetKeyDown(KeyCode.X))
                Instantiate<GameObject>(Block, hit.transform.position + hit.normal, hit.transform.rotation);

            // Create a block according to hit point and normal
            if (Input.GetKeyDown(KeyCode.C))
                Instantiate<GameObject>(Block, hit.point + 0.5f * hit.normal, Quaternion.LookRotation(hit.normal, Vector3.up));

            // Destroy the hit block
            if (Input.GetKeyDown(KeyCode.Z))
                Destroy(hit.transform.gameObject);
        }
    }
}
