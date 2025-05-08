using UnityEngine;

public class Microfono : MonoBehaviour
{
    private AudioSource audioSource;
    private string dispositivoMicrofono;
    private float volumenActual = 0f; // Aqu� guardaremos el volumen detectado

    void Start()
    {
        // Verificamos que hay al menos un micr�fono conectado
        if (Microphone.devices.Length > 0)
        {
            dispositivoMicrofono = Microphone.devices[0];
            audioSource = GetComponent<AudioSource>();

            // Comenzamos a grabar con bucle, 10 segundos, 44100 Hz
            audioSource.clip = Microphone.Start(dispositivoMicrofono, true, 10, 44100);
            audioSource.loop = true;

            // Esperamos a que comience la grabaci�n
            while (!(Microphone.GetPosition(dispositivoMicrofono) > 0)) { }

            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No hay micr�fonos conectados.");
        }
    }

    void Update()
    {
        volumenActual = CalcularVolumen(); // Actualizamos el volumen en cada frame

        // Puedes usar este valor como condici�n para "hay ruido"
        if (volumenActual > 0.01f)
        {
            Debug.Log("Hay ruido. Volumen: " + volumenActual);
        }
        else
        {
            Debug.Log("Silencio. Volumen: " + volumenActual);
        }
    }

    // Devuelve un flotante representando el volumen actual del micr�fono
    float CalcularVolumen()
    {
        float[] samples = new float[256]; // Tama�o peque�o para an�lisis r�pido
        audioSource.GetOutputData(samples, 0); // Extrae datos del canal izquierdo

        float sum = 0f;

        foreach (float sample in samples)
        {
            sum += sample * sample; // Cuadrado del valor para obtener energ�a
        }

        return Mathf.Sqrt(sum / samples.Length); // RMS (root mean square)
    }

    // M�todo p�blico opcional para que otras clases obtengan el volumen
    public float GetVolumenActual()
    {
        return volumenActual;
    }
}