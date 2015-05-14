using UnityEngine;
using System.Collections;

public class Door : Bolt.EntityBehaviour<IDoor>
{
    bool startCheck = false;
    public override void Attached()
    {
        closed = this.transform.localRotation;
        startCheck = true;
    }

    Quaternion closed;
    public Quaternion opened;
    void Update()
    {
        if (startCheck)
        {
            if (state.open == true)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(closed.x + opened.x, closed.y + opened.y, closed.z + opened.z, closed.w), 0.5f);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(closed.x, closed.y, closed.z, closed.w), 0.5f);
            }
        }
    }
}
