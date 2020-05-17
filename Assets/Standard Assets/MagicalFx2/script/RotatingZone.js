#pragma strict
var rotationSpeed : float = 1;


function Update () {

 transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);

}