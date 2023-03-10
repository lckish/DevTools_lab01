using HtmlElements.Elements;
using HtmlElements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    internal class Login : HtmlPage
    {
        [FindsBy(How = How.Id, Using = "Input_Email")]
        private HtmlInput email;

        [FindsBy(How = How.Id, Using = "Input_Password")]
        private HtmlInput password;

        [FindsBy(How = How.Id, Using = "login-submit")]
        private HtmlInput loginButton;

        public Login(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
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
        public MainPage LoginAndGoToMainPage()
        {
            loginButton.Click();
            return PageObjectFactory.Create<MainPage>(this);
        }
    }
}
