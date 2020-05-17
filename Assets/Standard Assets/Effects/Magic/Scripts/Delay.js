
function Update () {
}
// Emit particles delay of 0.5 seconds
GetComponent.<ParticleEmitter>().emit = false;
yield WaitForSeconds(0.5);
// Then start
GetComponent.<ParticleEmitter>().emit = true;
// Then stop
yield WaitForSeconds(1);
GetComponent.<ParticleEmitter>().emit = false;