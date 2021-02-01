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
        //change the current mode to opposite
        miningScript.currentMode = (miningScript.currentMode == GameMode.Extract ? GameMode.Scan: GameMode.Extract);
        //update the button label
        ButtonText.text = (miningScript.currentMode == GameMode.Extract ? "Extract" : "Scan");
    }

    private void OnEnable()
    {
        //start default of extract mode
        ButtonText.text = "Extract";
        
    }

}
