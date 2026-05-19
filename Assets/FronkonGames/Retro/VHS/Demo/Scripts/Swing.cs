using UnityEngine;

namespace FronkonGames.Retro.VHS
{
  /// <summary> Object swing. </summary>
  /// <remarks> This code is designed for a simple demo, not for production environments. </remarks>
  public sealed class Swing : MonoBehaviour
  {
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 lookAtOffset;

    [SerializeField]
    private Vector3 swingStrength;

    [SerializeField]
    private Vector3 swingVelocity;

    private Vector3 originalPosition;

    private void Awake()
    {
      originalPosition = GetComponent<Camera>().transform.position;
    }

    private void Update()
    {
      Vector3 position = originalPosition;
      position.x += Mathf.Sin(Time.time * swingVelocity.x) * swingStrength.x;
      position.y += Mathf.Cos(Time.time * swingVelocity.y) * swingStrength.y;
      position.z += Mathf.Sin(Time.time * swingVelocity.z) * swingStrength.z;
      
      this.transform.position = position;

      if (target != null)
        this.transform.LookAt(target.position + lookAtOffset);
    }
  }  
}
