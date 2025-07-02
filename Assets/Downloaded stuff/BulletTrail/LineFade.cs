using UnityEngine;

namespace Bolt
{
    public class LineFade : MonoBehaviour
    {
        [SerializeField] Color color;

        [SerializeField] float speed = 15f;

        LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
            //για να κανει fade out
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * speed);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}
