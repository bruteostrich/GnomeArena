using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public enum MovementType
    {
        ClickToMove,
        KeyboardMove,
        FollowMove
    }
    public MovementType movementType = MovementType.KeyboardMove;
    Vector3 movement;
    Vector3 target;
    Animator anim;
    Rigidbody playerRigidBody;
    int floorMask;
    float camRayLength = 100.0f;
    bool moving = false;
    public float stopSnapThreshold = 0.5f;
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody>();
        floorMask = LayerMask.GetMask("Floor");
    }

    void FixedUpdate()
    {
        if (movementType == MovementType.KeyboardMove)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Move(h, v);
            Turning();
            Animating(h, v);
        }
        else if(movementType == MovementType.ClickToMove)
        {
            if (moving == true)
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
                {
                    RaycastHit hit;
                    Ray ray;
                    #if UNITY_EDITOR || UNITY_STANDALONE
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    //for touch device
                    #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    #endif
                    //Check if the ray hits any collider
                    if (Physics.Raycast(ray, out hit))
                    {
                        //set a flag to indicate to move the gameobject
                        moving = true;
                        //save the click / tap position
                        target = hit.point;
                        target.y = 0.0f;
                    }
                }
                MoveTo(target);
                Turning();
                Animating(target.x, target.z);
                if(Vector3.Distance(playerRigidBody.position, target) < stopSnapThreshold)
                {
                    moving = false;
                    playerRigidBody.MovePosition(target);
                    Animating(0.0f, 0.0f);
                }
            }
            else
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
                {
                    RaycastHit hit;
                    Ray ray;
                    #if UNITY_EDITOR || UNITY_STANDALONE
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    //for touch device
                    #elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    #endif
                    //Check if the ray hits any collider
                    if (Physics.Raycast(ray, out hit))
                    {
                        //set a flag to indicate to move the gameobject
                        moving = true;
                        //save the click / tap position
                        target = hit.point;
                        target.y = 0.0f;
                    }
                }
                else
                {
                    MoveTo(playerRigidBody.position);
                    Turning();
                    Animating(0.0f, 0.0f);
                }
            }
        }
    }
    void MoveTo(Vector3 point)
    {
        movement.Set(point.x - playerRigidBody.position.x, 0.0f, point.z - playerRigidBody.position.z);
        Move(movement.x, movement.z);
    }
    void Move(float h, float v)
    {
        movement.Set(h, 0.0f, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidBody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        if(Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0.0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidBody.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {
        bool walking = h != 0.0f || v != 0.0f;
        anim.SetBool("IsWalking", walking);
    }
}
