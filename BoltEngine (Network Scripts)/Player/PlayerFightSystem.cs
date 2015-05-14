using UnityEngine;
using System.Collections;

public class PlayerFightSystem : MonoBehaviour
{
    BoltEntity entity;
    ITFSPlayerState state;

    PlayerWeaponController pwc;

    public Transform point0;
    public Transform point1;

    public int pointCount = 10;

    public Transform[] points;
    public Vector3[] lastPosition;

    void Awake()
    {
        entity = transform.root.GetComponent<BoltEntity>();
        pwc = transform.root.gameObject.GetComponent<PlayerWeaponController>();
    }
    void Update()
    {        
        if (state == null && entity != null)
        if(entity.isAttached)
        {
            state = entity.GetState<ITFSPlayerState>();
        }

        if (state.attacking && state.canAttack)
        {
            Attacking();
        }
        else
        {
            once = true;
        }
    }

    float delady = 0;
    bool once = true;

    void Attacking()
    {
        if (BoltNetwork.isClient)
        {
            entity = transform.root.GetComponent<BoltEntity>();
            this.enabled = false;
            return;
        }

         int i = 0;
        foreach (Transform t in points)
        {
            float d = Vector3.Distance(lastPosition[i], t.position);

            Vector3 fwd = (t.position - lastPosition[i]).normalized;

            if (d > 0.08f)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(t.position, fwd), out hit, 1f))
                {
                    if (!state.canAttack || hit.transform.tag == "Drop" || hit.transform.tag == "AirDrop" || hit.transform.tag == "Arrow") return;
                    BoltEntity enemy = hit.transform.gameObject.GetComponent<BoltEntity>();

                    if (enemy == null || enemy == entity)
                    {
                        if (hit.transform.tag == "Terrain")
                        {
                            Terrain terrain = hit.transform.GetComponent<Terrain>();
                            float groundHeight = terrain.SampleHeight(hit.point);

                            if (hit.point.y - 0.1f > (groundHeight - 58.88699f) && delady < Time.time)
                            {
                                if (AddItem(state, ItemsDatabase.getItem("wood"), Random.Range(0, 2)))
                                {
                                    Destroy(GameObject.Instantiate(ItemsDatabase.instance.chopwood, hit.point, Quaternion.LookRotation(transform.forward)), 1.5f);
                                    delady = Time.time + 0.5f;

                                    continue;
                                }
                            }
                        }
                        continue;
                    }

                    if (!state.canAttack) return;
                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = enemy;
                        pS.soundID = 16;
                    }
                    pS.Send();

                    state.canAttack = false;

                    if (hit.transform.tag == "Animal")
                    {
                        Item item = ItemsDatabase.getItem(state.selectedItemID);

                        int takeH = 0;
                        if (item == null)
                            takeH = 10;
                        else
                            takeH = Mathf.FloorToInt(ItemsDatabase.getItem(state.selectedItemID).ItemAttackDamage);

                        IAnimals ans = enemy.GetState<IAnimals>();
                        ans.health -= Mathf.FloorToInt(takeH);
                        state.stamina -= 15;
                    }
                    else if (hit.transform.tag == "Wall" || hit.transform.tag == "Foundation" || hit.transform.tag == "Stairs" || hit.transform.tag == "Door")
                    {
                        Item item = ItemsDatabase.getItem(state.selectedItemID);
                        IBuildings ps = enemy.GetState<IBuildings>();

                        int takeH = 5;

                        ps.health -= takeH;

                        if (ps.health <= 0)
                        {
                            foreach (Collider c in Physics.OverlapSphere(hit.transform.position, 4f))
                            {
                                if (c.tag == "Wall" || hit.transform.tag == "Stairs" || hit.transform.tag == "Door" || hit.transform.tag == "Roof")
                                {
                                    Physics.IgnoreLayerCollision(8, 9);
                                    Physics.IgnoreLayerCollision(8, 12);
                                    Physics.IgnoreLayerCollision(8, 15);
                                    Physics.IgnoreLayerCollision(8, 20);

                                    Physics.IgnoreLayerCollision(11, 9);
                                    Physics.IgnoreLayerCollision(11, 12);
                                    Physics.IgnoreLayerCollision(11, 15);
                                    Physics.IgnoreLayerCollision(11, 20);


                                    c.transform.gameObject.isStatic = false;
                                    c.rigidbody.isKinematic = false;

                                    c.transform.Translate(new Vector3(0, 0.01f, 0));

                                    foreach (Collider collider in c.transform.gameObject.GetComponentsInChildren<Collider>())
                                    {
                                        collider.enabled = false;
                                    }
                                }
                            }

                            BoltNetwork.Destroy(enemy);
                        }

                        continue;
                    }
                    else
                    {
                        Item item = ItemsDatabase.getItem(state.selectedItemID);
                        ITFSPlayerState ps = enemy.GetState<ITFSPlayerState>();
                        if (ps == null) return;

                        if(ps.blocking && ps.blocked == false)
                        {
                            ps.blocked = true;
                            StartCoroutine(stopBlock(ps));
                        }

                        if (ps.blocked == false)
                        {
                            float takeH = 5;
                            if (item != null)
                                takeH = Mathf.FloorToInt(item.ItemAttackDamage);

                            for (int sa = 0; sa < 4; sa++)
                            {
                                Item a = ItemsDatabase.getItem(ps.armorSlots[sa].ItemID);
                                if (a != null)
                                    takeH -= a.ProtectionValue;
                            }

                            ps.health -= Mathf.FloorToInt(takeH);

                            GotHit gH = GotHit.Create(enemy);
                            //using (var evnt = GotHit.Raise(enemy))
                            {
                                gH.entity = enemy;
                                gH.blood = true;
                            }
                            gH.Send();

                            if (Random.Range(0, 5) == 2)
                            {
                                ps.bleading = true;
                            }

                            pwc.GetAnimator().SetTrigger("Hit");
                            PullKnockBack(enemy, (state.cameraForward));
                            state.stamina -= 15;
                        }
                        else
                        {
                            GotHit gH = GotHit.Create(enemy);
                            {
                                gH.entity = enemy;
                                gH.blood = false;
                            }
                            gH.Send();
                            pwc.GetAnimator().SetTrigger("Hit");

                            ps.stamina -= 10;
                        }
                    }
                }
            }
            lastPosition[i] = t.position;
            i++;
        }
    }

    IEnumerator stopBlock(ITFSPlayerState s)
    {
        yield return new WaitForSeconds(3);
        s.blocked = false;
    }

    public bool AddItem(ITFSPlayerState state, Item e, int count)
    {
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (isChestSlot(slotID))
                {
                    slotID++;
                    continue;
                }

                if (state.inventory[slotID].ItemID == 0 || state.inventory[slotID].ItemID == e.ItemID)
                {
                    if (state.inventory[slotID].ItemNum < e.ItemMaxStack)
                    {
                        int rest = 0;

                        state.inventory[slotID].ItemID = e.ItemID;
                        //Inventory[x, z] = e.ItemID;

                        if (state.inventory[slotID].ItemNum + count > e.ItemMaxStack)
                        {
                            rest = (state.inventory[slotID].ItemNum + count) - e.ItemMaxStack;
                            state.inventory[slotID].ItemNum = e.ItemMaxStack;
                        }
                        else
                        {
                            state.inventory[slotID].ItemNum += count;
                        }

                        if (rest > 0)
                        {
                            AddItem(state, e, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }
    public bool isChestSlot(int slot)
    {
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (slotID == slot)
                {
                    if (x >= 4)
                        return true;
                }
                slotID++;
            }
        }
        return false;
    }
    public bool AddItem(ITFSPlayerState state, int id, int count)
    {
        Item e = ItemsDatabase.getItem(id);
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (isChestSlot(slotID))
                {
                    slotID++;
                    continue;
                }

                if (state.inventory[slotID].ItemID == 0 || state.inventory[slotID].ItemID == e.ItemID)
                {
                    if (state.inventory[slotID].ItemNum < e.ItemMaxStack)
                    {
                        int rest = 0;

                        state.inventory[slotID].ItemID = e.ItemID;
                        //Inventory[x, z] = e.ItemID;

                        if (state.inventory[slotID].ItemNum + count > e.ItemMaxStack)
                        {
                            rest = (state.inventory[slotID].ItemNum + count) - e.ItemMaxStack;
                            state.inventory[slotID].ItemNum = e.ItemMaxStack;
                        }
                        else
                        {
                            state.inventory[slotID].ItemNum += count;
                        }

                        if (rest > 0)
                        {
                            AddItem(state, e, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }

    public void PullKnockBack(BoltEntity e, Vector3 direction)
    {
        direction /= 8;
        KnockBack kB = KnockBack.Create(e);
        {
            kB.direction = direction;
        }
        kB.Send();

        e.gameObject.GetComponent<CharacterController>().Move((direction.normalized));
    }

    [ContextMenu("Create RayCastPoints")]
    void SetupSkinnedMesh()
    {
        foreach (Transform t in points)
        {
            if (t != null)
            {
                DestroyImmediate(t.gameObject);
            }
        }

        points = new Transform[pointCount + 1];
        lastPosition = new Vector3[pointCount + 1];

        Vector3 distance = ((point1.position) - point0.position) / pointCount;

        for (int i = 0; i <= pointCount; i++)
        {
            GameObject point = new GameObject("point" + i);

            point.transform.parent = point0.transform;

            point.transform.position = point0.position + (distance * i);
            points[i] = point.transform;
            lastPosition[i] = point.transform.position;
        }
    }
}
