using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public enum GameMode
{
    Extract,
    Scan
}

public class MiningGameScript : MonoBehaviour
{
    

    //grid 
    [Header("Grid")]
    private int gridSize = 32;
    public GameObject[,] grid = new GameObject[32, 32];
    [SerializeField]
    private int NumResourceTiles = 5;

    //tile prefab to spawn in
    [Header("Tile Prefab")]
    [SerializeField]
    private GameObject TilePrefab;

    //gameplay restrictions
    [Header("Gameplay")]
    private int MaxNumScans = 6;
    private int numScans = 0;
    private int MaxNumExtractions = 3;
    private int numExtractions = 0;
    public GameMode currentMode = GameMode.Extract;

    //scoring
    [Header("Scoring")]
    int gold = 0;
    [SerializeField]
    private TMP_Text NumGoldText;

    //display
    [Header("Display")]
    [SerializeField]
    private TMP_Text Messages;
    [SerializeField]
    private TMP_Text NumExcavationsRemaining;
    [SerializeField]
    private TMP_Text NumScansRemaining;
    //peripherals to control
    [SerializeField] private GameObject peripherals;


    private void OnEnable()
    {
        ResetGame();
        peripherals.SetActive(true);
    }

    private void OnDisable()
    {

        peripherals.SetActive(false);
    }

    void ResetGame()
    {
        if (transform.childCount != 0) //if we have already have all of the tiles made
        {
            //remove all the children so we can reset the grid
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                //spawn in the grid tiles and set their indicies
                grid[r, c] = Instantiate(TilePrefab, gameObject.transform);
                grid[r, c].GetComponent<TileBehaviour>().SetGridIndices(r, c);
                
            }
        }

        //reset our restrictions and scoring and update the respective labels
        numScans = 0;
        numExtractions = 0;
        gold = 0;
        UpdateGoldLabel();
        SpawnResources();

        currentMode = GameMode.Extract;

        DisplayMessage("");

        UpdateNumScansRemaining();
        UpdateNumExtractionsRemaining();
    }

    //update label for number of extractions remaining
    void UpdateNumExtractionsRemaining()
    {
        NumExcavationsRemaining.text = (MaxNumExtractions - numExtractions).ToString();

    }


    //update label for number of scans remaining
    void UpdateNumScansRemaining()
    {
        NumScansRemaining.text = (MaxNumScans - numScans).ToString();

    }
    
    //update label for number of gold
    void UpdateGoldLabel()
    {
        NumGoldText.text = gold.ToString();
    }

    //choose random grid locations to spawn in a resource
    void SpawnResources()
    {
        for (int i = 0; i < NumResourceTiles; i++)
        {
            int row = Random.Range(0, gridSize); //determine random row
            int col = Random.Range(0, gridSize); //determine random col

            AddResourcesAround(row, col);
        }
    }

    //adds recources for the currounding 2 layers of a point
    void AddResourcesAround(int row, int col)
    {
        //center piece
        AddFullResource(row, col);

        //middle ring
        for (int midRowOffset = -1; midRowOffset <= 1; midRowOffset++)
        {
            for (int midColOffset = -1; midColOffset <= 1; midColOffset++)
            {
                if (midRowOffset != 0 || midColOffset != 0) //make sure it is not the middle piece
                {
                    AddHalfResource(row + midRowOffset, col+ midColOffset);
                }
            }
        }

        //outer ring
        for (int outerRowOffset = -2; outerRowOffset <= 2; outerRowOffset++)
        {
            for (int outerColOffset = -2; outerColOffset <= 2; outerColOffset++)
            {
                if (Mathf.Abs(outerRowOffset) > 1 || Mathf.Abs(outerColOffset) > 1) //make sure that it is only the outer ring piece
                {
                    AddQuarterResource(row + outerRowOffset, col + outerColOffset);
                }
            }
        }
    }

    void AddQuarterResource(int row, int col)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize) return; // do nothing  if the tile is out of bounds of the grid

        TileBehaviour quarterTile = grid[row, col].GetComponent<TileBehaviour>();
        if (quarterTile.ResourceValue < 1)
        {
            quarterTile.ResourceValue = 1;
            //grid[row, col].GetComponent<Image>().color = Color.green; //debug to show the value at start
        }
    }

    void AddHalfResource(int row, int col)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize) return; // do nothing  if the tile is out of bounds of the grid

        TileBehaviour HalfTile = grid[row, col].GetComponent<TileBehaviour>();
        if (HalfTile.ResourceValue < 2)
        {
            HalfTile.ResourceValue = 2;
            //grid[row, col].GetComponent<Image>().color = Color.yellow; //debug to show the value at start
        }
    }

    void AddFullResource(int row, int col)
    {
        TileBehaviour mainTile = grid[row, col].GetComponent<TileBehaviour>();
        if (mainTile.ResourceValue != 4)
        {
            mainTile.ResourceValue = 4;
            //grid[row, col].GetComponent<Image>().color = Color.red; //debug to show the value at start
        }

    }

    //perform specific action when i tile is pressed based on the game mode
    public void TilePressed(int row, int col)
    {
        if (currentMode == GameMode.Extract)
        {
            Extract(row, col);
        }
        else
        {
            ScanTiles(row, col);
        }
    }

    void ScanTiles(int row, int col)
    {
        if (numScans == MaxNumScans || numExtractions == MaxNumExtractions) return;//dont allow for any more scans if above limit

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {

                DisplayTileColour(row + rowOffset, col + colOffset); //display tile colours for tiles around scan point
                
            }
        }

        //update the num scans
        numScans++;
        UpdateNumScansRemaining();
    }

    /// <summary>
    /// display the tile colour based on the resource value of the tile
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    void DisplayTileColour(int row, int col)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize) return; // do nothing  if the tile is out of bounds of the grid

        TileBehaviour Tile = grid[row, col].GetComponent<TileBehaviour>();
        if (Tile.ResourceValue == 4)
        {
            grid[row, col].GetComponent<Image>().color = Color.red;
        }
        else if (Tile.ResourceValue == 2)
        {
            grid[row, col].GetComponent<Image>().color = Color.yellow;
        }
        else if (Tile.ResourceValue == 1)
        {
            grid[row, col].GetComponent<Image>().color = Color.green;
        }
        else
        {
            grid[row, col].GetComponent<Image>().color = Color.grey;
        }
    }

    void Extract(int row, int col)
    {
        if (numExtractions == MaxNumExtractions) return;//dont allow for any more extractions if above limit

        //center piece
        CollectResource(row, col);


        //outer rings
        for (int rowOffset = -2; rowOffset <= 2; rowOffset++)
        {
            for (int colOffset = -2; colOffset <= 2; colOffset++)
            {
                if (rowOffset != 0 || colOffset != 0)
                {
                    HalveResource(row + rowOffset, col + colOffset); //reduce the resources of the surrounding tiles
                }
            }
        }
        //update num extractions
        numExtractions++;
        UpdateNumExtractionsRemaining();

        //display a message if at max extractions
        if (numExtractions == 3)
        {
            DisplayMessage("You managed to obtain " + gold + " gold!");
        }
    }

    /// <summary>
    /// consume the resource amount and update labels
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    void CollectResource(int row, int col)
    {
        TileBehaviour Tile = grid[row, col].GetComponent<TileBehaviour>();
        gold += Tile.ResourceValue;
        Tile.ResourceValue = 0;

        grid[row, col].GetComponent<Image>().color = Color.grey;
        UpdateGoldLabel();
    }

    void HalveResource(int row, int col)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize) return; // do nothing  if the tile is out of bounds of the grid
        
        TileBehaviour Tile = grid[row, col].GetComponent<TileBehaviour>();
        Tile.ResourceValue = Mathf.RoundToInt(Tile.ResourceValue - Tile.ResourceValue*0.5f);
        if(Tile.gameObject.GetComponent<Image>().color != Color.white) //only update the colour for a tile that already had a colour showing
        {

                DisplayTileColour(row, col);

        }
    }

    /// <summary>
    /// dislay text in the message box
    /// </summary>
    /// <param name="message"></param>
    void DisplayMessage(string message)
    {
        Messages.text = message;
    }


    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    
}
