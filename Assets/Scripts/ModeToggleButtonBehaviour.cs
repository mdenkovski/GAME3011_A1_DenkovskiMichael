using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModeToggleButtonBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject miningGame;

    private MiningGameScript miningScript;

    [SerializeField]
    private TMP_Text ButtonText;

    private void Start()
    {
        miningScript = miningGame.GetComponent<MiningGameScript>();
    }

    public void OnToggleButtonPressed()
    {
        miningScript.currentMode = (miningScript.currentMode == GameMode.Extract ? GameMode.Scan: GameMode.Extract);
        ButtonText.text = (miningScript.currentMode == GameMode.Extract ? "Extract" : "Scan");
    }

    private void OnEnable()
    {
        ButtonText.text = "Extract";
        
    }

}
