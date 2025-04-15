using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ColorManager colorManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
