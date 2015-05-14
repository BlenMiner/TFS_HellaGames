using UnityEngine;
using System.Collections;

public class TutorialPlayerController : Bolt.EntityBehaviour<ITFSPlayerState>
{
    const float MOUSE_SENSITIVITY = 2f;

    float inputX;
    float inputY;
    bool sprinting;
    bool jump;
    float mouseX;
    float pitch;
    bool showexit;
    bool crouch;

    CharacterControls _motor;
    public Animator anim;
    public Transform target;

    public GameObject neckBone;
    public GameObject weaponC;
    public Vignetting cameraVigneting;

    void Awake()
    {
        _motor = GetComponent<CharacterControls>();
    }

    void Update()
    {
        if(state.dead == true && entity.isOwner)
        {
            /*try
            {
                cameraVigneting.intensity = Vector2.Lerp(new Vector2(cameraVigneting.intensity, 0), new Vector2(10, 0), 0.3f).x;
                cameraVigneting.chromaticAberration = Vector2.Lerp(new Vector2(cameraVigneting.chromaticAberration, 0), new Vector2(120, 0), 0.6f).x;
            }
            catch { }*/
            BleedBehavior.BloodAmount += 0.5f;
            BleedBehavior.minBloodAmount += 0.5f;

            weaponC.transform.parent = neckBone.transform;
        }

        if (entity.isAttached && state != null)
            target.transform.position = state.lookatTarget;

        PollKeys(true);
    }
    public override void Attached()
    {
        state.transform.SetTransforms(transform);
        state.SetAnimator(anim);
    }

    void PollKeys(bool mouse)
    {
        if (entity.isAttached == true)
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
            sprinting = Input.GetButton("Run");
            jump = Input.GetKey(KeyCode.Space);
            showexit = Input.GetKey(KeyCode.Escape);
            crouch = Input.GetKey(KeyCode.LeftControl);

            if (mouse && !state.showInventory && !state.exitMenu && !state.crafting && !state.Skinning)
            {
                mouseX += Input.GetAxis("Mouse X") * 2;
                mouseX = ClampAngle(mouseX, -360, 360);


                pitch += (-Input.GetAxisRaw("Mouse Y") * 2);
                pitch = Mathf.Clamp(pitch, -85f, +85f);
            }
        }
    }

    bool calledIt = false;
    bool calledItH = false;
    bool calledItB = false;

    bool onceD = true;
    public override void SimulateOwner()
    {
        if ((BoltNetwork.frame % 3) == 0 && loadedHealth == false)
        {
            state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
            state.stamina = 100;
            state.hunger = 800;
            state.canSprint = true;
            state.dead = false;
            state.inventoryHeight = 150;
        }
        else if (loadedHealth == true)
        {
            if(state.health <= 0)
            {
                state.dead = true;
                if(onceD)
                {
                    BoltEntity e = BoltNetwork.Instantiate(BoltPrefabs.DeathSound);
                    e.TakeControl();

                    onceD = false;
                }
                //BoltNetwork.Destroy(this.gameObject);
            }
            if (state.hunger < 150 && Mathf.FloorToInt(Time.time) % 3 == 0)
            {
                if (calledItH == false)
                {
                    state.health -= 1;
                    calledItH = true;
                }
            }
            else
                calledItH = false;

            if (state.steps == 20)
            {
                state.hunger -= 20;
                state.steps = 0;
            }
        }

        if (Mathf.FloorToInt(Time.time) % 1 == 0)
        {
            int wheight = inventoryHeight();

            state.inventoryHeight = wheight;
        }

        if (Mathf.FloorToInt(Time.time % 60) == 0 )
        {
            if (calledIt == false)
            {
                calledIt = true;
                state.hunger -= 20;
            }
        }
        else
            calledIt = false;

        if (state.stamina <= 5)
            state.canSprint = false;

        if (state.stamina > 50)
            state.canSprint = true;

        if(Mathf.FloorToInt(Time.time) % 3 == 0 && state.bleading)
        {
            if (calledItB == false)
            {
                state.health -= 1;
                calledItB = true;
            }
        }
        else
        {
            calledItB = false;
        }

        if ((BoltNetwork.frame % 3) == 0 && (state.sprinting))
        {
            state.stamina = (byte)Mathf.Clamp(state.stamina - 1, 0, 100);
        }
        else if ((BoltNetwork.frame % 4) == 0 && !state.blocking)
        {
            state.stamina = (byte)Mathf.Clamp(state.stamina + 1, 0, 100);
        }

        if(state.health == 100)
        {
            loadedHealth = true;
        }
    }
    int inventoryHeight()
    {
        int totalWheight = 0;

        int id = 0;
        for(int x = 0; x < 8; x ++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (state.inventory[id].ItemID == 0)
                {
                    id++;
                    continue;
                }
                Item i = ItemsDatabase.getItem(state.inventory[id].ItemID);
                int itemC = state.inventory[id].ItemNum;

                totalWheight += (i.Height * itemC);
                
                id++;
            }
        }

        return totalWheight;
    }

    bool loadedHealth = false;
    public override void SimulateController()
    {
        PollKeys(false);

        ITFSPlayerCommandInput input = TFSPlayerCommand.Create();

        input.inputX = inputX;
        input.inputY = inputY;
        input.jump = jump;
        input.mouseX = mouseX;
        input.pitch = pitch;
        input.sprinting = sprinting;
        input.showexit = showexit;
        input.crouch = crouch;

        entity.QueueInput(input);
    }

    Vector3 oPos = new Vector3(0, 0.142f, 0.012f);
    Vector3 cPos = new Vector3(0, 0.9f, 0.012f);
    void AnimatePlayer(TFSPlayerCommand cmd)
    {
        float inputX = 0;
        float inputY = 0;
        if (state.showInventory == false && ServerManager.matchStarted)
        {
            inputX = float.Parse(cmd.Input.inputX.ToString("F1"));
            inputY = float.Parse(cmd.Input.inputY.ToString("F1"));
        }

        if (state.crouching)
        {
            anim.transform.localPosition = Vector3.Lerp(anim.transform.localPosition, cPos, 0.2f);
            anim.SetBool("crouching", true);
        }
        else
        {
            anim.transform.localPosition = Vector3.Lerp(anim.transform.localPosition, oPos, 0.2f);
            anim.SetBool("crouching", false);
        }

        if(state.dead == true)
        {
            anim.SetBool("dead", true);
        }

        if (inputX > 0 || inputX < 0)
        {
            if (BoltNetwork.isServer)
                state.strafing = true;

            anim.SetFloat("inputX", cmd.Input.inputX);
            anim.SetBool("strafing", true);
        }
        else
        {
            if (BoltNetwork.isServer)
                state.strafing = false;
            anim.SetBool("strafing", false);
        }
        if (inputY > 0 || inputY < 0 || inputX > 0 || inputX < 0)
        {
            if (cmd.Input.sprinting && state.canSprint)
            {
                if (BoltNetwork.isServer)
                    state.sprinting = true;
                anim.SetBool("sprinting", true);

                if (BoltNetwork.isServer)
                    state.walking = false;
                anim.SetBool("walking", false);
            }
            else
            {
                if (BoltNetwork.isServer)
                    state.sprinting = false;
                anim.SetBool("sprinting", false);

                if (BoltNetwork.isServer)
                    state.walking = true;
                anim.SetBool("walking", true);
            }
        }
        else
        {
            if (BoltNetwork.isServer)
                state.walking = false;
            anim.SetBool("walking", false);

            if(BoltNetwork.isServer)
                state.sprinting = false;
            anim.SetBool("sprinting", false);
        }

        // JUMP
        if (!_motor._state.grounded)
        {
            /*state.falling = false;
            anim.SetBool("falling", false);*/

            if (BoltNetwork.isServer)
                state.jump = true;
            anim.SetBool("jump", true);
        }
        else
        {
            if (BoltNetwork.isServer)
                state.jump = false;
            anim.SetBool("jump", false);
        }
        /*else if (_motor._state.falling == true)
        {
            state.falling = true; 
            anim.SetBool("falling", true);

            state.jump = false; 
            anim.SetBool("jump", false);
        }*/

        anim.SetBool("attacking", state.attacking);
    }

    public override void ExecuteCommand(Bolt.Command c, bool resetState)
    {
        TFSPlayerCommand cmd = (TFSPlayerCommand)c;

        if (resetState)
        {
            _motor.SetState(cmd.Result);
        }
        else
        {
            // move and save the resulting state
            var result = _motor.Move(cmd.Input);

            cmd.Result.position = result.Position;
            cmd.Result.rotation = result.rotation;            
            cmd.Result.moveDirection = result.moveDirection;
            //cmd.Result.cameraForward
            cmd.Result.grounded = result.grounded;
            cmd.Result.jumpFrames = result.jumpTimer;
            //cmd.Result.cameraRotation
            cmd.Result.falling = result.falling;
            cmd.Result.cameraAngle = result.cameraAngle;
            cmd.Result.speed = result.speed;
            cmd.Result.sliding = result.sliding;
            cmd.Result.crounching = result.crounching;

            if (cmd.IsFirstExecution)
            {
                AnimatePlayer(cmd);
            }
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
