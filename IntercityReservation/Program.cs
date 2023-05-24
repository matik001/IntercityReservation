using IntercityReservation;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

public class Program {
    public static void Main(string[] args) {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        //options.AddArgument("headless");

        var ticketInfo = new IntercityTicketPage.TicketInfo() {
            DiscountCnt = 1,
            NormalCnt = 0,
            DiscountType = "Studenci do 26 lat",
            PreferredCarriageType = CarriageType.Any,
            PreferredSitType = SitType.Window
        };
        var loginData = new IntercityLoginPage.ICLoginData(
            Environment.GetEnvironmentVariable("LOGIN"),
            Environment.GetEnvironmentVariable("PASS")
            );
        ChromeDriver driver = null;
        while (true) {
            try {
                driver = new ChromeDriver(options);
                IntercitySearchPage searchPage = new IntercitySearchPage(driver);
                var page = searchPage.NavigateToSearching()
                    .SearchConnections("Bielsko-Biała Główna", "Wrocław Główny",
                        new DateTime(2023, 5, 3, 16, 0, 0))
                    .SearchTicket(ticketInfo);

                while (page.IsAnnouncement() || !page.CanSit()) {
                    driver.Navigate().Back();
                    page = new IntercityTicketPage(driver).SearchTicket(ticketInfo);
                }
      
                page.OrderTicket("Mateusz Kisiel")
                    .Login(loginData)
                    .PayLater()
                    .GoOn();

                
                while(true)
                {
                    Console.Beep(760, 1000);
                }

                Thread.Sleep(10000);
                driver.Close();
                break;

            }
            catch(Exception e)  {
                Console.WriteLine(e.Message);
                driver?.Close();
                // driver.Manage().Cookies.DeleteAllCookies();
                // (driver as IJavaScriptExecutor).ExecuteScript("sessionStorage.clear();");
                // (driver as IJavaScriptExecutor).ExecuteScript("localStorage.clear();");
                continue;
            }
        }


        Console.ReadLine();
        
    }
}