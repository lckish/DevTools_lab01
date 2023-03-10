using HtmlElements.Elements;
using HtmlElements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    internal class Registration : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private HtmlInput email;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private HtmlInput password;

        [FindsBy(How = How.Id, Using = "Input_ConfirmPassword")]
        private HtmlInput confirmPassword;

        [FindsBy(How = How.Id, Using = "registerSubmit")]
        private HtmlInput registerButton;

        public Registration(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }
        public string Email
        {
            get { return email.Value; }
            set { email.SendKeys(value); }
        }
        public string Password
        {
            get { return password.Value; }
            set { password.SendKeys(value); }
        }
        public string ConfirmPassword
        {
            get { return confirmPassword.Value; }
            set { confirmPassword.SendKeys(value); }
        }
        public MainPage RegisterAndGoToMainPage()
        {
            registerButton.Click();
            return PageObjectFactory.Create<MainPage>(this);
        }
    }
}
