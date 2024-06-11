using System.Collections;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public float maxDownRotation = 37f;
    public float maxUpRotation = -37f;
    public float rotationSpeed = 100f;
    public FruitChoppingManager choppingManager;
    public PlateMovement plateMovement;

    private bool knifeIsUp = true;
    private bool knifeIsDown = false;
    private bool initiatedLeft = false;

    void Update()
    {
        HandleKnifeMovement();
    }

    private void HandleKnifeMovement()
    {
        float rotation = transform.localEulerAngles.z;
        if (rotation > 180) rotation -= 360;

        if (Input.GetKey(KeyCode.UpArrow) && rotation > maxUpRotation)
        {
            transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && rotation < maxDownRotation && !choppingManager.AllPartsSliced())
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if (rotation >= maxDownRotation - 1f && !knifeIsDown)
        {
            knifeIsDown = true;
            knifeIsUp = false;
            choppingManager.CutCurrentPart();
        }
        else if (rotation <= maxUpRotation + 1f && !knifeIsUp)
        {
            knifeIsUp = true;
            knifeIsDown = false;
            choppingManager.MoveFruitLeft();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !initiatedLeft)
        {
            if (choppingManager.AllPartsSliced())
            {
                initiatedLeft = true;
                StartCoroutine(FinalActions());
            }
        }
    }

    private IEnumerator FinalActions()
    {
        choppingManager.ApplyForceToSlices();
        yield return new WaitForSeconds(0.5f);
        choppingManager.MoveAllSlicedParts();
        plateMovement.MovePlateToLeft();
        yield return new WaitForSeconds(2.0f);
        choppingManager.ResetFruit();
        choppingManager.SwitchToNextFruit();
        initiatedLeft = false;
    }
}
