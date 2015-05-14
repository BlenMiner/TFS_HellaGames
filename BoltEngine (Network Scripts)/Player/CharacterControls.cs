using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class CharacterControls : MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float crouchSpeed = 3.0f;
    public float runSpeed = 11.0f;
    public bool limitDiagonalSpeed = true;

    public bool toggleRun = false;

    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float fallingDamageThreshold = 10.0f;
    public bool slideWhenOverSlopeLimit = false;
    public bool slideOnTaggedObjects = false;
    public float slideSpeed = 12.0f;
    public bool airControl = false;
    public float antiBumpFactor = .75f;
    public int antiBunnyHopFactor = 1;
    
    public CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;

    public LayerMask world;

    public GameObject camera;
    public Transform cFollow;
    
    CharacterController _cc;

    public struct State
    {
        public Vector3 Position;
        public Vector3 moveDirection;
        public Vector3 cameraAngle;
        public Quaternion rotation;
        public bool grounded;
        public bool falling;
        public int jumpTimer;
        public float speed;
        public bool sliding;

        public bool crounching { get; set; }
    }
    public State _state;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _state = new State();
        _state.Position = transform.localPosition;

        Physics.IgnoreLayerCollision(14, 8);
        Physics.IgnoreLayerCollision(14, 9);
        Physics.IgnoreLayerCollision(14, 10);
        Physics.IgnoreLayerCollision(14, 15);
        Physics.IgnoreLayerCollision(14, 17);
        Physics.IgnoreLayerCollision(14, 19);
        Physics.IgnoreLayerCollision(14, 20);
    }
    void Start()
    {
        originalRotation = transform.localRotation;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        _state.jumpTimer = antiBunnyHopFactor;
    }
    Quaternion originalRotation;

    ITFSPlayerState state;
    public void SetState(ITFSPlayerCommandResult result)
    {
        // assign new state
        _state.Position = result.position;
        _state.rotation = result.rotation;
        _state.grounded = result.grounded;
        _state.falling = result.falling;
        _state.cameraAngle = result.cameraAngle;
        _state.moveDirection = result.moveDirection;
        _state.speed = result.speed;
        _state.sliding = result.sliding;
        _state.jumpTimer = result.jumpFrames;

        // assign local position
        transform.localPosition = _state.Position;
        transform.rotation = _state.rotation;
        camera.transform.localEulerAngles = _state.cameraAngle;
    }
    bool once = true;
    bool onccc = true;
    public State Move(ITFSPlayerCommandInput input)
    {
        Item i = null;
        if(state != null)
            i = ItemsDatabase.getItem(state.selectedItemID);

        float inputX = 0;
        float inputY = 0;

        if (state != null && state.dead == true)
            return _state;

        if (state != null && state.showInventory == false && ServerManager.matchStarted && !state.exitMenu && !state.Skinning)
        {
            inputX = input.inputX;
            inputY = input.inputY;
        }

        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if(_state.crounching)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out hit, 1.7f, world))
            {
                Debug.Log(hit.transform.name);
                onccc = false;
            }
            else
            {
                onccc = true;
            }
        }

        if (input.crouch)
        {
            _state.crounching = true;
            controller.height = Vector2.Lerp(new Vector2(controller.height, 0), new Vector2(0.5f, 0), 0.2f).x;
        }
        else if (onccc)
        {
            _state.crounching = false;
            controller.height = 1.7f;

            if (Physics.Raycast(transform.position + new Vector3(0,1,0), Vector3.down, out hit, 2f))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            onccc = false;
        }

        if (_state.grounded)
        {
            _state.sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(_state.Position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    _state.sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    _state.sliding = true;
            }

            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (_state.falling)
            {
                _state.falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
            }

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            /*if (toggleRun && _state.grounded && input.sprinting)
                speed = (speed == walkSpeed ? runSpeed : walkSpeed);*/
            if (!toggleRun && state.stamina > 0 && state.canSprint && !_state.crounching && !state.Skinning)
                _state.speed = input.sprinting ? runSpeed : walkSpeed;
            else
            {
                _state.speed = walkSpeed;
            }

            if (state.crouching)
                _state.speed = crouchSpeed;

            if (i != null)
                _state.speed -= i.SlowValue;

            if (state != null)
            {
                int slowAmmout = Mathf.FloorToInt(state.inventoryHeight / 50);
                float percentageToSlowDown = 5f * slowAmmout;

                _state.speed -= (_state.speed * percentageToSlowDown) / 100;
            }
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ((_state.sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = hit.normal;
                _state.moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref _state.moveDirection);
                _state.moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                _state.moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                _state.moveDirection = myTransform.TransformDirection(_state.moveDirection) * _state.speed;
                playerControl = true;
            }

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!input.jump || _state.crounching || state.Skinning)
                _state.jumpTimer++;
            else if (_state.jumpTimer >= antiBunnyHopFactor)
            {
                if (BoltNetwork.isServer && state != null)
                    state.stamina -= 10;

                _state.moveDirection.y = jumpSpeed;
                _state.jumpTimer = 0;
            }
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!_state.falling && myTransform != null)
            {
                _state.falling = true;
                fallStartLevel = myTransform.position.y;
            }

            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl)
            {
                _state.moveDirection.x = inputX * _state.speed * inputModifyFactor;
                _state.moveDirection.z = inputY * _state.speed * inputModifyFactor;
                _state.moveDirection = myTransform.TransformDirection(_state.moveDirection);
            }
        }

        // Apply gravity
        _state.moveDirection.y -= gravity * BoltNetwork.frameDeltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        _state.grounded = (controller.Move(_state.moveDirection * BoltNetwork.frameDeltaTime) & CollisionFlags.Below) != 0;

        if (state != null && state.Skinning)
        {
            camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, new Quaternion(Quaternion.LookRotation(transform.forward).x, 0, 0, camera.transform.localRotation.w), 0.5f);
        }
        else
        {
            camera.transform.localEulerAngles = new Vector3(input.pitch, camera.transform.localEulerAngles.y, 0);
        }

        if (state == null)
        {
            state = GetComponent<BoltEntity>().GetState<ITFSPlayerState>();
        }
        else if(BoltNetwork.isServer)
        {
            //if (camera.transform.localRotation.x < 60 && camera.transform.localRotation.x > -60)
            {
                state.lookatTarget = cFollow.transform.position - new Vector3(0, 0.319f, 0);
            }

            state.cameraForward = camera.transform.forward;
            state.crouching = _state.crounching;
        }

            Quaternion xQuaternion = Quaternion.AngleAxis(input.mouseX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        _state.Position = transform.localPosition;
        _state.rotation = transform.rotation;
        _state.cameraAngle = camera.transform.localEulerAngles;

        return _state;
    }

    void FallingDamageAlert(float fallLevel)
    {
        if (BoltNetwork.isServer)
        {
            state.health -= Mathf.FloorToInt(fallLevel * 1.5f);

            GotHit gH = GotHit.Create(GetComponent<BoltEntity>());
            //using (var evnt = GotHit.Raise(GetComponent<BoltEntity>()))
            {
                gH.entity = GetComponent<BoltEntity>();
                gH.blood = true;
            }
            gH.Send();
        }
    }
    float ApplyDrag(float value, float drag)
    {
        if (value < 0)
        {
            return Mathf.Min(value + (drag * BoltNetwork.frameDeltaTime), 0);
        }

        else if (value > 0)
        {
            return Mathf.Max(value - (drag * BoltNetwork.frameDeltaTime), 0);
        }

        return value;
    }
}