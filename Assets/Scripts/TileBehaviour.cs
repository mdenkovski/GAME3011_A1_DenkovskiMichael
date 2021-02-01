using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileBehaviour : MonoBehaviour, IPointerClickHandler
{

    public int row;
    public int col;

    public int ResourceValue = 0;

    private MiningGameScript mgs;

    public void SetGridIndices(int r, int c)
    {
        row = r;
        col = c;
    }

    // Start is called before the first frame update
    void Start()
    {
        mgs = transform.parent.GetComponent<MiningGameScript>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Row: " + row + " Column: " + col);

        mgs.TilePressed(row, col);
    }
}
