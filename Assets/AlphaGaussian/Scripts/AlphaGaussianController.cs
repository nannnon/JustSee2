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
        const int kBallsNum = 100;
        for (int  i = 0; i < kBallsNum; ++i)
        {
            this.AddRandomBall();
        }
    }

    void AddRandomBall()
    {
        Ball ball = new Ball();

        ball.r = Random.value * 7 + 1;
        ball.pos = new Vector2((Random.value * this.m_tex.width  - 2 * ball.r) + ball.r,
                               (Random.value * this.m_tex.height - 2 * ball.r) + ball.r);
        ball.vel = new Vector2(Random.value * 3 + 1, 0);
        ball.color = new Color32(this.Random255(), this.Random255(), this.Random255(), 255);

        this.m_balls.Add(ball);
    }

    byte Random255()
    {
        float val = Random.value * 255;
        byte val2 = (byte)Mathf.RoundToInt(val);
        return val2;
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
                int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
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
                        sumR += color.r;
                        sumG += color.g;
                        sumB += color.b;
                        sumA += color.a;
                        ++counter;
                    }
                }

                int index = xy2i(x, y, this.m_tex.width);
                pixels[index].r = (byte)Mathf.RoundToInt((float)sumR / counter);
                pixels[index].g = (byte)Mathf.RoundToInt((float)sumG / counter);
                pixels[index].b = (byte)Mathf.RoundToInt((float)sumB / counter);
                pixels[index].a = (byte)Mathf.RoundToInt((float)sumA / counter);
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
