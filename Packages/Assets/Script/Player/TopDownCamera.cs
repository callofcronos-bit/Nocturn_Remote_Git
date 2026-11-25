using UnityEngine;

// กล้อง Top-down แบบง่าย ไม่ต้องใช้ Cinemachine: แปะที่ Main Camera
public class TopDownCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 14, -8);
    [SerializeField] float smooth = 8f;

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
        transform.rotation = Quaternion.LookRotation(-offset);
    }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
    }
}
