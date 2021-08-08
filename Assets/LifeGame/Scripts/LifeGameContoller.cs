using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeGameContoller : MonoBehaviour
{
    bool[,] m_cells;
    Texture2D m_tex;

    // Start is called before the first frame update
    void Start()
    {
        // セルの大きさ決定
        Vector2 areaLB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 areaRT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        const int kHorCellsNum = 200;
        float cellSize = (areaRT.x - areaLB.x) / kHorCellsNum;
        int verCellsNum = Mathf.FloorToInt((areaRT.y - areaLB.y) / cellSize);

        // セルの状態変数の初期化
        this.m_cells = new bool[kHorCellsNum, verCellsNum];
        for (int  i = 0; i < this.m_cells.GetLength(0); ++i)
        {
            for (int j = 0; j < this.m_cells.GetLength(1); ++j)
            {
                this.m_cells[i, j] = false;
            }
        }

        // セルの描画を行う平面の大きさを、カメラのサイズにフィットさせる
        GameObject plane = GameObject.Find("Plane");
        Vector2 planeDefaultSise = new Vector2(10, 10);
        Vector2 areaSize = areaRT - areaLB;
        plane.transform.localScale = new Vector3(areaSize.x / planeDefaultSise.x, 1, areaSize.y / planeDefaultSise.y);

        // テクスチャを生成・設定
        this.m_tex = new Texture2D(kHorCellsNum, verCellsNum);
        for (int i = 0; i < this.m_tex.width; ++i)
        {
            for (int j = 0; j < this.m_tex.height; ++j)
            {
                this.m_tex.SetPixel(i, j, Color.black);
            }
        }
        this.m_tex.Apply();
        this.m_tex.filterMode = FilterMode.Point;
        plane.GetComponent<MeshRenderer>().material.mainTexture = this.m_tex;

        // 乱数をばら撒く
        this.SetRandom();
    }

    // Update is called once per frame
    void Update()
    {
        int width = this.m_cells.GetLength(0);
        int height = this.m_cells.GetLength(1);

        bool[,] preCells = new bool[width, height];
        Array.Copy(this.m_cells, preCells, this.m_cells.Length);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                // 誕生/死滅
                int neighboringAliveCellsNum = 0;
                for (int dx = -1; dx < 2; ++dx)
                {
                    for (int dy = -1; dy < 2; ++dy)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        int x = LoopCoord(i + dx, width);
                        int y = LoopCoord(j + dy, height);
                        if (preCells[x, y])
                        {
                            ++neighboringAliveCellsNum;
                        }
                    }
                }

                if (preCells[i, j])
                {
                    if (neighboringAliveCellsNum == 2 || neighboringAliveCellsNum == 3)
                    {
                        this.m_cells[i, j] = true;
                    }
                    else
                    {
                        this.m_cells[i, j] = false;
                    }
                }
                else
                {
                    if (neighboringAliveCellsNum == 3)
                    {
                        this.m_cells[i, j] = true;
                    }
                }


                // 色反映
                if (m_cells[i, j])
                {
                    this.m_tex.SetPixel(i, j, Color.green);
                }
                else
                {
                    this.m_tex.SetPixel(i, j, Color.black);
                }
            }
        }
        this.m_tex.Apply();

        // クリックされたらランダムに
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
