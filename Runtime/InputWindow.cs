using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sayollo {
    public class InputWindow : MonoBehaviour
    {
        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        [SerializeField] private TMP_InputField inputFieldCreditCard;
        [SerializeField] private TMP_InputField inputFieldCardExpired;
        [SerializeField] private TMP_InputField inputFieldEmail;

        [Header("Colors")]
        [SerializeField]
        private Color fieldWrongColor;
        private Color fieldCorrectColor;

        private Canvas canvas;

        private void Awake()
        {
            System.Random rnd;
            canvas = GetComponent<Canvas>();
            Hide();
            fieldCorrectColor = inputFieldEmail.image.color;
        }

        public void Show(Action<BuyDataModel> onOk, Action onCancel)
        {
            canvas.enabled = true;

            InitializeInputField(ref inputFieldCreditCard, "0123456789", 16);
            InitializeInputField(ref inputFieldCardExpired, "0123456789", 4);


            okBtn.onClick.AddListener(() => {

                if (ValidateInput())
                {
                    Hide();
                    BuyDataModel buyDataModel = new BuyDataModel(inputFieldEmail.text, long.Parse(inputFieldCreditCard.text), inputFieldCardExpired.text);

                    onOk(buyDataModel);
                }
            });

            cancelBtn.onClick.AddListener(() => {
                Hide();
                onCancel();
            });
        }

        private bool ValidateInput()
        {
            //Email
            bool isEmail = Regex.IsMatch(inputFieldEmail.text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                inputFieldEmail.image.color = fieldWrongColor;
            }
            else
            {
                inputFieldEmail.image.color = fieldCorrectColor;
            }

            //Credit card
            bool isCardNumberLength = inputFieldCreditCard.text.Length == 16;
            bool isLong = long.TryParse(inputFieldCreditCard.text, out long cardNumber);
            if (!isCardNumberLength || !isLong)
            {
                inputFieldCreditCard.image.color = fieldWrongColor;
            }
            else
            {
                inputFieldCreditCard.image.color = fieldCorrectColor;
            }

            //Expired
            bool isNumber = int.TryParse(inputFieldCardExpired.text, out int expired);
            bool isExpiredLength = inputFieldCardExpired.text.Length == 4;
            if (!isNumber || !isExpiredLength)
            {
                inputFieldCardExpired.image.color = fieldWrongColor;
            }
            else
            {
                inputFieldCardExpired.image.color = fieldCorrectColor;
            }

            return isEmail && isCardNumberLength && isLong && isNumber && isExpiredLength;
        }

        private void InitializeInputField(ref TMP_InputField inputField, string validCharacters, int characterLimit)
        {
            inputField.characterLimit = characterLimit;
            if (validCharacters != null)
            {
                inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
                    return ValidateChar(validCharacters, addedChar);
                };
            }

        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        private char ValidateChar(string validCharacters, char addedChar)
        {
            if (validCharacters.IndexOf(addedChar) != -1)
            {
                return addedChar;
            }
            else
            {
                return '\0';
            }
        }
    }

}