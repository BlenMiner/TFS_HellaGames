using UnityEngine;
using System.Collections;

public class PlayerWeaponController : Bolt.GlobalEventListener
{
    public WeaponSettings wepSettings;
    public BoltEntity entity;
    ITFSPlayerState state;
    public PlayerInventoryShow pis;

    public Animator H1;
    public Animator H2;
    public Animator Spear;
    public Animator Crossbow;
    public Animator Fists;
    public Animator Torch;

    public Animator thirdPersonPlayer;
    Vector3 originalCameraP;
    Quaternion originalCameraR;

    public Transform camera;
    public Transform weapons;
    public Transform parent;
    public Transform originalparent;

    public Transform machete;
    public Transform macheteGoTO;

    bool controller;
    Item selectedItem = new Item();
    void Awake()
    {
        selectedAnimator = Fists;
        if (pis.me != null || BoltNetwork.isServer)
        {
            DiselectAll();
        }

        originalCameraP = camera.localPosition;
        originalCameraR = camera.localRotation;

    }
    public void DiselectAll()
    {
        if(BoltNetwork.isClient)
        {
            selectedItem = null;
            if (wepSettings.H1.gameObject != null)
                wepSettings.H1.gameObject.SetActive(false);
            if (wepSettings.H2.gameObject != null)
                wepSettings.H2.gameObject.SetActive(false);
            if (wepSettings.Spear.gameObject != null)
                wepSettings.Spear.gameObject.SetActive(false);
            if (wepSettings.CrossBow.gameObject != null)
                wepSettings.CrossBow.gameObject.SetActive(false);
            if (wepSettings.Torch.gameObject != null)
                wepSettings.Torch.gameObject.SetActive(false);

            wepSettings.Fists.gameObject.SetActive(true);

            return;
        }

        selectedItem = null;
        if (wepSettings.H1.gameObject != null)
            wepSettings.H1.gameObject.SetActive(false);
        if (wepSettings.H2.gameObject != null)
            wepSettings.H2.gameObject.SetActive(false);
        if (wepSettings.Spear.gameObject != null)
            wepSettings.Spear.gameObject.SetActive(false);
        if (wepSettings.CrossBow.gameObject != null)
            wepSettings.CrossBow.gameObject.SetActive(false);
        if (wepSettings.Torch.gameObject != null)
            wepSettings.Torch.gameObject.SetActive(false);

        DiselectWeapons(wepSettings.H1);
        DiselectWeapons(wepSettings.H2);
        DiselectWeapons(wepSettings.Spear);
        DiselectWeapons(wepSettings.CrossBow);
        DiselectWeapons(wepSettings.Torch);

        wepSettings.Fists.gameObject.SetActive(true);
    }
    
    public void SetSelectedWeapon(Item i)
    {
        if (pis.me == null && BoltNetwork.isClient)
        {
            if (wepSettings.H1.gameObject != null)
                wepSettings.H1.gameObject.SetActive(false);
            if (wepSettings.H2.gameObject != null)
                wepSettings.H2.gameObject.SetActive(false);
            if (wepSettings.Spear.gameObject != null)
                wepSettings.Spear.gameObject.SetActive(false);
            if (wepSettings.CrossBow.gameObject != null)
                wepSettings.CrossBow.gameObject.SetActive(false);
            if (wepSettings.Torch.gameObject != null)
                wepSettings.Torch.gameObject.SetActive(false);

            wepSettings.Fists.gameObject.SetActive(false);

            return;
        }

        if (!BoltNetwork.isClient)
            state.Skinning = false;

        DiselectAll();
        
        if (i.WeaponType == WeaponType.H1)
        {
            wepSettings.Fists.gameObject.SetActive(false);

            selectedItem = i;
            SetWeaponActive(i.ItemName, wepSettings.H1);
        }
        else  if (i.WeaponType == WeaponType.Spear)
        {
            wepSettings.Fists.gameObject.SetActive(false);

            selectedItem = i;
            SetWeaponActive(i.ItemName, wepSettings.Spear);
        }
        else if (i.WeaponType == WeaponType.H2)
        {
            wepSettings.Fists.gameObject.SetActive(false);

            selectedItem = i;
            SetWeaponActive(i.ItemName, wepSettings.H2);
        }
        else if (i.WeaponType == WeaponType.CrossBow)
        {
            wepSettings.CrossBow.gameObject.SetActive(false);

            selectedItem = i;
            SetWeaponActive(i.ItemName, wepSettings.CrossBow);
        }
        else if (i.WeaponType == WeaponType.Torch)
        {
            wepSettings.Torch.gameObject.SetActive(false);

            selectedItem = i;
            SetWeaponActive(i.ItemName, wepSettings.Torch);
        }

        GetAnimator();
    }
    
    public override void ControlOfEntityGained(BoltEntity arg)
    {
        if (!arg.name.Contains("Player"))
            return;

        controller = true;
    }

    public void SetWeaponActive(string name, WeaponPlacers weapons)
    {
        if (name == "Fists")
            weapons.gameObject.SetActive(true);
        else
            wepSettings.Fists.gameObject.SetActive(false);

        weapons.gameObject.SetActive(true);
        if (weapons.hasMultipleWeapon)
        {
            foreach (GameObject obj in weapons.weapons)
            {
                obj.SetActive(false);

                if (obj.name == name)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
    public void DiselectWeapons(WeaponPlacers weapons)
    {
        if (weapons.hasMultipleWeapon)
        {
            foreach (GameObject obj in weapons.weapons)
            {
                obj.SetActive(false);
            }
        }
    }

    bool attached = false;
    bool isServer = false;
    public override void EntityAttached(BoltEntity entity)
    {
        if (!entity.name.Contains("TFS")) return;
        state = entity.GetState<ITFSPlayerState>();

        attached = true;
        if (BoltNetwork.isServer)
            isServer = true;
    }

    float delayBetweenAtack;
    void Update()
    {
        if (!attached)
            return;

        if (state == null)
        {
            state = entity.GetState<ITFSPlayerState>();
            return;
        }

        if (isServer)
        {
            OnlyServer();
        }
        else
        {
            UpdateAnimations();
        }

        if (state.showInventory == true || state.sprinting == true)
            return;
        
        if (delayBetweenAtack < Time.time)
        {
            if (Input.GetMouseButtonDown(0) && controller)
            {
                WeaponSwing evnt = WeaponSwing.Create(Bolt.GlobalTargets.OnlyServer);
                {
                    evnt.entity = entity;
                    evnt.mode = 1;
                }
                evnt.Send();

                delayBetweenAtack = Time.time + 0.1f;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && controller)
            {
                WeaponSwing evnt = WeaponSwing.Create(Bolt.GlobalTargets.OnlyServer);
                {
                    evnt.entity = this.GetComponent<BoltEntity>();
                    evnt.mode = 3;
                }
                evnt.Send();

                delayBetweenAtack = Time.time + 0.1f;
            }
            if (Input.GetMouseButtonDown(1) && controller)
            {
                WeaponSwing evnt = WeaponSwing.Create(Bolt.GlobalTargets.OnlyServer);
                {
                    evnt.entity = this.GetComponent<BoltEntity>();
                    evnt.mode = 5;
                }
                evnt.Send();

                delayBetweenAtack = Time.time + 0.1f;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && controller)
            {
                WeaponSwing evnt = WeaponSwing.Create(Bolt.GlobalTargets.OnlyServer);
                {
                    evnt.entity = this.GetComponent<BoltEntity>();
                    evnt.mode = 4;
                }
                evnt.Send();

                delayBetweenAtack = Time.time + 0.1f;
            }
        }
    }

    bool once = false;

    int swingID = -1;
    bool blocking = false;
    void UpdateAnimations()
    {
        if (!isServer)
        {
            thirdPersonPlayer.SetBool("attacking", state.attacking);
            thirdPersonPlayer.SetInteger("SwingID", state.swingid);
            thirdPersonPlayer.SetBool("blocking", state.blocking);

            if (controller == false) return;

            selectedAnimator.SetBool("Skinning", state.Skinning);
            if (!state.Skinning)
            {
                camera.transform.parent = originalparent;

                machete.gameObject.SetActive(true);
                macheteGoTO.gameObject.SetActive(false);

                camera.transform.localPosition = originalCameraP;
                weapons.transform.localPosition = camera.transform.localPosition;

                camera.transform.localRotation = new Quaternion(0, 0, 0, originalCameraR.w);
                weapons.transform.localRotation = camera.transform.localRotation;

                selectedAnimator.SetBool("attacking", state.attacking);

                if (swingID != state.swingid)
                {
                    if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
                    {
                        selectedAnimator.SetInteger("SwingID", state.swingid);
                    }
                    swingID = state.swingid;
                }

                if (blocking != state.blocking)
                {
                    if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
                    {
                        selectedAnimator.SetBool("blocking", state.blocking);
                    }

                    blocking = state.blocking;
                }

                if (running != state.sprinting)
                {
                    selectedAnimator.SetBool("Running", state.sprinting);
                    running = state.sprinting;
                }
                if (walking != state.walking)
                {
                    selectedAnimator.SetBool("Moving", state.walking);
                    walking = state.walking;
                }
            }
            else
            {
                camera.transform.parent = parent;
                machete.gameObject.SetActive(false);
                macheteGoTO.gameObject.SetActive(true);
            }
        }
    }

    bool lastSkinning = false;
    bool running = false;
    bool walking = false;
    void OnlyServer()
    {
        if (lastSkinning != state.Skinning)
        {
            lastSkinning = state.Skinning;
            selectedAnimator.SetBool("Skinning", state.Skinning);
        }
        if (!state.Skinning)
        {
            {
                camera.transform.parent = originalparent;

                if (selectedItem != null && selectedItem.ItemName.Equals("Machete"))
                    machete.gameObject.SetActive(true);
                macheteGoTO.gameObject.SetActive(false);

                camera.transform.localPosition = originalCameraP;
                weapons.transform.localPosition = camera.transform.localPosition;

                camera.transform.localRotation = new Quaternion(0, 0, 0, originalCameraR.w);
                weapons.transform.localRotation = camera.transform.localRotation;
            }

            state.attacking = selectedAnimator.GetBool("attacking");

            if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && 
                selectedItem.WeaponType != WeaponType.Torch)
                state.swingid = selectedAnimator.GetInteger("SwingID");

            if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow &&
                selectedItem.WeaponType != WeaponType.Torch)
                state.blocking = selectedAnimator.GetBool("blocking");
            blocking = state.blocking;

            if (running != state.sprinting)
            {
                selectedAnimator.SetBool("Running", state.sprinting);
                running = state.sprinting;
            }
            if (walking != state.walking)
            {
                selectedAnimator.SetBool("Moving", state.walking);
                walking = state.walking;
            }
            //selectedAnimator.SetBool("Moving", state.walking);
        }
        else
        {
            camera.transform.parent = parent;
            machete.gameObject.SetActive(false);
            macheteGoTO.gameObject.SetActive(true);
        }
    }

    public override void OnEvent(WeaponSwing e)
    {
        if (e.entity != this.GetComponent<BoltEntity>()) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        if (state.dead == true) return;

        Item holdingItem = pis.getItem(state.selectedItemID);

        if (state.showInventory == true || state.sprinting == true)
            return;
        if (holdingItem.ItemID > 0 && holdingItem.ItemType != Item.ItemTypeE.Weapon || holdingItem.WeaponType == WeaponType.Torch)
            return;

        if (e.mode == 1 && state.attacking == false && state.blocking == false && 
            GetAnimator().GetBool("attacking") == false &&
            GetAnimator().GetBool("blocking") == false)
        {
            state.stamina -= 15;
            AttackNormal();
        }
        if (e.mode == 1 && GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("Swing1"))
        {
            mustSwitchToAttack2 = true;
        }

        if (e.mode == 4 && state.attacking == false && state.blocking == false &
            GetAnimator().GetBool("attacking") == false &&
            GetAnimator().GetBool("blocking") == false)
        {
            state.stamina -= 15;
            AttackOverhead();
        }
        if (e.mode == 3 && state.attacking == false && state.blocking == false &
            GetAnimator().GetBool("attacking") == false &&
            GetAnimator().GetBool("blocking") == false)
        {
            state.stamina -= 15;
            Stab();
        }
        if (e.mode == 5 && state.attacking == false && state.blocking == false &
            GetAnimator().GetBool("attacking") == false &&
            GetAnimator().GetBool("blocking") == false &&
            state.blocked == false)
        {
            Block();
        }
    }
    public override void OnEvent(StopAttacking e)
    {
        EndAttack();
    }
    public override void OnEvent(StopAnimation e)
    {
        defaultSpeed = GetAnimator().speed;

        GetAnimator().speed = 0.01f;
        thirdPersonPlayer.speed = 0.01f;

        StartCoroutine(reset(e.time));
    }

    bool mustSwitchToAttack2 = false;

    public void AttackNormal()
    {
        GetAnimator().SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetInteger("SwingID", 1);

        thirdPersonPlayer.SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            thirdPersonPlayer.SetInteger("SwingID", 1);
    }
    public void AttackOverhead()
    {
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetInteger("SwingID", 4);

        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            thirdPersonPlayer.SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            thirdPersonPlayer.SetInteger("SwingID", 4);
    }
    public void Stab()
    {
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetInteger("SwingID", 3);

        thirdPersonPlayer.SetBool("attacking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            thirdPersonPlayer.SetInteger("SwingID", 3);
    }
    public void Block()
    {
        state.blocking = true;
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            thirdPersonPlayer.SetBool("blocking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            GetAnimator().SetBool("blocking", true);
        if (selectedItem == null || selectedItem.WeaponType != WeaponType.CrossBow && selectedItem.WeaponType != WeaponType.Torch)
            StartCoroutine(stopBlocking());
    }

    public void FireArrow()
    {
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        BoltEntity arrow = BoltNetwork.Instantiate(BoltPrefabs.Sphere, cameraPos, Quaternion.identity);
        arrow.TakeControl();

        arrow.transform.forward = state.cameraForward;
        arrow.rigidbody.AddForce(state.cameraForward * 80, ForceMode.Impulse);
    }

    public void Blocked()
    {
        GetAnimator().SetTrigger("Hit");
    }
    public void CanAttack()
    {
        state.canAttack = true;
    }

    float defaultSpeed;
    public void ChargeWeapon(float time)
    {
        if (BoltNetwork.isServer)
        {
            StopAnimation e = StopAnimation.Create(Bolt.GlobalTargets.Everyone);
            {
                e.time = time;
            }
            e.Send();
        }
    }
    IEnumerator reset(float time)
    {
        yield return new WaitForSeconds(time);
        GetAnimator().speed = 1;
        thirdPersonPlayer.speed = 1;
    }

    IEnumerator stopBlocking()
    {
        yield return new WaitForSeconds(0.7f);
        thirdPersonPlayer.SetBool("blocking", false);
        GetAnimator().SetBool("blocking", false);
        state.blocking = false;
    }

    public void SwitchCombo()
    {
        if(mustSwitchToAttack2)
        {
            GetAnimator().SetInteger("SwingID", 2);
            mustSwitchToAttack2 = false;
        }
    }

    public void EndAttack()
    {
        state.canAttack = false;

        mustSwitchToAttack2 = false;
        GetAnimator().SetBool("attacking", false);
        GetAnimator().SetBool("Skinning", false);

        if(BoltNetwork.isServer)
        {
            state.Skinning = false;
        }
    }

    Animator selectedAnimator;
    Item lastI;
    public Animator GetAnimator()
    {
        if (selectedItem != lastI)
        {
            lastI = selectedItem;

            if (selectedItem == null)
            {
                selectedAnimator = Fists;
                return selectedAnimator;
            }
            if (selectedItem.WeaponType == WeaponType.H1)
            {
                //if (H1.gameObject.activeSelf == true)
                {
                    selectedAnimator = H1;
                    return selectedAnimator;
                }
            }
            if (selectedItem.WeaponType == WeaponType.H2)
            {
                //if (H2.gameObject.activeSelf == true)
                {
                    selectedAnimator = H2;
                    return selectedAnimator;
                }
            }
            if (selectedItem.WeaponType == WeaponType.Spear)
            {
                //if (Spear.gameObject.activeSelf == true)
                {
                    selectedAnimator = Spear;
                    return selectedAnimator;
                }
            }
            if (selectedItem.WeaponType == WeaponType.CrossBow)
            {
                //if (Crossbow.gameObject.activeSelf == true)
                {
                    selectedAnimator = Crossbow;
                    return selectedAnimator;
                }
            }
            if (selectedItem.WeaponType == WeaponType.Torch != null)
            {
                //if (Torch.gameObject.activeSelf == true)
                {
                    selectedAnimator = Torch;
                    return selectedAnimator;
                }
            }
            return selectedAnimator;
        }
        else
        {
            return selectedAnimator;
        }
    }
}

[System.Serializable]
public class WeaponSettings
{
    public WeaponPlacers H1;
    public WeaponPlacers H2;
    public WeaponPlacers CrossBow;
    public WeaponPlacers Spear;
    public WeaponPlacers Fists;
    public WeaponPlacers Torch;
}
public enum WeaponType
{
    None,
    H1,
    H2,
    CrossBow,
    Spear,
    Torch
}
[System.Serializable]
public class WeaponPlacers
{
    public GameObject gameObject;
    public GameObject[] weapons;

    public bool hasMultipleWeapon = true;
    public bool hasAttackAnim = true;
}