using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float width = 10; // ������ �������
    private float height = 10; // ������ �������
    [SerializeField] private float thickness = 0.1f;
    [SerializeField] private float padding = 1f;
    [SerializeField] private float elavation = 0f;
    [SerializeField] private Material mat = null;

    public GameObject terrain;



    [HideInInspector]
    public static TerrainGenerator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instance of TerrainGenerator");
        }
    }


    public void CreatNewTerrain()
    {
        Vector2 border = GridManager.Instance.GetSize();
        width = border.x;
        height = border.y;
        CreatePerimeterBackground();
    }

    private void CreatePerimeterBackground()
    {

        float minx = -padding;
        float miny = -padding;
        float maxx = width + padding;
        float maxy = height + padding;


        Destroy(terrain);

        // ������� ������ ��� �������
        terrain = new GameObject("PerimeterBackground");
        MeshFilter meshFilter = terrain.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer>();

        // ������� �������� (����� ��������� ��� ���� �����)
        Material material = mat == null ? new Material(Shader.Find("Standard")): mat;
        meshRenderer.material = material;

        // ������� Mesh
        Mesh mesh = new Mesh();

        // ������� ��� �������
        Vector3[] vertices = new Vector3[16];
        int[] triangles = new int[24];

        // ���������� ������� ��� ������� �������
        // ������ ������
        vertices[0] = new Vector3(maxx, elavation, miny);
        vertices[1] = new Vector3(minx, elavation, miny);
        vertices[2] = new Vector3(minx, elavation, miny + thickness);
        vertices[3] = new Vector3(maxx, elavation, miny + thickness);

        // ������� ������
        vertices[4] = new Vector3(maxx, elavation, maxy);
        vertices[5] = new Vector3(minx, elavation, maxy);
        vertices[6] = new Vector3(minx, elavation, maxy - thickness);
        vertices[7] = new Vector3(maxx, elavation, maxy - thickness);

        // ����� ������
        vertices[8] = new Vector3(minx, elavation, maxy);
        vertices[9] = new Vector3(minx, elavation, miny);
        vertices[10] = new Vector3(minx + thickness, elavation, miny);
        vertices[11] = new Vector3(minx + thickness, elavation, maxy);

        // ������ ������
        vertices[12] = new Vector3(maxx, elavation, miny);
        vertices[13] = new Vector3(maxx, elavation, maxy);
        vertices[14] = new Vector3(maxx - thickness, elavation, maxy);
        vertices[15] = new Vector3(maxx - thickness, elavation, miny);

        // ���������� ������������
        // ������ ������
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
        triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;

        // ������� ������
        triangles[6] = 4; triangles[7] = 6; triangles[8] = 5;
        triangles[9] = 4; triangles[10] = 7; triangles[11] = 6;

        // ����� ������
        triangles[12] = 8; triangles[13] = 10; triangles[14] = 9;
        triangles[15] = 8; triangles[16] = 11; triangles[17] = 10;

        // ������ ������
        triangles[18] = 12; triangles[19] = 14; triangles[20] = 13;
        triangles[21] = 12; triangles[22] = 15; triangles[23] = 14;

        // ����������� ������� � ������������ � Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // ����������� Mesh � MeshFilter
        meshFilter.mesh = mesh;

        // ������������� ������ �� ������ �������
        terrain.transform.position = new Vector3(0, 0, 0);
    }
}