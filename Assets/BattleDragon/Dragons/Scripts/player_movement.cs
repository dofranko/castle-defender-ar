using UnityEngine;

namespace SwordWorld
{
    public class player_movement
        : MonoBehaviour
    {
        public float walk_speed = 6f;
        public float run_speed = 12f;


        private Vector3 movement;
        private Animator animator;
        private Rigidbody playerRigidbody;

        // rotate
        public float turnSmoothing = 3.0f;
        private Transform cameraTransform;
        private bool isWalk;
        private bool isRun;
        private float h;
        private float v;

        // jump
        public float jumpHeight = 5.0f;
        public float jumpCooldown = 1.0f;
        private bool isJump;
        
        void Awake()
        {
            // Set up references.
            animator = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();

            cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
            isJump = Input.GetButtonDown("Jump");
            isWalk = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

            if (isWalk)
            {
                if (isRun)
                {
                    isRun = !Input.GetButtonUp("Run");
                }
                else
                {
                    isRun = Input.GetButtonDown("Run");
                }
            }
            else
            {
                isRun = false;
            }
        }

        void FixedUpdate()
        {
            // Move the player around the scene.
            Move(h, v);

            // Turn the player to face the mouse cursor. 
            Rotate(h, v);

            // Jump
            Jump(h, v);
        }

        void Move(float h, float v)
        {
            float speed = isRun ? run_speed : walk_speed;

            // Set the movement vector based on the axis input.
            movement.Set(h, 0.0f, v);

            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition(transform.position + movement);

            // Animator
            {
                if (isRun)
                {
                    animator.SetBool("IsRun", isRun);
                }
                else
                {
                    animator.SetBool("IsRun", isRun);
                    animator.SetBool("IsWalk", isWalk);
                }
            }
        }

        void Jump(float h, float v)
        {
            if (isJump)
            {
                animator.SetTrigger("Jump");
                playerRigidbody.velocity = new Vector3(0, jumpHeight, 0);
            }
        }

        Vector3 Rotate(float h, float v)
        {
            Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
            forward = forward.normalized;

            Vector3 right = new Vector3(forward.z, 0, -forward.x);

            Vector3 targetDirection;
            targetDirection = forward * v + right * h;

            if ((isWalk && targetDirection != Vector3.zero))
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                Quaternion newRotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);

                // TODO：不知为毛，Rigid 的约束不起作用，只能手动设置为 0 
                newRotation.x = 0f;
                newRotation.z = 0f;
                GetComponent<Rigidbody>().MoveRotation(newRotation);
            }

            return targetDirection;
        }
    }
}