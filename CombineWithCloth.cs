using UnityEngine;
using System.Collections;

public class CombineWithCloth : MonoBehaviour 
{
    public SkinnedMeshRenderer t;
    public GameObject g;

    void Start()
    {
        ProcessBonedObject(t, g);
    }

    private void ProcessBonedObject(SkinnedMeshRenderer ThisRenderer, GameObject RootObj)
    {
        /*      Create the SubObject        */
        var NewObj = new GameObject( ThisRenderer.gameObject.name );
        NewObj.transform.parent = RootObj.transform;
        /*      Add the renderer        */
        NewObj.AddComponent< SkinnedMeshRenderer >();
        var NewRenderer = NewObj.GetComponent< SkinnedMeshRenderer >();
        /*      Assemble Bone Structure     */
        var MyBones = new Transform[ ThisRenderer.bones.Length ];
        for ( var i=0; i<ThisRenderer.bones.Length; i++ )
            MyBones[ i ] = FindChildByName( ThisRenderer.bones[ i ].name, RootObj.transform );
        /*      Assemble Renderer       */
        NewRenderer.bones = MyBones;
        NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
        NewRenderer.materials = ThisRenderer.materials;
    }
    private Transform FindChildByName(string ThisName, Transform ThisGObj )
    {
        Transform ReturnObj = null;
        if( ThisGObj.name==ThisName )
            return ThisGObj.transform;
        foreach (Transform child in ThisGObj )
        {
            ReturnObj = FindChildByName( ThisName, child );
            if( ReturnObj )
                return ReturnObj;
        }
        return null;
    }
}
