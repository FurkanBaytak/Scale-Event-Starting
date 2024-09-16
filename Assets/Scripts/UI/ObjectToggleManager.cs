using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectToggleManager : MonoBehaviour
{
    public List<GameObject> objectsToEnable;   
    public List<GameObject> objectsToDisable;  
    public Button toggleButton;                

    void Start()
    {
        toggleButton.onClick.AddListener(() => ToggleObjects());
    }

    void ToggleObjects()
    {
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }
}
