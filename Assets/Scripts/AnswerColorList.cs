using System.Collections.Generic;
using UnityEngine;

public class AnswerColorList : MonoBehaviour
{
    [SerializeField] private List<NewColor> answerColorList;

    public List<NewColor> GetAnswerColorList(){
        return answerColorList;
    }

}
