using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class MozaicManager : MonoBehaviour
{

  public int width;
  public int height;
  public List<Transform> cells;

  public Transform[][] mozaic;

  [System.NonSerialized]
  public Rigidbody2D phyics;

  void Awake()
  {
    phyics = GetComponent<Rigidbody2D>();
    mozaic = new Transform[width][];
    for (int x = 0; x < width; ++x)
    {
      mozaic[x] = new Transform[height];
    }

    foreach (Transform cell in cells)
    {
      mozaic[(int)(cell.localPosition.x / 10)][(int)(cell.localPosition.y / 10)] = cell;
    }
  }

  public bool MoveCell(Transform cell, MoveDirection dir)
  {
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {
        if (mozaic[x][y] == cell)
        {
          int sourceX = x;
          int sourceY = y;
          if (dir == MoveDirection.UP)
          {
            ++y;
          }
          else if (dir == MoveDirection.LEFT)
          {
            --x;
          }
          else if (dir == MoveDirection.DOWN)
          {
            --y;
          }
          else if (dir == MoveDirection.RIGHT)
          {
            ++x;
          }
          if (x >= 0 && x < width && y >= 0 && y < height && mozaic[x][y] == null)
          {
            mozaic[x][y] = mozaic[sourceX][sourceY];
            mozaic[sourceX][sourceY] = null;
            return true;
          }
          return false;
        }
      }
    }
    Debug.LogError("The moved cell was not in the mozaic!");
    return false;
  }

  void Update()
  {
    bool newWorld = true;
    foreach (PlayerController p in RefManager.instance.allCharacters)
    {
      if (p.gameObject.activeInHierarchy)
      {
        bool inWorld = false;
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
          if (c.OverlapPoint(p.transform.position))
          {
            p.spaceTimer = 0;
            if (p.transform.parent != transform)
            {
              p.transform.SetParent(transform);
              p.transform.localRotation = Quaternion.identity;
            }
            inWorld = true;
            break;
          }
        }
        if (!inWorld)
        {
          newWorld = false;
        }
      }
    }

    if (newWorld)
    {
      RefManager.instance.inputController.currentWorld = this;
    }
  }

  public enum MoveDirection
  {
    UP,
    LEFT,
    RIGHT,
    DOWN
  }

}
