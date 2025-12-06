using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float positionSpeed = 8f;
    public float rotationSpeed = 5f;
    
    [Header("Local Position")]
    public Vector3 localPos = new Vector3(0, 10, -6);  
    
    private Vector3 positionVelocity = Vector3.zero;
    private Vector3 rotationVelocity = Vector3.zero;
    
    void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition, 
            localPos, 
            ref positionVelocity, 
            0.3f,
            positionSpeed,
            Time.deltaTime
        );
        
        Quaternion targetRot = Quaternion.Euler(20, 0, 0);  // Fixed angled view
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, 
            targetRot, 
            rotationSpeed * Time.deltaTime
        );
    }
}
