// **********************************************************************
// Copyright (C) XM
// Author: 吴肖牧
// Date: 2018-02-15
// Desc: 
// **********************************************************************

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace XMtileMap
{
    public class XMMapData
    {
        public static string MapDataPath = "Assets/RefResources/ScriptableObjects/MapData.asset";
        public static string SourceDataPath = "Assets/XMtileMap/Data/SourceData.asset";
        public static string TargetDataPath = "Assets/XMtileMap/Data/TargetData.asset";

        private static TileMapSerialize mapData;
        /// <summary>
        /// 地图数据，游戏使用的数据
        /// </summary>
        public static TileMapSerialize MapData
        {
            get
            {
                if (mapData == null)
                {
                    // TODO runtime LoadData

#if UNITY_EDITOR
                    mapData = UnityEditor.AssetDatabase.LoadAssetAtPath<TileMapSerialize>(MapDataPath);
#endif
                }
                return mapData;
            }
        }

        private static TileMapSerialize sourceData;
        /// <summary>
        /// 地图源数据，保存用
        /// </summary>
        public static TileMapSerialize SourceData
        {
            get
            {
                if (sourceData == null)
                {
                    // TODO runtime LoadData

#if UNITY_EDITOR
                    sourceData = UnityEditor.AssetDatabase.LoadAssetAtPath<TileMapSerialize>(SourceDataPath);
#endif
                }
                return sourceData;
            }
        }

        private static TileMapSerialize targetData;
        /// <summary>
        /// 副本数据，编辑用
        /// </summary>
        public static TileMapSerialize TargetData
        {
            get
            {
                if (targetData == null)
                {
                    // TODO runtime LoadData

#if UNITY_EDITOR
                    targetData = UnityEditor.AssetDatabase.LoadAssetAtPath<TileMapSerialize>(TargetDataPath);
#endif
                }
                return targetData;
            }
        }

        /// <summary>
        /// 地图ID
        /// </summary>
        public static int MapID = 0;

        /// <summary>
        /// A星寻路的地图数据
        /// </summary>
        public static Dictionary<Vector2,Point> map;

        public static Vector2 tileOffset2 = new Vector2(0.5f, 0.5f);
        public static Vector3 tileOffset3 = new Vector3(0.5f, 0.5f, -0.5f);

#if UNITY_EDITOR
        /// <summary>
        /// 载入地图数据
        /// </summary>
        /// <param name="path"></param>
        private static void LoadData(string path)
        {
            TileMapSerialize targetData = UnityEditor.AssetDatabase.LoadAssetAtPath<TileMapSerialize>(path);
            if (targetData == null)
            {
                string newPath = UnityEditor.EditorUtility.SaveFilePanelInProject("Save TileMapSerialize", "New TileMapSerialize", "asset", "Save TileMapSerialize", "Assets");

                if (newPath == "")
                    return;

                UnityEditor.AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TileMapSerialize>(), newPath);
                targetData = UnityEditor.AssetDatabase.LoadAssetAtPath<TileMapSerialize>(path);
            }
        }

        public static void SaveData(List<TileMapDataList> target, List<TileMapDataList> source)
        {
            target.Clear();
            foreach (var item in source)
            {
                TileMapDataList list = new TileMapDataList();
                list.tileMapDataList = new List<TileMapData>();
                foreach (var item1 in item.tileMapDataList)
                {
                    TileMapData mapdata = new TileMapData();
                    mapdata.OrderInLayer = item1.OrderInLayer;
                    mapdata.SortingLayerIndex = item1.SortingLayerIndex;
                    mapdata.SortOrderIndex = item1.SortOrderIndex;
                    mapdata.TilemapName = item1.TilemapName;
                    mapdata.tileInfoList = new List<TileInfo>();
                    if (item1.tileInfoList == null || item1.tileInfoList.Count == 0)
                    {
                        Debug.LogError(item1.TilemapName + " tileInfoList is null or count is zero");
                    }
                    else
                    {
                        foreach (var item2 in item1.tileInfoList)
                        {
                            TileInfo tile = new TileInfo();
                            tile.ipos = item2.ipos;
                            tile.pos = item2.pos;
                            tile.tile = item2.tile;
                            mapdata.tileInfoList.Add(tile);
                        }
                    }
                    list.tileMapDataList.Add(mapdata);
                }
                target.Add(list);
            }
            if (target == SourceData.Data)
            {
                UnityEditor.EditorUtility.SetDirty(TargetData);
                UnityEditor.EditorUtility.SetDirty(SourceData);
            }
            else if (target == TargetData.Data)
            {
                UnityEditor.EditorUtility.SetDirty(TargetData);
            }
            File.Copy(Application.dataPath + "/XMtileMap/Data/SourceData.asset", Application.dataPath + "/RefResources/ScriptableObjects/MapData.asset",true);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        

        /// <summary>
        /// 保存json
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void SaveToJSON(string path, TileMapSerialize data)
        {
            Debug.LogFormat("Saving config to {0}", path);
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }

        /// <summary>
        /// 添加地图数据
        /// </summary>
        /// <param name="pos">世界坐标</param>
        /// <param name="data">单位数据</param>
        public static void AddData(GameObject brushTarget, Vector3 pos, TileInfo data)
        {
            Tilemap tile = brushTarget.GetComponent<Tilemap>();
            foreach (var item in TargetData.Data[MapID].tileMapDataList)
            {
                if (item.TilemapName == tile.name)
                {
                    if (item.tileInfoList == null)
                    {
                        item.tileInfoList = new List<TileInfo>();
                    }
                    bool isadd = true;
                    for (int i = 0; i < item.tileInfoList.Count; i++)
                    {
                        if (item.tileInfoList[i].pos == pos)
                        {
                            isadd = false;
                            item.tileInfoList[i] = data;
                            break;
                        }
                    }
                    if (isadd)
                    {
                        item.tileInfoList.Add(data);
                    }
                }
            }
        }

        /// <summary>
        /// 清除地图数据
        /// </summary>
        /// <param name="pos">世界坐标</param>
        public static void ClearData(GameObject brushTarget, Vector3 pos)
        {
            Tilemap tile = brushTarget.GetComponent<Tilemap>();
            foreach (var item in TargetData.Data[MapID].tileMapDataList)
            {
                if (item.TilemapName == tile.name)
                {
                    for (int i = 0; i < item.tileInfoList.Count; i++)
                    {
                        if (item.tileInfoList[i].pos == pos)
                        {
                            item.tileInfoList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public static void ClearDataForPos(Vector3 pos)
        {
            foreach (var item in TargetData.Data[MapID].tileMapDataList)
            {
                for (int i = 0; i < item.tileInfoList.Count; i++)
                {
                    if (item.tileInfoList[i].pos == pos)
                    {
                        item.tileInfoList.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public static void ClearAllData()
        {
            TargetData.Data[MapID].tileMapDataList.Clear();
        }
#endif

    }
}