using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace IntercityReservation {
    class NoSeatException : Exception {
        public NoSeatException() : base("Nie ma miejsc siedzacych") {
        }

        public NoSeatException(string? message) : base(message) {
        }
    }
    internal class IntercityDetailsPage {
        private static By BY_SEAT_INFO = By.XPath("//div[@id='external_data']//div[text()='NUMER MIEJSCA/OPIS']/following-sibling::div[@class='label_std']");
        private static By BY_NAME_FIELD = By.Id("imie_nazwisko_podroznego");
        private static By BY_SUBMIT_BTN = By.ClassName("kup_bilet_button");


        private IWebDriver _driver;


        public IntercityDetailsPage(IWebDriver driver) {
            _driver = driver;
        }

        public string SeatInfo {
            get {
                var seatInfo = new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
                    .Until(ExpectedConditions.ElementIsVisible(BY_SEAT_INFO));
                return seatInfo.Text.ToLowerInvariant();
            }
        }

        public bool IsAnnouncement() {
            return _driver.Url.Contains("komunikat.jsp");
        }
        public bool CanSit() {
            return !SeatInfo.Contains("brak gwarancji");
        }

        public bool IsWindow() {
            return SeatInfo.Contains("okno");
        }
        public bool IsMiddle() {
            return SeatInfo.Contains("środek");
        }
        public bool IsCorridor() {
            return SeatInfo.Contains("korytarz");
        }

        public IntercityDetailsPage ThrowExceptionIfCannotSit() {
            if (!CanSit()) {
                throw new NoSeatException();
            }

            return this;
        }
        public IntercityLoginPage OrderTicket(string name) {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

            ThrowExceptionIfCannotSit();
            var nameInput = _driver.FindElement(BY_NAME_FIELD);
            var submitBtn = _driver.FindElement(BY_SUBMIT_BTN);

            nameInput.SendKeys(name);
            submitBtn.Submit();


            return new IntercityLoginPage(_driver);
        }
    }
}
