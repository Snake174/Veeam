using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            INI ini = new INI("settings.ini");
            int waitingVacanciesCount = Int32.Parse(ini.Read("WaitingVacanciesCount", "Settings"));
            string Department = ini.Read("Department", "Settings");
            string Language = ini.Read("Language", "Settings");

            var options = new ChromeOptions();
            options.BinaryLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GoogleChromePortable", "App", "Chrome-bin", "chrome.exe");
            options.AddArgument("--window-position=0,0");
            options.AddArgument("--start-fullscreen");
            options.AddArgument("--incognito");
            options.AddArgument("--enable-aggressive-domstorage-flushing");
            options.AddArgument("--enable-fast-unload");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--mute-audio");
            options.AddArgument("--no-sandbox");
            options.AddExcludedArgument("--disable-default-apps");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("disable-popup-blocking", true);

            using (var driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options, TimeSpan.FromSeconds(300)))
            {
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                driver.Manage().Cookies.DeleteAllCookies();
                driver.Navigate().GoToUrl("https://careers.veeam.ru/vacancies");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                try
                {
                    // Закрываем окно с подтверждением cookie файлов
                    const string cookieXPath = "#cookiescript_injected";
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cookieXPath)));

                    const string cookieCloseXPath = "#cookiescript_close";
                    IWebElement cookieClose = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(cookieCloseXPath)));
                    cookieClose.Click();

                    // Выбор отдела
                    //const string departmentXPath = "/html/body/div[1]/div/div[1]/div/div[2]/div[1]/div/div[2]/div/div/button";
                    IWebElement department = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[.='Все отделы']")));
                    department.Click();

                    //const string devProductsXPath = "/html/body/div[1]/div/div[1]/div/div[2]/div[1]/div/div[2]/div/div/div/a[4]";
                    IWebElement devProducts = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//a[.='{Department}']")));
                    devProducts.Click();

                    // Выбор языка
                    //const string languagesXPath = "/html/body/div[1]/div/div[1]/div/div[2]/div[1]/div/div[3]/div/div/button";
                    IWebElement languages = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[.='Все языки']")));
                    languages.Click();

                    //const string englishXPath = "/html/body/div[1]/div/div[1]/div/div[2]/div[1]/div/div[3]/div/div/div/div[2]/label";
                    IWebElement english = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//label[.='{Language}']")));
                    english.Click();

                    // Количество выданных вакансий
                    const string vacanciesXPath = "/html/body/div[1]/div/div[1]/div/div[2]/div[2]/div";
                    IWebElement vacancies = wait.Until(ExpectedConditions.ElementExists(By.XPath(vacanciesXPath)));

                    int vacanciesCount = vacancies.FindElements(By.CssSelector("a.card")).Count;

                    Console.WriteLine("========================================");
                    Console.WriteLine($"Кол-во полученных вакансий : {vacanciesCount}");
                    Console.WriteLine($"Кол-во ожидаемых вакансий  : {waitingVacanciesCount}");
                    Console.WriteLine("========================================");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка: {e.Message}");
                }
            }

            Console.ReadLine();
        }
    }
}
