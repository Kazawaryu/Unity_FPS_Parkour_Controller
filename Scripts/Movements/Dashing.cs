using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovementAdvanced pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("CameraEffect")]
    public PlayerCam cam;
    public float dashFov;

    [Header("Setting")]
    public bool useCameraForward = true;
    public bool allowAllDiretions = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }
        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFov);

        Transform forwardT;
        if (useCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction* dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForcceToApply = forceToApply;
        Invoke(nameof(DelayDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }


    private Vector3 delayedForcceToApply;

    private void DelayDashForce()
    {

        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForcceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;

        cam.DoFov(85f);
        if (disableGravity)
            rb.useGravity = true;
        
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direciton = new Vector3();
        if (allowAllDiretions)
            direciton = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direciton = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direciton = forwardT.forward;

        return direciton.normalized;
    }
}
