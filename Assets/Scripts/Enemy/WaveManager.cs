using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {
    public PlayerUIController puic;

    public List<Dictionary<EnemyType, int>> waveComposition = new List<Dictionary<EnemyType, int>>() {
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 8},
            {EnemyType.fast, 2}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 10},
            {EnemyType.fast, 6},
            {EnemyType.strong, 1}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 20},
            {EnemyType.fast, 6},
            {EnemyType.strong, 2}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 15},
            {EnemyType.fast, 20},
            {EnemyType.strong, 2}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 20},
            {EnemyType.fast, 6},
            {EnemyType.strong, 8}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 20},
            {EnemyType.fast, 6},
            {EnemyType.strong, 2}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 20},
            {EnemyType.fast, 6},
            {EnemyType.strong, 2}
        },
        new Dictionary<EnemyType, int>() {
            {EnemyType.basic, 20},
            {EnemyType.fast, 20},
            {EnemyType.strong, 10}
        },
    };
    private List<EnemyContainer> spawners;
    public float timeBetweenWaves;
    private float waveEndTime;
    private int nActiveSpawners;
    private int wave;
    private bool waveActive;
    private Text nextWaveText;
    private Text enemiesLeftText;
    public bool hasWon;

    void Start() {
        waveEndTime = -timeBetweenWaves + 5;
        wave = 0;
        nActiveSpawners = 0;
        waveActive = false;
        hasWon = false;
        nextWaveText = PlayerUIController.instance.nextWaveText;
        enemiesLeftText = PlayerUIController.instance.enemiesLeftText;
        spawners = new List<EnemyContainer>();
        foreach (Transform child in transform) {
            EnemyContainer spawner = child.gameObject.GetComponent<EnemyContainer>();
            spawners.Add(spawner);
        }
        puic = GameObject.FindWithTag("PlayerUI").GetComponent<PlayerUIController>();
    }

    public void startWave() {
        if (!existsActiveSpawners()) {
            return;
        }
        nActiveSpawners = 0;
        Queue<EnemyType> spawnQueue = generateEnemySequence(wave);
        while (spawnQueue.Count > 0) {
            int i = Random.Range(0, spawners.Count);
            EnemyContainer c = spawners[i];
            if (!c.isActive()) {
                continue;
            }
            c.addToSpawnQueue(spawnQueue.Dequeue());
        }
        foreach (EnemyContainer c in spawners) {
            if (c.isActive()) {
                c.triggerSpawnWave();
            }
        }
    }

    public bool existsActiveSpawners() {
        foreach (EnemyContainer c in spawners) {
            if (c.isActive()) {
                return true;
            }
        }
        return false;
    }

    private void startBetweenWaveDialog() {
        waveEndTime = Time.time;
        waveActive = false;
        wave++;
        if (wave >= waveComposition.Count) {
            hasWon = true;
            puic.win();
            return;
        }
        nextWaveText.gameObject.SetActive(true);
    }

    private void endBetweenWaveDialog() {
        waveActive = true;
        nextWaveText.gameObject.SetActive(false);
    }

    private Queue<EnemyType> generateEnemySequence(int wave) {
        Dictionary<EnemyType, int> comp = new Dictionary<EnemyType, int>(
            waveComposition[wave]);
        List<EnemyType> types = new List<EnemyType>(comp.Keys);
        Queue<EnemyType> q = new Queue<EnemyType>();
        while (types.Count > 0) {
            EnemyType type = types[Random.Range(0, types.Count)];
            q.Enqueue(type);
            comp[type]--;
            if (comp[type] == 0) {
                types.Remove(type);
            }
        }
        return q;
    }

    private int numberofenemiesleft() {
        int n = 0;
        foreach (EnemyContainer c in spawners) {
            n += c.getNEnemies();
        }
        return n;
    }

    void Update() {
        if (hasWon) {
            return;
        }
        if (!waveActive) {
            int t = (int)(timeBetweenWaves - Time.time + waveEndTime);
            nextWaveText.text = "Wave " + (wave + 1) + " in\n" + t + " Seconds";
            if (Time.time > waveEndTime + timeBetweenWaves) {
                endBetweenWaveDialog();
                startWave();
            }
        } else {
            enemiesLeftText.text = numberofenemiesleft().ToString() + " Enemies left";
            bool allEmpty = true;
            foreach (EnemyContainer c in spawners) {
                if (c.getNEnemies() > 0) {
                    allEmpty = false;
                    break;
                }
            }
            if (allEmpty) {
                startBetweenWaveDialog();
            }
        }
    }
}
