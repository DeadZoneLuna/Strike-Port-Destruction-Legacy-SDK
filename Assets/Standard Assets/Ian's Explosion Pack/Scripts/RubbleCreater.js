@script AddComponentMenu("ExplosionScripts/BetCopyer")
var bits : Rigidbody;
var vel : float = 20;
var up : float = 20;
var amount : int = 20;
var amountRandom : int = 5;
var useUp = false;
private var soundClip : AudioClip;

function Start () {
//	InvokeRepeating("CreateObject",0,.2);
	CreateObject();
}
function CreateObject () {
	amount += Random.Range(-amountRandom,amountRandom);
	for (var i = 0; i < amount; i++) {
		
	 var stuff = Instantiate(bits,transform.position,transform.rotation);
	 var random1 = Random.Range(-vel,vel);
	 var random2 = Random.Range(5,up);
	 var random3 = Random.Range(-vel,vel);
	 if(useUp == true){
	 stuff.GetComponent.<Rigidbody>().velocity = transform.TransformDirection(random1,Random.Range(up/2,up),random3);
	 }
	 else{
	 	stuff.GetComponent.<Rigidbody>().velocity = transform.TransformDirection(random1,Random.Range(-up,up),random3);
	 }
}



}