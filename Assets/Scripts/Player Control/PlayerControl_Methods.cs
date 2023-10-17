using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Add NumpadVertical/Horizontal to Unity's Input Manager
 * 
 * Edit -> Project Settings -> Input Manager
 * 
 * NumpadHorizontal
 * pos: [6] for keypad6, turn right
 * neg: [4] for keypad4, turn left
 * 
 * NumpadVertical
 * pos: [8] for keypad8, look upward
 * neg: [2] for keypad2, look downward
 */

public partial class PlayerControl : MonoBehaviour
{
    private void MovePlayerCharacterByUser()
    {
        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");

        Vector3 moveDirectionForward = transform.forward * Input.GetAxis("Vertical");
        Vector3 moveDirectionRightside = transform.right * Input.GetAxis("Horizontal") * -1f;

        Vector3 moveDirection = (moveDirectionForward - moveDirectionRightside).normalized;
        characon.Move(moveDirection * playerMoveSpeed * Time.deltaTime);
    }
    
    private void ChangeCameraAnchorAndViewport()
    {
        if (Input.GetKeyDown(keyCameraViewFirstPerson))
            OnButtonClicked_ChangeViewToFirstPerson();

        if (Input.GetKeyDown(keyCameraViewTopView))
            OnButtonClicked_ChangeViewToTopView();

        if (Input.GetKeyDown(keyCameraViewFieldView))
            OnButtonClicked_ChangeViewToFieldView();
    }

    public void OnButtonClicked_ChangeViewToFirstPerson()
    {
        cameraViewValue = CAMERAVIEWSTATUS.FIRSTPERSON;
        cameraPlayerOnly.transform.position = anchorFPS.position;
        cameraPlayerOnly.transform.rotation = anchorFPS.rotation;
        cameraPlayerOnly.transform.SetParent(anchorFPS);
        anchorStatusText.text = "FPS";
    }

    public void OnButtonClicked_ChangeViewToTopView()
    {
        cameraViewValue = CAMERAVIEWSTATUS.TOPVIEW;
        cameraPlayerOnly.transform.position = anchorTopView.position;
        cameraPlayerOnly.transform.rotation = anchorTopView.rotation;
        cameraPlayerOnly.transform.SetParent(anchorTopView);

        cameraPlayerOnly.transform.LookAt(transform);
        anchorStatusText.text = "TopView";
    }

    public void OnButtonClicked_ChangeViewToFieldView()
    {
        cameraViewValue = CAMERAVIEWSTATUS.FIELDVIEW;
        cameraPlayerOnly.transform.position = anchorFieldView.localPosition;
        cameraPlayerOnly.transform.rotation = anchorFieldView.localRotation;
        cameraPlayerOnly.transform.SetParent(anchorFieldView);
        anchorStatusText.text = "FieldView";
    }

    private void CameraMovement_FirstPerson()
    {
        if (cameraViewValue != CAMERAVIEWSTATUS.FIRSTPERSON) return;

        if (!GetComponent<Renderer>().enabled) GetComponent<Renderer>().enabled = true;

        if (targetStorage.GetTargetTransform() != null)
            CameraMovement_TargetLocked();
        else
        {
            transform.Rotate(Vector3.up * Input.GetAxis("NumpadViewHorizontal") * playerRotateSpeed * Time.deltaTime);
            cameraPlayerOnly.transform.RotateAround(
                anchorFPS.position,
                anchorFPS.right,
                Input.GetAxis("NumpadViewVertical") * cameraRotateSpeed * Time.deltaTime
                );
        }
    }

    private void CameraMovement_Topview()
    {
        anchorTopView.LookAt(transform);

        if (cameraViewValue != CAMERAVIEWSTATUS.TOPVIEW) return;

        if (!GetComponent<Renderer>().enabled) GetComponent<Renderer>().enabled = true;

        if (targetStorage.GetTargetTransform() != null)
            CameraMovement_TargetLocked();
        else
        {
            transform.Rotate(
                Vector3.up,
                Input.GetAxis("NumpadViewHorizontal") * playerRotateSpeed * Time.deltaTime
                );
        }
    }

    private void CameraMovement_FieldView()
    {
        if (cameraViewValue != CAMERAVIEWSTATUS.FIELDVIEW) return;

        GetComponent<Renderer>().enabled = false;
        Debug.LogWarning("FiledView Mode - Cannot Move Player Character");
    }

    private void CameraMovement_TargetLocked()
    {
        transform.LookAt(targetStorage.GetTargetTransform());
    }    

    // = = = = = = = = = = unity = = = = = = = = = =
    private void Awake()
    {
        characon = GetComponent<CharacterController>();

        anchorStatusText.text = cameraViewValue == CAMERAVIEWSTATUS.FIRSTPERSON ? "FPS" : "NULL";
        targetStorage = gameObject.GetComponent<PlayerLockonTarget>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 30f, Color.red);

        MovePlayerCharacterByUser();
        ChangeCameraAnchorAndViewport();

        CameraMovement_FirstPerson();
        CameraMovement_Topview();
        CameraMovement_FieldView();
    }
}
