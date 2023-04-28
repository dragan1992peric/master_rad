using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumVTSNS
{
	public class Tests2
	{
		[SetUp]
		public void Setup2()
		{

		}

		[Test]
		public void Test2()
		{
			IWebDriver webDriver = new ChromeDriver(@"C:\Users\Samurai Guzonja\Downloads");

			webDriver.Navigate().GoToUrl("https://localhost:7081/");

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

			var test = webDriver.FindElement(By.Id("pretraga"));

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

			test.SendKeys("sci");

		}
	}
}