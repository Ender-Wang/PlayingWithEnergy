using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    public class ButtonChangeColor : MonoBehaviour
    {
        [Header("Necessary components")]
        public ButtonWithSound button;
        public Image buttonImage;
        [Header("Optional components")]
        public Image toggleImage;
        public Transform switchButton;


        private Color defaultColor;

        // switch button content
        private Animator animator;

        private Image bgEnabledImage;
        private Image bgDisabledImage;

        private Image handleEnabledImage;
        private Image handleDisabledImage;

        private bool switchEnabled;
        // ----------------------


        void Awake()
        {
            defaultColor = buttonImage.color;
            button.onClick.AddListener(ChangeColor);
            switchEnabled = false;
            if (switchButton != null)
            {
                animator = switchButton.GetComponent<Animator>();
                bgEnabledImage = switchButton.GetChild(0).GetChild(0).GetComponent<Image>();
                bgDisabledImage = switchButton.GetChild(0).GetChild(1).GetComponent<Image>();
                handleEnabledImage = switchButton.GetChild(1).GetChild(0).GetComponent<Image>();
                handleDisabledImage = switchButton.GetChild(1).GetChild(1).GetComponent<Image>();
            }
        }

        public void ChangeColor()
        {
            switchEnabled = !switchEnabled;
            if (switchEnabled)
            {
                buttonImage.color = Settings.green;
                if (toggleImage != null)
                    toggleImage.color = Settings.green;
                if (switchButton != null)
                    Toggle();
            }
            else
            {
                buttonImage.color = defaultColor;
                if (toggleImage != null)
                    toggleImage.color = defaultColor;
                if (switchButton != null)
                    Toggle();
            }
        }


        private void Toggle()
        {
            if (switchEnabled)
            {
                bgDisabledImage.gameObject.SetActive(false);
                bgEnabledImage.gameObject.SetActive(true);
                handleDisabledImage.gameObject.SetActive(false);
                handleEnabledImage.gameObject.SetActive(true);
            }
            else
            {
                bgEnabledImage.gameObject.SetActive(false);
                bgDisabledImage.gameObject.SetActive(true);
                handleEnabledImage.gameObject.SetActive(false);
                handleDisabledImage.gameObject.SetActive(true);
            }
            animator.SetTrigger(switchEnabled ? "Enable" : "Disable");
        }

    }

}