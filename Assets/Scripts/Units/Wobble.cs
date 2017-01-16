using UnityEngine;

public class Wobble : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            wobble();
        }
    }

    private void wobble()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Wobble"))
        {
            anim.Play("Idle");
        }
        anim.SetTrigger("Wobble");
    }
}
