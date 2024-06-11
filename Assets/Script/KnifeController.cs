using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private bool isPCPlatform = false;

    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    private bool upButtonHeld = false;
    private bool downButtonHeld = false;
    private bool leftButtonHeld = false;

    private bool downButtonColourChanged = true;

    private void Start()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
        {
            isPCPlatform = true;
        }
    }
    void Update()
    {
        HandleKnifeMovement();
    }

    private void HandleKnifeMovement()
    {
        if (!knifeIsDown)
        {
            ChangeButtonColor(downButton, Color.green);
            ChangeButtonColor(upButton, Color.white);
        }

        if (!knifeIsUp && knifeIsDown)
        {
            ChangeButtonColor(upButton, Color.green);
            ChangeButtonColor(downButton, Color.white);
        }

        if (choppingManager.AllPartsSliced())
        {
            ChangeButtonColor(leftButton, Color.green);
            ChangeButtonColor(upButton, Color.white);
            ChangeButtonColor(downButton, Color.white);
            downButtonColourChanged = false;
        }

        if (!choppingManager.AllPartsSliced() && !downButtonColourChanged)
        {
            ChangeButtonColor(downButton, Color.green);
            ChangeButtonColor(leftButton, Color.white);
            downButtonColourChanged = true;
        }

        float rotation = transform.localEulerAngles.z;
        if (rotation > 180) rotation -= 360;

        if (isPCPlatform && Input.GetKey(KeyCode.UpArrow) && rotation > maxUpRotation || upButtonHeld && rotation > maxUpRotation)
        {
            OnUpArrowPressed();
        }
        else if (isPCPlatform && Input.GetKey(KeyCode.DownArrow) && rotation < maxDownRotation && !choppingManager.AllPartsSliced() || downButtonHeld && rotation < maxDownRotation && !choppingManager.AllPartsSliced())
        {
            OnDownArrowPressed();
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

        if (isPCPlatform && Input.GetKeyDown(KeyCode.LeftArrow) && !initiatedLeft || leftButtonHeld && !initiatedLeft)
        {
            OnLeftArrowPressed();
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
    
    public void OnUpArrowPressed()
    {
        transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
    }

    public void OnDownArrowPressed()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public void OnLeftArrowPressed()
    {
        if (choppingManager.AllPartsSliced())
        {
            if (transform.localEulerAngles.z > maxUpRotation)
            {
                StartCoroutine(RotateKnifeToMaxUpRotation());
            }
            initiatedLeft = true;
            StartCoroutine(FinalActions());
        }
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        if (eventData.selectedObject == upButton)
        {
            upButtonHeld = true;
        }
        else if (eventData.selectedObject == downButton)
        {
            downButtonHeld = true;
        }
        else if (eventData.selectedObject == leftButton)
        {
            leftButtonHeld = true;
        }
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        if (eventData.selectedObject == upButton)
        {
            upButtonHeld = false;
        }
        else if (eventData.selectedObject == downButton)
        {
            downButtonHeld = false;
        }
        else if (eventData.selectedObject == leftButton)
        {
            leftButtonHeld = false;
        }
    }

    private void ChangeButtonColor(GameObject button, Color color)
    {
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
    }

    private IEnumerator RotateKnifeToMaxUpRotation()
    {
        while (NormalizeAngle(transform.localEulerAngles.z) > maxUpRotation)
        {
            transform.Rotate(-Vector3.forward, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, maxUpRotation);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        else if (angle < -180f)
        {
            return angle + 360f;
        }
        else
        {
            return angle;
        }
    }
}
