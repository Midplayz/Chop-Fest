using UnityEngine;
using EzySlice;
using System.Collections;

public class VegetableSlicer : MonoBehaviour
{
    public Material cutMaterial;  // Material to apply to the cut surfaces
    public float sliceOffset = 0.1f;  // Offset to move the vegetable after each slice
    public float scaleMultiplier = 5f;  // Multiplier to rescale sliced objects
    public GameObject vegetableToCut;  // Vegetable to be cut assigned in inspector

    private bool isCutting = false;
    private int sliceCount = 0;

    void Start()
    {
        if (vegetableToCut != null)
        {
            Rigidbody rb = vegetableToCut.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  // Make the main vegetable kinematic
            }

            vegetableToCut.name = "Main_" + vegetableToCut.name;  // Rename the main vegetable
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isCutting)
        {
            isCutting = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject == vegetableToCut)
            {
                Debug.Log("Hit vegetable: " + hit.transform.name);
                Vector3 localHitPoint = hit.transform.InverseTransformPoint(hit.point);
                StartCoroutine(SliceObject(hit.transform.gameObject, localHitPoint));
            }
        }
    }

    private IEnumerator SliceObject(GameObject obj, Vector3 localHitPoint)
    {
        yield return new WaitForSeconds(0.1f);  // Small delay to simulate cutting action

        // Define the slicing plane in local space of the object
        EzySlice.Plane slicingPlane = new EzySlice.Plane(Vector3.right, localHitPoint);

        SlicedHull slicedHull = obj.Slice(slicingPlane, cutMaterial);
        if (slicedHull != null)
        {
            GameObject upperHull = slicedHull.CreateUpperHull(obj, cutMaterial);
            GameObject lowerHull = slicedHull.CreateLowerHull(obj, cutMaterial);

            if (upperHull != null && lowerHull != null)
            {
                // Set the parent to match the original object's hierarchy
                upperHull.transform.SetParent(obj.transform.parent);
                lowerHull.transform.SetParent(obj.transform.parent);

                // Copy position, rotation, and scale, then apply the scale multiplier
                CopyTransformAndRescale(obj.transform, upperHull.transform);
                CopyTransformAndRescale(obj.transform, lowerHull.transform);

                // Tag the new slices for subsequent slicing
                upperHull.tag = "Vegetable";
                lowerHull.tag = "Vegetable";

                // Add necessary components to the sliced parts
                AddComponentsToSlicedPart(upperHull);
                AddComponentsToSlicedPart(lowerHull);

                // Rename the sliced parts
                string originalName = obj.name.Replace("Main_", "");
                upperHull.name = originalName + "_slice_" + sliceCount.ToString("00");
                lowerHull.name = originalName + "_slice_" + (sliceCount + 1).ToString("00");
                sliceCount += 2;

                // Make the lower hull the new main vegetable
                lowerHull.name = "Main_" + originalName;
                vegetableToCut = lowerHull;
                Rigidbody lowerHullRb = lowerHull.GetComponent<Rigidbody>();
                if (lowerHullRb != null)
                {
                    lowerHullRb.isKinematic = true;  // Make the new main vegetable kinematic
                }

                // Destroy the original object
                Destroy(obj);

                // Move the remaining vegetable part to the left
                lowerHull.transform.position += lowerHull.transform.right * sliceOffset;

                Debug.Log("Slicing successful.");
            }
            else
            {
                Debug.LogError("Upper or lower hull creation failed.");
                if (upperHull != null) Destroy(upperHull);
                if (lowerHull != null) Destroy(lowerHull);
            }
        }
        else
        {
            Debug.LogError("Slicing failed.");
        }

        isCutting = false;
    }

    void CopyTransformAndRescale(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
        to.localScale = from.localScale * scaleMultiplier;
    }

    void AddComponentsToSlicedPart(GameObject obj)
    {
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        obj.AddComponent<Rigidbody>().isKinematic = false;  // Ensure sliced parts have physics
    }
}
