using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject filePanel;

    private bool cursorVisible = false;

    void Update()
    {
        // Press TAB to show/hide mouse
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorVisible = !cursorVisible;

            if (cursorVisible)
            {
                // SHOW CURSOR
                Cursor.lockState =
                    CursorLockMode.None;

                Cursor.visible = true;

                // Optional UI panel
                if (filePanel != null)
                    filePanel.SetActive(true);
            }
            else
            {
                // HIDE CURSOR
                Cursor.lockState =
                    CursorLockMode.Locked;

                Cursor.visible = false;

                if (filePanel != null)
                    filePanel.SetActive(false);
            }
        }
    }
}