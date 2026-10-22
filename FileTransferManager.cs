using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;

public class FileTransferManager : NetworkBehaviour
{
    public static FileTransferManager Instance;

    [Header("UI")]
    public TMP_Text statusText;

    // Chunk size
    private const int CHUNK_SIZE = 8192;

    // Selected target player
    private ulong targetClientId;

    // Store received chunks
    private Dictionary<string, List<byte>> receivedFiles =
        new Dictionary<string, List<byte>>();

    private void Awake()
    {
        Instance = this;
    }

    // ===================================
    // SELECT PLAYER
    // ===================================
    public void SetTargetPlayer(ulong clientId)
    {
        targetClientId = clientId;

        Debug.Log("Selected Player ID: " + clientId);

        if (statusText != null)
        {
            statusText.text =
                "Selected Player: " + clientId;
        }
    }

    // ===================================
    // BUTTON CLICK
    // ===================================
    public void SelectFile()
    {
        // Check target selected
        if (targetClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogError("Cannot send file to yourself!");

            if (statusText != null)
            {
                statusText.text =
                    "Select another player!";
            }

            return;
        }

        StartCoroutine(SelectAndSend());
    }

    // ===================================
    // OPEN FILE PICKER
    // ===================================
    IEnumerator SelectAndSend()
    {
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Select File",
            "Select"
        );

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];

            // Read file
            byte[] fileData =
                File.ReadAllBytes(path);

            // File name
            string fileName =
                Path.GetFileName(path);

            Debug.Log(
                "Sending File: " + fileName
            );

            // Start chunk sending
            StartCoroutine(
                SendFileInChunks(
                    fileName,
                    fileData
                )
            );
        }
    }

    // ===================================
    // SEND FILE IN CHUNKS
    // ===================================
    IEnumerator SendFileInChunks(
        string fileName,
        byte[] fileData)
    {
        int totalChunks =
            Mathf.CeilToInt(
                (float)fileData.Length /
                CHUNK_SIZE
            );

        for (int i = 0; i < totalChunks; i++)
        {
            int offset = i * CHUNK_SIZE;

            int length = Mathf.Min(
                CHUNK_SIZE,
                fileData.Length - offset
            );

            byte[] chunk =
                new byte[length];

            System.Array.Copy(
                fileData,
                offset,
                chunk,
                0,
                length
            );

            SendChunkServerRpc(
                fileName,
                chunk,
                i,
                totalChunks,
                targetClientId
            );

            yield return null;
        }

        Debug.Log("File Send Complete");

        if (statusText != null)
        {
            statusText.text =
                "File Sent Successfully";
        }
    }

    // ===================================
    // CLIENT -> SERVER
    // ===================================
    [ServerRpc(RequireOwnership = false)]
    private void SendChunkServerRpc(
        string fileName,
        byte[] chunk,
        int chunkIndex,
        int totalChunks,
        ulong targetId)
    {
        ClientRpcParams rpcParams =
            new ClientRpcParams
            {
                Send =
                    new ClientRpcSendParams
                    {
                        TargetClientIds =
                            new ulong[]
                            {
                                targetId
                            }
                    }
            };

        ReceiveChunkClientRpc(
            fileName,
            chunk,
            chunkIndex,
            totalChunks,
            rpcParams
        );
    }

    // ===================================
    // SERVER -> TARGET CLIENT
    // ===================================
    [ClientRpc]
    private void ReceiveChunkClientRpc(
        string fileName,
        byte[] chunk,
        int chunkIndex,
        int totalChunks,
        ClientRpcParams rpcParams = default)
    {
        Debug.Log(
            "Received Chunk: " +
            chunkIndex +
            "/" +
            totalChunks
        );

        // Create file storage
        if (!receivedFiles.ContainsKey(fileName))
        {
            receivedFiles[fileName] =
                new List<byte>();
        }

        // Add chunk
        receivedFiles[fileName]
            .AddRange(chunk);

        // Last chunk received
        if (chunkIndex ==
            totalChunks - 1)
        {
            // Desktop path
            string desktopPath =
                System.Environment.GetFolderPath(
                    System.Environment
                    .SpecialFolder.Desktop
                );

            // Final save path
            string savePath =
                Path.Combine(
                    desktopPath,
                    fileName
                );

            // Save file
            File.WriteAllBytes(
                savePath,
                receivedFiles[fileName]
                    .ToArray()
            );

            Debug.Log(
                "Saved File To Desktop: " +
                savePath
            );

            // Open desktop
            Application.OpenURL(
                "file://" + desktopPath
            );

            // UI
            if (statusText != null)
            {
                statusText.text =
                    "Received: " + fileName;
            }

            // Clear memory
            receivedFiles.Remove(
                fileName
            );
        }
    }
}