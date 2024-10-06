using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTextSpawner : MonoBehaviour
{
    public ComboText comboTextPrefab;
    Queue<ComboText> pool = new();
    Dictionary<Creature, ComboText> comboTexts = new();

    private void Start()
    {
        ScoreSystem.Instance.onComboHit += OnComboHit;
        ScoreSystem.Instance.onComboClear += OnComboClear; ;
    }

    private void OnDestroy()
    {
        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.onComboHit -= OnComboHit;
            ScoreSystem.Instance.onComboClear -= OnComboClear;
        }
    }

    private void OnComboHit(Creature root, int count, Vector3 hitPosition)
    {
        if (!comboTexts.ContainsKey(root))
        {
            if (!pool.TryDequeue(out ComboText comboText))
            {
                //Instantiate if missing
                comboText = Instantiate(comboTextPrefab);
            }

            comboTexts.Add(root, comboText);
            comboText.LinkToCombo(root);
            comboText.OnComboHit(root, count, hitPosition);
        }
    }

    private void OnComboClear(Creature root)
    {
        //Back to the pool if any
        if (comboTexts.TryGetValue(root, out ComboText comboText))
        {
            pool.Enqueue(comboText);
            comboTexts.Remove(root);
        }
    }
}
