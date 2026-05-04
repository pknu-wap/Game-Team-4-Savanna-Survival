using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private float currntTime;
    [SerializeField] private GameObject Skill_BasicAttck;

    private void Update() //Skill_BasicAttck 쿨타임
    {
        currntTime += Time.deltaTime;
        if (currntTime >= 1)
        {
            Skill_BasicAttck.SetActive(true);
            currntTime = 0;
            //Debug.Log("작동함");
        }
        else if (currntTime > 0.2) Skill_BasicAttck.SetActive(false);
    }
}
