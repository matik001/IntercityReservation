using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IntercityReservation {
    internal enum SitType {
        [Description("dowolne")] Any,
        [Description("okno")] Window,
        [Description("środek")] Middle, 
        [Description("korytarz")] Corridor
    }
    internal enum CarriageType {
        [Description("dowolny")] Any,
        [Description("Wagon z przedziałami")] WithCompartments,
        [Description("Wagon bez przedziałów")] WithoutCompartments,
        [Description("Miejsce dla osoby na wózku")] ForWheelchair,
        [Description("Miejsce dla OzN - nie na wózku")] ForDisabled,
        [Description("Przedział dla osoby z dzieckiem do lat 6")] WithChildren
    }

    static class EnumExtensions {
        public static string GetEnumDescription(this Enum enumValue) {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }
    internal class IntercityTicketPage {
        public struct TicketInfo {
            public int NormalCnt = 0;
            public int DiscountCnt = 0;
            public string DiscountType = "";
            public SitType PreferredSitType = SitType.Any;
            public CarriageType PreferredCarriageType = CarriageType.Any;

            public TicketInfo() {
            }

            public TicketInfo(int normalCnt, int discountCnt, string discountType, 
                    SitType preferredSitType = SitType.Any, CarriageType preferredDarriageType = CarriageType.Any) {
                NormalCnt = normalCnt;
                DiscountCnt = discountCnt;
                DiscountType = discountType;
                PreferredSitType = preferredSitType;
                PreferredCarriageType = preferredDarriageType;
            }
        }

        private static By BY_NORMAL_CNT = By.Id("liczba_n");
        private static By BY_DISCOUNT_CNT = By.Id("liczba_u");
        private static By BY_DISCOUNT_TYPE = By.Id("kod_znizki");
        private static By BY_SUBMIT_BTN = By.Id("strefa_modal");
        private static By BY_SIT_TYPE = By.Id("usytuowanie");
        private static By BY_CARRIAGE_TYPE = By.Id("rodzaj_wagonu");
        

        private IWebDriver _driver;


        public IntercityTicketPage(IWebDriver driver) {
            _driver = driver;
        }
        public IntercityDetailsPage SearchTicket(TicketInfo ticket) {
            var oldImplicitWait = _driver.Manage().Timeouts().ImplicitWait;
            if (_driver.Url.Contains("komunikat.jsp")) {
                throw new Exception("przerwa techniczna");
            }

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var normalCntSelect = new SelectElement(_driver.FindElement(BY_NORMAL_CNT));
            var discountCntSelect = new SelectElement(_driver.FindElement(BY_DISCOUNT_CNT));
            var discountTypeSelect = new SelectElement(_driver.FindElement(BY_DISCOUNT_TYPE));
            var sitTypeSelect = new SelectElement(_driver.FindElement(BY_SIT_TYPE));
            var carriageTypeSelect = new SelectElement(_driver.FindElement(BY_CARRIAGE_TYPE));
            var submitBtn = _driver.FindElement(BY_SUBMIT_BTN);

            Thread.Sleep(1000);
            normalCntSelect.SelectByValue(ticket.NormalCnt.ToString());
            Thread.Sleep(500);
            discountCntSelect.SelectByValue(ticket.DiscountCnt.ToString());
            Thread.Sleep(500);
            discountTypeSelect.SelectByText(ticket.DiscountType, true);
            Thread.Sleep(500);
            // sitTypeSelect.SelectByText(ticket.PreferredSitType.GetEnumDescription());
            // Thread.Sleep(500);
            // carriageTypeSelect.SelectByText(ticket.PreferredCarriageType.GetEnumDescription());
            // Thread.Sleep(500);

            submitBtn.Submit();


            _driver.Manage().Timeouts().ImplicitWait = oldImplicitWait;

            return new IntercityDetailsPage(_driver);
        }
    }
}
