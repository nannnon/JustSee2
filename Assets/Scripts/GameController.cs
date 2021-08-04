using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject m_rectPrefab = null;

    List<RectsController> m_rects;

    Vector2 m_areaLB;   // 画面の左下のワールド座標
    Vector2 m_areaRT;   // 画面の右上のワールド座標


    void Start()
    {
        // 画面端のワールド座標を取得
        this.m_areaLB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        this.m_areaRT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // 各矩形を生成
        this.m_rects = new List<RectsController>();

        const int kRectsNum = 60;
        for (int i = 0; i < kRectsNum; ++i)
        {
            // 矩形の初期位置とサイズを決める
            Vector2 pos = new Vector2();
            Vector2 size = new Vector2();
            {
                int counter = 0;
                const int kCounterTh = 1000;

                while (counter < kCounterTh)
                {
                    size.x = Random.Range(0.1f, 1f);
                    size.y = Random.Range(0.1f, 1f);
                    pos.x = Random.Range(this.m_areaLB.x, this.m_areaRT.x - size.x);
                    pos.y = Random.Range(this.m_areaLB.y, this.m_areaRT.y - size.y);

                    bool hit = this.CheckHit(pos.x, pos.y, size.x, size.y, -1);
                    if (!hit)
                    {
                        break;
                    }

                    ++counter;
                }
                if (counter >= kCounterTh)
                {
                    break;
                }
            }

            // インスタンス化、初期化
            GameObject go = Instantiate(m_rectPrefab);
            RectsController rc = go.GetComponent<RectsController>();
            rc.Init(this, pos, size, m_rects.Count);
            m_rects.Add(rc);
        }
    }

    public bool CheckHit(float x, float y, float w, float h, int ignore_index)
    {
        // 画面端との接触チェック
        if (x < this.m_areaLB.x || x + w > this.m_areaRT.x ||
            y < this.m_areaLB.y || y + h > this.m_areaRT.y)
        {
            return true;
        }

        // 個々の矩形との接触チェック
        for (int i = 0; i < this.m_rects.Count; ++i)
        {
            if (i == ignore_index)
            {
                continue;
            }

            bool hit = m_rects[i].CheckHit(x, y, w, h);
            if (hit)
            {
                return true;
            }
        }

        return false;
    }
}
