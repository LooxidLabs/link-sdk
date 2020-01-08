using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Looxid.Link
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineChart : MonoBehaviour
    {
        public int Width = 1000;
        public int Height = 240;
        public bool center_is_zero = false;

        private LineRenderer line;
        private double[] datalist;

        void OnEnable()
        {
            line = GetComponent<LineRenderer>();
            StartCoroutine(DrawChart());
        }

        IEnumerator DrawChart()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(0.1f);

                if (datalist != null)
                {
                    if (datalist.Length > 1)
                    {
                        line.positionCount = datalist.Length;
                        float xDist = (float)Width / (float)(datalist.Length - 1);

                        for (int x = 0; x < datalist.Length; x++)
                        {
                            int dataHeight = Height / 2;

                            if (x < datalist.Length)
                            {
                                if (double.IsNaN(datalist[x]))
                                {
                                    dataHeight = 0;
                                }
                                else
                                {
                                    if (center_is_zero)
                                    {
                                        dataHeight = Mathf.FloorToInt((float)(datalist[x] + 0.5) * (float)Height);
                                    }
                                    else
                                    {
                                        dataHeight = Mathf.FloorToInt((float)datalist[x] * (float)Height);
                                    }
                                }
                            }

                            float pos_x = xDist * (float)x;
                            line.SetPosition(x, new Vector3(pos_x, dataHeight, 0.0f));
                        }
                    }
                }
            }
        }

        public void SetValue(double[] datalist)
        {
            this.datalist = datalist;
        }
    }
}