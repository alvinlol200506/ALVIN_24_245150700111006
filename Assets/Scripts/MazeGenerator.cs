using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] mazeNode nodePrefab;
    [SerializeField] mazeNode nodeNoWallPrefab;
    [SerializeField] Vector2Int mazeSize;

    public PlayerSpawner player; // menyinggung script PlayerGameplay
    public zaRotatorScript rotate; // menyinggung script zaRotationScript
    public CanvasScript canvas;
    public TMP_Text countdownText; // Referensi ke TextMeshPro 
    public GameObject startButton; // tombol start di awal game
    public Material targetMaterial; // material untuk warna maze
    public float countdownTime = 30f; // Waktu hitung mundur
    private bool isCountingDown = false; // Status hitung mundur
    public float greenTimer;
    public float yellowTimer;
    public float redTimer;
    public bool automaticGenerator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpawner>(); // memanggil kode dari script PlayerGameplay
        rotate = GameObject.FindGameObjectWithTag("Rotate").GetComponent<zaRotatorScript>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>();
        // StartCoroutine(GenerateMaze(mazeSize)); // adalah code untuk melihat random maze generatornya bekerja 
        ShowPrefab(targetMaterial);

        if (automaticGenerator)
        {
            StartCoroutine(GenerateMaze(mazeSize));
        }
    }



    private void ShowPrefab(Material material) // membuat material tampak mata
    {
        Color color = material.color;
        color.a = 1f;
        material.color = color;
    }


    public void HidePrefab(Material material) // membuat material terawang
    {
        StartCoroutine(FadeOutMaterial(material, 1f)); // Durasi fading 1 detik
    }


    private IEnumerator FadeOutMaterial(Material material, float fadeDuration)
    {
        Color color = material.color;
        float startAlpha = color.a; // Ambil transparansi awal (1.0)
        float elapsedTime = 0f; // Waktu yang sudah berlalu

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration); // Lerp dari 1 ke 0
            color.a = newAlpha;
            material.color = color;

            yield return null; // Tunggu frame berikutnya
        }

        color.a = 0f; // Pastikan alpha benar-benar 0 di akhir
        material.color = color;
    }


    public void onButtonClicked() // ketika tombol start dipencet
    {

        GenerateMazeInstant(mazeSize); // mengaktifkan random maze generator
        canvas.StartHeart();

        if (!isCountingDown) // Cegah pengulangan hitung mundur jika sedang berjalan
        {
            isCountingDown = true;
            StartCoroutine(StartCountdown());
        }
    }

    private System.Collections.IEnumerator StartCountdown() // hitung mundur
    {
        float currentTime = countdownTime; // Waktu hitung mundur dalam detik
        float endTime = Time.time + currentTime; // Waktu akhir countdown

        while (currentTime >= 0)
        {
            DestroyButton();
            rotate.StartRotating();

            if (currentTime > greenTimer)
                countdownText.color = Color.green;
            else if (currentTime > yellowTimer)
                countdownText.color = Color.yellow;
            else
                countdownText.color = Color.red;

            if (currentTime <= redTimer)
                StartCoroutine(ShakeText());

            // Hitung waktu tersisa dalam detik dan milidetik
            currentTime = endTime - Time.time;

            // Hitung detik dan milidetik
            int seconds = Mathf.FloorToInt(currentTime); // Detik
            float milliseconds = (currentTime % 1) * 100; // Milidetik (sisa waktu)

            // Update tampilan hitung mundur di UI dengan format detik dan milidetik
            countdownText.text = string.Format("{0:00}:{1:00}", seconds, Mathf.FloorToInt(milliseconds));

            // Tunggu sedikit sebelum melanjutkan ke frame berikutnya
            yield return null; // Menunggu hingga frame berikutnya
        }

        countdownText.text = "Start!"; // Menampilkan "Start!" setelah countdown selesai
        isCountingDown = false; // Reset status
        onCountDownFinished(mazeSize);
    }


    private IEnumerator ShakeText() // method untuk memberikan efek getar pada detik detik akhir
    {
        Vector3 originalPosition = countdownText.transform.localPosition;

        float duration = 0.1f; // Durasi getaran per frame
        float magnitude = 10f; // Kekuatan getaran

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-magnitude, magnitude) * 0.1f;
            float yOffset = Random.Range(-magnitude, magnitude) * 0.1f;

            countdownText.transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localPosition = originalPosition;

    }

        void onCountDownFinished(Vector2Int size)
        {
        HidePrefab(targetMaterial);
        player.StartPlayer();
        rotate.RemoveRotate();

        List<mazeNode> nodesNoWall = new List<mazeNode>();
        // create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.x; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                mazeNode newNode = Instantiate(nodeNoWallPrefab, nodePos, Quaternion.identity, transform);
                nodesNoWall.Add(newNode);
            }
        }
    }

    private void DestroyButton()
    {
        if (startButton != null)
        {
            Destroy(startButton); // Hancurkan tombol
            Debug.Log("Start button has been destroyed.");
        }
    }

    void GenerateMazeInstant(Vector2Int size)
    {
        List<mazeNode> nodes = new List<mazeNode>();

        // create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.x; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                mazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }
        }

        List<mazeNode> currentPath = new List<mazeNode>();
        List<mazeNode> completedNodes = new List<mazeNode>();

        // randomly choose starting node
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);

        while (completedNodes.Count < nodes.Count)
        {
            // check nodes next to the current node
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / size.y;
            int currentNodeY = currentNodeIndex % size.y;

            // check if there wall
            if (currentNodeX < size.x - 1)
            {
                // check node to the right of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                // check node to the left of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            if (currentNodeY < size.y - 1)
            {
                // check node to the above of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            if (currentNodeY > 0)
            {
                // check node to the below of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // choose next nodes that available
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                mazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1: // menghancurkan tembok kiri
                        chosenNode.RemoveWall(1);
                        currentPath[currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2: // menghancurkan tembok kanan
                        chosenNode.RemoveWall(0);
                        currentPath[currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3: // menghancurkan tembok bawah
                        chosenNode.RemoveWall(3);
                        currentPath[currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4: // menghancurkan tembok atas
                        chosenNode.RemoveWall(2);
                        currentPath[currentPath.Count - 1].RemoveWall(3);
                        break;
                }

                currentPath.Add(chosenNode);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);

                currentPath[currentPath.Count - 1].SetState(NodeState.Completed);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }

    /*
     DIBAWAH INI ADALAH PROGRAM RANDOM MAZE GENERATOR DIMANA BERJALAN LAMBAT AGAR BISA DILIHAT MATA
     BISA DIAKTIFKAN DENGAN UNCOMMENT STARTCOURUTINE DI PRIVATE VOID START 
     */


    IEnumerator GenerateMaze(Vector2Int size)
    {
        List<mazeNode> nodes = new List<mazeNode>();

        // create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.x; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                mazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);

                yield return null;
            }
        }

        List<mazeNode> currentPath = new List<mazeNode>();
        List<mazeNode> completedNodes = new List<mazeNode>();

        // randomly choose starting node
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            // check nodes next to the current node
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / size.y;
            int currentNodeY = currentNodeIndex % size.y;


                if (currentNodeX < size.x - 1)
                {
                    // check node to the right of the current node
                    if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                        !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                    {
                        possibleDirections.Add(1);
                        possibleNextNodes.Add(currentNodeIndex + size.y);
                    }
                }
                if (currentNodeX > 0)
                {
                    // check node to the left of the current node
                    if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                        !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                    {
                        possibleDirections.Add(2);
                        possibleNextNodes.Add(currentNodeIndex - size.y);
                    }
                }
                if (currentNodeY < size.y - 1)
                {
                    // check node to the above of the current node
                    if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                        !currentPath.Contains(nodes[currentNodeIndex + 1]))
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(currentNodeIndex + 1);
                    }
                }
                if (currentNodeY > 0)
                {
                    // check node to the below of the current node
                    if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                        !currentPath.Contains(nodes[currentNodeIndex - 1]))
                    {
                        possibleDirections.Add(4);
                        possibleNextNodes.Add(currentNodeIndex - 1);
                    }
                }

                // choose next nodes that available
                if (possibleDirections.Count > 0)
                {
                    int chosenDirection = Random.Range(0, possibleDirections.Count);
                    mazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];

                    switch (possibleDirections[chosenDirection])
                    {
                        case 1: // menghancurkan tembok kiri
                            chosenNode.RemoveWall(1);
                            currentPath[currentPath.Count - 1].RemoveWall(0);
                            break;
                        case 2: // menghancurkan tembok kanan
                            chosenNode.RemoveWall(0);
                            currentPath[currentPath.Count - 1].RemoveWall(1);
                            break;
                        case 3: // menghancurkan tembok bawah
                            chosenNode.RemoveWall(3);
                            currentPath[currentPath.Count - 1].RemoveWall(2);
                            break;
                        case 4: // menghancurkan tembok atas
                            chosenNode.RemoveWall(2);
                            currentPath[currentPath.Count - 1].RemoveWall(3);
                            break;
                    }

                    currentPath.Add(chosenNode);
                    chosenNode.SetState(NodeState.Current);
                }
                else
                {
                    completedNodes.Add(currentPath[currentPath.Count - 1]);

                    currentPath[currentPath.Count - 1].SetState(NodeState.Completed);
                    currentPath.RemoveAt(currentPath.Count - 1);
                }

                yield return new WaitForSeconds(0.05f);

            
            // check if there wall
            
        }
    }
}
