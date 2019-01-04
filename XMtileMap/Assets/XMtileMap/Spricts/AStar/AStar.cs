using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMtileMap;

public class AStar : MonoBehaviour {

    /// <summary>
    /// 8个可走方向,false只能走4个方向
    /// </summary>
    public bool DirOfWalk8 = true;

    List<Point> openList;
    List<Point> closeList;
    public List<Vector2> path;

    // Use this for initialization  
    void Start()
    {

    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="_start">起始坐标</param>
    /// <param name="_end">终点坐标</param>
    public void Move(Vector2 _start,Vector2 _end)
    {
        Point start = XMMapData.map[_start];
        Point end = XMMapData.map[_end];
        FindPath(start, end);
        ShowPath(start, end);
    }


    private void ShowPath(Point start, Point end)
    {
        path.Clear();
        Point temp = end;
        while (true)
        {
            //Debug.Log(temp.X + "," + temp.Y);
            Vector2 pos = new Vector2(temp.X, temp.Y);
            //path.Add(pos);
            path.Insert(0, pos);
            //Color c = Color.gray;
            //if (temp == start)
            //{
            //    c = Color.green;
            //}
            //else if (temp == end)
            //{
            //    c = Color.red;
            //}

            if (temp.Parent == null)
                break;
            temp = temp.Parent;
        }
    }

    public void OnDrawGizmosSelected()
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (i + 1 < path.Count)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.green);
            }
        }
    }

    /// <summary>  
    /// 查找最优路径  
    /// </summary>  
    /// <param name="start"></param>  
    /// <param name="end"></param>  
    private void FindPath(Point start, Point end)
    {
        openList = new List<Point>();
        closeList = new List<Point>();
        openList.Add(start);    //将开始位置添加进Open列表  
        while (openList.Count > 0)//查找退出条件  
        {
            Point point = FindMinFOfPoint(openList);//查找Open列表中最小的f值  
            //print(point.F + ";" + point.X + "," + point.Y);
            openList.Remove(point); closeList.Add(point);//不再考虑当前节点  

            List<Point> surroundPoints = GetSurroundPoints(point);//得到当前节点的四周8个节点
            PointsFilter(surroundPoints, closeList);//将周围节点中已经添加进Close列表中的节点移除
            foreach (Point surroundPoint in surroundPoints)
            {
                if (openList.IndexOf(surroundPoint) > -1)//如果周围节点在open列表中  
                {
                    float nowG = CalcG(surroundPoint, surroundPoint.Parent);//计算经过的Open列表中最小f值到周围节点的G值  
                    if (nowG < surroundPoint.G)
                    {
                        print("123");
                        surroundPoint.UpdateParent(point, nowG);
                    }
                }
                else//周围节点不在Open列表中  
                {
                    surroundPoint.Parent = point;//设置周围列表的父节点  
                    CalcF(surroundPoint, end);//计算周围节点的F，G,H值
                    openList.Add(surroundPoint);//最后将周围节点添加进Open列表  
                }
            }
            //判断一下  
            if (openList.IndexOf(end) > -1)
            {
                break;
            }
        }
    }

    private void PointsFilter(List<Point> src, List<Point> closeList)
    {
        foreach (Point p in closeList)
        {
            if (src.IndexOf(p) > -1)
            {
                src.Remove(p);
            }
        }
    }

    private List<Point> GetSurroundPoints(Point point)
    {
        Point up = null, down = null, left = null, right = null;
        Point lu = null, ru = null, ld = null, rd = null;

        Vector2 pos = new Vector2(point.X, point.Y);

        up = XMMapData.map[pos + new Vector2(0, 1)];
        down = XMMapData.map[pos + new Vector2(0, -1)];
        left = XMMapData.map[pos + new Vector2(-1, 0)];
        right = XMMapData.map[pos + new Vector2(1, 0)];
        if (DirOfWalk8)
        {
            lu = XMMapData.map[pos + new Vector2(-1, 1)];
            ru = XMMapData.map[pos + new Vector2(1, 1)];
            ld = XMMapData.map[pos + new Vector2(-1, -1)];
            rd = XMMapData.map[pos + new Vector2(1, -1)];
        }


        List<Point> list = new List<Point>();
        if (down != null && down.Walkable)
        {
            list.Add(down);
        }
        if (up != null && up.Walkable)
        {
            list.Add(up);
        }
        if (left != null && left.Walkable)
        {
            list.Add(left);
        }
        if (right != null && right.Walkable)
        {
            list.Add(right);
        }
        if (DirOfWalk8)
        {
            if (lu != null && lu.Walkable && left.Walkable && up.Walkable)
            {
                list.Add(lu);
            }
            if (ld != null && ld.Walkable && left.Walkable && down.Walkable)
            {
                list.Add(ld);
            }
            if (ru != null && ru.Walkable && right.Walkable && up.Walkable)
            {
                list.Add(ru);
            }
            if (rd != null && rd.Walkable && right.Walkable && down.Walkable)
            {
                list.Add(rd);
            }
        }
        return list;
    }

    private Point FindMinFOfPoint(List<Point> openList)
    {
        float f = float.MaxValue;
        Point temp = null;
        foreach (Point p in openList)
        {
            if (p.F < f)
            {
                temp = p;
                f = p.F;
            }
        }
        //print("返回open列表中最小的f:" + temp.F);
        return temp;
    }

    private float CalcG(Point now, Point parent)
    {
        return Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(parent.X, parent.Y)) + parent.G;
    }

    private void CalcF(Point now, Point end)
    {
        //F = G + H  
        float h = Mathf.Abs(end.X - now.X) + Mathf.Abs(end.Y - now.Y);
        float g = 0;
        if (now.Parent == null)
        {
            g = 0;
        }
        else
        {
            g = Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(now.Parent.X, now.Parent.Y)) + now.Parent.G;
        }
        float f = g + h;
        now.F = f;
        now.G = g;
        now.H = h;
    }
}
