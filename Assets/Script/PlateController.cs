using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateMovement : MonoBehaviour
{
    private Vector3 initialPosition;
    public float moveDistance = 1.0f;
    public float moveSpeed = 1.0f;
    public Material plateMaterial;
    public List<Color> colors = new List<Color>();

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void MovePlateToLeft()
    {
        StartCoroutine(MovePlateCoroutine());
    }

    private IEnumerator MovePlateCoroutine()
    {
        Vector3 targetPosition = initialPosition + -Vector3.forward * moveDistance;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);


        Color randomColor = colors[Random.Range(0, colors.Count)];
        plateMaterial.SetColor("_BaseColor", randomColor);

        t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(targetPosition, initialPosition, t);
            yield return null;
        }
        transform.position = initialPosition;
    }
}