using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<Creature> availablePrefabs;
    Queue<Creature> pool;
    Queue<float> poolingTimes;
    float SpawnInterval => 1;
    List<Creature> hitCreatures;
    Camera cam;

    int instantiateCount = 0;
    //Don't use the object asap, to let the combo system work before using the creature again
    const float MIN_POOLING_DURATION = 3;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pool = new Queue<Creature>();
        poolingTimes = new Queue<float>();
        hitCreatures = new();
        GameManager.Instance.onGameStart.AddListener(OnGameStart);
        if (GameManager.Instance.gameHasStarted)
            OnGameStart();
    }

    private void OnGameStart()
    {
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        for (int i = hitCreatures.Count - 1; i >= 0; i--)
        {
            if (IsOutOfGameView(hitCreatures[i].transform.position))
            {
                //disable
                hitCreatures[i].Disable();
            }
        }
    }

    IEnumerator SpawnLoop()
    {
        Creature newCreature;
        while (GameManager.Instance.GameIsPlaying)
        {
            yield return new WaitForSeconds(SpawnInterval);

            Vector3 pos = GetRandomPositionAroundScreen();
            if (poolingTimes.TryPeek(out float poolingTime) && Time.time >= poolingTime + MIN_POOLING_DURATION)
            {
                newCreature = pool.Dequeue();
                newCreature.gameObject.SetActive(true);
                poolingTimes.Dequeue();
            }
            else
            {
                newCreature = Instantiate(availablePrefabs[0], transform);
                newCreature.gameObject.name = "Creature_"+ instantiateCount.ToString();
                instantiateCount++;
            }
            newCreature.Init(this);
            newCreature.transform.position = pos;
        }
    }

    public void AddToPool(Creature creature)
    {
        hitCreatures.Remove(creature);
        poolingTimes.Enqueue(Time.time);
        pool.Enqueue(creature);
    }

    const float VIEWPORT_MARGINS = 0.1f;
    Vector3 GetRandomPositionAroundScreen()
    {
        Vector3 posOnViewportEdges = new Vector3();
        float rand = Random.Range(0f, 1f);
        switch (Random.Range(0, 4))
        {
            case 0:
                posOnViewportEdges.y = 1 + VIEWPORT_MARGINS;
                posOnViewportEdges.x = rand;
                break;
            case 1:
                posOnViewportEdges.y = -VIEWPORT_MARGINS;
                posOnViewportEdges.x = rand;
                break;
            case 2:
                posOnViewportEdges.x = 1 + VIEWPORT_MARGINS;
                posOnViewportEdges.y = rand;
                break;
            case 3:
                posOnViewportEdges.x = -VIEWPORT_MARGINS;
                posOnViewportEdges.y = rand;
                break;
        }
        posOnViewportEdges.z = -cam.transform.position.z;
        return cam.ViewportToWorldPoint(posOnViewportEdges);
    }

    public bool IsOutOfGameView(Vector3 pos)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(pos);
        return viewportPos.x < -VIEWPORT_MARGINS || viewportPos.x > 1 + VIEWPORT_MARGINS || viewportPos.y < -VIEWPORT_MARGINS || viewportPos.y > 1 + VIEWPORT_MARGINS;
    }

    public void RegisterHit(Creature creature)
    {
        hitCreatures.Add(creature);
    }
}
