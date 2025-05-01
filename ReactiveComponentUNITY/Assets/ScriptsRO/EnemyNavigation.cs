using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class EnemyNavigation : MonoBehaviour
{
    public List<Transform> wayPoint;
    Transform playerTransform;
    NavMeshAgent navMeshAgent;
    bool roam = true;
    private AudioSource audioSource;
    private string dispositivoMicrofono;
    private float volumenActual = 0f; // Aquí guardaremos el volumen detectado

    public int index = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;

        // Verificamos que hay al menos un micrófono conectado
        if (Microphone.devices.Length > 0)
        {
            dispositivoMicrofono = Microphone.devices[0];
            audioSource = GetComponent<AudioSource>();

            // Comenzamos a grabar con bucle, 10 segundos, 44100 Hz
            audioSource.clip = Microphone.Start(dispositivoMicrofono, true, 10, 44100);
            audioSource.loop = true;

            // Esperamos a que comience la grabación
            while (!(Microphone.GetPosition(dispositivoMicrofono) > 0)) { }

            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No hay micrófonos conectados.");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            roam = false;
        }
        if (roam)
        {
            Walking();
        }

        volumenActual = CalcularVolumen(); // Actualizamos el volumen en cada frame

        // Puedes usar este valor como condición para "hay ruido"
        if (volumenActual > 0.1f)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            roam = false;
        }
        else
        {
            Debug.Log("Silencio. Volumen: " + volumenActual);
        }

    }

    private void Walking()
    {



        if (wayPoint.Count == 0)
        {

            return;
        }


        float distanceToWaypoint = Vector3.Distance(wayPoint[index].position, transform.position);

        // Check if the agent is close enough to the current waypoint
        if (distanceToWaypoint <= 2)
        {

            index = (index + 1) % wayPoint.Count;
        }

        // Set the destination to the current waypoint
        navMeshAgent.SetDestination(wayPoint[index].position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        roam = true;
    }
    float CalcularVolumen()
    {
        float[] samples = new float[256]; // Tamaño pequeño para análisis rápido
        audioSource.GetOutputData(samples, 0); // Extrae datos del canal izquierdo

        float sum = 0f;

        foreach (float sample in samples)
        {
            sum += sample * sample; // Cuadrado del valor para obtener energía
        }

        return Mathf.Sqrt(sum / samples.Length); // RMS (root mean square)
    }

    // Método público opcional para que otras clases obtengan el volumen
    public float GetVolumenActual()
    {
        return volumenActual;
    }
}
