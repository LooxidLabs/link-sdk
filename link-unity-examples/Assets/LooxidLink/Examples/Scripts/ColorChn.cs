using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public class ColorChn : MonoBehaviour
    {
        private Image activityImgs;

        private void Start()
        {
            activityImgs = GetComponent<Image>();
            StartCoroutine(Increase());
        }

        IEnumerator Increase()
        {
            float time = 0;
            float value = Random.Range(0.2f, 1.5f);
            while (time < 1)
            {
                yield return null;
                activityImgs.color = new Color(activityImgs.color.r, activityImgs.color.g, activityImgs.color.b, Mathf.Lerp(0f, 1f, time));
                time += Time.deltaTime * value;
            }
            StartCoroutine(Decrease());
        }

        IEnumerator Decrease()
        {
            float time = 1;
            float value = Random.Range(0.2f, 1.5f);
            while (time > 0)
            {
                yield return null;
                activityImgs.color = new Color(activityImgs.color.r, activityImgs.color.g, activityImgs.color.b, Mathf.Lerp(0f, 1f, time));
                time -= Time.deltaTime * value;
            }
            StartCoroutine(Increase());
        }
    }
}