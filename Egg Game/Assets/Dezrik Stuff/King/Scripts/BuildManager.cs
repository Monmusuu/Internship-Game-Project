using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{ 
    public Tilemap tilemap;
    public Tilemap playerTilemap;
    public Tile[] tiles;
    public List<GameObject> UITiles;

    public int selectedTile = 0;
    public int removeTile = 0;

    public Transform tileGridUI;

    private void Start()
    {
       tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        int i = 0;
        foreach (Tile tile in tiles)
        {
            GameObject UITile = new GameObject("UI Tile");
            UITile.transform.parent = tileGridUI;
            UITile.transform.localScale = new Vector3(1f, 1f, 1f);

            Image UIImage = UITile.AddComponent<Image>();
            UIImage.sprite = tile.sprite;

            Color tileColor = UIImage.color;
            tileColor.a = 0.5f;

            if(i == selectedTile)
            {
                tileColor.a = 1f;
            }

            UIImage.color = tileColor;
            UITiles.Add(UITile);

            i++;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedTile = 0;
            RenderUITiles();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedTile = 1;
            RenderUITiles();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedTile = 2;
            RenderUITiles();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (gameObject.layer == 7)
            {
                if (selectedTile == 0)
                {
                    selectedTile = 3;
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    tilemap.SetTile(tilemap.WorldToCell(position), tiles[selectedTile]);
                    selectedTile = 0;
                    RenderUITiles();
                }
                if (selectedTile == 1)
                {
                    selectedTile = 3;
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    tilemap.SetTile(tilemap.WorldToCell(position), tiles[selectedTile]);
                    selectedTile = 1;
                    RenderUITiles();
                }
                if (selectedTile == 2)
                {
                    selectedTile = 3;
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    tilemap.SetTile(tilemap.WorldToCell(position), tiles[selectedTile]);
                    selectedTile = 2;
                    RenderUITiles();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tilemap.SetTile(tilemap.WorldToCell(position), tiles[selectedTile]);
            gameObject.layer = 7;
        }

    }
    void RenderUITiles()
    {
        int i = 0;
        foreach (GameObject tile in UITiles)
        {
            Image UIImage = tile.GetComponent<Image>();
            Color tileColor = UIImage.color;
            tileColor.a = 0.5f;

            if (i == selectedTile)
            {
                tileColor.a = 1f;
            }

            UIImage.color = tileColor;

            i++;
        }
    }
}