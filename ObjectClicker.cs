using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
    public Camera cam; // assign in inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.Log("Clicked: " + hit.collider.name);

                // OPTIONAL: change color
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = Color.red;
                }
            }
            else
            {
                Debug.Log("No object picked");
            }
        }
    }
}