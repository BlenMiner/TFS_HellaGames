//attach to any gameobject, preferably the parent object root
var bones : Transform[];
//this is the mesh of the parent object
var mesh : SkinnedMeshRenderer[];
//this would be the root of the parent object. It's whatever the root is for when you drag the model file into the scene
var objRoot : Transform;

@ContextMenu ("Autopopulate") //autopopulate for child and parent should result in the same if they use the same bone structure
function Autofill(){
	for(var i = 0; i < mesh.Length; i++)
        {
        	bones += mesh[i].bones;
        }
}
//you can double check by looking at the bones array after autopopulating both