using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateClean
{

    public class PopupAnimation : MonoBehaviour
    {
        [SerializeField]
        private RuntimeAnimatorController popAnimator;
        [SerializeField]
        private RuntimeAnimatorController fadeAnimator;
        private RuntimeAnimatorController origin;

        private Animator animator;

        private void Awake()
        {
            if (!GetComponent<Animator>()) { gameObject.AddComponent<Animator>(); }
            animator = GetComponent<Animator>();
            origin = animator.runtimeAnimatorController;
        }

        void OnEnable()
        {
            if (GetComponent<ButtonWithSound>())
                GetComponent<ButtonWithSound>().enabled = false;
            animator.runtimeAnimatorController = popAnimator;
            animator.SetTrigger("Open");
            Invoke("ChangeBackAnimation", 0.5f);
        }

        void OnDisable()
        {
            if (GetComponent<ButtonWithSound>())
                GetComponent<ButtonWithSound>().enabled = false;
            animator.runtimeAnimatorController = fadeAnimator;
            animator.SetTrigger("Close");
            Invoke("ChangeBackAnimation", 0.5f);
        }

        void ChangeBackAnimation()
        {
            animator.runtimeAnimatorController = origin;
            if (GetComponent<ButtonWithSound>())
                GetComponent<ButtonWithSound>().enabled = true;
        }


    }
}
