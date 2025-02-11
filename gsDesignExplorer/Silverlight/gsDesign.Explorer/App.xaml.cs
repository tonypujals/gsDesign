﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;
using gsDesign.Explorer.ViewModels;

namespace gsDesign.Explorer
{
	using Views.Dialogs;

	public partial class App : Application
	{
		public App()
		{
			Startup += Application_Startup;
			Exit += Application_Exit;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		/// <summary>
		/// Provides application-wide access to the view model
		/// </summary>
		public static AppViewModel AppViewModel
		{
			get { return (AppViewModel) Current.Resources["AppViewModel"]; }
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			RootVisual = new MainPage();
			TestElevatedPermissions();
			TestConnection();
		}

		/// <summary>
		/// Tests for elevated permissions and prompts user to switch to out-of-browser mode, if necessary
		/// </summary>
		private void TestElevatedPermissions()
		{
			if (Current.HasElevatedPermissions == false)
			{
				var dialog = new ElevatedPermissionsNeededDialog();

				// will not allow the user to continue while elevated permissions are not available
				dialog.Closed += (sender2, e2) => TestElevatedPermissions();

				dialog.Show();
			}
		}

		/// <summary>
		/// Attempts to connect to Rserve and will cause a dialog to be displayed if it's not available
		/// </summary>
		private void TestConnection()
		{
			AppViewModel.Connect();
		}

		private void Application_Exit(object sender, EventArgs e)
		{
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!Debugger.IsAttached)
			{
				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}