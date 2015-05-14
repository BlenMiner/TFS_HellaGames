//attach this to any game object. It will parent all child objects to the parent object
var parentM : SkinParent;

//a list of all the child objects to parent to the SkinParent
var children : SkinChild[];

//upon this scene starting, parent all the children to the parent.
function Start(){
	
	//move roots to parents position
	/*for (var childM : SkinChild in children){
		childM.objRoot.position = parentM.objRoot.position;
	}*/
	
	for (var childM : SkinChild in children){
		
		for (var i : int = 0; i < childM.bones.length; i++)
        {
			childM.bones[i].parent = parentM.bones[i];
		}
		//now that we've changed the bone values, parent it to the correct transform
		childM.mesh.transform.parent = parentM.mesh.transform.parent;
		childM.mesh.transform.localPosition = Vector3.zero;
	}
}