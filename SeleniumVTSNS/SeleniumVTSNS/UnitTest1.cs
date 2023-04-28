using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumVTSNS
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
			
		}

		[Test]
		public void Test1()
		{
			IWebDriver webDriver = new ChromeDriver(@"C:\Users\Samurai Guzonja\Downloads");

			webDriver.Navigate().GoToUrl("https://localhost:7081/");

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

			IWebElement dugmeTest = webDriver.FindElement(By.Id("dugmepretraga"));

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

			dugmeTest.Click();

			var proba = webDriver.FindElement(By.Id("test"));
			
			Assert.That(proba.Displayed, Is.True);

		}
	}
}