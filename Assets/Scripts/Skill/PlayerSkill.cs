using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private GameObject Skill_BasicAttck;

    private void Start()
    {
        if (Skill_BasicAttck != null)
            Skill_BasicAttck.SetActive(true);
    }
}
