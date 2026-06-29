using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    [SerializeField] private Transform target;

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 5f; 
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); 

    private void LateUpdate()
    {
        // LateUpdate вызывается ПОСЛЕ всех Update и FixedUpdate.
        // Это критически важно для камер, чтобы убрать микро-дергания (jittering), 
        // так как персонаж уже завершил свое физическое движение в этом кадре.
        if (target == null) return;

        // Идеальная позиция, где должна быть камера
        Vector3 desiredPosition = target.position + offset;

        // Плавно интерполируем от текущей позиции камеры к идеальной
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
