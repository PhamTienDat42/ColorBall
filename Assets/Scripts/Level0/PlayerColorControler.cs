using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerColorControler : MonoBehaviour
{
    public GameObject player;
    public GameObject ballPrefab;
    public int numBalls = 3;
    public float spawnRange = 10f;
    public Collider floorCollider;

    private Color[] ballColors = { Color.red, Color.green, Color.blue };
    private Color playerColor;
    private Color eatenBallColor;
    private int score = 0;
    [SerializeField] private Text scoreText;
    public GameObject panelWin;
    public GameObject panelLose;
    public AudioSource deathSound;
    public AudioSource collectSound;
    public AudioSource winSound;


    private void Start()
    {
       
        playerColor = GetRandomColor();
        player.GetComponent<MeshRenderer>().material.color = playerColor;
        player.GetComponent<CapsuleCollider>().enabled = true;
    
        StartCoroutine(SpawnBallsWithDelay());
    }

    private void Update()
    {
        if (transform.position.y < floorCollider.bounds.min.y)
        {
            Debug.Log("Player died!");
            panelLose.SetActive(true);          
        }
    }

    private System.Collections.IEnumerator SpawnBallsWithDelay()
    {
        for (int i = 0; i < numBalls; i++)
        {
            yield return new WaitForSeconds(1f); 
            SpawnBall(i);
        }
    }

    private void SpawnBall(int index)
    {
       
        Color ballColor = ballColors[index];

       
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 0.5f, Random.Range(-spawnRange, spawnRange));
        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        ball.GetComponent<MeshRenderer>().material.color = ballColor;
        ball.GetComponent<SphereCollider>().enabled = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1f;
    }

    private System.Collections.IEnumerator RespawnBallWithDelay(Color color)
    {
        yield return new WaitForSeconds(1f); // Delay for 1 second
        SpawnBall(GetBallColorIndex(color));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            GameObject ball = collision.gameObject;
            Color ballColor = ball.GetComponent<MeshRenderer>().material.color;

            if (ballColor == playerColor)
            {
                collectSound.Play();
                score++;
                Debug.Log("Score: " + score);
                scoreText.text = "Score: " + score;      
                if(score >= 10)
                {
                    winSound.Play();
                    panelWin.SetActive(true);
                    Time.timeScale = 0f;
                }
            }          
            else
            {
                deathSound.Play();
                Debug.Log("Player died!");         
                panelLose.SetActive(true);
                Time.timeScale = 0f;
            }

            eatenBallColor = ballColor;
            Destroy(ball);

            
            playerColor = GetRandomColor();
            player.GetComponent<MeshRenderer>().material.color = playerColor;
            player.GetComponent<CapsuleCollider>().enabled = true;


            StartCoroutine(RespawnBallWithDelay(eatenBallColor));
        }
    }
    
    private Color GetRandomColor()
    {
        return ballColors[Random.Range(0, ballColors.Length)];
    }

    private int GetBallColorIndex(Color color)
    {
        for (int i = 0; i < ballColors.Length; i++)
        {
            if (ballColors[i] == color)
            {
                return i;
            }
        }
        return 0;
    }
}