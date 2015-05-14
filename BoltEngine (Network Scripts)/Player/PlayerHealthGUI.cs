using UnityEngine;
using System.Collections;

public class PlayerHealthGUI : Bolt.GlobalEventListener
{
    public BoltEntity me;
    public ITFSPlayerState meState;

    public OptionMenuItems optionMenu;

    public Animator anim;

    int health = 0;
    int stamina = 0;
    int hunger = 0;

    public bool showGUI = false;

    public override void ControlOfEntityGained(BoltEntity arg)
    {
        if (!arg.name.Contains("TFSPlayer"))
            return;

        if (optionMenu == null)
        {
            GameObject g = GameObject.FindGameObjectWithTag("OptionMenu");
            if (g != null)
                optionMenu = GameObject.FindGameObjectWithTag("OptionMenu").GetComponent<OptionMenuItems>();
        }

        if (optionMenu != null)
            optionMenu.gameObject.SetActive(false);

        me = arg;
        meState = me.GetState<ITFSPlayerState>();

        meState.AddCallback("health", UpdateHealh);
        meState.AddCallback("stamina", UpdateHealh);
        meState.AddCallback("hunger", UpdateHealh);

        meState.AddCallback("exitMenu", UpdateExitMenu);
        GetComponent<PlayerCraftingSystem>().AttachedD();
        showGUI = true;
    }
    public override void ControlOfEntityLost(BoltEntity arg)
    {
        showGUI = false;

        me = null;
        meState = null;
    }

    void UpdateHealh()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)meState;
        health = actorState.health;
        stamina = actorState.stamina;
        hunger = actorState.hunger;
    }
    void UpdateExitMenu()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)meState;
        if (optionMenu != null)
            optionMenu.gameObject.SetActive(actorState.exitMenu);
    }
    
    BindCircleForShader image;
    BindCircleForShader hungeri;
    BindCircleForShader staminai;
    //public AmplifyMotionEffect mb;
    //public SSAOPro ssao;
    //public SENaturalBloomAndDirtyLens bloom;

    public Texture2D deathScreen;

    void Update()
    {
        if(showGUI)
        {
            if (optionMenu != null)
                optionMenu.gameObject.SetActive(meState.exitMenu);

            //mb.enabled = optionMenu.MotionBlur.isOn;
            //ccc.enabled = optionMenu.ColorCorrection.isOn;
            //ssao.enabled = optionMenu.SSAOEffect.isOn;
            //bloom.enabled = optionMenu.BloomLens.isOn;

            if(meState.showInventory || meState.exitMenu || meState.crafting)
            {
                Screen.showCursor = true;
                Screen.lockCursor = false;
            }
            else
            {
                Screen.showCursor = false;
                Screen.lockCursor = true;
            }

            if (health <= 0 && deathScreen != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), deathScreen);
            }
        }

        if (showGUI)
        {
            if (image == null || hungeri == null || staminai == null)
            {
                GameObject result = GameObject.FindGameObjectWithTag("Health");
                GameObject result1 = GameObject.FindGameObjectWithTag("Hunger");
                GameObject result2 = GameObject.FindGameObjectWithTag("Stamina");
                if (result != null || result1 != null || result2 != null)
                {
                    image = result.GetComponent<BindCircleForShader>();
                    hungeri = result1.GetComponent<BindCircleForShader>();
                    staminai = result2.GetComponent<BindCircleForShader>();
                }
            }
            else if (image != null)
            {
                image.value = (float)((float)health / (float)100);
                hungeri.value = (float)((float)hunger / (float)800);
                staminai.value = (float)((float)stamina / (float)100);
            }
        }
    }
}