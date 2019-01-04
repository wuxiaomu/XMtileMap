// **********************************************************************
// Copyright (C) XM
// Author: 吴肖牧
// Date: 2018-02-15
// Desc: 
// **********************************************************************

using UnityEditor;
using UnityEngine;

namespace XMtileMap
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "XM Brush")]
    public class XMBrush : GridBrush
    {
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            AddTileMapData(gridLayout, brushTarget, position);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Erase(gridLayout, brushTarget, position);
            ClearTileMapData(gridLayout, brushTarget, position);
        }

        /// <summary>
        /// 添加地图数据
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="position"></param>
        private void AddTileMapData(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            TileInfo data = new TileInfo
            {
                //tile的中心点为四个顶点的其中一个点，默认左下角，我们偏移一下保证和其他游戏对象的中心点一致,这里是还原创建Grid时的偏移，保证对象刚好在tile的中心点
                pos = gridLayout.CellToWorld(position) + XMMapData.tileOffset3,
                ipos = position
            };
            for (int i = 0; i < cells.Length; i++)
            {
                XMTile xmtile = (XMTile)cells[i].tile;
                data.tile = xmtile;
            }
            XMMapData.AddData(brushTarget, data.pos, data);
        }

        /// <summary>
        /// 清除地图数据
        /// </summary>
        /// <param name="position"></param>
        private void ClearTileMapData(GridLayout gridLayout,GameObject brushTarget, Vector3Int position)
        {
            Vector3 pos = gridLayout.CellToWorld(position) + XMMapData.tileOffset3;
            XMMapData.ClearData(brushTarget, pos);
        }
    }

    [CustomEditor(typeof(XMBrush))]
    public class XMBrushEditor : GridBrushEditor
    {
        private XMBrushEditor prefabBrush { get { return target as XMBrushEditor; } }

        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            base.PaintPreview(grid, brushTarget, position);
        }

        public override void OnPaintInspectorGUI()
        {
            EditorGUILayout.LabelField(XMConst.CopyRight);
        }
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            Handles.Label(grid.CellToWorld(new Vector3Int(position.x, position.y, position.z)), new Vector3Int(position.x, position.y, position.z).ToString());
        }
    }
}
