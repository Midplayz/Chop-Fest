using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsInformation : MonoBehaviour
{
    [System.Serializable]
    public struct TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
    }
    public float amountToMove = 1.5f;
    public TransformData fruitTransformData;
    public List<TransformData> partTransformData = new List<TransformData>();

    void Start()
    {
        fruitTransformData = new TransformData
        {
            localPosition = transform.localPosition,
            localRotation = transform.localRotation
        };

        foreach (Transform child in transform)
        {
            partTransformData.Add(new TransformData
            {
                localPosition = child.localPosition,
                localRotation = child.localRotation
            });
        }
    }

    public void ResetTransforms()
    {
        transform.localPosition = fruitTransformData.localPosition;
        transform.localRotation = fruitTransformData.localRotation;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = partTransformData[i].localPosition;
            child.localRotation = partTransformData[i].localRotation;
        }
    }
}
