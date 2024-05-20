using UnityEngine;
using UnityEngine.Tilemaps;

public class DropShadow_Tilemap : MonoBehaviour
{
    public Material shadowMaterial;  // Assign the ShadowMaterial in the Inspector
    public Vector3 shadowOffset = new Vector3(0.1f, -0.1f, 0);  // Adjust the shadow offset as needed

    private void Start()
    {
        CreateShadowTilemap();
    }

    private void CreateShadowTilemap()
    {
        // Get the original Tilemap and TilemapRenderer components
        Tilemap originalTilemap = GetComponent<Tilemap>();
        TilemapRenderer originalRenderer = GetComponent<TilemapRenderer>();

        if (originalTilemap == null || originalRenderer == null)
        {
            Debug.LogError("Tilemap or TilemapRenderer component missing on the GameObject.");
            return;
        }

        // Create a new GameObject for the shadow tilemap
        GameObject shadowTilemapObject = new GameObject("ShadowTilemap");
        shadowTilemapObject.transform.SetParent(transform);
        shadowTilemapObject.transform.localPosition = shadowOffset;

        // Add Tilemap and TilemapRenderer components to the new GameObject
        Tilemap shadowTilemap = shadowTilemapObject.AddComponent<Tilemap>();
        TilemapRenderer shadowRenderer = shadowTilemapObject.AddComponent<TilemapRenderer>();

        // Copy the tiles from the original tilemap to the shadow tilemap
        BoundsInt bounds = originalTilemap.cellBounds;
        TileBase[] allTiles = originalTilemap.GetTilesBlock(bounds);
        shadowTilemap.SetTilesBlock(bounds, allTiles);

        // Set the sorting order and material for the shadow tilemap
        shadowRenderer.sortingOrder = originalRenderer.sortingOrder - 1;
        shadowRenderer.material = shadowMaterial;
    }
}
