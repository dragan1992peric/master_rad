using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.DevTools.V110.Network;
using OpenQA.Selenium.Support.UI;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace SeleniumVTSNS
{
	public class Tests3
	{
		[SetUp]
		public void Setup3()
		{

		}

		[Test]
		public void Test3()
		{
			IWebDriver webDriver = new ChromeDriver(@"C:\Users\Samurai Guzonja\Downloads");

			webDriver.Navigate().GoToUrl("https://localhost:7081/");

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

			var test = webDriver.FindElement(By.Id("pretraga"));

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

			test.SendKeys("ready");

			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

			var selectElement = webDriver.FindElement(By.LinkText("Ready Player One"));

			selectElement.Click();




		}
	}
}