using UnityEngine;

public class zaRotatorScript : MonoBehaviour
{
    public GameObject object1;  // Benda pertama
    public GameObject object2;  // Benda kedua
    public GameObject object3;  // Benda ketiga

    public float rotationSpeed = 30f; // Kecepatan rotasi
    private Vector3 randomRotation; // Arah rotasi random
    private float changeInterval = 2f; // Interval perubahan arah rotasi dan arah gerak

    public float moveSpeed = 2f; // Kecepatan pergerakan
    public float minX = -9f, maxX = 9f;
    public float minZ = -6f, maxZ = 4.5f;
    private Vector3[] directions;

    public bool rotate;

    public void StartRotating()
    {
        if (rotate)
        {
            object1.SetActive(true);
            object2.SetActive(true);
            object3.SetActive(true);
        }

        directions = new Vector3[] {
            GetRandomDirection(),
            GetRandomDirection(),
            GetRandomDirection()
        };

        // Pastikan objek yang dimasukkan tidak null
        if (object1 != null) GenerateNewRotation(object1);
        if (object2 != null) GenerateNewRotation(object2);
        if (object3 != null) GenerateNewRotation(object3);

        InvokeRepeating("ChangeDirection", changeInterval, changeInterval);
    }

    void Update()
    {

        if (directions == null) return;

        MoveObject(object1, directions[0]);
        MoveObject(object2, directions[1]);
        MoveObject(object3, directions[2]);

        // Rotasi masing-masing objek
        if (object1 != null) object1.transform.Rotate(randomRotation * rotationSpeed * Time.deltaTime);
        if (object2 != null) object2.transform.Rotate(randomRotation * rotationSpeed * Time.deltaTime);
        if (object3 != null) object3.transform.Rotate(randomRotation * rotationSpeed * Time.deltaTime);
    }

    void MoveObject(GameObject obj, Vector3 direction)
    {
        if (obj == null) return;

        Vector3 pos = obj.transform.position + direction * moveSpeed * Time.deltaTime;
        obj.transform.position = new Vector3(
            Mathf.Clamp(pos.x, minX, maxX),
            pos.y,
            Mathf.Clamp(pos.z, minZ, maxZ)
        );
    }

    void ChangeDirection()
    {
        directions[0] = GetRandomDirection();
        directions[1] = GetRandomDirection();
        directions[2] = GetRandomDirection();
    }

    Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)).normalized;
    }

    void GenerateNewRotation(GameObject obj)
    {
        // Pilih arah rotasi random di sekitar sumbu X, Y, atau Z untuk masing-masing objek
        randomRotation = new Vector3(
            Random.Range(10f, 10f),
            Random.Range(10f, 10f),
            Random.Range(10f, 10f)
        ).normalized; // Pastikan vektor bernilai 1 agar rotasi tetap stabil
    }

    // Method untuk menghilangkan objek
    public void RemoveRotate()
    {
        Destroy(object1); // menghilangkan objek yang berputar
        Destroy(object2);
        Destroy(object3);
    }
}