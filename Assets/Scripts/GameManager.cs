using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //phases for handle the flow of game
    public enum GamePhase {
        Menu,
        Spawn, //spawning is in progress 
        ColorDetection, //colordetecting is in progress 
        Coloring, //coloring is in progress 
        Evaluation //evaluating is in progress 
    }

    [SerializeField] private Camera arCamera;
    [SerializeField] private ColorManager colorManager;
    [SerializeField] private ModelSpawner modelSpawner;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private ARRaycastManager arRaycast;
    [SerializeField] private HidePlaneMesh hidePlaneManager;
    private GameObject modelInScene;
    private List<ARRaycastHit> hits = new();
    private int prefabIndex = 0;
    private GameObject presentModel;
    //present phase
    private GamePhase phase;
    //flags
    private bool isModelSpawned = false;
    [SerializeField] private CutsceneManager cm;

    void Start()
    {
        phase = GamePhase.Menu;
        modelSpawner.OnModelSpawn.AddListener((modelInScene) => AfterModelSpawned(modelInScene));
        colorManager.OnEndColorDetect.AddListener(StartColoring);
        Debug.Log(phase);
    }

    void Update()
    {
        if(phase==GamePhase.Spawn)
        {
#if ENABLE_INPUT_SYSTEM
#if UNITY_EDITOR
            if (Mouse.current.leftButton.wasPressedThisFrame && !isModelSpawned){
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if(!uIManager.IsOverUI(mousePos))
                    SpawnModel(mousePos);
            }
#else
            if (Touchscreen.current == null) return;

            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame && !isModelSpawned)
            {
                Vector2 touchPos = touch.position.ReadValue();
                if(!uIManager.IsOverUI(touchPos))
                    SpawnModel(touchPos);
            }
#endif
#endif
        }

        if(phase==GamePhase.Coloring)
        {
#if ENABLE_INPUT_SYSTEM
#if UNITY_EDITOR

#else
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = touch.position.ReadValue();
                Ray ray = arCamera.ScreenPointToRay(touchPos);
                if (!uIManager.IsOverUI(touchPos) && Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject target = hit.collider.gameObject;
                    if (target != null)
                        PaintToPart(target);
                }
            }
#endif
#endif
        }
    }

    public void StartGame()
    {
        if (cm.isCheck())
        {
            cm.gameObject.SetActive(true);
            SoundManager.Instance.CutScene();
        } else
        {
            SoundManager.Instance.Tutorial();
        }
        phase = GamePhase.Spawn;
        hidePlaneManager.ShowAllPlanes();
    }

    private void PaintToPart(GameObject gameObject)
    {
        colorManager.Paint(gameObject);
    }

    private void SpawnModel(Vector2 screenPos)
    {
        if(prefabs==null){
            return;
        }
        if(prefabIndex<prefabs.Count){
            presentModel = prefabs[prefabIndex];
            if (arRaycast.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.05f, 0);
                modelSpawner.SpawnModel(adjustedPosition, presentModel);
            }
        }
    }

    private void AfterModelSpawned(GameObject gameObject)
    {
        Debug.Log("model spawned");
        modelInScene = gameObject;
        isModelSpawned = true;
        //hidePlaneManager.ResetPlane();
        hidePlaneManager.HideAllPlanes();
        phase = GamePhase.ColorDetection;
        StartCoroutine(ResetColor());
    }

    private IEnumerator ResetColor()
    {
        colorManager.SetAnswerColorList(modelInScene);
        colorManager.SetHaveToDetectList();
        yield return new WaitForSeconds(1f);
        Debug.Log("reset color");
        colorManager.SetWhite();
        if(phase==GamePhase.ColorDetection)
            StartColorDetect();
        else if(phase==GamePhase.Evaluation)
        {
            phase = GamePhase.Coloring;
            uIManager.SetColoringButton(true);
        }
        yield break;
    }

    private void StartColorDetect()
    {
        Debug.Log("Start Color Detect");
        uIManager.SetColorDetectUI(true);
        colorManager.MakeTargetColorUI();
    }

    private void StartColoring()
    {
        Debug.Log("start coloring phase");
        phase = GamePhase.Coloring;
        uIManager.SetColorDetectUI(false);
        uIManager.SetColoringUI(true);
        uIManager.SetColoringButton(true);
        colorManager.MakeColoringUI();
    }

    public void Submit()
    {
        phase = GamePhase.Evaluation;
        if(colorManager.CheckCorrected())
        {
            uIManager.SetSuccssUI(true);
            uIManager.SetColoringUI(false);
            SoundManager.Instance.Success();
            StartCoroutine(ToNextModel());
        }
        else
        {
            uIManager.SetFailUI(true);
            uIManager.SetColoringButton(false);
            SoundManager.Instance.Fail();
        }
    }

    public void Retry()
    {
        uIManager.SetFailUI(false);
        colorManager.ShowCorrect();
        StartCoroutine(ResetColor());
    }

    public void RePose()
    {
        float distance = 1.0f;
        Vector3 forward = arCamera.transform.forward;
        forward.Normalize();

        Vector3 spawnPos = arCamera.transform.position + forward * distance;
        spawnPos.y = spawnPos.y - 0.3f;

        modelInScene.transform.position = spawnPos;
    }

    private IEnumerator ToNextModel()
    {
        yield return new WaitForSeconds(2f);
        uIManager.SetSuccssUI(false);
        prefabIndex++;
        ResetAll();

        if (prefabIndex < prefabs.Count)
        {
            phase = GamePhase.Spawn;
            hidePlaneManager.ShowAllPlanes();
        } else
        {
            uIManager.End();
            prefabIndex = 0;
        }
            yield break;
    }

    private void ResetAll()
    {
        hidePlaneManager.ResetPlane();
        Destroy(modelInScene);
        isModelSpawned = false;
        colorManager.ResetColorManager();
    }

    public void Restart(){
        ResetAll();
        phase = GamePhase.Menu;
        prefabIndex = 0;
        uIManager.ReturnStart();
        uIManager.SetColorDetectUI(false);
        uIManager.SetColoringUI(false);
        hidePlaneManager.ResetPlane();
    }
}
