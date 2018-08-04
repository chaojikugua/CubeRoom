﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sorumi.Util;

public class GridGroup : MonoBehaviour
{
    private const int MAX_SIZE = 5;
    private const float GRID_OFFSET = 0.002f;
    // material
    public Material correctMaterial;
    public Material errorMaterial;

    // gameobject

    private Vector2Int bottomSize;
    private Vector2Int sideSize;
    private GameObject[,] bottomGrids;
    private GameObject[,] sideGrids;

    private Transform bottomGridsGroup;
    private Transform sideGridsGroup;

    // pool
    private GameObject poolGO;
    private ObjectPool<GameObject> gridPool;

    public void Init()
    {
        bottomGrids = new GameObject[MAX_SIZE, MAX_SIZE];
        sideGrids = new GameObject[MAX_SIZE, MAX_SIZE];

        bottomGridsGroup = transform.Find("BottomGrids");
        sideGridsGroup = transform.Find("SideGrids");

        gridPool = new ObjectPool<GameObject>(CreateGrid, ResetGrid);

        poolGO = new GameObject("PoolGameObject");
        poolGO.transform.position = Vector3.zero;
        poolGO.SetActive(false);
    }

    private GameObject CreateGrid()
    {
        GameObject grid = Instantiate(Resources.Load("Prefabs/ItemGrid")) as GameObject;
        grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 1);
        grid.transform.SetParent(poolGO.transform);
        return grid;
    }

    private void ResetGrid(GameObject grid)
    {
        grid.transform.SetParent(poolGO.transform);
    }

    public void SetGrids(ItemObject item)
    {
        sideGridsGroup.gameObject.SetActive(item.Type == ItemType.Vertical);

        Vector3Int size = item.Item.Size;
        bottomSize = new Vector2Int(size.x, size.z);
        sideSize = new Vector2Int(size.x, size.y);

        float offsetX = 0.5f - size.x / 2.0f;
        float offsetZ = item.Type == ItemType.Vertical ? 0.5f : 0.5f - size.z / 2.0f;
        float offsetY = 0.5f - size.y / 2.0f;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (bottomGrids[i, j] == null)
                {
                    GameObject grid = gridPool.GetObject();
                    grid.transform.SetParent(bottomGridsGroup);
                    float x = i + offsetX;
                    float z = j + offsetZ;
                    grid.transform.position = new Vector3(x, GRID_OFFSET, z);
                    bottomGrids[i, j] = grid;
                }
            }
        }

        if (item.Type == ItemType.Vertical)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (sideGrids[i, j] == null)
                    {
                        GameObject grid = gridPool.GetObject();
                        grid.transform.SetParent(sideGridsGroup);
                        float x = i + offsetX;
                        float y = j + offsetY;
                        grid.transform.position = new Vector3(x, y, GRID_OFFSET);
                        grid.transform.eulerAngles = new Vector3(90, 0, 0);
                        sideGrids[i, j] = grid;
                    }
                }
            }
        }

        Vector3 position = item.gameObject.transform.position;

    }

    public void SetTransform(ItemObject item)
    {
        Vector3 position = item.transform.position;
        Vector3 eulerAngles = item.transform.eulerAngles;
        bottomGridsGroup.position = new Vector3(position.x, GRID_OFFSET, position.z);
        bottomGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0); ;

        sideGridsGroup.position = position;
        sideGridsGroup.eulerAngles = new Vector3(0, eulerAngles.y, 0);
    }

    public void SetBottomGridsType(bool[,] gridTypes)
    {

        int sizeX = gridTypes.GetLength(0);
        int sizeY = gridTypes.GetLength(1);

        if (sizeX > bottomSize.x || sizeY > bottomSize.y)
        {
            Debug.LogWarning("SetBottomGridsType: GridTypes size error");
        }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bottomGrids[i, j].GetComponent<Renderer>().sharedMaterial = gridTypes[i, j] ? correctMaterial : errorMaterial;
            }
        }
    }
}
