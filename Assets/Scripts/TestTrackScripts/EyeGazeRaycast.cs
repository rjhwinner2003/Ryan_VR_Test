using UnityEngine;
using ViveSR.anipal.Eye; // Importing the namespace for the Vive Eye Tracker SDK.
using System.Text.RegularExpressions; // For using regular expressions.
using System.Collections; // For using coroutines.
using TMPro;

public class EyeGazeRaycast : MonoBehaviour // Defining a new class EyeGazeRaycast, inheriting from MonoBehaviour.
{
    public static Vector3? CurrentGazeHitPoint { get; private set; } = null; // A nullable static property to store the current gaze hit point.
    public float rayLength = 100f; // Public variable to set the length of the ray.
    public Color rayColor = Color.red; // Public variable to set the color of the ray.

    private LineRenderer l_lineRenderer; // Private variable to hold a reference to the LineRenderer component.
    private LineRenderer r_lineRenderer; // Private variable to hold a reference to the LineRenderer component.

    public TextMeshProUGUI output;

    private Ray left_gazeRay, right_gazeRay; // Declaring a Ray variable to store the gaze direction.
    private RaycastHit l_hitInfo, r_hitinfo; // Declaring a RaycastHit variable to store information about what the ray hits.

    private void Awake() // Awake is called when the script instance is being loaded.
    {
        l_lineRenderer = GameObject.Find("LineRenderer1").GetComponent<LineRenderer>();
        r_lineRenderer = GameObject.Find("LineRenderer2").GetComponent<LineRenderer>();
       

        if (l_lineRenderer == null || r_lineRenderer == null) // Checking if the LineRenderer component is not found.
        {
            Debug.LogError("No LineRenderer component found on this game object. Please add one."); // Logging an error message.
            return; // Exiting the method.
        }

        l_lineRenderer.startColor = rayColor; // Setting the start color of the line.
        l_lineRenderer.endColor = rayColor; // Setting the end color of the line.
        l_lineRenderer.positionCount = 2; // Setting the number of positions in the LineRenderer to 2 (start and end points).

        r_lineRenderer.startColor = rayColor; // Setting the start color of the line.
        r_lineRenderer.endColor = rayColor; // Setting the end color of the line.
        r_lineRenderer.positionCount = 2; // Setting the number of positions in the LineRenderer to 2 (start and end points).

        output.gameObject.SetActive(true);
    }

    private void Update() // Update is called once per frame.
    {
        PerformEyeGazeRaycast(); // Calling the method to perform the eye gaze raycast.
    }

    private void PerformEyeGazeRaycast() // Method to perform the eye gaze raycast.
    {

        // Obtaining the combined gaze direction for both eyes and storing it in gazeRay.
        if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out left_gazeRay))
        {
            left_gazeRay.origin = transform.position; // Setting the origin of the gazeRay to the current GameObject's position.

            l_lineRenderer.positionCount = 2; // Ensuring the LineRenderer has two positions.

            // Performing a raycast in the direction of gazeRay.
            if (Physics.Raycast(left_gazeRay, out l_hitInfo))
            {
                CurrentGazeHitPoint = l_hitInfo.point; // Updating the current gaze hit point with the point of collision.
                GameObject hitObject = l_hitInfo.collider.gameObject; // Getting the GameObject that was hit.

                // Checking if the hit object's name matches a specific pattern.
                if (Regex.IsMatch(hitObject.name, @"Cube \(\d+\)"))
                {
                    hitObject.SetActive(false); // Disabling the hit GameObject.
                    StartCoroutine(ActivateAfterDelay(hitObject, 2.0f)); // Starting a coroutine to reactivate the object after a delay.
                }

                // Logging the name and position of the hit object.
                Debug.Log("Gazed at: " + hitObject.name + " at position: " + l_hitInfo.point);

                // Setting the start and end positions of the LineRenderer to visualize the ray.
                l_lineRenderer.SetPosition(0, left_gazeRay.origin);
                l_lineRenderer.SetPosition(1, l_hitInfo.point);
            }
            else
            {
                CurrentGazeHitPoint = null; // Resetting the current gaze hit point if nothing was hit.
                // Drawing the ray for the specified length if no collision is detected.
                l_lineRenderer.SetPosition(0, left_gazeRay.origin);
                l_lineRenderer.SetPosition(1, left_gazeRay.origin + left_gazeRay.direction * rayLength);
            }
        }
        else
        {
            // Hiding the LineRenderer if no valid gaze data is obtained.
            l_lineRenderer.positionCount = 0;
        }

        if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out right_gazeRay))
        {
            right_gazeRay.origin = transform.position; // Setting the origin of the gazeRay to the current GameObject's position.

            r_lineRenderer.positionCount = 2; // Ensuring the LineRenderer has two positions.

            // Performing a raycast in the direction of gazeRay.
            if (Physics.Raycast(right_gazeRay, out r_hitinfo))
            {
                CurrentGazeHitPoint = r_hitinfo.point; // Updating the current gaze hit point with the point of collision.
                GameObject hitObject = r_hitinfo.collider.gameObject; // Getting the GameObject that was hit.

                // Checking if the hit object's name matches a specific pattern.
                if (Regex.IsMatch(hitObject.name, @"Cube \(\d+\)"))
                {
                    hitObject.SetActive(false); // Disabling the hit GameObject.
                    StartCoroutine(ActivateAfterDelay(hitObject, 2.0f)); // Starting a coroutine to reactivate the object after a delay.
                }

                // Logging the name and position of the hit object.
                Debug.Log("Gazed at: " + hitObject.name + " at position: " + r_hitinfo.point);

                // Setting the start and end positions of the LineRenderer to visualize the ray.
                r_lineRenderer.SetPosition(0, right_gazeRay.origin);
                r_lineRenderer.SetPosition(1, r_hitinfo.point);
            }
            else
            {
                CurrentGazeHitPoint = null; // Resetting the current gaze hit point if nothing was hit.
                // Drawing the ray for the specified length if no collision is detected.
                r_lineRenderer.SetPosition(0, right_gazeRay.origin);
                r_lineRenderer.SetPosition(1, right_gazeRay.origin + right_gazeRay.direction * rayLength);
            }
        }
        else
        {
            // Hiding the LineRenderer if no valid gaze data is obtained
            r_lineRenderer.positionCount = 0;
        }

        output.text = Vector3.Angle(l_hitInfo.point, r_hitinfo.point).ToString();

    }

    private IEnumerator ActivateAfterDelay(GameObject obj, float delay) // Coroutine to activate a GameObject after a specified delay.
    {
        yield return new WaitForSeconds(delay); // Waiting for the specified amount of time.
        obj.SetActive(true); // Reactivating the GameObject.
    }
}