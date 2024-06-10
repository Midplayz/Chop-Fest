using UnityEngine;
using EzySlice;

public class VegetableSlicer : MonoBehaviour
{
    public Material cutMaterial;  // Material to apply to the cut surfaces

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit object: " + hit.transform.name);
                SliceObject(hit.transform.gameObject, hit.point, hit.normal);
            }
        }
    }

    void SliceObject(GameObject obj, Vector3 position, Vector3 normal)
    {
        SlicedHull slicedHull = obj.Slice(position, normal, cutMaterial);
        if (slicedHull != null)
        {
            GameObject upperHull = slicedHull.CreateUpperHull(obj, cutMaterial);
            GameObject lowerHull = slicedHull.CreateLowerHull(obj, cutMaterial);

            if (upperHull != null && lowerHull != null)
            {
                // Copy position, rotation, and scale
                CopyTransform(obj.transform, upperHull.transform);
                CopyTransform(obj.transform, lowerHull.transform);

                // Set the parent to match the original object's hierarchy
                upperHull.transform.SetParent(obj.transform.parent);
                lowerHull.transform.SetParent(obj.transform.parent);

                // Add necessary components to the sliced parts
                AddComponentsToSlicedPart(upperHull);
                AddComponentsToSlicedPart(lowerHull);

                // Destroy the original object
                Destroy(obj);

                Debug.Log("Slicing successful.");
            }
            else
            {
                Debug.LogError("Upper or lower hull creation failed.");
            }
        }
        else
        {
            Debug.LogError("Slicing failed.");
        }
    }

    void CopyTransform(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
        to.localScale = from.localScale;
    }

    void AddComponentsToSlicedPart(GameObject obj)
    {
        MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        obj.AddComponent<Rigidbody>();
    }
}
