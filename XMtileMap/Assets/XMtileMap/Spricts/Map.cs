// **********************************************************************
// Copyright (C) XM
// Author: 吴肖牧
// Date: 2018-02-15
// Desc: 
// **********************************************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace XMtileMap
{
    public class Map : MonoBehaviour
    {

        /// <summary>
        /// 设置Tile
        /// </summary>
        /// <param name="map"></param>
        /// <param name="pos"></param>
        /// <param name="tilebase"></param>
        public static void SetTile(Tilemap map, Vector3Int pos, TileBase tilebase)
        {
            map.SetTile(pos, tilebase);
        }

        /// <summary>
        /// 设置TileMap
        /// </summary>
        /// <param name="map"></param>
        /// <param name="tileMapDataList"></param>
        public static void SetTileMap(Tilemap map, TileMapData tileMapData)
        {
            if (tileMapData.tileInfoList != null)
            {
                foreach (var tile in tileMapData.tileInfoList)
                {
                    map.SetTile(tile.ipos, tile.tile);
                }
            }
        }

        /// <summary>
        /// 创建TileMap
        /// </summary>
        /// <param name="tilemapData">地图ID</param>
        public static void CreateTileMap(int mapid)
        {
            XMMapData.MapID = mapid;
            if (XMMapData.MapData.Data.Count < mapid + 1)
            {
                Debug.LogError("MapID " + mapid.ToString() + " is null");
                return;
            }
            List<TileMapData> tilemapData = XMMapData.MapData.Data[XMMapData.MapID].tileMapDataList;
            if (tilemapData == null)
            {
                Debug.LogError("MapID " + mapid.ToString() + " tileMapDataList is null");
                return;
            }

            GameObject grid = GameObject.Find("Grid");
            if (!grid)
            {
                grid = new GameObject("Grid");
                grid.AddComponent<Grid>();
            }
            //tile的中心点为四个顶点的其中一个点，默认左下角，我们偏移一下保证和其他游戏对象的中心点一致
            grid.transform.position = new Vector3(-0.5f, -0.5f, 0);

            for (int i = 0; i < grid.transform.childCount; i++)
            {
                Destroy(grid.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < tilemapData.Count; i++)
            {
                GameObject tilemap = new GameObject(tilemapData[i].TilemapName);
                tilemap.transform.SetParent(grid.transform);
                tilemap.transform.localPosition = Vector3.zero;
                Tilemap map = tilemap.AddComponent<Tilemap>();
                TilemapRenderer render = tilemap.AddComponent<TilemapRenderer>();
                render.sortOrder = (TilemapRenderer.SortOrder)tilemapData[i].SortOrderIndex;
                render.sortingOrder = tilemapData[i].OrderInLayer;
                render.sortingLayerName = SortingLayer.layers[tilemapData[i].SortingLayerIndex].name;
                SetTileMap(map, tilemapData[i]);
            }
            //初始化地图,绑定寻路数据
            InitMap();
        }
        
        /// <summary>
        /// 初始化地图,绑定寻路数据
        /// </summary>
        public static void InitMap()
        {
            //Debug.Log(XMMapData.mapSize);
            XMMapData.map = new Dictionary<Vector2, Point>();
            List<TileMapData> tilemapData = XMMapData.MapData.Data[XMMapData.MapID].tileMapDataList;
            foreach (var item in tilemapData)
            {
                for (int i = 0; i < item.tileInfoList.Count; i++)
                {
                    int x = item.tileInfoList[i].ipos.x;
                    int y = item.tileInfoList[i].ipos.y;
                    //Debug.Log(x + " " + y);
                    XMMapData.map.Add(new Vector2(x, y), new Point(x, y));
                    bool walkable = ((XMTile)item.tileInfoList[i].tile).walkable;
                    if (walkable)
                    {
                        XMMapData.map[new Vector2(x, y)].Walkable = walkable;
                    }
                }
            }
        }
    }
}
