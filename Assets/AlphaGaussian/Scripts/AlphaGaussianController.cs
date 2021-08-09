using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaGaussianController : MonoBehaviour
{
    struct Ball
    {
        public Vector2 pos;
        public Vector2 vel;
        public float r;
        public Color32 color;
    }

    Texture2D m_tex;
    List<Ball> m_balls;

    // Start is called before the first frame update
    void Start()
    {
        // セルの描画を行う平面の大きさを、カメラのサイズにフィットさせる
        GameObject plane = GameObject.Find("Plane");
        Vector2 planeMeshSize = new Vector2(10, 10);
        Vector2 areaLB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 areaRT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector2 areaSize = areaRT - areaLB;
        plane.transform.localScale = new Vector3(areaSize.x / planeMeshSize.x, 1, areaSize.y / planeMeshSize.y);

        // テクスチャを生成・設定
        const int kHeight = 128;
        int width = Mathf.FloorToInt(kHeight * Camera.main.aspect);

        this.m_tex = new Texture2D(width, kHeight);
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < kHeight; ++y)
            {
                this.m_tex.SetPixel(x, y, Color.white);
            }
        }
        this.m_tex.Apply();
        plane.GetComponent<MeshRenderer>().material.mainTexture = this.m_tex;

        // ボール追加
        this.m_balls = new List<Ball>();
        {
            Ball ball = new Ball();
            ball.pos = new Vector2(20, 64);
            ball.vel = new Vector2(4, 0);
            ball.r = 8;
            ball.color = new Color32(222, 0, 0, 255);
            this.m_balls.Add(ball);
        }
        {
            Ball ball = new Ball();
            ball.pos = new Vector2(100, 84);
            ball.vel = new Vector2(4, 0);
            ball.r = 8;
            ball.color = new Color32(20, 220, 80, 255);
            this.m_balls.Add(ball);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color32[] pixels = this.m_tex.GetPixels32();
        Color32[] prePixels = new Color32[pixels.Length];
        pixels.CopyTo(prePixels, 0);

        // ガウシアン
        for (int x = 0; x < this.m_tex.width; ++x)
        {
            for (int y = 0; y < this.m_tex.height; ++y)
            {
                int sum = 0;
                int counter = 0;
                for (int dx = -1; dx < 2; ++dx)
                {
                    for (int dy = -1; dy < 2; ++dy)
                    {
                        int i = x + dx;
                        int j = y + dy;

                        if (i < 0 || i >= this.m_tex.width ||
                            j < 0 || j >= this.m_tex.height)
                        {
                            continue;
                        }

                        Color32 color = prePixels[xy2i(i, j, this.m_tex.width)];
                        sum += (color.r << 16) + (color.g << 8) + color.b;
                        ++counter;
                    }
                }

                int mean = Mathf.RoundToInt((float)sum / counter);
                int index = xy2i(x, y, this.m_tex.width);
                pixels[index].r = (byte)((mean >> 16) & 0xFF);
                pixels[index].g = (byte)((mean >>  8) & 0xFF);
                pixels[index].b = (byte)( mean        & 0xFF);
            }
        }

        for (int i = 0; i < this.m_balls.Count; ++i)
        {
            Ball ball = this.m_balls[i];

            // ボールを動かす
            ball.pos += ball.vel;

            // 壁に当たったら反射
            if (ball.pos.x - ball.r < 0 ||
                ball.pos.x + ball.r >= this.m_tex.width)
            {
                ball.pos.x -= ball.vel.x;
                ball.vel.x *= -1;
            }
            if (ball.pos.y - ball.r < 0 ||
                ball.pos.y + ball.r >= this.m_tex.height)
            {
                ball.pos.y -= ball.vel.y;
                ball.vel.y *= -1;
            }

            // 重力
            ball.vel.y += 1;

            // ボールを描画
            for (int x = Mathf.FloorToInt(ball.pos.x - ball.r); x <= Mathf.CeilToInt(ball.pos.x + ball.r); ++x)
            {
                for (int y = Mathf.FloorToInt(ball.pos.y - ball.r); y <= Mathf.CeilToInt(ball.pos.y + ball.r); ++y)
                {
                    if (x < 0 || x >= this.m_tex.width ||
                        y < 0 || y >= this.m_tex.height)
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(ball.pos, new Vector2(x, y));
                    if (distance <= ball.r)
                    {
                        pixels[xy2i(x, y, this.m_tex.width)] = ball.color;
                    }
                }
            }

            this.m_balls[i] = ball;
        }

        // 反映
        this.m_tex.SetPixels32(pixels);
        this.m_tex.Apply();
    }

    int xy2i(int x, int y, int width)
    {
        int i = x + y * width;
        return i;
    }
}
