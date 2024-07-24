using UnityEngine;
using UnityEngine.UI;

public class RadialLayout : MonoBehaviour
{
    public float Radius;
    public void OnValidate()
    {
        UpdateLayout();
    }
    public void UpdateLayout()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var angle = i * Mathf.PI * 2f / transform.childCount;
            var x = Mathf.Cos(angle) * Radius;
            var y = Mathf.Sin(angle) * Radius;
            transform.GetChild(i).transform.localPosition = new Vector3(x, y, 0);
        }
    }
}
