using HtmlElements.Elements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    public class Profile : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "profile")]
        private HtmlInput profileTab;

        [FindsBy(How = How.CssSelector, Using = "body > header > nav > div > div > ul > li:nth-child(1) > a")]
        private HtmlElement userName;

        [FindsBy(How = How.Id, Using = "Input_PhoneNumber")]
        private HtmlInput phoneNumber;

        [FindsBy(How = How.Id, Using = "update-profile-button")]
        private HtmlInput saveButton;

        [FindsBy(How = How.Id, Using = "change-password")]
        private HtmlInput changePasswordTab;

        [FindsBy(How = How.Id, Using = "Input_OldPassword")]
        private HtmlInput oldPassword;

        [FindsBy(How = How.Id, Using = "Input_NewPassword")]
        private HtmlInput newPassword;

        [FindsBy(How = How.Id, Using = "Input_ConfirmPassword")]
        private HtmlInput confirmPassword;

        [FindsBy(How = How.CssSelector, Using = "#change-password-form > button")]
        private HtmlInput updatePasswordButton;

        [FindsBy(How = How.CssSelector, Using = "body > div > main > div > div > div.col-md-9 > div.alert.alert-success.alert-dismissible")]
        private HtmlElement passwordUpdateSuccesfulAlert;

        [FindsBy(How = How.CssSelector, Using = "body > div > main > div > div > div.col-md-9 > div.alert.alert-success.alert-dismissible")]
        private HtmlElement phoneChangeSuccessfullAlert;

        public Profile(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }
        public string UserName
        {
            get { return userName.Text; }
        }
        public void ProfileTab()
        {
            profileTab.Click();
        }
        public string PhoneNumber
        {
            get { return phoneNumber.Value; }
            set { phoneNumber.SendKeys(value); }
        }
        public void SavePhoneNumber()
        {
            saveButton.Click();
        }
        public void ChangePasswordTab()
        {
            changePasswordTab.Click();
        }
        public string OldPassword
        {
            get { return oldPassword.Value; }
            set { oldPassword.SendKeys(value); }
        }
        public string NewPassword
        {
            get { return newPassword.Value; }
            set { newPassword.SendKeys(value); }
        }
        public string ConfirmPassword
        {
            get { return confirmPassword.Value; }
            set { confirmPassword.SendKeys(value); }
        }
        public void UpdatePassword()
        {
            updatePasswordButton.Click();
        }
        public HtmlElement PasswordUpdateSuccesfulAlert()
        {
            return passwordUpdateSuccesfulAlert;
        }
        public HtmlElement PhoneChangeSuccessfullAlert()
        {
            return phoneChangeSuccessfullAlert;
        }
    }
}
