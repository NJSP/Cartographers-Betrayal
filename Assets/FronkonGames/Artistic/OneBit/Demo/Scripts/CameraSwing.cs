using UnityEngine;

namespace FronkonGames.Artistic
{
  /// <summary> Camera swing. </summary>
  [RequireComponent(typeof(Camera))]
  public sealed class CameraSwing : MonoBehaviour
  {
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 lookAtOffset;

    [SerializeField]
    private Vector3 swingStrength;

    [SerializeField]
    private Vector3 swingVelocity;

    private Camera cam;

    private Vector3 originalPosition;

    private void Awake()
    {
      cam = GetComponent<Camera>();
      originalPosition = cam.transform.position;
    }

    private void Update()
    {
      Vector3 position = originalPosition;
      position.x += Mathf.Sin(Time.time * swingVelocity.x) * swingStrength.x;
      position.y += Mathf.Cos(Time.time * swingVelocity.y) * swingStrength.y;
      position.z += Mathf.Sin(Time.time * swingVelocity.z) * swingStrength.z;

      cam.transform.position = position;

      cam.transform.LookAt(target.position + lookAtOffset);
    }
  }
}
