using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 LookAtPos = Vector3.zero;

    float _theta = Mathf.PI / 4;
    float _phi = Mathf.PI / 4;
    float _radius = 1;
    
    // Update is called once per frame
    void Update()
    {
        {
            float omega = Time.deltaTime;

            if (Input.GetKey(KeyCode.W))
            {
                if (_phi + omega < Mathf.PI / 2)
                {
                    _phi += omega;
                }
                else
                {
                    _phi = Mathf.PI / 2 - 0.01f;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (_phi - omega > -Mathf.PI / 2)
                {
                    _phi -= omega;
                }
                else
                {
                    _phi = -Mathf.PI / 2 + 0.01f;
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                _theta -= omega;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _theta += omega;
            }
        }

        {
            float dr = 1 * Time.deltaTime;

            if (Input.GetKey(KeyCode.E))
            {
                _radius += dr;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                if (_radius - dr >= 0.01f)
                {
                    _radius -= dr;
                }
                else
                {
                    _radius = 0.01f;
                }
            }
        }


        Vector3 pos;
        pos.x = _radius * Mathf.Cos(_theta) * Mathf.Cos(_phi);
        pos.y = _radius * Mathf.Sin(_phi);
        pos.z = _radius * Mathf.Sin(_theta) * Mathf.Cos(_phi);

        transform.position = LookAtPos + pos;

        transform.LookAt(LookAtPos);
    }
}