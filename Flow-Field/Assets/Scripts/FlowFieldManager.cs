using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    private float screenHeight;
    private float screenWidth;

    public GameObject arrowPrefab;
    public GameObject particlePrefab;
    public GameObject cellPrefab;

    public int rows = 20;
    public int cols = 20;
    public int pCount = 10000;

    private float margin = 0f;
    private float height;
    private float width;
    private GameObject[,] arrows;
    private GameObject[,] cells;
    private GameObject[] particles;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started the FlowField.");
        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;

        height = (screenHeight - margin) / rows - margin;
        width = (screenWidth - margin) / cols - margin;

        arrows = new GameObject[cols, rows];
        //cells = new GameObject[cols, rows];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            { 
                //cells[i, j] = Instantiate(cellPrefab);
                //cells[i, j].transform.position = GridToPos(i, j) - new Vector3(screenWidth / 2, screenHeight / 2, 0);
                //cells[i, j].transform.localScale = new Vector3(width, height, 1);
                arrows[i, j] = Instantiate(arrowPrefab);
                arrows[i, j].transform.position = GridToPos(i, j) - new Vector3(screenWidth / 2, screenHeight / 2, 0);
                arrows[i, j].transform.localScale = new Vector3(width, height, 1) * 0.01f;
                // Add Perlin noise-based rotation
                float noise = Mathf.PerlinNoise(i * 0.1f, j * 0.1f);  // Using 0.1f as scale factor
                float angle = noise * 360f;  // Mapping noise [0, 1] to angle [0, 360]
                arrows[i, j].transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }

        particles = new GameObject[pCount];
        for(int i = 0; i < pCount; i++)
        {
            particles[i] = Instantiate(particlePrefab);
            Particle p = particles[i].GetComponent<Particle>();
            if (p != null)
            {
                Vector2 rangeW = new Vector2(-screenWidth/2, screenWidth/2);
                Vector2 rangeH = new Vector2(-screenHeight/2, screenHeight/2);
                p.SetRandPos(rangeW, rangeH);
                particles[i].transform.position = new Vector3(p.pos.x, p.pos.y, 0);
                //particles[i].transform.localScale = new Vector3(1,1,1);
            }
        }
    }


    private Vector3 GridToPos(int column, int row)
    {
        float x = (margin + width) * column + margin + width / 2;
        float y = (margin + height) * row + margin + height / 2;
        return new Vector3(x, y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // Existing code for Perlin noise-based rotation
                float noise = Mathf.PerlinNoise(i * 0.1f + Time.time * 0.5f, j * 0.1f + Time.time * 0.5f);
                float angle = noise * 360f;
                arrows[i, j].transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }

        foreach (GameObject particle in particles)
        {
            Particle p = particle.GetComponent<Particle>();
            if (p != null)
            {
                Debug.Log($"{p.pos}, {p.vel}, {p.acc}");
                // Calculate which grid cell the particle is in
                int col = Mathf.FloorToInt((p.pos.x + screenWidth / 2) / (width + margin));
                int row = Mathf.FloorToInt((p.pos.y + screenHeight / 2) / (height + margin));

                // Check for out-of-bounds
                if (col >= 0 && row >= 0 && col < cols && row < rows)
                {
                    float angle = arrows[col, row].transform.eulerAngles.z;

                    // Convert angle to vector
                    Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    force *= 0.00001f;

                    // Apply the force
                    p.ApplyForce(force);

                    // Move the particle
                    p.Move(screenWidth, screenHeight);

                    // Update the GameObject's position
                    particle.transform.position = new Vector3(p.pos.x, p.pos.y, 0);
                }
            }
        }
    }


}
