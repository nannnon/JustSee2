using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject m_rectPrefab = null;

    List<RectsController> m_rects;

    const float kX0 = -12;
    const float kY0 = -5;
    const float kX1 = 12;
    const float kY1 = 5;


    void Start()
    {
        this.m_rects = new List<RectsController>();

        const int kRectsNum = 30;
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
                    pos.x = Random.Range(kX0, kX1 - size.x);
                    pos.y = Random.Range(kY0, kY1 - size.y);

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
        if (x < kX0 || x + w > kX1 ||
            y < kY0 || y + h > kY1)
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
