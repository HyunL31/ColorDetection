using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

/*
 * GameManager.cs
 * 
 * [Summary]
 *  - Core controller that manages the overall game flow.
 *  - Handles transitions between game phases (Spawning, Detection, Coloring, Evaluation).
 *  - Coordinates major systems such as color detection, model spawning, and user input.
 * 
 * [Responsibilities]
 * 1. Controls timing for showing the correct color briefly and resetting the object.
 * 2. Switches to coloring phase when all required colors are found.
 * 3. Handles input during coloring phase (palette selection and object painting).
 * 4. Manages submit and retry logic, including answer checking and success/failure flow.
 * 5. Repeats the cycle for the next object after successful submission.
 * 
 * [Referenced Components]
 *  - ColorManager: Handles color tracking, matching, and state updates.
 *  - ModelSpawner: Responsible for placing and resetting paintable objects.
 *  - UIManager: Updates the user interface based on the current phase.
 * 
 * [Remarks]
 *  - This script acts as the “brain” of the gameplay loop and communicates with almost all major systems.
 *  - Ensure that all event subscriptions and cleanups are properly handled to avoid memory leaks.
 */
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
    [SerializeField] private List<GameObject> prefabs; //prefabs list for spawning
    [SerializeField] private ARRaycastManager arRaycast;
    [SerializeField] private HidePlaneMesh hidePlaneManager; //This manager can control plane detection

    //This empty gameobject is filled with Instatiated model from modelSpawner. If you want to control
    //or access the model already spawned, please use this variable.
    private GameObject modelInScene;
    private List<ARRaycastHit> hits = new();

    //Index of model. modelSpawner instantiate from prefabs with this index, so if you want to spawn next model,
    //please increase this variable.
    private int prefabIndex = 0;
    private GamePhase phase; //present phase
    private bool isModelSpawned = false; //flags
    [SerializeField] private CutsceneManager cm;

    /// <summary>
    /// Set phase, subscribe unityevents.
    /// </summary>
    void Start()
    {
        phase = GamePhase.Menu;
        modelSpawner.OnModelSpawn.AddListener((modelInScene) => AfterModelSpawned(modelInScene));
        colorManager.OnEndColorDetect.AddListener(StartColoring);
    }

    /// <summary>
    /// If phase is spawning, update() check 'plane touching event' and spawn model to the point.
    /// If phase is coloring, update() check 'collider touching event' and change the color of material in the part.
    /// </summary>
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
                        colorManager.Paint(target);
                }
            }
#endif
#endif
        }
    }

    /// <summary>
    /// In main menu, when start button is pressed, this method works.
    /// Change phase to spawning(now, user can spawn model) and start plane detecting.
    /// </summary>
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

    /// <summary>
    /// Spawn model to adjusted position made from screenPos.
    /// </summary>
    /// <param name="screenPos"></param>
    private void SpawnModel(Vector2 screenPos)
    {
        if(prefabs==null){
            return;
        }
        if(prefabIndex<prefabs.Count){
            GameObject presentModel = prefabs[prefabIndex];
            if (arRaycast.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.05f, 0);
                modelSpawner.SpawnModel(adjustedPosition, presentModel);
            }
        }
    }

    /// <summary>
    /// This method is invoked with OnModelSpawn event. Model spawner send thier spawn model
    /// and this method catch it, hide planes, chang phase to colordetection, and start coroutine.
    /// </summary>
    /// <param name="gameObject"></param>
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

    /// <summary>
    /// This coroutine reset model's color to white after 1 second.
    /// Also, make link colormanager and answercolorlist in the model and ready for detecting.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Start color detection. Add goal color UI and turn on whole detecting UI.
    /// </summary>
    private void StartColorDetect()
    {
        Debug.Log("Start Color Detect");
        uIManager.SetColorDetectUI(true);
        colorManager.MakeTargetColorUI();
    }

    /// <summary>
    /// This method connect with OnEndColorDetect event.
    /// Start coloring phase, make coloring plalette UI, turn on coloring UI and turn off detection UI 
    /// </summary>
    private void StartColoring()
    {
        Debug.Log("start coloring phase");
        phase = GamePhase.Coloring;
        uIManager.SetColorDetectUI(false);
        uIManager.SetColoringUI(true);
        uIManager.SetColoringButton(true);
        colorManager.MakeColoringUI();
    }

    /// <summary>
    /// After finish coloring, when user press submit button, this method is worked.
    /// It checks the coloring is right.
    /// 
    /// If all colors are correct, turn on successUI and success sound. Finally, go to nextmodel part.
    /// If that are not correct, turn on FailUI and fail sound. In the scene, Retry button is turned on.
    /// </summary>
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

    /// <summary>
    /// When user press retry button, this method is worked.
    /// Return to coloring part and invoke resetcolor() for show correct color.
    /// </summary>
    public void Retry()
    {
        uIManager.SetFailUI(false);
        colorManager.ShowCorrect();
        StartCoroutine(ResetColor());
    }

    /// <summary>
    /// When user press repose button, this method is invoked. 
    /// Change the model position to the front of camera.
    /// </summary>
    public void RePose()
    {
        float distance = 1.0f;
        Vector3 forward = arCamera.transform.forward;
        forward.Normalize();

        Vector3 spawnPos = arCamera.transform.position + forward * distance;
        spawnPos.y = spawnPos.y - 0.3f;

        modelInScene.transform.position = spawnPos;
    }

    /// <summary>
    /// This coroutine is invoked if subit is success. Reset all variable about current model
    /// and go to spawn phase.
    /// If current model is last, it turns on End UI.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// When changing model and back to main menu, the variables about specific model need to be
    /// deleted. 
    /// So this method reset planes, destroy current model, 
    /// change flag and invoke colormanager's reset method.
    /// </summary>
    private void ResetAll()
    {
        hidePlaneManager.ResetPlane();
        Destroy(modelInScene);
        isModelSpawned = false;
        colorManager.ResetColorManager();
    }

    /// <summary>
    /// When back to main menu(user press main menu button), this method is invoked.
    /// First reset data, chang phase to menu(first phase), 
    /// reset model index and set ui to default setting.
    /// </summary>
    public void Restart(){
        ResetAll();
        phase = GamePhase.Menu;
        prefabIndex = 0;
        uIManager.ReturnStart();
        uIManager.SetColorDetectUI(false);
        uIManager.SetSuccssUI(false);
        uIManager.SetFailUI(false);
        uIManager.SetColoringUI(false);
        hidePlaneManager.ResetPlane();
    }
}
