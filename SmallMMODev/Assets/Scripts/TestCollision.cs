using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    // [ Collision Event �߻� ���� ]
    // (1) �� �Ǵ� ������� RigidBody�� �־�� �Ѵ�. (IsKinematic : OFF)
    // (2) ������ Collider�� �־�� �Ѵ�. (IsTrigger : OFF)
    // (3) ������� Collider�� �־�� �Ѵ�. (IsTrigger: OFF)

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"collision : {collision.gameObject.name}");
    }

    // [ Trigger Event �߻� ���� ]
    // (1) �� �� Collider�� �־�� �Ѵ�. 
    // (2) �� �� �ϳ��� RigidBody�� �־�� �Ѵ�.
    // (3) �� �� �ϳ��� Trigger�� ���� �־�� �Ѵ�.
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
        // ��ǥ��
        // local <---> world <---> (viewPort) <---> screen 
        // Debug.Log(Input.mousePosition); // screen ��ǥ��
        // Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // viewport ��ǥ��
#if false
        // version #1  
        if (Input.GetMouseButtonDown(0))
        {
            // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ : ������ 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            // ���� ���� : ����
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

        // local to world ��ǥ��� ��ǥ �̵�
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