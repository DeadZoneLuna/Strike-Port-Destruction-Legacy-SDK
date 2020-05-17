
function Update () {
}
// Emit particles for 2 seconds
GetComponent.<ParticleEmitter>().emit = true;
yield WaitForSeconds(2);
// Then stop
GetComponent.<ParticleEmitter>().emit = false;