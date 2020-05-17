var target : Transform;
var speed : float;
var smokeDestroyTime : float;
var smokeStem : ParticleRenderer;
var destroySpeed : float;
var destroySpeedStem : float;

private var destroyEnabled = false;

function Start () {
	yield WaitForSeconds(smokeDestroyTime);
	destroyEnabled = true;
}

function Update () {
	transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime*speed);
	if(destroyEnabled == true){
		var render = GetComponent(ParticleRenderer);
		var col : Color = render.material.GetColor("_TintColor");
		var col2 : Color = smokeStem.material.GetColor("_TintColor");
		if(col.a > 0){
			col.a -= destroySpeed*Time.deltaTime;
		}	
		if(col2.a > 0){
			col2.a -= destroySpeedStem*Time.deltaTime;
		}	
		smokeStem.material.SetColor("_TintColor",col2);
		render.material.SetColor("_TintColor",col);
	}
	if(col.a < 0){
		Destroy(transform.root.gameObject);
	}
}