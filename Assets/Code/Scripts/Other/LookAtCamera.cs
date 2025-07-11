using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private LookAtCameraType billboardType;
    [SerializeField] private Vector3 offset;
    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;

    public enum LookAtCameraType { LookAtCamera, CameraForward };

    private void Awake() {
        originalRotation = transform.rotation.eulerAngles;
    }

    // Use Late update so everything should have finished moving.
    void LateUpdate() {
        // There are two ways people billboard things.
        switch (billboardType) {
        case LookAtCameraType.LookAtCamera:
            transform.LookAt(Camera.main.transform.position, Vector3.up);
            transform.rotation *= Quaternion.Euler(offset);
            break;
        case LookAtCameraType.CameraForward:
            transform.forward = Camera.main.transform.forward;
            transform.rotation *= Quaternion.Euler(offset);
            break;
        default:
            break;
        }
        
        // Modify the rotation in Euler space to lock certain dimensions.
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }
}