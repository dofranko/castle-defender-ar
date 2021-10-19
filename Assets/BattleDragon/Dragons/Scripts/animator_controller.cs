using UnityEngine;

public class animator_controller : MonoBehaviour
{

    public Animator animator;

    public void OnClick(GameObject sender)
    {
        if (null != sender)
        {
            animator.Play(sender.name);
        }
    }
}