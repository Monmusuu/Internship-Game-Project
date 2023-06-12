using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Debug = UnityEngine.Debug;

public class BuildManager : MonoBehaviour
{ 
public Tilemap KingTilemap;
public GameObject[] autoTileObjects;
public GameObject[] trapTileObjects;
public GameObject[] blockTileObjects;
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
        foreach (GameObject tileObject in autoTileObjects)
        {
            GameObject UITile = new GameObject("UI Tile");
            UITile.transform.parent = tileGridUI;
            UITile.transform.localScale = new Vector3(1f, 1f, 1f);

            UnityEngine.UI.Image UIImage = UITile.AddComponent<UnityEngine.UI.Image>();
            SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer ? spriteRenderer.sprite : null;
            UIImage.sprite = sprite;

            Color tileColor = UIImage.color;
            tileColor.a = 0.5f;

            if (i == selectedTile)
            {
                tileColor.a = 1f;
            }

            UIImage.color = tileColor;
            UITiles.Add(UITile);

            i++;
        }

        foreach (GameObject tileObject in trapTileObjects)
        {
            GameObject UITile = new GameObject("UI Tile");
            UITile.transform.parent = tileGridUI;
            UITile.transform.localScale = new Vector3(1f, 1f, 1f);

            UnityEngine.UI.Image UIImage = UITile.AddComponent<UnityEngine.UI.Image>();
            SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer ? spriteRenderer.sprite : null;
            UIImage.sprite = sprite;

            Color tileColor = UIImage.color;
            tileColor.a = 0.5f;

            if (i == selectedTile)
            {
                tileColor.a = 1f;
            }

            UIImage.color = tileColor;
            UITiles.Add(UITile);

            i++;
        }

        foreach (GameObject tileObject in blockTileObjects)
        {
            GameObject UITile = new GameObject("UI Tile");
            UITile.transform.parent = tileGridUI;
            UITile.transform.localScale = new Vector3(1f, 1f, 1f);

            UnityEngine.UI.Image UIImage = UITile.AddComponent<UnityEngine.UI.Image>();
            SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer ? spriteRenderer.sprite : null;
            UIImage.sprite = sprite;

            Color tileColor = UIImage.color;
            tileColor.a = 0.5f;

            if (i == selectedTile)
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

            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = KingTilemap.WorldToCell(mousePosition);
            Vector3 tilePosition = KingTilemap.CellToWorld(cellPosition) + KingTilemap.cellSize / 2f;

            
            if (selectedTile == 0)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Instantiate(tileObjects[selectedTile], KingTilemap.WorldToCell(position), Quaternion.identity);
                gameObject.layer = 7;
                autoPlaced = true;
                RenderUITiles();
                GameObject tileObject = Instantiate(autoTileObjects[0], tilePosition, Quaternion.identity);

                if (trapPlaced == true && blockPlaced == true)
                {
                    selectedTile = 3;
                    RenderUITiles();
                }
                else if (trapPlaced == true)
                {
                    selectedTile = 2;
                    RenderUITiles();
                }
                else if (blockPlaced == true)
                {
                    selectedTile = 1;
                    RenderUITiles();
                }
                else
                {
                    selectedTile += 1;
                    RenderUITiles();
                }

            }
            else if (selectedTile == 1)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                gameObject.layer = 7;
                trapPlaced = true;
                RenderUITiles();
                GameObject tileObject = Instantiate(trapTileObjects[0], tilePosition, Quaternion.identity);

                if (autoPlaced == true && blockPlaced == true)
                {
                    selectedTile = 3;
                    RenderUITiles();
                }
                else if (autoPlaced == true)
                {
                    selectedTile = 2;
                    RenderUITiles();
                }
                else if (blockPlaced == true)
                {
                    selectedTile = 0;
                    RenderUITiles();
                }else
                {
                    selectedTile += 1;
                    RenderUITiles();
                }
            }
            else if (selectedTile == 2)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                gameObject.layer = 7;
                blockPlaced = true;
                RenderUITiles();
                GameObject tileObject = Instantiate(blockTileObjects[0], tilePosition, Quaternion.identity);

                if (trapPlaced == true && autoPlaced == true)
                {
                    selectedTile = 3;
                    RenderUITiles();
                }
                else if (trapPlaced == true)
                {
                    selectedTile = 0;
                    RenderUITiles();
                }
                else if (autoPlaced == true)
                {
                    selectedTile = 1;
                    RenderUITiles();
                }
                else
                {
                    selectedTile = 0;
                    RenderUITiles();
                }
            }
        }

        if (blockPlaced == true && autoPlaced == true && trapPlaced == true)
        {
            selectedTile = 3;
        }

        void RenderUITiles()
        {
            int i = 0;
            foreach (GameObject tileObject in UITiles)
            {
                UnityEngine.UI.Image UIImage = tileObject.GetComponent<UnityEngine.UI.Image>();
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
}

