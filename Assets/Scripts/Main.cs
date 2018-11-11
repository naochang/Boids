using UnityEngine;

public class Main : MonoBehaviour
{
    /// <summary>
    /// 壁とみなすサイズ。
    /// </summary>
    private static float WallSize = 100;

    [SerializeField]
    private GameObject boidPrefab;

    private void Start()
    {
        Boid.WallSize = Main.WallSize;

        for (int i = 0; i < 300; ++i)
        {
            GameObject boidGO = Instantiate(this.boidPrefab,
                new Vector3(
                    Random.Range(-Main.WallSize * Boid.WallAvoidPercentage / 2f, Main.WallSize * Boid.WallAvoidPercentage / 2f),
                    Random.Range(-Main.WallSize * Boid.WallAvoidPercentage / 2f, Main.WallSize * Boid.WallAvoidPercentage / 2f),
                    Random.Range(-Main.WallSize * Boid.WallAvoidPercentage / 2f, Main.WallSize * Boid.WallAvoidPercentage / 2f)
                ),
                Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))),
                null);

            float velocity = 10f;

            boidGO.GetComponent<Boid>().Velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized * velocity;
        }
    }
}
