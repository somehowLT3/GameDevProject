using UnityEngine;
using System.Collections.Generic;


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
    public float columnWidth = 1.5f;
    public float columnLength = 3f;

    [Header("Height Range")]
    public float minHeight = 2f;
    public float maxHeight = 8f;

    [Header("Spacing")]
    public float gapFromCenter = 10f;

    [Header("Visual")]
    public Material topMaterial;
    public Material bottomMaterial;
    public Material finishMaterial;

    [Header("Turrets")]
    public GameObject turretPrefab;
    public float turretStartingZ = 40f;
    public float turretMinHeight = 3f;
    public float turretMaxHeight = 6f;
    public float turretChance = 0.25f;

    [Header("Spikes")]
    public List<GameObject> spikePrefabs;
    public float spikeChance = 0.05f;

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

        segments = GameSettings.segments;
        depth = GameSettings.depth;

        turretChance = GameSettings.turretChance;
        spikeChance = GameSettings.spikeChance;

        minHeight = GameSettings.minHeight;
        maxHeight = GameSettings.maxHeight;

        turretMinHeight = Mathf.Max(minHeight * 1.5f, 2f);
        turretMaxHeight = Mathf.Min(maxHeight * 0.8f, maxHeight - 2f);

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

            TrySpawnSpikeRow(zPos);
        }

        CreateStartLine();
        CreateFinishLine();
    }

    void TrySpawnSpikeRow(float zPos)
    {

        if (spikePrefabs.Count == 0)
            return;

        // only if above chance
        if (Random.value > spikeChance)
            return;

        // start after start line
        if (zPos < columnLength * 6)
            return;

        // random x pos
        float xPos = Random.Range(
            -gapFromCenter + 1f,
            gapFromCenter - 1f
        );

        Vector3 spikePos = new(xPos, 0f, zPos);

        //get random spike prefab
        GameObject spike = Instantiate(
            spikePrefabs[Random.Range(0, spikePrefabs.Count)],
            spikePos,
            Quaternion.identity,
            transform
        );

        // random rotation
        spike.transform.rotation = Quaternion.Euler(
            0,
            Random.Range(0f, 360f),
            0
        );

        // make damaging
        spike.tag = "Damage";
    }

    void CreateStartLine()
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // remove collider without destroying object
        DestroyImmediate(visual.GetComponent<Collider>());

        Renderer visualRenderer = visual.GetComponent<Renderer>();

        if (finishMaterial != null)
        {
            visualRenderer.sharedMaterial = finishMaterial;
        }

        visual.name = "StartVisual";

        visual.transform.position = new Vector3(
            0,
            0.05f,
            columnLength * 5
        );

        visual.transform.localScale = new Vector3(gapFromCenter * 2f, 0.1f, 3f);

        visual.transform.parent = transform;
    }

    void CreateFinishLine()
    {
        GameObject finish = GameObject.CreatePrimitive(PrimitiveType.Cube);

        finish.name = "FinishLine";

        float width = gapFromCenter * 2f;

        finish.transform.position = new Vector3(
            0,
            1f,
            segments * columnLength
        );

        finish.transform.localScale = new Vector3(width, 2f, 1f);

        BoxCollider col = finish.GetComponent<BoxCollider>();
        col.isTrigger = true;

        // make trigger invisible
        Renderer renderer = finish.GetComponent<Renderer>();
        renderer.enabled = false;

        finish.tag = "Finish";

        finish.transform.parent = transform;

        // visual

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // remove collider without destroying object
        DestroyImmediate(visual.GetComponent<Collider>());

        Renderer visualRenderer = visual.GetComponent<Renderer>();

        if (finishMaterial != null)
        {
            visualRenderer.sharedMaterial = finishMaterial;
        }

        visual.name = "FinishVisual";

        visual.transform.position = new Vector3(
            0,
            0.05f,
            segments * columnLength
        );

        visual.transform.localScale = new Vector3(width, 0.1f, 3f);

        visual.transform.parent = transform;
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