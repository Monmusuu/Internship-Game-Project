using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class BuildManager : MonoBehaviour
{ 
    public Tilemap KingTilemap;
    public Tile[] tiles;
    public List<GameObject> UITiles;

    public int selectedTile = 0;
    public int removeTile = 0;
    public bool trapPlaced = false;
    public bool autoPlaced = false;
    public bool blockPlaced = false;

    public Transform tileGridUI;

    private void Start()
    {
       KingTilemap = GameObject.Find("KingTilemap").GetComponent<Tilemap>();

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
            if (autoPlaced == false)
            {
                selectedTile = 0;
                RenderUITiles();
                  
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (trapPlaced == false)
            {
                selectedTile = 1;
                RenderUITiles();
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (blockPlaced == false)
            {
                selectedTile = 2;
                RenderUITiles();
                
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (gameObject.layer == 7)
            {
                if (selectedTile == 0)
                {
                        selectedTile = 3;
                        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        KingTilemap.SetTile(KingTilemap.WorldToCell(position), tiles[selectedTile]);
                        selectedTile = 0;
                        RenderUITiles();
                }
                else if (selectedTile == 1)
                {
                        selectedTile = 3;
                        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        KingTilemap.SetTile(KingTilemap.WorldToCell(position), tiles[selectedTile]);
                        selectedTile = 1;
                        RenderUITiles();
                }
                else if (selectedTile == 2)
                {
                        selectedTile = 3;
                        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        KingTilemap.SetTile(KingTilemap.WorldToCell(position), tiles[selectedTile]);
                        selectedTile = 2;
                        RenderUITiles();
                }
                Debug.Log("7 clicked");
            }
            else
            {
                Debug.Log("not 7 clicked");
            }
        }
       

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            KingTilemap.SetTile(KingTilemap.WorldToCell(position), tiles[selectedTile]);
            gameObject.layer = 7;
        }

        if (blockPlaced == true && autoPlaced == true && trapPlaced == true)
        {
            //selectedTile = 3;
            Debug.Log("none selected");
        }

        Debug.Log(selectedTile + " is selected");

        void OnMouseEnter()
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            KingTilemap.SetTile(KingTilemap.WorldToCell(position), tiles[selectedTile]);
            Debug.Log("Mouse is over "+ gameObject.layer+ ".");
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