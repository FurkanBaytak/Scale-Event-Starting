using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    private static DeathHandler instance;
    public bool IsAlive;
    
    public static DeathHandler GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }

    private DeathHandler() { }

    public void KillPlayer()
    {
        // TODO KILL LOGIC
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        bool isGodMode = player.GetComponent<PlayerMovement>().isGodMode;
        if (!isGodMode)
        {
            Destroy(player);
            Debug.Log("Player killed!");
            IsAlive = false;
        }
    }
    
}
