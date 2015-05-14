using UnityEngine;
using UnityEngine.UI;

public class MainMenus : MonoBehaviour
{
    public Text ModeText;
    public enum Modes
    {
        Normal,
        Insane
    }
    public Modes Mode = Modes.Normal;

    void Update()
    {
        CheckMode();
    }

    public void SwitchMode(bool Next = true)
    {
        if (Next)
        {
            if (Mode == Modes.Normal)
            {
                Mode = Modes.Insane;
                ModeText.text = "Insane";
            }
        }
        else
        {
            if (Mode == Modes.Insane)
            {
                Mode = Modes.Normal;
                ModeText.text = "Normal";
            }
        }
    }

    void CheckMode()
    {
        switch (ModeText.text)
        {
            case "Normal":
                Mode = Modes.Normal;
                break;

            case "Insane":
                Mode = Modes.Insane;
                break;
        }
    }
}