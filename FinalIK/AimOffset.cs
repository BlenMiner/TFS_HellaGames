using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class AimOffset : MonoBehaviour 
{
    public AimIK aimIK;
    public Transform target;

    void LateUpdate()
    {
        aimIK.solver.IKPosition = target.position;
    }

}
