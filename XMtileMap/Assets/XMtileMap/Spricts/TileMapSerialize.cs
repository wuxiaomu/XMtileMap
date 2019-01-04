// **********************************************************************
// Copyright (C) XM
// Author: 吴肖牧
// Date: 2018-02-15
// Desc: 
// **********************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace XMtileMap
{
    [CreateAssetMenu]
    public class TileMapSerialize : ScriptableObject
    {
        //基本数据类型以外的成员类型需要加 SerializeField 关键字
        [SerializeField]
        public List<TileMapDataList> Data = new List<TileMapDataList>();
    }

    //[CustomEditor(typeof(TileMapSerialize))]
    //public class TestClassEditor : Editor
    //{
    //    private TileMapSerialize tileMapSerialize { get { return target as TileMapSerialize; } }

    //    public override void OnInspectorGUI()
    //    {
    //        serializedObject.Update();
    //        //TODO

    //        serializedObject.ApplyModifiedProperties();
    //        base.OnInspectorGUI();
    //    }
    //}

    //自定义数据类型被ScriptableObject对象使用的时候，该类需要加 Serializable 关键字
    [Serializable]
    public class TileMapDataList
    {
        public List<TileMapData> tileMapDataList = new List<TileMapData>();
    }

    [Serializable]
    public class TileMapData
    {
        public string TilemapName = "Tilemap";
        public int SortOrderIndex = 0;
        public int SortingLayerIndex = 0;
        public int OrderInLayer = 0;
        public List<TileInfo> tileInfoList;
    }

    [Serializable]
    public class TileInfo
    {
        public Tile tile;
        public Vector3 pos;
        public Vector3Int ipos;
        // DOTO other data

    }
}