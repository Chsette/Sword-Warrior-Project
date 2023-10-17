using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    //variavel que comporte mais de uma coisa dentro dela, por ex uma q se possa ter o 5 e 6 nela(uma array)
    // e para configurar para que um tipo vire array é so colocar [] apos designar qual é o tipo 
    public GameObject[] objSpawner;

    // pontos de spawn
    public Transform[] spawnPoint;

    //cronometro 
    public float timeSpawn;
    public float currentTime;

    void Start()
    {

    }


    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            //criando variaveis para que escolham tanto um obj aleatorio quanto um spawn
            int randomObj = Random.Range(0, objSpawner.Length);
            int randomPoint = Random.Range(0, spawnPoint.Length);

            //spaner dos obj nos pontos
            Instantiate(objSpawner[randomObj], spawnPoint[randomPoint].position, spawnPoint[randomPoint].rotation);
            currentTime = timeSpawn;
        }
    }
}
