using HtmlElements.Elements;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Northwind.Web.Tests.SeleniumTests.Pages
{
    public class MainPage : HtmlPage
    {
        [FindsBy(How = How.CssSelector, Using = "a[href*='Categories'].nav-link")]
        private HtmlLink categoriesLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Register'].nav-link")]
        private HtmlLink registrationLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Login'].nav-link")]
        private HtmlLink loginLink;

        [FindsBy(How = How.CssSelector, Using = "a[href*='Identity/Account/Manage'].nav-link")]
        private HtmlLink profileLink;

        [FindsBy(How = How.CssSelector, Using = "body > header > nav > div > div > ul > li:nth-child(2) > form > button")]
        private HtmlLink exitLink;


        public MainPage(ISearchContext webDriverOrWrapper) : base(webDriverOrWrapper)
        {
        }

        public CategoryListPage GoToCategoriesListPage()
        {
            categoriesLink.Click();
            return PageObjectFactory.Create<CategoryListPage>(this);
        }
        public string ExitText
        {
            get { return exitLink.Text; }
        }
        public Registration GoToRegistration()
        {
            registrationLink.Click();
            return PageObjectFactory.Create<Registration>(this);
        }
        public Login GoToLogin()
        {
            loginLink.Click();
            return PageObjectFactory.Create<Login>(this);
        }
        public Profile GoToProfile()
        {
            profileLink.Click();
            return PageObjectFactory.Create<Profile>(this);
        }
        public string RegisterText
        {
            get { return registrationLink.Text; }
        }
        public string EnterText
        {
            get { return loginLink.Text; }
        }
        public MainPage LogOut()
        {
            exitLink.Click();
            return PageObjectFactory.Create<MainPage>(this);
        }
    }
}
