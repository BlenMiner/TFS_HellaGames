using UnityEngine;
using System.Collections;
using Bolt;

public class FootstepSounds : MonoBehaviour {

    public PlayerWeaponController pwc;
    public LayerMask lm;

    Terrain terrain;

    float lastStep = 0;

    void Step()
    {
        if (lastStep > Time.time)
        {
            return;
        }

        lastStep = Time.time + 0.2f;

        if (this.GetIMaterial() == IMaterial.Dirt)
        {
            //playSound.entity = this.pwc.entity;
            //playSound.soundID = Random.Range(22, 35);
            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(22, 35)]);
        }
        else
        {
            if (this.GetIMaterial() == IMaterial.DirtLeaves)
            {
                //playSound.entity = this.pwc.entity;
                //playSound.soundID = Random.Range(36, 52);
                this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(36, 52)]);
            }
            else
            {
                if (this.GetIMaterial() == IMaterial.Stone)
                {
                    //playSound.entity = this.pwc.entity;
                    //playSound.soundID = Random.Range(53, 67);
                    this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(53, 67)]);
                }
                else
                {
                    if (this.GetIMaterial() == IMaterial.Sand)
                    {
                        //playSound.entity = this.pwc.entity;
                        //playSound.soundID = Random.Range(68, 81);
                        this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(68, 81)]);
                    }
                    else
                    {
                        if (this.GetIMaterial() == IMaterial.Wood)
                        {
                            //playSound.entity = this.pwc.entity;
                            //playSound.soundID = Random.Range(112, 126);
                            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(112, 126)]);
                        }
                    }
                }
            }
        }
    }
    void StepRunning()
    {
        if (lastStep > Time.time)
        {
            return;
        }
        lastStep = Time.time + 0.2f;

        if (this.GetIMaterial() == IMaterial.Dirt)
        {
            //playSound.entity = this.pwc.entity;
            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(186, 204)]);
            //playSound.soundID = Random.Range(186, 204);
        }
        else
        {
            if (this.GetIMaterial() == IMaterial.DirtLeaves)
            {
                //playSound.entity = this.pwc.entity;
                //playSound.soundID = Random.Range(167, 185);

                this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(167, 185)]);
            }
            else
            {
                if (this.GetIMaterial() == IMaterial.Stone)
                {
                //    playSound.entity = this.pwc.entity;
                //    playSound.soundID = Random.Range(147, 166);

                    this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(147, 166)]);
                }
                else
                {
                    if (this.GetIMaterial() == IMaterial.Sand)
                    {
                        //playSound.entity = this.pwc.entity;
                        //playSound.soundID = Random.Range(84, 103);

                        this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(84, 103)]);
                    }
                    else
                    {
                        if (this.GetIMaterial() == IMaterial.Wood)
                        {
                            //playSound.entity = this.pwc.entity;
                            //playSound.soundID = Random.Range(127, 146);

                            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(127, 146)]);
                        }
                    }
                }
            }
        }
    }
    void Land()
    {
        if (lastStep > Time.time)
        {
            return;
        }
        lastStep = Time.time + 0.2f;

        if (this.GetIMaterial() == IMaterial.Dirt)
        {
            //playSound.entity = this.pwc.entity;
            //playSound.soundID = Random.Range(108, 109);
            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(108, 109)]);
        }
        else
        {
            if (this.GetIMaterial() == IMaterial.DirtLeaves)
            {
                //playSound.entity = this.pwc.entity;
                //playSound.soundID = Random.Range(104, 105);
                this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(104, 105)]);
            }
            else
            {
                if (this.GetIMaterial() == IMaterial.Stone)
                {
                    //playSound.entity = this.pwc.entity;
                    //playSound.soundID = Random.Range(106, 107);
                    this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(106, 107)]);
                }
                else
                {
                    if (this.GetIMaterial() == IMaterial.Sand)
                    {
                        //playSound.entity = this.pwc.entity;
                        //playSound.soundID = Random.Range(82, 83);
                        this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(82, 83)]);
                    }
                    else
                    {
                        if (this.GetIMaterial() == IMaterial.Wood)
                        {
                            //playSound.entity = this.pwc.entity;
                            //playSound.soundID = Random.Range(110, 111);
                            this.pwc.entity.gameObject.audio.PlayOneShot(ClientSoundCallbacks.instance.weaponSounds[Random.Range(110, 111)]);
                        }
                    }
                }
            }
        }
    }

    private IMaterial GetIMaterial()
    {
        bool flag = false;
        RaycastHit raycastHit;
        if (Physics.Raycast(this.pwc.entity.transform.position + new Vector3(0f, 2f, 0f), Vector3.down, out raycastHit, 2.5f, this.lm))
        {
            if (raycastHit.transform.tag == "Terrain")
            {
                if (this.terrain == null)
                {
                    this.terrain = raycastHit.transform.gameObject.GetComponent<Terrain>();
                }
                TerrainData terrainData = this.terrain.terrainData;
                Vector3 position = this.terrain.transform.position;
                int x = Mathf.FloorToInt((this.pwc.entity.transform.position.x - position.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
                int y = Mathf.FloorToInt((this.pwc.entity.transform.position.z - position.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
                float[, ,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);
                if (alphamaps[0, 0, 0] >= 0.5f)
                {
                    return IMaterial.Sand;
                }
                if (alphamaps[0, 0, 1] >= 0.5f)
                {
                    return IMaterial.Stone;
                }
                if (alphamaps[0, 0, 2] >= 0.5f)
                {
                    return IMaterial.DirtLeaves;
                }
                if (alphamaps[0, 0, 3] >= 0.5f)
                {
                    return IMaterial.Dirt;
                }
            }
            else
            {
                if (raycastHit.transform.name == null)
                {
                    return IMaterial.None;
                }
                if (raycastHit.transform.name.Contains("Rock") || raycastHit.transform.name.Contains("Cliff"))
                {
                    return IMaterial.Stone;
                }
                if (raycastHit.transform.name.Contains("Scaff") || raycastHit.transform.name.Contains("Shed") || raycastHit.transform.name.Contains("Tower") || raycastHit.transform.name.Contains("Chest") || (raycastHit.transform.parent != null && raycastHit.transform.parent.name.Contains("House")) || raycastHit.transform.name.Contains("Platform") || raycastHit.transform.name.Contains("Barrel") || raycastHit.transform.name.Contains("Stairs") || raycastHit.transform.name.Contains("Shed"))
                {
                    return IMaterial.Wood;
                }
            }
            return IMaterial.Wood;
        }
        if (!flag)
        {
            return IMaterial.None;
        }
        return IMaterial.None;
    }

    public enum IMaterial
    {
        None,
        Wood,
        Dirt,
        Stone,
        Sand,
        DirtLeaves
    }
}
