using System.Collections;
using TMPro;
using UnityEngine;

public class HideAfterTime : MonoBehaviour
{

    public float hideTime;

    public float fadeDuration;

    TextMeshProUGUI text;

    float timeElapsed = 0f;

    Color startColor;

    public bool fadeIn;

    float alpha;

    bool isFadeIn;
    bool isFadeOut;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        startColor = text.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            FadeIn();
        }

        if (isFadeOut) 
        {
            FadeOut();
        }

    }

    void FadeIn()
    {
        if (timeElapsed < fadeDuration)
        {

            timeElapsed += Time.deltaTime;
            alpha = Mathf.Lerp(0, 1, timeElapsed / fadeDuration);
            // Update alpha without fetching the component every frame
            Color newColor = startColor;
            newColor.a = alpha;
            text.color = newColor;

        }
        else if(timeElapsed >= fadeDuration)
        {
            timeElapsed = 0;
            isFadeIn = false;
        }
    }

    void FadeOut() 
    {
        if (timeElapsed < fadeDuration)
        {

            timeElapsed += Time.deltaTime;
            alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            // Update alpha without fetching the component every frame
            Color newColor = startColor;
            newColor.a = alpha;
            text.color = newColor;

        }
        else if (timeElapsed >= fadeDuration)
        {
            timeElapsed = 0;
            isFadeOut = false;
        }
    }

    

    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / fadeDuration);

            // Update alpha without fetching the component every frame
            Color newColor = startColor;
            newColor.a = alpha;
            text.color = newColor;

            yield return null; // Wait for the next frame
        }

        // Ensure the final alpha is set correctly
        startColor.a = endAlpha;
        text.color = startColor;
    }

    public void ShowText(bool fadeMode)
    {
        gameObject.SetActive(true);
        fadeIn = fadeMode;
        if (fadeMode)
        {
            isFadeIn = true;
            isFadeOut = false;
        }
        else 
        {
            isFadeIn = false;
            isFadeOut = true;
        }
        //StartCoroutine(FadeText(0, 1));
        StartCoroutine(Hide());
    }

    private void OnEnable()
    {

    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(hideTime);
        fadeIn = false;
        isFadeOut = true;
    }

}
