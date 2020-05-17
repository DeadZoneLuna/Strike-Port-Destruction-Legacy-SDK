
function Update () {
}
// Emit particles delay of 1.05 seconds
GetComponent.<ParticleEmitter>().emit = false;
yield WaitForSeconds(1.05);
// Then start
GetComponent.<ParticleEmitter>().emit = true;
// Then stop
yield WaitForSeconds(0.7);
GetComponent.<ParticleEmitter>().emit = false;