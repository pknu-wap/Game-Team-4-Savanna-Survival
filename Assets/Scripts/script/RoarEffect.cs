using UnityEngine;
using System.Collections;

public class RoarSkill : MonoBehaviour
{
    [SerializeField] private GameObject roarEffect; 

    public void UseRoarSkill()
    {
        StartCoroutine(RoarRoutine());
    }

    private IEnumerator RoarRoutine()
    {
        // 1. 이펙트 활성화
        roarEffect.SetActive(true);
        roarEffect.transform.position = transform.position;

        // 2. 3초 대기
        yield return new WaitForSeconds(3f);

        // 3. 이펙트 끄기
        roarEffect.SetActive(false);
    }
}