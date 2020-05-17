var projectile : GameObject;
var fireRate = 2;



private var nextFire = 0.0;

function Update () {
    if (Time.time > nextFire) {
        nextFire = Time.time + fireRate;
        var clone = Instantiate (projectile, transform.position, transform.rotation);
    }
}