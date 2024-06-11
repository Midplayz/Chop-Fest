using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitChoppingManager : MonoBehaviour
{
    public KnifeController knifeController;  
    public float sliceMoveDistance = 0.15f; 
    public float sliceForce = 5f;

    public float moveSpeed = 1.0f; 
    public float moveDistance = 1.0f;

    private int currentFruitIndex = 0;
    private int currentPartIndex = 0;
    private List<Transform> fruits = new List<Transform>();
    private List<Vector3> initialFruitPositions = new List<Vector3>();
    private List<List<Vector3>> initialPartPositions = new List<List<Vector3>>();

    private bool shouldStopSlicesMoving = false;

    void Start()
    {
        foreach (Transform fruit in transform)
        {
            fruits.Add(fruit);
            initialFruitPositions.Add(fruit.localPosition);

            List<Vector3> partPositions = new List<Vector3>();
            foreach (Transform part in fruit)
            {
                partPositions.Add(part.localPosition);
            }
            initialPartPositions.Add(partPositions);
            fruit.gameObject.SetActive(false);
        }

        if (fruits.Count > 0)
        {
            ActivateFruit(0);
        }
    }

    public void CutCurrentPart()
    {
        Transform currentFruit = fruits[currentFruitIndex];
        if (currentPartIndex >= 0)
        {
            Transform currentPart = currentFruit.GetChild(currentPartIndex);
            Rigidbody rb = currentPart.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            currentPartIndex--;
        }
    }

    public void ApplyForceToSlices()
    {
        Transform currentFruit = fruits[currentFruitIndex];
        for (int i = 0; i < currentFruit.childCount; i++)
        {
            Transform part = currentFruit.GetChild(i);
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(-Vector3.forward * sliceForce, ForceMode.Impulse);  
            }
        }
    }

    public void MoveFruitLeft()
    {
        if (currentPartIndex > 0)
        {
            Transform currentFruit = fruits[currentFruitIndex];
            sliceMoveDistance = currentFruit.GetComponent<FruitsInformation>().amountToMove;
            currentFruit.localPosition += new Vector3(sliceMoveDistance, 0, 0);
        }
    }


    public bool AllPartsSliced()
    {
        return currentPartIndex == 0;
    }

    public void ResetFruit()
    {
        Transform currentFruit = fruits[currentFruitIndex];
        FruitsInformation fruitsInfo = currentFruit.GetComponent<FruitsInformation>();
        if (fruitsInfo != null)
        {
            fruitsInfo.ResetTransforms();
            shouldStopSlicesMoving = true;
        }

        for (int i = 0; i < currentFruit.childCount; i++)
        {
            Transform part = currentFruit.GetChild(i);
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    public void SwitchToNextFruit()
    {
        fruits[currentFruitIndex].gameObject.SetActive(false);
        currentPartIndex = 0;
        currentFruitIndex++;
        if (currentFruitIndex < fruits.Count)
        {
            ActivateFruit(currentFruitIndex);
        }
        else
        {
            currentFruitIndex = 0;
            ActivateFruit(currentFruitIndex);
        }
    }

    private void ActivateFruit(int index)
    {
        currentPartIndex = fruits[index].childCount - 1;
        fruits[index].gameObject.SetActive(true);
        MoveFruitLeft();
    }

    public void MoveAllSlicedParts()
    {
        shouldStopSlicesMoving = false;
        Transform currentFruit = fruits[currentFruitIndex]; 
        Vector3 targetPosition = currentFruit.position - Vector3.forward * moveDistance;

        List<Transform> slicedParts = new List<Transform>();
        foreach (Transform part in currentFruit)
        {
            if (part.gameObject.activeSelf) 
            {
                slicedParts.Add(part);
            }
        }

        StartCoroutine(MoveAllSlicedPartsCoroutine(slicedParts, targetPosition));
    }

    private IEnumerator MoveAllSlicedPartsCoroutine(List<Transform> slicedParts, Vector3 targetPosition)
    {
        float t = 0.0f;
        while (t < 1.0f && !shouldStopSlicesMoving)
        {
            t += Time.deltaTime * moveSpeed;
            foreach (Transform part in slicedParts)
            {
                part.position = Vector3.Lerp(part.position, targetPosition, t);
            }
            yield return null;
        }
    }
}
