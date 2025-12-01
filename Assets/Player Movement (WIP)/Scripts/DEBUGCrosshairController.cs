using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Camera cam;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float crosshairOffset = 0.01f;
    [SerializeField] private LayerMask crosshairMask = ~0;
    
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    
    void Update()
    {
        Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.ScreenPointToRay(screenCentre);

        Vector3 targetPosition;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, crosshairMask))
        {
            targetPosition = hit.point + hit.normal * crosshairOffset;
            crosshair.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            targetPosition = ray.GetPoint(maxDistance);
            crosshair.forward = cam.transform.forward;
            
        }
        crosshair.position = targetPosition;
    }
}
