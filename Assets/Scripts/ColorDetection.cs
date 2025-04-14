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
    public float colorTolerance = 0.2f;  // 색상 허용 오차 범위
    [SerializeField] private GameObject targetColorListUI;
    [SerializeField] private GameObject referenceUI;
    [SerializeField] private GameObject centerUI;
    [SerializeField] private ColorPaletteUI colorPaletteUI;
    private Texture2D cameraTexture;
    private List<TargetColor> targetColorList = new List<TargetColor>();  // 감지하려는 목표 색상
    private int findColorNum = 0;

    public void StartColorDetect(Color[] colorList){
        if(centerUI!=null && targetColorListUI!=null && referenceUI!=null){
            MakeTargetColorList(targetColorList,colorList);
            targetColorListUI.SetActive(true);
            centerUI.SetActive(true);
        }      
    }

    private void FinishColorDetect(){
            targetColorListUI.SetActive(false);
            centerUI.SetActive(false);
    }

    public void DetectColorOnDemand()
    {
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            ProcessCameraImage(image);
            image.Dispose();
        }
        else
        {
            Debug.Log("camera error!");
        }
    }

    void ProcessCameraImage(XRCpuImage image)
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
        Color averageColor = CalculateAverageColor(cameraTexture);
        Debug.Log(averageColor.r);
        Debug.Log(averageColor.g);
        Debug.Log(averageColor.b);
        if (IsColorMatch(averageColor, targetColorList))
        {
            //Finish Color Detection
            FinishColorDetect();
        }
        else
        {
            //Debug.Log("please check right color");
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

    bool IsColorMatch(Color averageColor, List<TargetColor> targetColorList)
    {
        for(int i = 0 ; i<targetColorList.Count ; i++)
        {
            if(!targetColorList[i].isDetected){
                float rDiff = Mathf.Abs(averageColor.r - targetColorList[i].color.r);
                float gDiff = Mathf.Abs(averageColor.g - targetColorList[i].color.g);
                float bDiff = Mathf.Abs(averageColor.b - targetColorList[i].color.b);
                
                //success color detect in list.
                if (rDiff <= colorTolerance && gDiff <= colorTolerance && bDiff <= colorTolerance)
                {
                    TargetColor tc = targetColorList[i];
                    tc.isDetected = true;
                    targetColorList[i] = tc;
                    findColorNum++;
                    break;
                }
                else Debug.Log("please check right color");
            }
        }
        if(findColorNum==targetColorList.Count)
            return true;
        else return false;
    }

    public struct TargetColor{
        public bool isDetected;
        public Color color;
    }

    private void MakeTargetColorList(List<TargetColor> target, Color[] colors){
        int posx = -375;
        int posy = -10;
        foreach(Color color in colors){
            TargetColor tc;
            tc.isDetected = false;
            tc.color = color;
            target.Add(tc);
            GameObject go = Instantiate(referenceUI,targetColorListUI.transform);
            go.GetComponent<Image>().color = color;
            go.transform.localPosition = new Vector3(posx,posy,0);
            if(posx!=375){
                posx+=150;
            }
            else{
                posx = -375;
                posy-= 140;
            }
        }
    }
    
}
