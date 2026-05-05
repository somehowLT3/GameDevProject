using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(CanyonGenerator))]
public class CanyonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CanyonGenerator gen = (CanyonGenerator)target;

        if (GUILayout.Button("Regenerate Canyon"))
        {
            gen.Generate();
        }
    }
}
#endif

[ExecuteAlways]
public class CanyonGenerator : MonoBehaviour
{
    [Header("Canyon Shape")]
    public int segments = 50;
    public int depth = 3;

    [Header("Column Size")]
    public float columnWidth = 1f;
    public float columnLength = 5f;

    [Header("Height Range")]
    public float minHeight = 2f;
    public float maxHeight = 8f;

    [Header("Spacing")]
    public float gapFromCenter = 5f;

    [Header("Visual")]
    public Material topMaterial;
    public Material bottomMaterial;

    [Header("Turrets")]
    public GameObject turretPrefab;
    public float turretStartingZ = 20f;
    public float turretMinHeight = 3f;
    public float turretMaxHeight = 6f;
    public float turretChance = 0.25f;

    private MaterialPropertyBlock propBlock;

    void OnEnable()
    {
        Generate();
    }

    void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void Generate()
    {
        Clear();

        float heightStep = (maxHeight - minHeight) / depth;

        // segment
        for (int z = 0; z < segments; z++)
        {
            float zPos = z * columnLength;

            // layer
            for (int d = 0; d < depth; d++)
            {
                // height range for this layer
                float layerMin = minHeight + heightStep * d;
                float layerMax = minHeight + heightStep * (d + 1);

                float height = Random.Range(layerMin, layerMax);

                // left (-x)
                float xLeft = -gapFromCenter - d * columnWidth;

                CreateColumn(new Vector3(xLeft, height / 2f, zPos), height);

                // right (+x)
                float xRight = gapFromCenter + d * columnWidth;

                CreateColumn(new Vector3(xRight, height / 2f, zPos), height);
            }
        }
    }

    void TrySpawnTurret(Transform column, Vector3 position, float height)
    {
        if (turretPrefab == null) return;

        if (height < turretMinHeight || height > turretMaxHeight)
            return;

        // spawn turret on top of column
        Vector3 turretPos = position + new Vector3(0, height / 2f, 0);

        GameObject turret = Instantiate(turretPrefab, turretPos, Quaternion.identity, transform);

        // align turret facing middle
        turret.transform.LookAt(new Vector3(0, turretPos.y, position.z));
    }

    void CreateColumn(Vector3 position, float height)
    {
        GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cube);

        column.transform.position = position;
        column.transform.localScale = new Vector3(columnWidth, height, columnLength);

        column.transform.parent = transform;

        // colour gradient from inputs
        if (bottomMaterial != null && topMaterial != null)
        {
            propBlock = new MaterialPropertyBlock();
            Renderer renderer = column.GetComponent<Renderer>();

            float t = Mathf.InverseLerp(minHeight, maxHeight, height);
            Color color = Color.Lerp(bottomMaterial.color, topMaterial.color, t);

            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", color);
            renderer.SetPropertyBlock(propBlock);
        }

        // spawn turret on random valid positions
        if (position.z > turretStartingZ && Random.value < turretChance)
        {
            TrySpawnTurret(column.transform, position, height);
        }
    }
}