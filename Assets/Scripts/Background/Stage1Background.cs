using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Stage1Background : MonoBehaviour
{
    public Transform groundTrans;
    public float groundMoveSpeed;
    private float groundOffset = 18f;

    public Material flowerMaterial;
    private Mesh[] flowerMeshes;
    private const int flowerCount = 512;
    private Transform[] flowerTrans;

    public Material fogMaterial;
    private Mesh[] fogMesh;

    private void Awake()
    {
        flowerMeshes = new Mesh[9];
        Vector3[] rectVerts = new Vector3[] {
            new Vector3(0, 1),
            new Vector3(1, 1),
            new Vector3(1, 0),
            new Vector3(0, 0),
        };
        int[] rectTris = new int[] {
            0, 1, 2,
            0, 2, 3,
        };
        float v1 = 0.5f;
        float v2 = 0f;
        for (int i = 0; i < flowerMeshes.Length; i++)
        {
            Mesh mesh = new Mesh();
            mesh.name = "FlowerMesh" + i;
            mesh.vertices = rectVerts;
            mesh.triangles = rectTris;
            float u1 = (i + 1) / 12f;
            float u2 = (i + 2) / 12f;
            mesh.uv = new Vector2[] {
                new Vector2(u1, v1),
                new Vector2(u2, v1),
                new Vector2(u2, v2),
                new Vector2(u1, v2),
            };
            flowerMeshes[i] = mesh;
        }

        flowerTrans = new Transform[flowerCount];
        GameObject flowers = new GameObject("Flowers");
        flowers.transform.parent = transform;
        for (int i = 0; i < flowerCount; i++)
        {
            GameObject flowerObj = new GameObject("Flower");
            flowerObj.transform.parent = flowers.transform;
            float x = (1 - Random.Range(0, 2) * 2) * Random.Range(2f, 8f) - 0.5f;
            float z = Random.Range(0f, 30f);
            flowerObj.transform.position = new Vector3(x, -0.1f, z);
            MeshRenderer renderer = flowerObj.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material = flowerMaterial;
            MeshFilter filter = flowerObj.AddComponent<MeshFilter>();
            filter.mesh = flowerMeshes[Random.Range(0, flowerMeshes.Length)];
            flowerTrans[i] = flowerObj.transform;
        }
        
        Vector3[] fogVerts = new Vector3[] {
            new Vector3(-10, 0, -12),
            new Vector3(-10, 10, -12),
            new Vector3(-8, 0, 0),
            new Vector3(-8, 10, 0),
            new Vector3(-2, 0, 5),
            new Vector3(-2, 10, 5),
            new Vector3(2, 0, 5),
            new Vector3(2, 10, 5),
            new Vector3(8, 0, 0),
            new Vector3(8, 10, 0),
            new Vector3(10, 0, -12),
            new Vector3(10, 10, -12),
        };
        int[] fogTris = new int[] {
            0, 1, 2,
            2, 1, 3,
            2, 3, 4,
            4, 3, 5,
            4, 5, 6,
            6, 5, 7,
            6, 7, 8,
            8, 7, 9,
            8, 9, 10,
            10, 9, 11,
        };
        Color[] fogColors = new Color[fogVerts.Length];
        for (int i = 0; i < fogColors.Length; i++)
            fogColors[i] = Color.white;

        GameObject fogs = new GameObject("Fogs");
        fogs.transform.parent = transform;
        fogMesh = new Mesh[50];
        for (int i = 0; i < fogMesh.Length; i++)
        {
            GameObject fog = new GameObject("Fog");
            fog.transform.parent = fogs.transform;
            fog.transform.position = new Vector3(0, 0, 15f + i * 0.3f);
            MeshRenderer renderer = fog.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material = fogMaterial;
            MeshFilter filter = fog.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            for (int k = 0; k < fogColors.Length; k++)
                fogColors[k].a = (i + 1f) / fogMesh.Length;
            mesh.vertices = fogVerts;
            mesh.triangles = fogTris;
            mesh.colors = fogColors;
            filter.mesh = mesh;
            fogMesh[i] = mesh;
        }
    }

    void Update () {
        groundOffset -= groundMoveSpeed * Time.timeScale;
        if (groundOffset < -12f)
            groundOffset += 30f;
        groundTrans.position = new Vector3(0, 0, groundOffset);

        for (int i = 0; i < flowerCount; i++)
        {
            Vector3 offset = flowerTrans[i].position;
            offset.z -= groundMoveSpeed * Time.timeScale;
            if (offset.z < 0)
                offset.z += 30f;
            flowerTrans[i].position = offset;
        }
    }
}
