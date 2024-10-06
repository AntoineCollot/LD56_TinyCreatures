using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    //Combo
    /// <summary>
    /// Hit, Source
    /// </summary>
    Dictionary<Creature, Creature> hitBy = new();
    Dictionary<Creature, int> comboBy = new();
    public static ScoreSystem Instance;

    //Score
    public int score { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI text;

    //Events
    public delegate void ComboHitEvent(Creature root, int count, Vector3 hitPosition);
    public event ComboHitEvent onComboHit;
    public delegate void ComboEvent(Creature root);
    public event ComboEvent onComboClear;

    private void Awake()
    {
        Instance = this;
    }

    void AddPointsForCombo(Creature root)
    {
        comboBy[root]++;
        score += comboBy[root];

        UpdateScore();
    }

    public void AddHitPoints()
    {
        score++;
        UpdateScore();
    }

    void UpdateScore()
    {
        text.text = score.ToString();
    }

    public void RegisterComboStarter(Creature root)
    {
        if (!comboBy.ContainsKey(root))
            comboBy.Add(root, 0);
    }

    public void RegisterCombo(Creature source, Creature hit, Vector3 hitPos)
    {
        if (hitBy.ContainsKey(hit))
        {
            Debug.Log("Already has " + hit.ToString());
            Debug.Break();
        }
        //register the hit chain
        hitBy.Add(hit, source);
        //Get the combo source creature
        Creature comboSource = GetComboSource(source);
        //Increase combo of combo source
        if (comboBy.ContainsKey(comboSource))
        {
            AddPointsForCombo(comboSource);
            onComboHit.Invoke(comboSource, comboBy[comboSource], hitPos);
        }
    }

    Creature GetComboSource(Creature creature)
    {
        Creature root = creature;
        const int MAX_LOOPS = 100;
        for (int i = 0; i < MAX_LOOPS; i++)
        {
            if (!hitBy.TryGetValue(creature, out creature))
            {
                //If no entry for this creature, it's the root
                break;
            }

            root = creature;
        }
        return root;
    }

    public void ClearCreatureCombo(Creature creature)
    {
        comboBy.Remove(creature);
        hitBy.Remove(creature);
        onComboClear?.Invoke(creature);
    }
}
