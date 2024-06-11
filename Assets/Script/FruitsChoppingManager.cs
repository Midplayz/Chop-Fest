using System.Collections.Generic;
using UnityEngine;

public class FruitChoppingManager : MonoBehaviour
{
    public GameObject knife;
    public float maxDownRotation = 37f;
    public float maxUpRotation = -37f;
    public float rotationSpeed = 100f;
    public float sliceMoveDistance = 0.15f;  // Distance to move the fruit after each slice

    private int currentFruitIndex = 0;
    private int currentPartIndex = 0;
    private List<Transform> fruits = new List<Transform>();
    private List<Vector3> initialFruitPositions = new List<Vector3>();
    private List<List<Vector3>> initialPartPositions = new List<List<Vector3>>();
    private bool knifeIsUp = true;
    private bool knifeIsDown = false;

    void Start()
    {
        // Get all fruit children and save their initial positions
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

            // Deactivate all fruits at the start
            fruit.gameObject.SetActive(false);
        }

        // Activate the first fruit
        if (fruits.Count > 0)
        {
            ActivateFruit(0);
        }
    }

    void Update()
    {
        HandleKnifeMovement();
    }

    private void HandleKnifeMovement()
    {
        float rotation = knife.transform.localEulerAngles.z;
        if (rotation > 180) rotation -= 360;

        if (Input.GetKey(KeyCode.UpArrow) && rotation > maxUpRotation)
        {
            knife.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && rotation < maxDownRotation && !AllPartsSliced())
        {
            knife.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if (rotation >= maxDownRotation - 1f && !knifeIsDown)
        {
            knifeIsDown = true;
            knifeIsUp = false;
            CutCurrentPart();
        }
        else if (rotation <= maxUpRotation + 1f && !knifeIsUp)
        {
            knifeIsUp = true;
            knifeIsDown = false;
            MoveFruitLeft();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Next Initiated");
            if (AllPartsSliced())
            {
                ResetFruit();
                SwitchToNextFruit();
                Debug.Log("Next Carried OUt");
            }
        }
    }

    private void CutCurrentPart()
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
        Debug.Log("Current Part: " + currentPartIndex);
    }

    private void MoveFruitLeft()
    {
        if (currentPartIndex > 0)
        {
            Transform currentFruit = fruits[currentFruitIndex];
            currentFruit.localPosition += new Vector3(sliceMoveDistance, 0, 0);
        }
    }

    private bool AllPartsSliced()
    {
        bool isAllPartsSliced = currentPartIndex == 0;
        Debug.Log("All Parts Sliced? " + isAllPartsSliced);
        return isAllPartsSliced;
    }

    private void ResetFruit()
    {
        Transform currentFruit = fruits[currentFruitIndex];
        currentFruit.localPosition = initialFruitPositions[currentFruitIndex];

        for (int i = 0; i < currentFruit.childCount; i++)
        {
            Transform part = currentFruit.GetChild(i);
            part.localPosition = initialPartPositions[currentFruitIndex][i];

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void SwitchToNextFruit()
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
}
