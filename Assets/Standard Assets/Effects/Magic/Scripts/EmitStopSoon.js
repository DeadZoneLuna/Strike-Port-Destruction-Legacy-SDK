
function Update () {
}
// Emit particles for 1 seconds
GetComponent.<ParticleEmitter>().emit = true;
yield WaitForSeconds(1);
// Then stop
GetComponent.<ParticleEmitter>().emit = false;