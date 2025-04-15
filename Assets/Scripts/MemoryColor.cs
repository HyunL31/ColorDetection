using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MemoryColor : MonoBehaviour
{
    [Header("�� ����")]
    public List<GameObject> modelPrefabs;
    private int currentModelIndex = 0;
    private GameObject modelInstance;
    private List<PaintTarget> paintTargets = new();
    private Color[] answerColors;

    [Header("���� ����")]
    public Color[] availableColors;
    private Color selectedColor = Color.white;

    [Header("AR ����")]
    public ARRaycastManager arRaycast;
    public Camera arCamera;
    private List<ARRaycastHit> hits = new();

    private bool modelSpawned = false;
    private bool isColoringPhase = false;

    public GameObject success;
    public GameObject fail;
    public Button retry;
    public Button submit;
    public GameObject colorPanel1;
    public GameObject colorPanel2;
    public GameObject colorPanel3;

    [SerializeField] private ColorDetector colorDetection;

    void Start()
    {
        success.SetActive(false);
        fail.SetActive(false);
        retry.gameObject.SetActive(false);
        colorPanel2.SetActive(false);
        colorPanel3.SetActive(false);

        if (arCamera == null)
            arCamera = Camera.main;

        if (arRaycast == null)
            arRaycast = FindAnyObjectByType<ARRaycastManager>();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
#if UNITY_EDITOR
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (!modelSpawned)
            {
                TrySpawnModel(mousePos);
                return;
            }
            if (isColoringPhase)
            {
                Ray ray = arCamera.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    PaintTarget target = hit.collider.GetComponent<PaintTarget>();
                    if (target != null)
                        target.Paint(selectedColor);
                }
            }
        }
#else
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = touch.position.ReadValue();
            if (!modelSpawned)
            {
                TrySpawnModel(touchPos);
                return;
            }
            if (isColoringPhase)
            {
                Ray ray = arCamera.ScreenPointToRay(touchPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    PaintTarget target = hit.collider.GetComponent<PaintTarget>();
                    if (target != null)
                        target.Paint(selectedColor);
                }
            }
        }
#endif
#endif
    }

    void TrySpawnModel(Vector2 screenPos)
    {
        if (arRaycast.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.05f, 0);
            LoadNextModel(adjustedPosition);
        }
    }

    void LoadNextModel(Vector3 spawnPosition)
    {
        LoadModelAtIndex(currentModelIndex, spawnPosition);
        currentModelIndex++;
    }

    void GenerateAnswerColors()
    {
        answerColors = new Color[paintTargets.Count];

        for (int i = 0; i < paintTargets.Count; i++)
        {
            Color col = paintTargets[i].GetCurrentColor();
            answerColors[i] = col;
            Debug.Log($"���� ���� - ���� {i}: {ColorTo255(col)}");
        }
        /*
        // ã�ƾ��ϴ� �÷�����Ʈ ����!!
        if (colorDetection != null)
        {
            List<Color> answerColorList = new List<Color>(answerColors);
            colorDetection.DetectColorOnDemand(answerColorList);
        }
        */
    }

    string ColorTo255(Color c)
    {
        return $"({Mathf.RoundToInt(c.r * 255)}, {Mathf.RoundToInt(c.g * 255)}, {Mathf.RoundToInt(c.b * 255)})";
    }

    IEnumerator ShowAnswerThenStartGame()
    {
        // ���� ���� ǥ��
        for (int i = 0; i < paintTargets.Count; i++)
        {
            paintTargets[i].Paint(answerColors[i]);
        }

        // 1�� ���
        yield return new WaitForSeconds(1f);

        // ������� �ʱ�ȭ
        foreach (var target in paintTargets)
        {
            target.Paint(Color.white);
        }

        isColoringPhase = true;
        //for end coroutine
        yield break;
    }


    public void SetSelectedColor(Color color)
    {
        selectedColor = color;
    }

    public void CheckAnswer()
    {
        if (!modelSpawned || !isColoringPhase)
            return;

        int correct = 0;
        for (int i = 0; i < paintTargets.Count; i++)
        {
            Color userColor = paintTargets[i].GetCurrentColor();
            Color answerColor = answerColors[i];

            if (ColorsAreSimilar(userColor, answerColor, 0.05f))
                correct++;
        }

        if (correct == paintTargets.Count)
        {
            Debug.Log("����");
            success.SetActive(true);
            isColoringPhase = false;

            // ���� �� �ڵ� �ε� (2�� ��)
            Invoke(nameof(LoadNextFromCurrentPos), 2f);
        }
        else
        {
            Debug.Log($"����. {correct}�� ���� / {paintTargets.Count}");
            fail.SetActive(true);
            retry.gameObject.SetActive(true);
            submit.gameObject.SetActive(false);
            isColoringPhase = false;
        }
    }


    void LoadNextFromCurrentPos()
    {
        success.SetActive(false);
        modelSpawned = false;

        Vector2 centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
        TrySpawnModel(centerScreen);
    }

    bool ColorsAreSimilar(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    public void Retry()
    {
        // �� ����
        if (modelInstance != null)
        {
            Destroy(modelInstance);
            modelInstance = null;
        }

        // UI �ʱ�ȭ
        fail.SetActive(false);
        retry.gameObject.SetActive(false);
        submit.gameObject.SetActive(true);
        success.SetActive(false);

        // ���� �ʱ�ȭ
        modelSpawned = false;
        isColoringPhase = false;

        // �ٽ� ����
        Vector2 centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (arRaycast.Raycast(centerScreen, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.05f, 0);

            LoadModelAtIndex(currentModelIndex - 1, adjustedPosition);
            modelSpawned = true;

            paintTargets.Clear();
            paintTargets.AddRange(modelInstance.GetComponentsInChildren<PaintTarget>());

            if (paintTargets.Count == 0)
            {
                return;
            }

            GenerateAnswerColors();
            StartCoroutine(ShowAnswerThenStartGame());
            StartColorDetect();
        }
    }

    void LoadModelAtIndex(int index, Vector3 spawnPosition)
    {
        if (modelInstance != null)
            Destroy(modelInstance);

        if (index >= modelPrefabs.Count)
            return;

        modelInstance = Instantiate(modelPrefabs[index], spawnPosition, Quaternion.identity);
        modelSpawned = true;

        paintTargets.Clear();
        paintTargets.AddRange(modelInstance.GetComponentsInChildren<PaintTarget>());

        if (paintTargets.Count == 0)
        {
            return;
        }

        GenerateAnswerColors();

        StartCoroutine(ShowAnswerThenStartGame());
        StartColorDetect();
    }

    public void NextColor()
    {
        colorPanel1.SetActive(false);
        colorPanel2.SetActive(true);
        colorPanel3.SetActive(false);
    }

    public void NextColor1()
    {
        colorPanel1.SetActive(false);
        colorPanel2.SetActive(false);
        colorPanel3.SetActive(true);
    }

    public void BackColor1()
    {
        colorPanel1.SetActive(true);
        colorPanel2.SetActive(false);
        colorPanel3.SetActive(false);
    }

    public void BackColor2()
    {
        colorPanel2.SetActive(true);
        colorPanel3.SetActive(false);
        colorPanel1.SetActive(false);
    }

    //for check condition for colordetection and execute it.
    private void StartColorDetect(){
        if (colorDetection != null)
            colorDetection.StartColorDetect(answerColors);
    }
}
