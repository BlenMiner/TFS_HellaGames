#pragma strict

var skiPar : SkinParenter;
var parentObj : GameObject;
var clothes : GameObject;

function Start () {
    skiPar.parentM = parentObj.GetComponent(SkinParent);
    skiPar.children = new SkinChild[1];
    skiPar.children[0] = clothes.GetComponent(SkinChild);
}

function Update () {

}