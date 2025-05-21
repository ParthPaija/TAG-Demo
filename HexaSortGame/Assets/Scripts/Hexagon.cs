using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public Color color;
    public Vector2Int gridPosition; // To store its coordinates within the grid
    public HexGridGenerator gridGeneratorReference; // Reference to the grid generator

    // Potential future state variables can be added here
    // public int type;
    // public bool isMatched;

    void OnMouseDown()
    {
        if (gridGeneratorReference != null)
        {
            gridGeneratorReference.OnHexagonSelected(this);
        }
        else
        {
            Debug.LogError("HexGridGenerator reference not set on " + gameObject.name);
        }
    }
}
