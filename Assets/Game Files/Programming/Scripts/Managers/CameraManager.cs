using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

public class CameraManager : Singleton<CameraManager> 
{
    public Camera MainCamera;
    public bool LockedOn;
    public CinemachineFreeLook FreeLookCam;
    public CinemachineFreeLook FreeLockOnCam;
    public CinemachineVirtualCamera RecenterCam;
    public CinemachineVirtualCamera LockOnCam;
    public float BufferLock;
    public float WaitTime;
    public CinemachineInputProvider InputProvider => FreeLookCam.GetComponent<CinemachineInputProvider>();
    public float PlaneThreshold;
    public bool ForceRecenter;

    public float RecenterTime = 0.25f;

    CinemachineOrbitalTransposer[] orbital = new CinemachineOrbitalTransposer[3];
    CinemachineVirtualCamera[] rigs = new CinemachineVirtualCamera[3];
    public Vector2 CameraSpeed;
    public CameraWorldUpOverride CameraWorldUpOverride;
    public CameraLookTarget CameraLookTarget1;
    public CameraLookTarget CameraLookTarget2;

    public CameraFollower CameraFollower1;
    public CameraFollower CameraFollower2;

    public override void Start()
    {
        base.Start();
        MainCamera = Camera.main;

        for (int i = 0; FreeLookCam != null && i < 3; ++i)
        {
            rigs[i] = FreeLookCam.GetRig(i);
            orbital[i] = rigs[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }
    }
	private void Update()
    {
        if (!PlayerManager.Instance.PlayerController)
            return;
        BufferActivity();
        CameraUpdate();
    }

    public void SetTarget(Transform transform)
	{
        CameraWorldUpOverride.Target = transform;
        CameraFollower1.Target = transform;
        CameraFollower2.Target = transform;
        CameraLookTarget1.target = transform;
        CameraLookTarget2.target = transform;
        //StartCoroutine(TemporarilyRecenter());
    }

    public void CameraUpdate()
	{
        bool useLockOn = (BufferLock < WaitTime);
        if (!useLockOn)
            (LockOnCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer).m_CameraDistance = Vector3.Distance(FreeLookCam.gameObject.transform.position, PlayerManager.Instance.PlayerController.gameObject.transform.position);
        LockOnCam.GetComponent<CinemachineCollider>().m_OptimalTargetDistance = Vector3.Distance(FreeLookCam.gameObject.transform.position, PlayerManager.Instance.PlayerController.gameObject.transform.position);
        //NO TRANSITIONAL CAM SETTINGS
        if(!ForceRecenter)
            FreeLookCam.Priority = useLockOn ? 0 : 10;

        //LockOnCam.Priority = useLockOn && (!TargetsVisible(PlayerManager.Instance.PlayerObject, PlayerManager.Instance.PlayerObject.Target.transform) || ForceRecenter) ? 10 : 0;
        if (ForceRecenter && useLockOn)
        {
            ForceRecenter = false;
        }

        if (PlayerManager.Instance.PlayerController.ButtonLockHold && LockedOn)
        {
            FreeLookCam.Priority = 0;
            LockOnCam.Priority = 10;
        }
        //FreeLockOnCam.Priority = useLockOn ? 10 : 0;

        //if(useLockOn)
        //    LockOnCam.Priority = !TargetsVisible(PlayerManager.Instance.PlayerObject, PlayerManager.Instance.PlayerObject.Target) ? 11 : 0;

        //if (BufferLock == 0)
        //{
        //	FreeLookCam.ForceCameraPosition(LockOnCam.transform.position, LockOnCam.transform.rotation);

        //}
        //else if (BufferLock == WaitTime)
        //{

        //	LockOnCam.ForceCameraPosition(FreeLookCam.transform.position, FreeLookCam.transform.rotation);
        //}
    }

    public void ResetCamera()
	{
        ForceRecenter = true;
        if (LockedOn)
        {
            if (FreeLookCam.Priority > 0)
            {
                LockOnCam.Priority = 10;
                FreeLookCam.Priority = 0;
            }
            else if (LockOnCam.Priority > 0)
            {
                LockOnCam.Priority = 0;
                FreeLookCam.Priority = 10;
            }
        }
		else
		{
            StartCoroutine(TemporarilyRecenter());
        }
    }

    IEnumerator TemporarilyRecenter()
	{
        RecenterCam.Priority = 11;
        yield return new WaitForSeconds(0.04f);
        RecenterCam.Priority = 0;
    }
    public void BufferActivity()
	{

        bool useLockOn = (LockedOn && (InputProvider.GetAxisValue(0) == 0 && InputProvider.GetAxisValue(1) == 0) && !PlayerManager.Instance.PlayerController.ButtonLockHold);
        if (!useLockOn)
            BufferLock += Time.deltaTime;
        else
            BufferLock -= Time.deltaTime;

        BufferLock = Mathf.Clamp(BufferLock, 0f, WaitTime) ;
    }

    public bool TargetsVisible(TangibleObject TargetA, Transform TargetB)
    {
        bool meshRendererVisible = (TargetB.TryGetComponent(out MeshRenderer meshRenderer));
        if (!meshRendererVisible)
            meshRendererVisible = meshRenderer = TargetB.parent.GetComponentInChildren<MeshRenderer>(); //need to learn why this works but I'm rollin with it
        if(meshRendererVisible)
            meshRendererVisible = (TargetA.MeshRenderer.isVisible && meshRenderer.isVisible);
        bool frustrumDetection = (IsTargetVisible(MainCamera, TargetA.gameObject, PlaneThreshold) && IsTargetVisible(MainCamera, TargetB.gameObject, PlaneThreshold));
        return frustrumDetection; // && meshRendererVisible
    }
    bool IsTargetVisible(Camera camera, GameObject obj, float threshold)//highernumbers make it more sensitive
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Vector3 point = obj.transform.position;
        foreach (Plane plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < threshold)
                return false;
        }
        return true;
    }

}
