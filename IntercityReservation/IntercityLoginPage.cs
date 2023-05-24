using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace IntercityReservation {


    internal class IntercityLoginPage {
        public struct ICLoginData {
            public string Username { get; set; }
            public string Password { get; set; }

            public ICLoginData(string username, string password) {
                Username = username;
                Password = password;
            }

            public ICLoginData() : this("", "") {
            }
        }
        
        private static By BY_USERNAME = By.Name("login");
        private static By BY_PASSWORD = By.Name("password");
        private static By BY_SUBMIT_BTN = By.Name("actlogin");

        private IWebDriver _driver;


        public IntercityLoginPage(IWebDriver driver) {
            _driver = driver;
        }
        
        public IntercityPaymentPage Login(ICLoginData loginData) {

            var oldImplicitWait = _driver.Manage().Timeouts().ImplicitWait;
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var nameInput = _driver.FindElement(BY_USERNAME);
            var passInput = _driver.FindElement(BY_PASSWORD);
            var submitBtn = _driver.FindElement(BY_SUBMIT_BTN);

            nameInput.SendKeys(loginData.Username);
            passInput.SendKeys(loginData.Password);
            submitBtn.Submit();

            _driver.Manage().Timeouts().ImplicitWait = oldImplicitWait;

            return new IntercityPaymentPage(_driver);
        }
    }
}
