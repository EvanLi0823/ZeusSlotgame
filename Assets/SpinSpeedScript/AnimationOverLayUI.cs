using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverLayUI : MonoBehaviour
{
    private Dictionary<int,Dictionary<int,Vector3>> ElementPositions = new Dictionary<int,Dictionary<int,Vector3>>(); //create已知的位置，提前create好
    private BoardConfigs m_BoardConfigs; //create
    private List<Vector2> ExistPositions = new List<Vector2>(); //已经生成的position,方便存储索引index
    public void InitUI(BoardConfigs _BoardConfigs)
    {
        m_BoardConfigs = _BoardConfigs;
        ElementPositions.Clear();

        float xCoordinate = m_BoardConfigs.ReelPositionX(); //当前中心点x的坐标
        for (int x = 0; x < m_BoardConfigs.ReelConfigs.Length; x++)
        {
            Dictionary<int, Vector3> eachReel = new Dictionary<int, Vector3>();

            float yCoordinate = m_BoardConfigs.ReelConfigs[x].ReelYOffset - m_BoardConfigs.ReelShowHeight(x) / 2 +
                                m_BoardConfigs.SymbolHeight/2;
            for (int y = 0; y < m_BoardConfigs.ReelConfigs[x].ReelShowNum; y++)
            {
                eachReel[y] = new Vector3(xCoordinate, yCoordinate, 0f);

                yCoordinate += m_BoardConfigs.SymbolHeight;
            }

            ElementPositions[x] = eachReel;

            xCoordinate += m_BoardConfigs.ReelSpace + m_BoardConfigs.ReelWidth;
        }
    }

    public Vector3 GetPosition(int x, int y)
    {
        if (ElementPositions.ContainsKey(x) && ElementPositions[x].ContainsKey(y))
        {
            return ElementPositions[x][y];
        }
        return Vector3.zero;
    }

    public void DestroyChildren()
    {
        ExistPositions.Clear();
        Util.DestroyChildren(this.transform);
    }
    
    

    public int FindSiblingIndex(int x,int y)
    {
        int ret = ExistPositions.Count;
        for (int i = 0; i < ExistPositions.Count; i++)
        {
            Vector2 v = ExistPositions[i];
            if (y > v.y)
            {
                ret = i;
                break;
            }
            else if (y == v.y)
            {
                if (x < v.x)
                {
                    ret = i;
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
        }
        ExistPositions.Insert(ret,new Vector2(x,y));
//        ExistPositions.Add();
        return ret;
    }

    public void ClearExistPosition()
    {
        ExistPositions.Clear();
    }
}
