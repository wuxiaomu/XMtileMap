// **********************************************************************
// Copyright (C) XM
// Author: 吴肖牧
// Date: 2018-02-15
// Desc: 
// **********************************************************************

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace XMtileMap
{
    public class MapEditor : EditorWindow
    {
        public List<TileMapData> tilemapData;

        public int toolbarOption = 0;
        public string[] toolbarTexts = { "编 辑", "设 置", "关 于" };

        public int IntPopupIndex = 0;
        public string[] strPopupSize;
        public int[] intPopupSize;

        public int tileMaptoolbarOption = 0;
        public string[] tileMaptoolbarTexts;

        Vector2 scrollPos;
        
        Vector3 delVecPos;

        #region ShowTileMapEditor
        [MenuItem("Tools/XMTileMap %t")]
        public static void ShowTileMapEditor()
        {
            if (EditorSceneManager.GetActiveScene().name == "XMtileMap")
            {
                MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
                window.titleContent = new GUIContent("XMTileMap");
                window.Show();
            }
            else
            {
                if (EditorUtility.DisplayDialog("提示", "打开编辑器需要跳转到MapEditor场景，点击确定将会自动保存，点击取消则继续留在当前场景", "确定", "取消"))
                {
                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.OpenScene("Assets/XMtileMap/Scenes/XMtileMap.unity");

                    MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
                    window.titleContent = new GUIContent("XMTileMap");
                    window.Show();
                }
            }
        }
        #endregion

        #region OnGUI
        void OnGUI()
        {
            EditorGUILayout.Space();
            MainGUI();
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Height(30));
            switch (toolbarOption)
            {
                case 0:
                    Title("Editor");
                    TileMapContent();
                    break;
                case 1:
                    Title("Setting");
                    SettingContent();
                    break;
                case 2:
                    Title("About");
                    AboutContent();
                    break;
            }
        }

        private void Title(string operationName)
        {
            GUILayout.Label(operationName, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.white;
        }
        #endregion

        #region MainGUI
        private void MainGUI()
        {
            if (XMMapData.TargetData == null)
            {
                GUILayout.Label("副本为空，请设置副本数据");
                return;
            }
            if (XMMapData.SourceData == null)
            {
                GUILayout.Label("源数据为空，请设置源数据");
                return;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            strPopupSize = new string[XMMapData.TargetData.Data.Count];
            intPopupSize = new int[XMMapData.TargetData.Data.Count];
            for (int i = 0; i < intPopupSize.Length; i++)
            {
                intPopupSize[i] = i;
                strPopupSize[i] = (i).ToString();
            }

            EditorGUIUtility.labelWidth = 60;
            IntPopupIndex = EditorGUILayout.IntPopup("Map ID：", IntPopupIndex, strPopupSize, intPopupSize, GUILayout.ExpandWidth(true), GUILayout.Width(150));
            XMMapData.MapID = IntPopupIndex;

            if (GUILayout.Button("-", GUILayout.Width(35)))
            {
                if (EditorUtility.DisplayDialog("提示", "确定要删除地图 【 " + IntPopupIndex + " 】 吗？", "确定", "取消"))
                {
                    XMMapData.TargetData.Data.RemoveAt(IntPopupIndex);
                    IntPopupIndex -= 1;
                    if (IntPopupIndex <= 0)
                    {
                        IntPopupIndex = 0;
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(35)))
            {
                TileMapDataList list = new TileMapDataList();
                XMMapData.TargetData.Data.Add(list);

                IntPopupIndex = XMMapData.TargetData.Data.Count - 1;
                tileMaptoolbarOption = 0;
                Transform grid = GameObject.Find("Grid").transform;
                tileMaptoolbarTexts = new string[grid.childCount];
                for (int i = 0; i < grid.childCount; i++)
                {
                    tileMaptoolbarTexts[i] = grid.GetChild(i).name;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(XMConst.CopyRight);
            GUILayout.EndVertical();

            if (GUILayout.Button("打开编辑场景", GUILayout.Height(35)))
            {
                EditorSceneManager.OpenScene("Assets/XMtileMap/Scenes/XMtileMap.unity");
            }
            if (GUILayout.Button("预 览", GUILayout.Height(35)))
            {
                if (CheckNameConflict())
                {
                    EditorUtility.DisplayDialog("提示", " Tilemap 名字冲突", "确定");
                    return;
                }
                if (EditorSceneManager.GetActiveScene().name != "XMtileMap")
                {
                    EditorUtility.DisplayDialog("提示", "请打开 XMtileMap 场景进行编辑", "确定");
                    return;
                }
                if (EditorApplication.isPlaying)
                {
                    EditorUtility.DisplayDialog("提示", "场景正在播放中，不能进行预览", "确定");
                    return;
                }
                CreateTileMap();
            }
            if (GUILayout.Button("重置副本", GUILayout.Height(35)))
            {
                if (EditorUtility.DisplayDialog("提示", "确定要重置副本吗", "确定", "取消"))
                {
                    IntPopupIndex = 0;
                    XMMapData.SaveData(XMMapData.TargetData.Data, XMMapData.SourceData.Data);
                }
            }
            if (GUILayout.Button("保存副本", GUILayout.Height(35)))
            {
                if (CheckNameConflict())
                {
                    EditorUtility.DisplayDialog("提示", " Tilemap 名字冲突", "确定");
                    return;
                }
                if (EditorUtility.DisplayDialog("提示", "确定要保存副本吗", "确定", "取消"))
                {
                    EditorUtility.SetDirty(XMMapData.TargetData);
                }
            }
            //GUI.backgroundColor = Color.green;
            if (GUILayout.Button("保存数据", GUILayout.Height(35)))
            {
                if (CheckNameConflict())
                {
                    EditorUtility.DisplayDialog("提示", " Tilemap 名字冲突", "确定");
                    return;
                }
                if (EditorUtility.DisplayDialog("提示", "确定要保存数据吗", "确定", "取消"))
                {
                    XMMapData.SaveData(XMMapData.SourceData.Data, XMMapData.TargetData.Data);
                }
            }

            //if (GUILayout.Button("保存JSON", GUILayout.Height(35)))
            //{
            //    if (CheckNameConflict())
            //    {
            //        EditorUtility.DisplayDialog("提示", " Tilemap 名字冲突", "确定");
            //        return;
            //    }
            //    if (EditorUtility.DisplayDialog("提示", "确定要保存数据吗", "确定", "取消"))
            //    {
            //        XMMapData.SaveData(XMMapData.SourceData.Data, XMMapData.TargetData.Data);
            //        XMMapData.SaveToJSON("Assets/XMtileMap/Data/SourceData.json", XMMapData.SourceData);
            //        XMMapData.SaveToJSON("Assets/XMtileMap/Data/TargetData.json", XMMapData.TargetData);
            //    }
            //}

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region TileMapContent

        private void TileMapContent()
        {
            if (XMMapData.TargetData == null || XMMapData.SourceData == null || XMMapData.TargetData.Data == null || XMMapData.TargetData.Data.Count == 0)
            {
                return;
            }

            tilemapData = XMMapData.TargetData.Data[IntPopupIndex].tileMapDataList;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                if (tilemapData.Count < 5)
                {
                    TileMapData data = new TileMapData();
                    data.TilemapName = data.TilemapName + (tilemapData.Count + 1).ToString();
                    data.OrderInLayer = 0;
                    data.SortingLayerIndex = 0;
                    data.SortOrderIndex = 0;
                    tilemapData.Add(data);
                    tileMaptoolbarOption = tilemapData.Count - 1;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "最多只能创建 5 个Tilemap", "确定", "取消");
                }
            }

            if (XMMapData.TargetData.Data.Count == 0)
            {
                return;
            }
            if (tilemapData == null)
            {
                GUILayout.Label("TileMaps Count: 0");
                return;
            }
            else
            {
                GUILayout.Label("TileMaps Count: " + tilemapData.Count);
            }
            GUILayout.EndHorizontal();


            if (tilemapData == null || tilemapData.Count == 0 ||
                tilemapData.Count < tileMaptoolbarOption + 1) //这里的作用是，切换地图的时候可能会因为Tilemap数量不一样导致报错
            {
                tileMaptoolbarOption = 0;
                return;
            }


            int count = tilemapData.Count;
            tileMaptoolbarTexts = new string[count];
            for (int i = 0; i < count; i++)
            {
                tileMaptoolbarTexts[i] = tilemapData[i].TilemapName;
            }
            tileMaptoolbarOption = GUILayout.Toolbar(tileMaptoolbarOption, tileMaptoolbarTexts, GUILayout.Height(20));
            switch (tileMaptoolbarOption)
            {
                case 0:
                    ShowTileMapInfo();
                    break;
                case 1:
                    ShowTileMapInfo();
                    break;
                case 2:
                    ShowTileMapInfo();
                    break;
                case 3:
                    ShowTileMapInfo();
                    break;
                case 4:
                    ShowTileMapInfo();
                    break;
            }
        }

        private void ShowTileMapInfo()
        {
            EditorGUIUtility.labelWidth = 120;
            TileMapData item = tilemapData[tileMaptoolbarOption];
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("提示", "确定要删除 " + item.TilemapName + " 吗？", "确定", "取消"))
                {
                    tilemapData.Remove(item);
                    tileMaptoolbarOption = 0;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            string[] StrlayersPopupSize = new string[SortingLayer.layers.Length];
            int[] intlayersPopupSize = new int[SortingLayer.layers.Length];
            for (int i = 0; i < intlayersPopupSize.Length; i++)
            {
                intlayersPopupSize[i] = i;
                StrlayersPopupSize[i] = SortingLayer.layers[i].name;
            }
            string[] SortOrderPopupSize = { TilemapRenderer.SortOrder.BottomLeft.ToString(), TilemapRenderer.SortOrder.BottomRight.ToString(), TilemapRenderer.SortOrder.TopLeft.ToString() , TilemapRenderer.SortOrder.TopRight.ToString() };
            int[] intSortOrderPopupSize = { (int)TilemapRenderer.SortOrder.BottomLeft, (int)TilemapRenderer.SortOrder.BottomRight, (int)TilemapRenderer.SortOrder.TopLeft, (int)TilemapRenderer.SortOrder.TopRight };

            item.TilemapName = EditorGUILayout.TextField("TileMap Name : ", item.TilemapName, GUILayout.Width(250));
            item.SortOrderIndex = EditorGUILayout.IntPopup("Sort Order：", item.SortOrderIndex, SortOrderPopupSize, intSortOrderPopupSize, GUILayout.ExpandWidth(true), GUILayout.Width(250));
            item.SortingLayerIndex = EditorGUILayout.IntPopup("Sorting Layer：", item.SortingLayerIndex, StrlayersPopupSize, intlayersPopupSize, GUILayout.ExpandWidth(true), GUILayout.Width(250));
            item.OrderInLayer = EditorGUILayout.IntField("Order in Layer : ", item.OrderInLayer, GUILayout.Width(250));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (GUILayout.Button("预  览", GUILayout.Width(100)))
            {
                CreateCurTileMap();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (item.tileInfoList == null)
            {
                GUILayout.Label("Tile Count : 0");
            }
            else
            {
                GUILayout.Label("Tile Count : " + item.tileInfoList.Count);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (item.tileInfoList != null)
            {
                for (int i = 0; i < item.tileInfoList.Count; i++)
                {
                    XMTile tile = (XMTile)item.tileInfoList[i].tile;
                    GUILayout.Label("【" + (i + 1).ToString("0000") + "】   walkable:" + tile.walkable + "      destroable:" + tile.destroable + "      Pos:" + item.tileInfoList[i].pos + "      Tile:" + tile);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private bool CheckNameConflict()
        {
            bool conflict = false;
            for (int i = 0; i < tilemapData.Count; i++)
            {
                for (int j = 0; j < tilemapData.Count; j++)
                {
                    if (i != j)
                    {
                        if (tilemapData[i].TilemapName == tilemapData[j].TilemapName)
                        {
                            conflict = true;
                            break;
                        }
                    }
                }
            }
            return conflict;
        }

        private void CreateCurTileMap()
        {
            GameObject grid = GameObject.Find("Grid");
            if (grid)
            {
                DestroyImmediate(grid);
            }
            grid = new GameObject("Grid");
            grid.transform.position = new Vector3(-0.5f, -0.5f, 0);
            grid.AddComponent<Grid>();
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                DestroyImmediate(grid.transform.GetChild(i).gameObject);
            }
            GameObject tilemap = new GameObject(tilemapData[tileMaptoolbarOption].TilemapName);
            tilemap.transform.SetParent(grid.transform);
            tilemap.transform.localPosition = Vector3.zero;
            Tilemap map = tilemap.AddComponent<Tilemap>();
            TilemapRenderer render = tilemap.AddComponent<TilemapRenderer>();
            render.sortOrder = (TilemapRenderer.SortOrder)tilemapData[tileMaptoolbarOption].SortOrderIndex;
            render.sortingOrder = tilemapData[tileMaptoolbarOption].OrderInLayer;
            render.sortingLayerName = SortingLayer.layers[tilemapData[tileMaptoolbarOption].SortingLayerIndex].name;
            Map.SetTileMap(map, tilemapData[tileMaptoolbarOption]);
        }

        private void CreateTileMap()
        {
            GameObject gridObj = GameObject.Find("Grid");
            if (gridObj)
            {
                DestroyImmediate(gridObj);
            }
            gridObj = new GameObject("Grid");
            gridObj.transform.position = new Vector3(-0.5f, -0.5f, 0);
            gridObj.AddComponent<Grid>();
            for (int i = 0; i < gridObj.transform.childCount; i++)
            {
                DestroyImmediate(gridObj.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < tilemapData.Count; i++)
            {
                GameObject tilemap = new GameObject(tilemapData[i].TilemapName);
                tilemap.transform.SetParent(gridObj.transform);
                tilemap.transform.localPosition = Vector3.zero;
                Tilemap map = tilemap.AddComponent<Tilemap>();
                TilemapRenderer render = tilemap.AddComponent<TilemapRenderer>();
                render.sortOrder = (TilemapRenderer.SortOrder)tilemapData[i].SortOrderIndex;
                render.sortingOrder = tilemapData[i].OrderInLayer;
                render.sortingLayerName = SortingLayer.layers[tilemapData[i].SortingLayerIndex].name;
                Map.SetTileMap(map, tilemapData[i]);
            }
        }
        #endregion

        #region SettingContent

        private void SettingContent()
        {
            GUILayout.Label("地图数据: " + XMMapData.MapDataPath);
            GUILayout.Label("数据源: " + XMMapData.SourceDataPath);
            GUILayout.Label("副本： " + XMMapData.TargetDataPath);

            EditorGUILayout.Space();

            if (GUILayout.Button("保存"))
            {

                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        #endregion

        #region AboutContent
        private void AboutContent()
        {
            GUILayout.Label(XMConst.version);
            GUILayout.Label(XMConst.Requires);
            EditorGUILayout.Space();
        }
        #endregion
    }

}