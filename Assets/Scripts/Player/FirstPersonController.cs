using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FirstPersonController : MonoBehaviour {
    [Serializable]
    public class MovementSettings {
        public float ForwardSpeed = 6.0f;
        // Speed when walking forward
        public float BackwardSpeed = 6.0f;
        // Speed when walking backwards
        public float StrafeSpeed = 6.0f;
        // Speed when walking sideways
        public float RunMultiplier = 1.5f;
        // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce = 50f;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        [HideInInspector] public float CurrentTargetSpeed = 6f;

        private bool m_Running;

        public void UpdateDesiredTargetSpeed(Vector2 input) {
            if (input == Vector2.zero)
                return;
            if (input.x > 0 || input.x < 0) {
                //strafe
                CurrentTargetSpeed = StrafeSpeed;
            }
            if (input.y < 0) {
                //backwards
                CurrentTargetSpeed = BackwardSpeed;
            }
            if (input.y > 0) {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                CurrentTargetSpeed = ForwardSpeed;
            }

            if (Input.GetKey(RunKey)) {
                CurrentTargetSpeed *= RunMultiplier;
                m_Running = true;
            } else {
                m_Running = false;
            }
        }

        public bool Running {
            get { return m_Running; }
        }
    }


    [Serializable]
    public class AdvancedSettings {
        public float groundCheckDistance = 0.01f;
        // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f;
        // stops the character
        public bool airControl;
        // can the user control the direction that is being moved in the air
    }


    public Camera cam;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();


    private Rigidbody m_RigidBody;
    private CapsuleCollider m_Capsule;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal;
    private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;


    public Vector3 Velocity {
        get { return m_RigidBody.velocity; }
    }

    public bool Grounded {
        get { return m_IsGrounded; }
    }

    public bool Jumping {
        get { return m_Jumping; }
    }

    public bool Running {
        get {
            return movementSettings.Running;
        }
    }


    private void Start() {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        mouseLook.Init(transform, cam.transform);
    }


    private void Update() {
        RotateView();

        if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump) {
            m_Jump = true;
        }
    }


    private void FixedUpdate() {
        GroundCheck();
        Vector2 input = GetInput();

        if (advancedSettings.airControl || m_IsGrounded) {
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon)) {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                //Scale the desired move direction vector by the target speed
                desiredMove *= movementSettings.CurrentTargetSpeed;
                //Set the player speed
                m_RigidBody.velocity = new Vector3(desiredMove.x, m_RigidBody.velocity.y, desiredMove.z);
            } else {
                m_RigidBody.velocity = new Vector3(0f, m_RigidBody.velocity.y, 0f);
            }
        }

        if (m_IsGrounded) {
            if (m_Jump) {
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                m_Jumping = true;
            }

            if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f) {
                m_RigidBody.Sleep();
            }
        } else {
            if (m_PreviouslyGrounded && !m_Jumping) {
                StickToGroundHelper();
            }
        }
        m_Jump = false;
    }


    private float SlopeMultiplier() {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return movementSettings.SlopeCurveModifier.Evaluate(angle);
    }


    private void StickToGroundHelper() {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                ((m_Capsule.height / 2f) - m_Capsule.radius) +
                advancedSettings.stickToGroundHelperDistance)) {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
            }
        }
    }


    private Vector2 GetInput() {
        Vector2 input = new Vector2(0f, 0f);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            input.y += 1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            input.y -= 1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow)) {
            input.x += 1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow)) {
            input.x -= 1f;
        }
        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }


    private void RotateView() {
        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon)
            return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        mouseLook.LookRotation(transform, cam.transform);

        if (m_IsGrounded || advancedSettings.airControl) {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
        }
    }


    /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    private void GroundCheck() {
        m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius, Vector3.down, out hitInfo,
                ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance)) {
            m_IsGrounded = true;
            m_GroundContactNormal = hitInfo.normal;
        } else {
            m_IsGrounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping) {
            m_Jumping = false;
        }
    }
}
