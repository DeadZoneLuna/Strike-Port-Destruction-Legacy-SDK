
function Update () {
}
// Emit particles for 3 seconds
GetComponent.<ParticleEmitter>().emit = true;
yield WaitForSeconds(3);
// Then stop
GetComponent.<ParticleEmitter>().emit = false;