using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SphareSpawner : MonoBehaviour
{
    public GameObject spherePrefab; // 구체 프리팹
    public float spawnRange = 10f;  // 생성 범위
    public int maxSpheres = 10;     // 최대 구체 개수
    public Material[] sphereMaterials; // 구체 색상 배열
    public Text Current_Stage;// 스테이지 UI
    public List<GameObject> rewardPrefabs; // 상품 프리팹 리스트
    public List<Transform> spawnPositions;  // 미리 정의된 위치 리스트
    public GameObject EndgameMessege; //게임 종료 메세지

    private int currentSphereCount = 0;
    private int stage = 1; // 현재 스테이지
    private HashSet<Transform> usedPositions = new HashSet<Transform>(); // 사용된 위치 추적
    void Start()
    {
        UpdateStageUI(); // UI 업데이트
        SpawnSpheres();
    }

    public void SpawnSpheres()
    {
        for (int i = 0; i < maxSpheres; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                Random.Range(0.5f, spawnRange), // 바닥에서 떨어진 위치
                Random.Range(-spawnRange, spawnRange)
            );

            GameObject newSphere = Instantiate(spherePrefab, randomPosition, Quaternion.identity);

            SphareTarget sphereTarget = newSphere.GetComponent<SphareTarget>();
            if (sphereTarget != null)
            {
                sphereTarget.spawner = this; // 현재 SphereSpawner 참조 전달
            }

            // 랜덤 크기 설정
            float randomScale = Random.Range(0.5f, 2f);
            newSphere.transform.localScale = Vector3.one * randomScale;

            // 랜덤 색상 설정
            Renderer renderer = newSphere.GetComponent<Renderer>();
            if (renderer != null && sphereMaterials.Length > 0)
            {
                renderer.material = sphereMaterials[Random.Range(0, sphereMaterials.Length)];
            }

            currentSphereCount++;
        }
    }

    public void SphereHit(GameObject sphere)
    {
        Destroy(sphere); // 구체 파괴
        currentSphereCount--;

        // 구체가 모두 사라지면 다시 생성
        if (currentSphereCount <= 0)
        {
            //SpawnSpheres();
            StageUP();
        }
    }

    private void SpawnReward()
    {
        if (rewardPrefabs == null || rewardPrefabs.Count == 0)
        {
            Debug.LogWarning("Reward Prefabs list is empty!");
            return;
        }

        if (spawnPositions == null || spawnPositions.Count == 0)
        {
            Debug.LogWarning("Spawn positions list is empty!");
            return;
        }

        // 현재 스테이지에 해당하는 상품 선택
        int rewardIndex = (stage - 1) % rewardPrefabs.Count; // 스테이지에 따라 상품 순환
        GameObject selectedReward = rewardPrefabs[rewardIndex];

        // 위치 선택 (중복되지 않은 위치)
        Transform spawnPosition = GetAvailablePoint();
        if (spawnPosition == null)
        {
            Debug.LogWarning("No available spawn positions!");
            return;
        }

        // 상품 생성
        Instantiate(selectedReward, spawnPosition.position, spawnPosition.rotation);

        Debug.Log($"Stage {stage}: Spawned {selectedReward.name}");
    }

    private Transform GetAvailablePoint()
    {
        foreach (Transform point in spawnPositions)
        {
            if (!usedPositions.Contains(point))
            {
                usedPositions.Add(point); // 사용된 위치로 등록
                return point;
            }
        }

        return null; // 사용할 위치가 없을 경우
    }
    private void StageUP()
    {
        stage++;
        if(stage > 5)
        {
            StartCoroutine(EndGame());
        }
        maxSpheres += 1; // 다음 스테이지에 구체 수 증가
        spawnRange += 1f; // 생성 범위 확장
        UpdateStageUI(); // UI 업데이트
        SpawnSpheres();
        SpawnReward();
    }

    private void UpdateStageUI()
    {
        if (Current_Stage != null)
        {
            Current_Stage.text = "Stage: " + stage; // UI 텍스트 업데이트
        }
    }

    private IEnumerator EndGame()
    {
        EndgameMessege.SetActive(true);
        // 게임 종료 메시지를 HUD 또는 VR UI로 표시
        Debug.Log("Game Over: You have reached the final stage!");

        // 종료 전 대기 시간 (선택 사항)
        yield return new WaitForSeconds(5f);
        EndgameMessege.SetActive(false);

        // 게임 종료 처리
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#else
    Application.Quit(); // VR 환경 포함 게임 종료
#endif
    }
}
