using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
public class ProceduralMeshGen : MonoBehaviour
{

  [TextAreaAttribute]
  public string vertex;

  private MeshFilter meshFilter;
  private PolygonCollider2D polygonCollider2D;

  void Awake()
  {
    meshFilter = GetComponent<MeshFilter>();
    polygonCollider2D = GetComponent<PolygonCollider2D>();

    List<Vector2> col = new List<Vector2>();

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();

    bool collider = true;
    foreach (string line in vertex.Split('\n'))
    {
      foreach (string vec in line.Split(','))
      {
        string[] xy = vec.Trim().Split(' ');
        Vector3 v = new Vector3(float.Parse(xy[0]), float.Parse(xy[1]), 0);
        if (collider)
        {
          col.Add(v);
        }
        else
        {
          if (verts.Contains(v))
          {
            Debug.Log("optimized out " + v);
            tris.Add(verts.IndexOf(v));
          }
          else
          {
            verts.Add(v);
            tris.Add(verts.Count - 1);
          }
        }
      }
      collider = false;
    }

    Mesh mesh = new Mesh();
    mesh.vertices = verts.ToArray();
    mesh.triangles = tris.ToArray();
    meshFilter.sharedMesh = mesh;
    polygonCollider2D.points = col.ToArray();
  }

}
