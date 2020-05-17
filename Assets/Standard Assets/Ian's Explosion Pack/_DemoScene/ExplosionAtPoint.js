//ExplosionAtPoint.js

var explosionPrefab : Transform;
var rate : float;

var nextCopy : float;
function Update () {
    if(Input.GetKeyDown("mouse 0")){
    	nextCopy = Time.time+rate;
    	var ray = GetComponent.<Camera>().ScreenPointToRay(Input.mousePosition);
    	var hit : RaycastHit;
    	if(Physics.Raycast(ray,hit)){
       		var rot = Quaternion.FromToRotation(Vector3.up,hit.normal);
        	Instantiate(explosionPrefab,hit.point,Quaternion.identity);
    	}
    }
}