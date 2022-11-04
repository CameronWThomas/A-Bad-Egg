using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggAnimator : MonoBehaviour
{
    public Animator animator;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //speed
        float animationSpeedPercent = ((playerController.running) ? playerController.currentSpeed / playerController.runSpeed : playerController.currentSpeed / playerController.walkSpeed * .5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, playerController.speedSmoothTime, Time.deltaTime);

    }


    public void SetArmed(bool isArmed)
    {
        animator.SetBool("armed", isArmed);
    }
    public void Swing()
    {
        animator.SetTrigger("swing");
    }

    public void OnJumping()
    {
        //trigger jump animation
    }
}
