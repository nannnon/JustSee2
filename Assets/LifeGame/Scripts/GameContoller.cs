using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameContoller : MonoBehaviour
{
    [SerializeField]
    GameObject m_cellPrefab = null;

    bool[,] m_cells;
    SpriteRenderer[,] m_srs;

    // Start is called before the first frame update
    void Start()
    {
        // セルの大きさ決定
        Vector2 areaLB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 areaRT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        const int kHorCellsNum = 200;
        float cellSize = (areaRT.x - areaLB.x) / kHorCellsNum;
        int verCellsNum = Mathf.FloorToInt((areaRT.y - areaLB.y) / cellSize);

        // 初期化
        this.m_cells = new bool[verCellsNum, kHorCellsNum];
        for (int  j = 0; j < this.m_cells.GetLength(0); ++j)
        {
            for (int i = 0; i < this.m_cells.GetLength(1); ++i)
            {
                this.m_cells[j, i] = false;
            }
        }

        this.m_srs = new SpriteRenderer[verCellsNum, kHorCellsNum];
        for (int j = 0; j < this.m_cells.GetLength(0); ++j)
        {
            for (int i = 0; i < this.m_cells.GetLength(1); ++i)
            {
                GameObject go = Instantiate(this.m_cellPrefab);
                this.m_srs[j, i] = go.GetComponent<SpriteRenderer>();
                this.m_srs[j, i].color = Color.black;

                // 位置
                float y = j * cellSize + cellSize / 2 + areaLB.y;
                float x = i * cellSize + cellSize / 2 + areaLB.x;
                go.transform.position = new Vector3(x, y);

                // スケール
                go.transform.localScale = new Vector3(cellSize, cellSize);
            }
        }

        // 乱数をばら撒く
        this.SetRandom();
    }

    // Update is called once per frame
    void Update()
    {
        bool[,] preCells = new bool[this.m_cells.GetLength(0), this.m_cells.GetLength(1)];
        Array.Copy(this.m_cells, preCells, this.m_cells.Length);

        int height = this.m_cells.GetLength(0);
        int width = this.m_cells.GetLength(1);
        for (int j = 0; j < height; ++j)
        {
            for (int i = 0; i < width; ++i)
            {
                // 誕生/死滅
                int neighboringAliveCellsNum = 0;
                for (int dy = -1; dy < 2; ++dy)
                {
                    for (int dx = -1; dx < 2; ++dx)
                    {
                        if (dy == 0 && dx == 0)
                        {
                            continue;
                        }

                        int y = LoopCoord(j + dy, height);
                        int x = LoopCoord(i + dx, width);
                        if (preCells[y, x])
                        {
                            ++neighboringAliveCellsNum;
                        }
                    }
                }

                if (preCells[j, i])
                {
                    if (neighboringAliveCellsNum == 2 || neighboringAliveCellsNum == 3)
                    {
                        this.m_cells[j, i] = true;
                    }
                    else
                    {
                        this.m_cells[j, i] = false;
                    }
                }
                else
                {
                    if (neighboringAliveCellsNum == 3)
                    {
                        this.m_cells[j, i] = true;
                    }
                }


                // 色反映
                if (m_cells[j, i])
                {
                    this.m_srs[j, i].color = Color.green;
                }
                else
                {
                    this.m_srs[j, i].color = Color.black;
                }
            }
        }

        // クリックされたランダムに
        if (Input.anyKeyDown)
        {
            this.SetRandom();
        }
    }

    int LoopCoord(int i, int length)
    {
        if (i < 0)
        {
            return i + length;
        }
        else if (i >= length)
        {
            return i - length;
        }
        else
        {
            return i;
        }
    }

    void SetRandom()
    {
        for (int j = 0; j < this.m_cells.GetLength(0); ++j)
        {
            for (int i = 0; i < this.m_cells.GetLength(1); ++i)
            {
                int val = UnityEngine.Random.Range(0, 2);
                this.m_cells[j, i] = (val == 0) ? false : true;
            }
        }
    }
}
