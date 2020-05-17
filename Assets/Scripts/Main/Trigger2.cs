#define Final
using UnityEngine;
using SPD.EditorAttributes;
#pragma warning disable 0618

public interface IReset
{
    void Reset();
}

[RequireComponent(typeof(Collider))]
public class Trigger2 : Bs, IReset
{
    [CustomToggle("Listing Events OnTriggerEnter")]
    public bool ListingOnTriggerEnter = true;
    [CustomToggle("OnTriggerEnter Once (Globally)")]
    public bool TriggerEnterOnce;
    public BetterEvent OnTriggerEnterEvents;

    [CustomToggle("Listing Events OnTriggerExit")]
    public bool ListingOnTriggerExit;
    [CustomToggle("OnTriggerExit Once (Globally)")]
    public bool TriggerExitOnce;
    public BetterEvent OnTriggerExitEvents;

#if UNITY_EDITOR
    [CustomToggle("Show Gizmos")]
    public bool ShowGizmos = true;
    void OnDrawGizmos()
    {
        if (ShowGizmos)
        {
            if (GetComponent<Collider>() != null)
            {
                Gizmos.color = new Color32(0, 255, 0, 150);
                Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
            }
        }
    }
#endif

    public override void Awake()
    {

    }

    public void Start()
    {

    }

    public override void OnPlConnected()
    {

    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void CreateObject(GameObject obj, Transform to, bool scaleByTransform)
    {

    }

    public void CreateObject(GameObject obj, Transform to, Vector3 scale)
    {

    }

    public void CreateObject(GameObject obj, Vector3 Position, Vector3 Rotation)
    {

    }

    public void CreateObject(GameObject obj, Vector3 Position, Vector3 Rotation, Vector3 scale)
    {

    }

    public void CreateNPC(int TeamType, int PlayerType, string[] Weapons, Transform to)
    {

    }

    public void CreateNPC(int count, int TeamType, int PlayerType, string[] Weapons, Transform to)
    {

    }

    public void CreateNPC_RandomSpawn(int count, int TeamType, int PlayerType, string[] Weapons, Transform[] to)
    {

    }

    public void RemoveNPC()
    {

    }

    public void Notification_Text(string Text, bool OnlyPlayerOnTrigger, bool Special)
    {

    }

    public void Chat_Global(string Text, bool OnlyPlayerOnTrigger)
    {

    }

    public void Chat_OnPlayerTrigger(string Text, bool OnlyPlayerOnTrigger)
    {

    }

    public void Player_SetHP(int Value)
    {

    }

    public void Player_Die()
    {

    }

    public void Player_Explode()
    {

    }

    public void Player_SetArmour(int Value, bool isArmour)
    {

    }

    public void Player_Teleport(Transform to, bool withSounds)
    {

    }

    public void Player_TeleportAll(Transform to)
    {
        
    }

    public void Player_MakeZombie()
    {

    }

    public void Player_GiveMoney(int Value)
    {

    }

    public void Player_GiveWeapon(string WeaponName, bool Replace)
    {

    }

    public void Player_GiveWeapon(string WeaponName, bool Replace, int Ammo, int Clips, bool inSilencer)
    {

    }

    public void Player_RemoveWeapons()
    {

    }

    public void Player_DropWeapon()
    {

    }

    public void Reset()
    {

    }
}