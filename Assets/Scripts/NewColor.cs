using UnityEngine;

public class NewColor : MonoBehaviour
{
    [SerializeField] Color color;
    private ColorPaletteUI paletteUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paletteUI = gameObject.GetComponent<ColorPaletteUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
