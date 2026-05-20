using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
public class Shooter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Settings")]
    public float maxDistance = 200f;
    public int damage = 10;

    [Header("Collision")]
    public LayerMask hitLayers;

    [Header("Feedback")]
    public MMFeedbacks shootFeedbacks;

    [Header("Crosshair")]
    public Image crosshair;
    public Color normalColor = Color.white;
    public Color enemyColor = Color.red;

    void Update()
    {
        CheckAim();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();

            if (shootFeedbacks != null)
                shootFeedbacks.PlayFeedbacks();
        }
    }

    public void Shoot()
    {
        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

            Debug.Log("Hit → " + hit.collider.name);

            Damageable target =
                hit.collider.GetComponent<Damageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 1f);
        }
    }

    void CheckAim()
    {
        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            Damageable target =
                hit.collider.GetComponent<Damageable>();

            if (target != null)
            {
                crosshair.color = enemyColor;
                return;
            }
        }

        crosshair.color = normalColor;
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxDistance);
    }
}