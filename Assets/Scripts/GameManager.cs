using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //phases for handle the flow of game
    public enum GamePhase {
        Spawn, //spawning is in progress 
        ColorDetection, //colordetecting is in progress 
        Coloring, //coloring is in progress 
        Evaluation //evaluating is in progress 
    }

    [SerializeField] private ColorManager colorManager;
    [SerializeField] private ModelSpawner modelSpawner;
    [SerializeField] private List<GameObject> prefabs;
    public ARRaycastManager arRaycast;
    private List<ARRaycastHit> hits = new();
    private int prefabIndex = 0;
    private List<NewColor> colors;
    private GameObject presentModel;
    private 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        presentModel = prefabs[0];
    }

    // Update is called once per frame
    void Update()
    {
        //input 관련. 두개의 if문을 통해 phase를 명확하게 구분 필요
    }

    private void SetColors(){
        if(presentModel!=null){
            colors = presentModel.GetComponent<AnswerColorList>().GetAnswerColorList();
        }
    }

    private void SpawnModel(Vector2 screenPos){
        if(prefabs==null){
            return;
        }
        if(prefabIndex<prefabs.Count){
            if (arRaycast.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                Vector3 adjustedPosition = hitPose.position + new Vector3(0, 0.05f, 0);
            }
            //modelSpawner.SpawnModel();
        }
    }

    /*
    여기서는 각 함수의 코드들을 직접실행
    중요 기능 -> 
    -ui관리: ui 오브젝트 활성, 비활성
    -오브젝트 생성: colorMemory에서 해당 기능 받아올 것
    -colordetection 시작: colordetection 시작, 관련 ui는 그쪽에서 알아서 킬것
    여기서는 나머지 ui 비활성화
    -> colordetection 결과는 newColor class들의 리스트로. 이는 생성되는 오브젝트에 저장되어 있을 예정

    */
}
