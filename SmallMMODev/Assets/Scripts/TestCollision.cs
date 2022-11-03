using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    // [ Collision Event 발생 조건 ]
    // (1) 나 또는 상대한테 RigidBody가 있어야 한다. (IsKinematic : OFF)
    // (2) 나한테 Collider가 있어야 한다. (IsTrigger : OFF)
    // (3) 상대한테 Collider가 있어야 한다. (IsTrigger: OFF)

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"collision : {collision.gameObject.name}");
    }

    // [ Trigger Event 발생 조건 ]
    // (1) 둘 다 Collider가 있어야 한다. 
    // (2) 둘 중 하나는 RigidBody가 있어야 한다.
    // (3) 둘 중 하나는 Trigger가 켜져 있어야 한다.
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"trigger : {other.gameObject.name}");
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    // int size = 20;

    // Update is called once per frame
    void Update()
    {
        // 좌표계
        // local <---> world <---> (viewPort) <---> screen 
        // Debug.Log(Input.mousePosition); // screen 좌표계
        // Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // viewport 좌표계
#if false
        // version #1  
        if (Input.GetMouseButtonDown(0))
        {
            // 스크린 좌표를 월드 좌표로 변환 : 시작점 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            // 방향 벡터 : 방향
            Vector3 dir = mousePos - Camera.main.transform.position;
            dir = dir.normalized;

            Debug.DrawRay(mousePos, dir* 100.0f, Color.red, 1.0f);
            RaycastHit hit;
            bool bRtn = Physics.Raycast(mousePos, dir, out hit, 100.0f);
            if (bRtn == true )
            {
                Debug.Log($"Raycast of Mouse Click : {hit.collider.gameObject.name}");
            }
        }

        // version #2 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue, 1.0f);

            // LayerMask
            LayerMask layerMask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
            // int mask = (1 << 6) | (1 << 7);
           
            RaycastHit hit;
            bool bRtn = Physics.Raycast(ray, out hit, 100.0f, layerMask);
            if (bRtn == true)
            {
                //Debug.Log($"Raycast of Mouse Click : {hit.collider.gameObject.tag}");
            }
#endif
    }

#if false // FundumentalRayCast 
        Vector3 pos = transform.position;
        pos.y += 1;

        // local to world 좌표계로 좌표 이동
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // for debug
        Debug.DrawRay(pos, forward * size, Color.red);
#if false
        RaycastHit hitInfo;
        bool bRtn = Physics.Raycast(pos, forward, out hitInfo, 10);
        if(bRtn == true)
        {
            Debug.Log($"Raycast : {hitInfo.collider.gameObject.name}");
        }
#else //Multi-hits
        RaycastHit[] hitInfoes = Physics.RaycastAll(pos, forward, size);
        foreach(var hit in hitInfoes)
        {
            Debug.Log($"Raycast : {hit.collider.gameObject.name}");
        }
#endif
#endif
}