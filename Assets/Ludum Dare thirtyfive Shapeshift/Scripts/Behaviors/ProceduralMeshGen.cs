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

    Vector3 sum = Vector3.zero;
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();

    bool collider = true;
    float minX = float.PositiveInfinity;
    float maxX = float.NegativeInfinity;
    float minY = float.PositiveInfinity;
    float maxY = float.NegativeInfinity;
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
            tris.Add(verts.IndexOf(v));
          }
          else
          {
            if (v.x < minX)
            {
              minX = v.x;
            }
            if (v.y < minY)
            {
              minY = v.y;
            }
            if (v.x > maxX)
            {
              maxX = v.x;
            }
            if (v.y > maxY)
            {
              maxY = v.y;
            }
            verts.Add(v);
            sum += v;
            tris.Add(verts.Count - 1);
          }
        }
      }
      collider = false;
    }
    sum /= verts.Count;

    minX -= sum.x;
    maxX -= sum.x;
    minY -= sum.y;
    maxY -= sum.y;

    Vector2[] uvs = new Vector2[verts.Count];
    for (int i = 0; i < verts.Count; ++i)
    {
      verts[i] -= sum;
      uvs[i] = new Vector2(
          (verts[i].x - minX) / (maxX - minX),
          (verts[i].y - minY) / (maxY - minY)
      );
    }

    for (int i = 0; i < col.Count; ++i)
    {
      col[i] -= (Vector2)sum;
    }

    Mesh mesh = new Mesh();
    mesh.vertices = verts.ToArray();
    mesh.triangles = tris.ToArray();
    mesh.uv = uvs;
    meshFilter.sharedMesh = mesh;
    polygonCollider2D.points = col.ToArray();
  }

}
