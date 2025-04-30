using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

/// <summary>
/// Detects the average color from the AR camera's view in a small central region of the image.
/// Used for color-based gameplay interactions.
/// </summary>
public class ColorDetector : MonoBehaviour
{
    // Reference to the AR camera manager component
    public ARCameraManager cameraManager;

    // Temporary texture to hold camera image data
    private Texture2D cameraTexture;

    // Last detected average color
    private NewColor averageColor = null;

    /// <summary>
    /// Called to detect a color from the center of the current camera frame.
    /// Returns a NewColor object containing the detected average color.
    /// </summary>
    public NewColor DetectColorOnDemand()
    {
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            ProcessCameraImage(image);
            image.Dispose();
            return averageColor;
        }
        else
        {
            Debug.Log("camera error!");
            return null;
        }
    }

    /// <summary>
    /// Converts a central region of the CPU image into a Texture2D,
    /// then calculates the average color of that region.
    /// </summary>
    private void ProcessCameraImage(XRCpuImage image)
    {

        // 이미지의 중앙 부분만 선택
        int centerX = image.width / 2;
        int centerY = image.height / 2;
        int regionWidth = image.width / 10;
        int regionHeight = image.height / 10;

        var inputRect = new RectInt(centerX - regionWidth / 2, centerY - regionHeight / 2, regionWidth, regionHeight);

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = inputRect,
            outputDimensions = new Vector2Int(regionWidth, regionHeight),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.None
        };

        var textureData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
        image.Convert(conversionParams, textureData);

        if (cameraTexture == null || cameraTexture.width != regionWidth || cameraTexture.height != regionHeight)
        {
            cameraTexture = new Texture2D(regionWidth, regionHeight, TextureFormat.RGBA32, false);
        }

        cameraTexture.LoadRawTextureData(textureData);
        cameraTexture.Apply();
        textureData.Dispose();

        averageColor = new NewColor(CalculateAverageColor(cameraTexture),false);
        Debug.Log(averageColor.answerColor.r);
        Debug.Log(averageColor.answerColor.g);
        Debug.Log(averageColor.answerColor.b);
    }

    /// <summary>
    /// Calculates the average color from all pixels in a texture.
    /// </summary>
    Color CalculateAverageColor(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels(); 
        Color sumColor = Color.black;
        int pixelCount = pixels.Length;

        foreach (Color pixel in pixels)
        {
            sumColor += pixel;
        }

        return sumColor / pixelCount;
    }
}
