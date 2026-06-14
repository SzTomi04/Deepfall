using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private Sprite blockSprite;
    [SerializeField] private int floorLength = 28;
    [SerializeField] private int floorY = -5;
    [SerializeField] private int ceilingY = 7;
    [SerializeField] private int leftWallX = -14;
    [SerializeField] private int rightWallX = 14;
    [SerializeField] private float blockScale = 0.2f;

    [ContextMenu("Build Map")]
    public void BuildMap()
    {
        if (blockSprite == null)
        {
            Debug.LogError("MapBuilder needs a blockSprite assigned.");
            return;
        }

#if UNITY_EDITOR
        // Teljesen töröljük a kijelölést, hogy az Inspector ne hivatkozzon törölt objektumokra
        UnityEditor.Selection.activeObject = null;
#endif

        // Sokkal biztonságosabb törlés transform alapján
        Transform existing = transform.Find("GeneratedMap");
        if (existing != null)
        {
            if (Application.isPlaying) Destroy(existing.gameObject);
            else DestroyImmediate(existing.gameObject);
        }

        GameObject mapRoot = new GameObject("GeneratedMap");
        mapRoot.transform.SetParent(transform);
        mapRoot.transform.localPosition = Vector3.zero;

        CreateLine(mapRoot.transform, "Floor", leftWallX, floorY, floorLength, 1, new Color(0.24f, 0.2f, 0.16f, 1f));
        CreateLine(mapRoot.transform, "Ceiling", leftWallX, ceilingY, floorLength, 1, new Color(0.22f, 0.22f, 0.25f, 1f));
        CreateLine(mapRoot.transform, "LeftWall", leftWallX, floorY + 1, 1, ceilingY - floorY, new Color(0.14f, 0.14f, 0.14f, 1f));
        CreateLine(mapRoot.transform, "RightWall", rightWallX, floorY + 1, 1, ceilingY - floorY, new Color(0.14f, 0.14f, 0.14f, 1f));

        CreatePlatform(mapRoot.transform, "SpawnPlatform", -12, -3, 5, new Color(0.42f, 0.31f, 0.2f, 1f));
        CreatePlatform(mapRoot.transform, "LowerMidLeft", -6, -2, 4, new Color(0.38f, 0.29f, 0.19f, 1f));
        CreatePlatform(mapRoot.transform, "LowerMidRight", 3, -1, 4, new Color(0.38f, 0.29f, 0.19f, 1f));
        CreatePlatform(mapRoot.transform, "Bridge", -1, 1, 4, new Color(0.48f, 0.36f, 0.24f, 1f));
        CreatePlatform(mapRoot.transform, "UpperLeft", -8, 3, 5, new Color(0.5f, 0.38f, 0.25f, 1f));
        CreatePlatform(mapRoot.transform, "UpperRight", 4, 4, 5, new Color(0.5f, 0.38f, 0.25f, 1f));
        CreatePlatform(mapRoot.transform, "ExitPlatform", 9, 0, 3, new Color(0.56f, 0.42f, 0.28f, 1f));

        CreateLine(mapRoot.transform, "CenterPillar", -1, floorY + 1, 1, 6, new Color(0.16f, 0.16f, 0.16f, 1f));
        CreateLine(mapRoot.transform, "RightPillar", 8, floorY + 1, 1, 5, new Color(0.16f, 0.16f, 0.16f, 1f));
    }

    private void CreateLine(Transform parent, string objectName, int startX, int y, int length, int thickness, Color tint)
    {
        for (int i = 0; i < length; i++)
        {
            CreateBlock(parent, objectName + "_" + i, new Vector2(startX + i, y), Vector2.one * thickness * blockScale, tint);
        }
    }

    private void CreatePlatform(Transform parent, string objectName, int startX, int y, int length, Color tint)
    {
        for (int i = 0; i < length; i++)
        {
            CreateBlock(parent, objectName + "_" + i, new Vector2(startX + i, y), Vector2.one * blockScale, tint);
        }
    }

    private void CreateBlock(Transform parent, string objectName, Vector2 position, Vector2 scale, Color tint)
    {
        GameObject block = new GameObject(objectName);
        block.transform.SetParent(parent);
        block.transform.localPosition = new Vector3(position.x, position.y, 0f);
        block.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = blockSprite;
        renderer.color = tint;
        renderer.sortingOrder = 0;

        BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }
}
