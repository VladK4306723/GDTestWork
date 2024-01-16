using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;
    public Scrollbar Scrollbar_singleAttack;
    public Scrollbar Scrollbar_heavyAttack;
    public Text CurrentWave_text;
    public Text AmountOfWaves_text;

    private bool isFillingScrollbarSingleAtc = false;
    private bool isFillingScrollbarHeavyAtc = false;
    private float attackSpeed;
    private float heavyAttackSpeed;
    private int currWave = 0;
    [SerializeField] private LevelConfig Config;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
        attackSpeed = Player.AtackSpeed;
        heavyAttackSpeed = Player.HeavyAtackSpeed;
    }

    private void Update()
    {
        Debug.Log(Enemies.Count);

        if (Player.IsSingleAttack && !isFillingScrollbarSingleAtc)
        {
            StartCoroutine(FillScrollbarOverTime(Scrollbar_singleAttack, attackSpeed));
        }

        if (Player.IsHeavyAttack && !isFillingScrollbarHeavyAtc)
        {
            StartCoroutine(FillScrollbarOverTime(Scrollbar_heavyAttack, heavyAttackSpeed));
        }

        CurrentWave_text.text = currWave.ToString();
        AmountOfWaves_text.text = "/ " + Config.Waves.Length;


        if (Enemies.Count == 0) 
        {
            Win.SetActive(true);
        }
        return;
        
    }

    private IEnumerator FillScrollbarOverTime(Scrollbar scrollbar, float duration)
    {
        Image scrollbarImage = scrollbar.GetComponent<Image>();

        if (scrollbarImage == null)
        {
            yield break;
        }

        if (scrollbar == Scrollbar_singleAttack)
        {
            isFillingScrollbarSingleAtc = true;
        }
        else if (scrollbar == Scrollbar_heavyAttack)
        {
            isFillingScrollbarHeavyAtc = true;
        }

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            scrollbarImage.fillAmount = Mathf.Lerp(0f, 1f, progress);
            yield return null;
        }

        scrollbarImage.fillAmount = 1f;

        if (scrollbar == Scrollbar_singleAttack)
        {
            isFillingScrollbarSingleAtc = false;
            Player.IsSingleAttack = false;
        }
        else if (scrollbar == Scrollbar_heavyAttack)
        {
            isFillingScrollbarHeavyAtc = false;
            Player.IsHeavyAttack = false;
        }
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length)
        {
            return;
        }

        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
