var timeKill : float;
private var randomSpeed : float;

function Start() {
 transform.eulerAngles = Vector3(0,180,0);
 timeKill = Time.time;
 randomSpeed = Random.Range(7.0,9.0);
 }

function Update () {
    // Move the object 10 meters per second!
    var translation : float = Time.deltaTime * randomSpeed;
    transform.Translate (translation, 0, 0);
    
    if ( Time.time > timeKill + 6 ) Destroy (gameObject);
    
}