using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CAMERAVIEWSTATUS
{
    FIRSTPERSON = 0,
    TOPVIEW,
    FIELDVIEW
};

public partial class PlayerControl : MonoBehaviour
{
    // public
    [Header("Player Key Binds - Movement")]
    public KeyCode keyPlayerMoveForward = KeyCode.W;
    public KeyCode keyPlayerMoveBackward = KeyCode.S;
    public KeyCode keyPlayerMoveToLeft = KeyCode.A;
    public KeyCode keyPlayerMoveToRight = KeyCode.D;

    [Header("Player Key Binds - Control")]
    public KeyCode keyCameraViewFirstPerson = KeyCode.Alpha1;
    public KeyCode keyCameraViewTopView = KeyCode.Alpha2;
    public KeyCode keyCameraViewFieldView = KeyCode.Alpha3;

    [Header("Player Key Binds - View Control")]
    public KeyCode keyPlayerViewUp = KeyCode.Keypad8;
    public KeyCode keyPlayerViewDown = KeyCode.Keypad2;
    public KeyCode keyPlayerViewLeft = KeyCode.Keypad4;
    public KeyCode keyPlayerViewRight = KeyCode.Keypad6;

    [Header("Player Status Values")]
    public float playerMoveSpeed = 14f;
    public float playerGravityForce = 20f;
    public float playerRotateSpeed = 50f;

    [Header("Camera Anchor for FPS/TopView")]
    public Camera cameraPlayerOnly = null;
    public Transform anchorFPS = null;
    public Transform anchorTopView = null;
    public Transform anchorFieldView = null;
    public CAMERAVIEWSTATUS cameraViewValue = CAMERAVIEWSTATUS.FIRSTPERSON;
    public float cameraAngleLowerLimit = 30f;
    public float cameraAngleUpperLimit = 80f;
    public float cameraRotateSpeed = 50f;

    [Header("Current Target by Player Action")]
    public PlayerLockonTarget targetStorage = null;
    public GameObject targetLockedOn = null;

    [Header("Camera Anchor Test")]
    public Text anchorStatusText = null;

    // private
    private CharacterController characon = null;
}
