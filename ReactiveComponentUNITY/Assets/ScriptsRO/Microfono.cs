using UnityEngine;

public class Microfono : MonoBehaviour
{
    private AudioSource audioSource;
    private string dispositivoMicrofono;
    private float volumenActual = 0f; // Aquí guardaremos el volumen detectado

    void Start()
    {
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

    void Update()
    {
        volumenActual = CalcularVolumen(); // Actualizamos el volumen en cada frame

        // Puedes usar este valor como condición para "hay ruido"
        if (volumenActual > 0.01f)
        {
            Debug.Log("Hay ruido. Volumen: " + volumenActual);
        }
        else
        {
            Debug.Log("Silencio. Volumen: " + volumenActual);
        }
    }

    // Devuelve un flotante representando el volumen actual del micrófono
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