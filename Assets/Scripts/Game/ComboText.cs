using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;

public class ComboText : MonoBehaviour
{
    Creature root;
    TMP_Text text;
    Color color;

    public AnimationCurve bounceCurve;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void LinkToCombo(Creature root)
    {
        this.root = root;
    }

    private void OnEnable()
    {
        ScoreSystem.Instance.onComboHit += OnComboHit;
    }

    private void OnDisable()
    {
        if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.onComboHit -= OnComboHit;
    }

    public void OnComboHit(Creature root, int count, Vector3 hitPosition)
    {
        if (root != this.root)
            return;

        string message = "Combo";
        if (count > 8)
            message = "StackOverflowError";
        if (count > 7)
            message = "Atchatchatcha";
        if (count > 6)
            message = "Fatality";
        if (count > 5)
            message = "Destruction";
        if (count > 4)
            message = "Wow";
        else if (count > 3)
            message = "Amazing";
        else if (count > 2)
            message = "Well Done";

        message += $" x<font=\"Comica BD\">{count.ToString()}</font>";
        text.text = message;

        transform.position = hitPosition;
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(-20f, 20f));
        color = Color.HSVToRGB(Mathf.Lerp(0.5f,0f,(float)(count) / 10), 0.7f, 0.72f);

        StopAllCoroutines();
        StartCoroutine(BounceScale());
    }

    IEnumerator BounceScale()
    {
        //Bounce scale
        text.color = color;
        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime / 0.3f;
            transform.localScale = Vector3.one * bounceCurve.Evaluate(t);
            yield return null;
        }
        //Fade out
        t = 0;
        Color c = color;
        while (t < 1)
        {
            t += Time.deltaTime / 0.1f;
            c.a = Mathf.Clamp01(1 - t);
            text.color = c;
            yield return null;
        }
    }
}
