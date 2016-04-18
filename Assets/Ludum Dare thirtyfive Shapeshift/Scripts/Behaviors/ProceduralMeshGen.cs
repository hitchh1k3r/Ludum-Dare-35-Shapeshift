using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class ProceduralMeshGen : MonoBehaviour
{

  [Range(0, 31)]
  public int colorIndex;
  public bool recalculateOrigin = false;

  [TextAreaAttribute]
  public string vertex;

  private int uvWidth = 32;
  private MeshFilter meshFilter;
  private PolygonCollider2D polygonCollider2D;

  void Awake()
  {
    meshFilter = GetComponent<MeshFilter>();
    polygonCollider2D = GetComponent<PolygonCollider2D>();

    #if UNITY_EDITOR
      allInstances.Add(this);
    #endif

    Rebuild();
  }

  void Rebuild()
  {
    Vector3 sum = Vector3.zero;
    List<Vector3> verts = new List<Vector3>();
    List<Vector2> col = new List<Vector2>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> tris = new List<int>();

    bool firstPass = true;

    foreach (string line in vertex.Split('\n'))
    {
      foreach (string vec in line.Split(','))
      {
        if (firstPass)
        {
          string[] xy = vec.Trim().Split(' ');
          Vector3 v = new Vector3(float.Parse(xy[0]), float.Parse(xy[1]), 0);
          verts.Add(v);
          col.Add(v);
          uvs.Add(new Vector2((colorIndex + 0.5f) / uvWidth, 0.5f));
          sum += v;
        }
        else
        {
          string[] abc = vec.Trim().Split(' ');
          tris.Add(int.Parse(abc[0]));
          tris.Add(int.Parse(abc[1]));
          tris.Add(int.Parse(abc[2]));
        }
      }
      firstPass = false;
    }
    sum /= verts.Count;

    if (recalculateOrigin)
    {
      for (int i = 0; i < verts.Count; ++i)
      {
        verts[i] -= sum;
        col[i] -= (Vector2)sum;
      }
    }

    Mesh mesh = new Mesh();
    mesh.vertices = verts.ToArray();
    mesh.triangles = tris.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.name = "procMesh";
    if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.name == "procMesh")
    {
      DestroyImmediate(meshFilter.sharedMesh);
    }
    meshFilter.sharedMesh = mesh;
    if (polygonCollider2D != null)
    {
      polygonCollider2D.points = col.ToArray();
    }
  }

  #if UNITY_EDITOR
    private int lastColor;
    private static List<ProceduralMeshGen> allInstances = new List<ProceduralMeshGen>();

    void Update()
    {
      if(lastColor != colorIndex)
      {
        Rebuild();
        lastColor = colorIndex;
      }
    }

    void OnDestroy()
    {
      allInstances.Remove(this);
    }

    [ContextMenu("Rebuild All Meshes")]
    void RebuildAllMeshes()
    {
      foreach(ProceduralMeshGen procGen in allInstances)
      {
        procGen.Rebuild();
      }
    }
  #endif

}
