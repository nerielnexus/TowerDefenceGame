using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRoad : MonoBehaviour
{
    // 길 큐브 Road Cube 가 어떤 종류인지 설정하는 열거형 값
    // 미설정NONE 그냥 길ROAD 시작지점START 웨이포인트WAYPOINT 도착지점END 로 구성되어있다
    public enum ROADIDTYPE
    {
        NONE,
        ROAD,
        START,
        WAYPOINT,
        END
    };

    [Header("Road Type")]
    public ROADIDTYPE identifier = ROADIDTYPE.NONE;

    // 하나의 길 큐브의 옆에 어떤 다른 길 큐브들이 있는지 저장하는 리스트
    // 기준은 y축 기준 전후좌우 4개이다
    [Header("Nearby Roads")]
    public CubeRoad[] nearbyCubes = new CubeRoad[4];

    // 자신 주변의 다른 길 큐브를 확인하고 그 목록을 저장하는 메서드
    public void GetNearbyRoadCubes()
    {
        // 진행하기 전 길 큐브를 저장하는 목록을 초기화한다
        for (int i = 0; i < nearbyCubes.Length; i++)
            nearbyCubes[i] = null;

        // 길 큐브 프리팹을 와바박 불러와서 맵을 만들고, 그 콜라이더를 이용하는 맵 매니저의 구현 상태 때문에
        // SyncTransforms 를 한 번 실행해서 transform 을 맞춰주는 작업이 필요한가보다
        // 왜 돌아가는지는 잘 모르겠다 더 찾아봐야할듯
        Physics.SyncTransforms();

        // 길 큐브를 중심으로 OverlapBox 를 호출해서, 길 큐브만한 크기의 box collider 에 닿는 다른 콜라이더들을 전부 불러온다
        // 이건 자신 포함임
        Collider[] nearby = Physics.OverlapBox(
            GetComponent<Renderer>().bounds.center,
            GetComponent<Renderer>().bounds.extents,
            Quaternion.identity,
            1<<LayerMask.NameToLayer("CubeRoad")
            );

        /*
        foreach (Collider col in nearby)
            Debug.Log(gameObject.name + " -> " + col.gameObject.name + ", " + Vector3.SignedAngle(this.transform.forward, (col.gameObject.GetComponent<Renderer>().bounds.center - this.GetComponent<Renderer>().bounds.center),this.transform.up));
        */

        // OverlapBox 로 불러온 콜라이더들을 조건에 맞게 필터링해 주변 길 큐브 배열에 저장한다
        foreach (Collider col in nearby)
        {
            // 주변 길 큐브를 판단하는 방식은 벡터의 각 계산
            // (판단할 대상의 중심점 - 나의 중심점) 계산을 하면, 나->대상 벡터를 만들 수 있다
            // 그 벡터와 나의 forward 벡터 각을 비교하여 전후좌우 판단을 하고 주변 길 큐브 배열에 저장한다.
            // SignedAngle 을 사용하면, 특정 축을 기준으로 두 벡터의 각 차이를 계산해주며 그 결과값이 음수도 나올 수 있다고 한다
            // 각 값만 이용해 전후좌우를 구분해야하는 내 구현 요구에 딱 맞는 메서드다
            Vector3 tmpDir = col.gameObject.GetComponent<Renderer>().bounds.center
                - GetComponent<Renderer>().bounds.center;
            float tmpAngle = Vector3.SignedAngle(this.transform.forward, tmpDir, this.transform.up);

            // OverlapBox 에서 받아온 콜라이더에는 자신의 콜라이더도 포함되어 있어 이를 걸러줄 구문이 필요하다
            if (col.gameObject == this.gameObject)
                continue;

            // 큐브 기준 전방
            if (tmpAngle == 0f || tmpAngle == 360f)
                nearbyCubes[0] = col.gameObject.GetComponent<CubeRoad>();

            // 큐브 기준 오른쪽
            if (tmpAngle == 90f || tmpAngle == -270f)
                nearbyCubes[1] = col.gameObject.GetComponent<CubeRoad>();

            // 큐브 기준 후방
            if (tmpAngle == 180f || tmpAngle == -180f)
                nearbyCubes[2] = col.gameObject.GetComponent<CubeRoad>();

            // 큐브 기준 왼쪽
            if (tmpAngle == 270f || tmpAngle == -90f)
                nearbyCubes[3] = col.gameObject.GetComponent<CubeRoad>();
        }
    }

    private void Start()
    {
        // 길 큐브의 종류가 설정되지 않으면 예외를 발생시키도록 했다
        if (identifier == ROADIDTYPE.NONE)
            throw new System.Exception("CubeRoad identifier is none");
    }
}
