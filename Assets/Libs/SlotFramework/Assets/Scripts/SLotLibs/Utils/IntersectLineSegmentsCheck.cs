using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Intersect line segments check.
/// </summary>
public class IntersectLineSegmentsCheck  {

    public class Point{
        public float x;
        public float y;
        public Point(float x,float y){
            this.x = x;
            this.y = y;
        }
        public Point(Vector2 pos){
            this.x = pos.x;
            this.y = pos.y;
        }
    }

    //判定两个float的大小
    static int fltcmp(float a, float b)
    {
        if (Mathf.Abs(a - b) <= 1E-3) return 0;
        if (a > b) return 1;
        else return -1;
    }
    //***************点积判点是否在线段上***************
    static float dot(float x1, float y1, float x2, float y2) //点积
    {
        return x1 * x2 + y1 * y2;
    }

    static int point_on_line(Point a, Point b, Point c) //求a点是不是在线段bc上，>0不在，=0与端点重合，<0在。
    {
        return fltcmp(dot(b.x - a.x, b.y - a.y, c.x - a.x, c.y - a.y), 0);
    }

    //**************************************************
    static float cross(float x1, float y1, float x2, float y2)
    {
        return x1 * y2 - x2 * y1;
    }
    static float ab_cross_ac(Point a, Point b, Point c) //ab与ac的叉积
    {
        return cross(b.x - a.x, b.y - a.y, c.x - a.x, c.y - a.y);
    }
    /// <summary>
    /// Abs the cross cd.
    /// 求ab是否与cd相交，交点为p。1规范相交，0交点是一线段的端点，-1不相交。
    /// </summary>
    /// <returns>The cross cd.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="c">C.</param>
    /// <param name="d">D.</param>
    /// <param name="p">P.</param>
    public static int ab_cross_cd(Point a, Point b, Point c, Point d,ref Point p) //求ab是否与cd相交，交点为p。1规范相交，0交点是一线段的端点，-1不相交。
    {
        float s1, s2, s3, s4;
        int d1, d2, d3, d4;

        d1 = fltcmp(s1 = ab_cross_ac(a, b, c), 0);
        d2 = fltcmp(s2 = ab_cross_ac(a, b, d), 0);
        d3 = fltcmp(s3 = ab_cross_ac(c, d, a), 0);
        d4 = fltcmp(s4 = ab_cross_ac(c, d, b), 0);

        //如果规范相交则求交点
        if ((d1 ^ d2) == -2 && (d3 ^ d4) == -2)
        {
            p.x = (c.x * s2 - d.x * s1) / (s2 - s1);
            p.y = (c.y * s2 - d.y * s1) / (s2 - s1);
            return 1;
        }

        //如果不规范相交
        if (d1 == 0 && point_on_line(c, a, b) <= 0)
        {
            p = c;
            return 0;
        }
        if (d2 == 0 && point_on_line(d, a, b) <= 0)
        {
            p = d;
            return 0;
        }
        if (d3 == 0 && point_on_line(a, c, d) <= 0)
        {
            p = a;
            return 0;
        }
        if (d4 == 0 && point_on_line(b, c, d) <= 0)
        {
            p = b;
            return 0;
        }
        //如果不相交
        return -1;
    }

    public static int LineSeg_ab_cross_cd(Vector2 a,Vector2 b,Vector2 c,Vector2 d,ref Vector2 result){
        Point res = new Point(result);
        int state =  ab_cross_cd(new Point(a), new Point(b), new Point(c), new Point(d),ref res);
        result.x = res.x;
        result.y = res.y;
        return state;
    }
}
