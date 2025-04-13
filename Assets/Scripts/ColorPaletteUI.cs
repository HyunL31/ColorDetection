using UnityEngine;

public class ColorPaletteUI : MonoBehaviour
{
    public MemoryColor gameManager;

    // Goose
    public void SetOrange()
    {
        Color32 customOrange = new Color32(255, 111, 0, 255);
        gameManager.SetSelectedColor(customOrange);
    }

    public void SetWhite()
    {
        Color32 customWhite = new Color32(255, 255, 255, 255);
        gameManager.SetSelectedColor(customWhite);
    }

    // Horse
    public void SetBrown()
    {
        Color32 customBrown = new Color32(157, 65, 47, 255);
        gameManager.SetSelectedColor(customBrown);
    }

    public void SetDarkBrown()
    {
        Color32 customDarkBrown = new Color32(52, 14, 12, 255);
        gameManager.SetSelectedColor(customDarkBrown);
    }
    public void SetLightYellow()
    {
        Color32 customLightYellow = new Color32(231, 221, 163, 255);
        gameManager.SetSelectedColor(customLightYellow);
    }

    public void SetBlack()
    {
        Color32 customBlack = new Color32(11, 11, 11, 255);
        gameManager.SetSelectedColor(customBlack);
    }

    // burger
    public void SetBread()
    {
        Color32 customBread = new Color32(255, 163, 93, 255);
        gameManager.SetSelectedColor(customBread);
    }

    public void SetPurple()
    {
        Color32 customPurple = new Color32(127, 63, 131, 255);
        gameManager.SetSelectedColor(customPurple);
    }

    public void SetGreen()
    {
        Color32 customGreen = new Color32(102, 161, 50, 255);
        gameManager.SetSelectedColor(customGreen);
    }

    public void SetYellow()
    {
        Color32 customYellow = new Color32(255, 163, 0, 255);
        gameManager.SetSelectedColor(customYellow);
    }

    public void SetRed()
    {
        Color32 customRed = new Color32(208, 29, 12, 255);
        gameManager.SetSelectedColor(customRed);
    }

    public void SetPatty()
    {
        Color32 customRed = new Color32(83, 40, 19, 255);
        gameManager.SetSelectedColor(customRed);
    }
}