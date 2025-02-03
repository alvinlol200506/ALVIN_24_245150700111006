using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Drag your VideoPlayer component here in the inspector
    public float videoDuration = 0.2f; // Duration for the video to play
    public float rotationSpeed = 10f; // Kecepatan rotasi kamera
    public Vector3 rotationCenter = Vector3.zero; // Titik pusat rotasi (0,0,0 secara default)
    public float rotationRadius = 5f; // Jarak kamera dari pusat rotasi

    private float angle = 0f; // Sudut awal

    

    private void Update()
    {
        RotateAroundPoint();
    }

    public void jumpScare()
    {
        StartCoroutine(PlayVideo());
    }

    private IEnumerator PlayVideo()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds(videoDuration);
        videoPlayer.Stop();
    }

    private void RotateAroundPoint()
    {
        // Menambah sudut berdasarkan kecepatan rotasi
        angle += rotationSpeed * Time.deltaTime;

        // Konversi sudut ke radian
        float radians = angle * Mathf.Deg2Rad;

        // Hitung posisi baru kamera di lingkaran mengelilingi rotationCenter
        float x = rotationCenter.x + Mathf.Cos(radians) * rotationRadius;
        float z = rotationCenter.z + Mathf.Sin(radians) * rotationRadius;

        // Tetapkan posisi baru kamera
        transform.position = new Vector3(x, transform.position.y, z);

        // Kamera tetap menghadap ke pusat rotasi
        transform.LookAt(rotationCenter);
    }
}
