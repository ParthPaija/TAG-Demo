using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Text
using System.Collections.Generic;
using System.Linq;

public class HexGridGenerator : MonoBehaviour
{
    public int gridWidth = 5;
    public int gridHeight = 5;
    public GameObject hexagonPrefab;
    public float horizontalSpacing = 1.0f;
    public float verticalSpacing = 0.866f;

    public List<Color> availableColors = new List<Color>() {
        new Color(1, 0, 0, 1),    // Red
        new Color(0, 1, 0, 1),    // Green
        new Color(0, 0, 1, 1),    // Blue
        new Color(1, 0.92f, 0.016f, 1) // Yellow
    };

    private Dictionary<Vector2Int, Hexagon> hexagonGrid = new Dictionary<Vector2Int, Hexagon>();
    private Hexagon selectedHexagon;
    private Vector3 originalScale = Vector3.one; // Store original scale, assuming all start at (1,1,1)
    private const float SELECTED_SCALE_FACTOR = 1.2f;


    // UI Elements
    public Text movesText; // Assign this in the Inspector
    private int moveCount = 0;

    // Axial coordinate neighbor offsets (flat-topped)
    private static readonly Vector2Int[] axialNeighborOffsets = new Vector2Int[] {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), // E, W
        new Vector2Int(0, 1), new Vector2Int(0, -1), // SE, NW
        new Vector2Int(1, -1), new Vector2Int(-1, 1)  // NE, SW
    };

    void Start()
    {
        // Store original scale from prefab if possible, otherwise assume Vector3.one
        if (hexagonPrefab != null) {
            originalScale = hexagonPrefab.transform.localScale;
        }
        ResetGame(); // Initial setup: generates grid, resets moves, and shuffles grid
    }

    public void GenerateGrid()
    {
        // Clear existing hexagons and grid data
        foreach (Transform child in transform)
        {
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
        hexagonGrid.Clear();

        if (hexagonPrefab == null)
        {
            Debug.LogError("Hexagon Prefab is not assigned!");
            return;
        }
        if (availableColors == null || availableColors.Count == 0)
        {
            Debug.LogError("Available colors list is not set or empty! Adding default white.");
            if (availableColors == null) availableColors = new List<Color>();
            if (availableColors.Count == 0) availableColors.Add(Color.white);
        }

        for (int r = 0; r < gridHeight; r++) 
        {
            for (int q_offset = 0; q_offset < gridWidth; q_offset++) 
            {
                float xPos = q_offset * horizontalSpacing;
                if (r % 2 == 1) 
                {
                    xPos += horizontalSpacing / 2.0f;
                }
                float yPos = r * verticalSpacing;

                GameObject hexInstance = Instantiate(hexagonPrefab, transform);
                hexInstance.transform.localPosition = new Vector3(xPos, yPos, 0);
                hexInstance.name = $"Hexagon_{q_offset}_{r}";
                // hexInstance.transform.localScale = originalScale; // Ensure it starts with original scale

                Hexagon hexagonScript = hexInstance.GetComponent<Hexagon>();
                SpriteRenderer spriteRenderer = hexInstance.GetComponent<SpriteRenderer>();
                Vector2Int gridPos = new Vector2Int(q_offset, r); 

                hexagonScript.gridPosition = gridPos;
                hexagonScript.gridGeneratorReference = this;

                if (availableColors.Count > 0)
                {
                    Color randomColor = availableColors[Random.Range(0, availableColors.Count)];
                    hexagonScript.color = randomColor;
                    if (spriteRenderer != null) spriteRenderer.color = randomColor;
                }
                hexagonGrid[gridPos] = hexagonScript;
            }
        }
    }

    public void IncrementMoveCount()
    {
        moveCount++;
        if (movesText != null)
        {
            movesText.text = "Moves: " + moveCount;
        }
    }
    
    public void ResetGame()
    {
        GenerateGrid(); // Regenerate the grid from scratch

        moveCount = 0;
        if (movesText != null)
        {
            movesText.text = "Moves: " + moveCount;
        }
        else
        {
            Debug.LogWarning("MovesText not assigned in HexGridGenerator.");
        }

        if (selectedHexagon != null)
        {
            selectedHexagon.transform.localScale = originalScale; // Reset scale if one was selected
            selectedHexagon = null;
        }
        
        ShuffleGrid(); // Re-shuffle the (newly generated) grid
        Debug.Log("Game Reset. Moves: 0.");
    }

    public void ShuffleGrid()
    {
        if (hexagonGrid.Count == 0) {
            Debug.LogWarning("Hexagon grid is empty, cannot shuffle.");
            return;
        }
        List<Hexagon> allHexagons = hexagonGrid.Values.ToList();
        
        List<Color> currentColors = allHexagons.Select(h => h.color).ToList();
        System.Random rng = new System.Random();
        List<Color> shuffledColors = currentColors.OrderBy(c => rng.Next()).ToList();

        for (int i = 0; i < allHexagons.Count; i++)
        {
            allHexagons[i].color = shuffledColors[i];
            SpriteRenderer sr = allHexagons[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = shuffledColors[i];
        }
        Debug.Log("Grid shuffled.");
        CheckWinCondition(); 
    }

    private Vector2Int OffsetToAxial(Vector2Int offsetCoords)
    {
        int q = offsetCoords.x - (offsetCoords.y - (offsetCoords.y & 1)) / 2;
        int r = offsetCoords.y;
        return new Vector2Int(q, r);
    }

    public void OnHexagonSelected(Hexagon hexagon)
    {
        if (selectedHexagon == null) // First hexagon selection
        {
            selectedHexagon = hexagon;
            selectedHexagon.transform.localScale = originalScale * SELECTED_SCALE_FACTOR;
            Debug.Log($"Selected: {hexagon.name} at {hexagon.gridPosition}");
        }
        else // Second hexagon selection
        {
            selectedHexagon.transform.localScale = originalScale; // Reset scale of the first selected

            if (selectedHexagon == hexagon) // Clicked the same hexagon again (deselect)
            {
                selectedHexagon = null; 
                Debug.Log($"Deselected: {hexagon.name}");
                return;
            }

            if (AreHexagonsAdjacent(selectedHexagon, hexagon))
            {
                Debug.Log($"Attempting swap between {selectedHexagon.name} and {hexagon.name}");
                SwapHexagons(selectedHexagon, hexagon);
                selectedHexagon = null; // Clear selection after successful swap
            }
            else // Clicked a non-adjacent hexagon
            {
                Debug.LogWarning($"Hexagons {selectedHexagon.name} and {hexagon.name} are not adjacent. Selecting new hexagon.");
                selectedHexagon = hexagon; // Select the new one
                selectedHexagon.transform.localScale = originalScale * SELECTED_SCALE_FACTOR; // Apply visual to new selection
            }
        }
    }

    private bool AreHexagonsAdjacent(Hexagon hex1, Hexagon hex2)
    {
        if (hex1 == null || hex2 == null) return false;
        Vector2Int axial1 = OffsetToAxial(hex1.gridPosition);
        Vector2Int axial2 = OffsetToAxial(hex2.gridPosition);
        int dq = Mathf.Abs(axial1.x - axial2.x);
        int dr = Mathf.Abs(axial1.y - axial2.y);
        int ds = Mathf.Abs(axial1.x + axial1.y - (axial2.x + axial2.y));
        return (dq + dr + ds) / 2 == 1;
    }

    private void SwapHexagons(Hexagon hex1, Hexagon hex2)
    {
        Color tempColor = hex1.color;
        hex1.color = hex2.color;
        hex2.color = tempColor;

        SpriteRenderer sr1 = hex1.GetComponent<SpriteRenderer>();
        SpriteRenderer sr2 = hex2.GetComponent<SpriteRenderer>();
        if (sr1 != null) sr1.color = hex1.color;
        if (sr2 != null) sr2.color = hex2.color;
        
        IncrementMoveCount(); 
        Debug.Log($"Swapped colors of {hex1.name} and {hex2.name}. Moves: {moveCount}");
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (hexagonGrid == null || hexagonGrid.Count == 0) {
            Debug.LogWarning("Win Condition Check: Grid is empty.");
            return;
        }
        if (availableColors == null || availableColors.Count == 0) {
            Debug.LogWarning("Win Condition Check: No available colors defined.");
            return;
        }

        foreach (Color colorToCheck in availableColors)
        {
            List<Hexagon> hexagonsOfColor = new List<Hexagon>();
            foreach (Hexagon hex in hexagonGrid.Values)
            {
                if (hex != null && hex.color.Equals(colorToCheck)) // Using Color.Equals for exact match
                {
                    hexagonsOfColor.Add(hex);
                }
            }

            if (hexagonsOfColor.Count == 0) continue; 

            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            Queue<Hexagon> queue = new Queue<Hexagon>();

            queue.Enqueue(hexagonsOfColor[0]);
            visited.Add(hexagonsOfColor[0]);

            int head = 0; // Using list as a queue for simplicity if Queue<T> is problematic for tool
            List<Hexagon> processingQueue = new List<Hexagon> { hexagonsOfColor[0] };
            
            while(head < processingQueue.Count)
            {
                Hexagon current = processingQueue[head++];
                
                foreach (Hexagon potentialNeighbor in hexagonsOfColor)
                {
                    if (!visited.Contains(potentialNeighbor) && AreHexagonsAdjacent(current, potentialNeighbor))
                    {
                        visited.Add(potentialNeighbor);
                        processingQueue.Add(potentialNeighbor);
                    }
                }
            }
            
            if (visited.Count != hexagonsOfColor.Count)
            {
                // Debug.Log($"Color {colorToCheck} is NOT fully connected. Visited: {visited.Count}, Total: {hexagonsOfColor.Count}");
                return; 
            }
            // Debug.Log($"Color {colorToCheck} IS fully connected.");
        }

        Debug.Log("Congratulations! You Win! All color groups are connected.");
        // Optionally, disable further interaction or show a win screen
        // Example: Time.timeScale = 0; // Pause game
    }
}
