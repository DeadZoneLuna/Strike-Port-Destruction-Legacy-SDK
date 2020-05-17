using SPD.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
[CreateAssetMenu(menuName = "SDK/Audio/GameSoundsObject")]
#endif
public class GameSoundsObject : ScriptableObject 
{
    [Header("Prefab: Game")]
    public BankEntry TeleportIn;
    public BankEntry TeleportOut;
    public BankEntry JumperSound;
    public BankEntry Medkit;
    [Header("Radio")]
    //public BankEntry RoundStart;
    public BankEntry TerroristWin;
    public BankEntry CounterTerroristWin;
    public BankEntry RoundDraw;
    public BankEntry ZombieWin;
    public BankEntry Go;
    public BankEntry SurvivalRound;
    public BankEntry DifuseRadio;
    public BankEntry BombPlanted;
    [Header("Weapons Sounds or missing")]
    public BankEntry Zoom;
    public BankEntry ClipOut;
    public BankEntry ClipIn;
    public BankEntry BullPush;
    public BankEntry DryFire;
    public BankEntry Pip;
    public BankEntry Explode;
    public BankEntry BombPlant;
    public BankEntry Difuse;
    public BankEntry[] WeaponActionDistance;
    public BankEntry[] WeaponAction;
    public BankEntry[] WeaponActionSilencerDistance;
    public BankEntry[] WeaponSilencerAction;
    //public BankEntry[] ShellIn;
    public BankEntry[] GrenadeExplosion;
    public BankEntry[] GrenadeHit;
    public BankEntry GrenadeThrow;
    [Header("Knife Sounds")]
    public BankEntry MidSlashKnife;
    //public BankEntry DrawKnife;
    public BankEntry HitWallKnife;
    public BankEntry HitKnife;
    public BankEntry StabKnife;
    [Header("Quake Sounds")]
    public BankEntry HeadShot;
    public BankEntry MultiKill;
    public BankEntry FragLeft;
    public BankEntry Excelent;
    public BankEntry LostTheLead;
    public BankEntry TieLead;
    public BankEntry TakenLead;
    public BankEntry BlueFlagReturn;
    public BankEntry RedFlagReturn;
    public BankEntry YouHaveAFlag;
    public BankEntry YourTeamHaveFlag;
    public BankEntry EnemyHaveYourFlag;
    public BankEntry BlueScore;
    public BankEntry RedScore;
    public BankEntry Score;
    [Header("Prefab: Player")]
    public BankEntry HeadShot2;
    public BankEntry Death;
    public BankEntry Footsteps;
    public BankEntry PlayerSpawn;
    public BankEntry BodyHit;
    public BankEntry Damage;
    public BankEntry FallDamage;
    public BankEntry Buy;
    public BankEntry Flashlight;
    public BankEntry Exhaused;
    public BankEntry Spray;
    public BankEntry Bulletric;
    [Header("Prefab: Zombie")]
    public BankEntry Sigh;
    public BankEntry Melee;
    [Header("Prefab: Hostage")]
    public BankEntry Rescued;
    public BankEntry LetsGo;
}
