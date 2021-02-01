using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameButtonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject minigame;
    private bool active = false;

    public void ToggleGame()
    {
        //activate minigame
        active = !active;
        minigame.SetActive(active);
    }
}
