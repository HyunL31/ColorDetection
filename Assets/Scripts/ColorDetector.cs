using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using UnityEngine.Android;

public class ColorDetector : MonoBehaviour
{
    public ARCameraManager cameraManager;
    private Texture2D cameraTexture;
    private NewColor averageColor = null;

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

        // 중앙 부분만 색상 분석
        averageColor = new NewColor(CalculateAverageColor(cameraTexture),false);
        Debug.Log(averageColor.answerColor.r);
        Debug.Log(averageColor.answerColor.g);
        Debug.Log(averageColor.answerColor.b);
    }

    Color CalculateAverageColor(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();  // 텍스처의 모든 픽셀 가져오기
        Color sumColor = Color.black;
        int pixelCount = pixels.Length;

        // 모든 픽셀의 색상 값 더하기
        foreach (Color pixel in pixels)
        {
            sumColor += pixel;
        }

        // 평균 색상 계산
        return sumColor / pixelCount;
    }
    
}
