using UnityEngine;

public class AmmoScaleEvent : BaseEvent
{

    private int defaultAmmo;
    private float defaultFireRate;

    public AmmoScaleEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        WeaponBase weaponBase = player.GetComponentInChildren<WeaponBase>();
        defaultAmmo = weaponBase.maxAmmo;
        defaultFireRate = weaponBase.fireRate;
    }

    public override void StartEvent()
    {
        Debug.Log("Ammo Scale Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        WeaponBase weaponBase = player.GetComponentInChildren<WeaponBase>();
        weaponBase.maxAmmo = 99999;
        weaponBase.SetAmmo(99999);
        weaponBase.fireRate = defaultFireRate / 5;
    }
    
    public override void StopEvent()
    {
        Debug.Log("Ammo Scale Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        WeaponBase weaponBase = player.GetComponentInChildren<WeaponBase>();
        weaponBase.maxAmmo = defaultAmmo;
        weaponBase.SetAmmo(defaultAmmo);
        weaponBase.fireRate = defaultFireRate;
    }
    
    public override string GetName() {
        return "Ammo Scale Up";
    }
    
    public override float GetEventDuration()
    {
        return 5;
    }
    
}
