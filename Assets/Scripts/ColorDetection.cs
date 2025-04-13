using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using UnityEngine.Android;

public class ColorDetection : MonoBehaviour
{
    public ARCameraManager cameraManager;
    private Texture2D cameraTexture;
    public float colorTolerance = 0.1f;  // 색상 허용 오차 범위
    private List<Color> targetColorList;  // 감지하려는 목표 색상

    public bool DetectColorOnDemand(List<Color> colorList)
    {
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            targetColorList = colorList;
            bool result = ProcessCameraImage(image);
            image.Dispose();
            return result;
        }
        else
        {
            Debug.Log("camera error!");
            return false;
        }
    }

    bool ProcessCameraImage(XRCpuImage image)
    {

        // 이미지의 중앙 부분만 선택
        int centerX = image.width / 2;
        int centerY = image.height / 2;
        int regionWidth = image.width / 10;
        int regionHeight = image.height / 10;

        Debug.Log(image.width);

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
        Color averageColor = CalculateAverageColor(cameraTexture);
        if (IsColorMatch(averageColor, targetColorList))
        {
            return true;
        }
        else
        {
            Debug.Log("please check right color");
            return false;
        }
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

    bool IsColorMatch(Color averageColor, List<Color> targetColorList)
    {
        foreach(var targetColor in targetColorList)
        {
            float rDiff = Mathf.Abs(averageColor.r - targetColor.r);
            float gDiff = Mathf.Abs(averageColor.g - targetColor.g);
            float bDiff = Mathf.Abs(averageColor.b - targetColor.b);
            if (rDiff <= colorTolerance && gDiff <= colorTolerance && bDiff <= colorTolerance)
            {
                return true;
            }
        }
        return false;
    }
}
