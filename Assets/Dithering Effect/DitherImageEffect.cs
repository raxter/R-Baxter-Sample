using UnityEngine;
using System.Collections;

public class DitherImageEffect : MonoBehaviour
{
    public Material mat;
    public bool offsetByPosition = true;

    // TODO make a copy of the aterial rather since we are editing values

        
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (offsetByPosition)
        {
            float aspectRatio = (float)Screen.width / Screen.height;
            mat.SetFloat("_CameraPositionX", transform.position.x );
            mat.SetFloat("_CameraPositionY", transform.position.y);
            mat.SetFloat("_CameraSizeX", GetComponent<Camera>().orthographicSize * aspectRatio);
            mat.SetFloat("_CameraSizeY", GetComponent<Camera>().orthographicSize);
        }
        Graphics.Blit(src, dest, mat);
    }
}