using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


/**
 * This component moves a player controlled with a CharacterController using the keyboard.
 */
[RequireComponent(typeof(CharacterController))]
public class CharacterKeyboardMover: MonoBehaviour {
    [Tooltip("Speed of player keyboard-movement, in meters/second")]
    [SerializeField] float speed = 3.5f;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float runSpeedMultiplier = 7f;

    private CharacterController cc;

    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] InputAction runAction;

    private void OnEnable() {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
    }
    private void OnDisable() { 
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
    }
    void OnValidate() {
        // Provide default bindings for the input actions.
        // Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (moveAction == null)
            moveAction = new InputAction(type: InputActionType.Button);
        if (moveAction.bindings.Count == 0)
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

        if (jumpAction == null)
            jumpAction = new InputAction(type: InputActionType.Button);
        if (jumpAction.bindings.Count == 0)
            jumpAction.AddBinding("<Keyboard>/space");

        if (runAction == null)
            runAction = new InputAction(type: InputActionType.Button);
        if (runAction.bindings.Count == 0)
            runAction.AddBinding("<Keyboard>/leftShift");
    }

    void Start() {
        cc = GetComponent<CharacterController>();
    }

    Vector3 velocity = new Vector3(0,0,0);
    bool isRunning = false;


    void Update()  {

        bool isJumping = jumpAction.triggered;
        bool isRunningInput = runAction.triggered;

        float currentSpeed = speed;

        
        if (moveAction.ReadValue<Vector2>().magnitude > 0.1f)
        {
            currentSpeed = speed;
            if (runAction.ReadValue<float>() > 0.1f)
            {
                currentSpeed *= runSpeedMultiplier;
            }
        }

        Vector3 movement = moveAction.ReadValue<Vector2>(); // Implicitly convert Vector2 to Vector3, setting z=0.
        velocity.x = movement.x * currentSpeed;
        velocity.z = movement.y * currentSpeed;


        

        // Move in the direction you look:
        velocity = transform.TransformDirection(velocity);

        cc.Move(velocity * Time.deltaTime);
        if (cc.isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        if (isJumping && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * gravity);
        }
    }
}
