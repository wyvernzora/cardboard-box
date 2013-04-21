using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MemoryDiagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;

namespace CardboardBox
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            Logging.D("Application Initializing...");

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
     //       if (System.Diagnostics.Debugger.IsAttached)
     //       {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                MemoryDiagnosticsHelper.Start(TimeSpan.FromMilliseconds(1000), true);

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;


       //     }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Logging.D("Application Launching...");
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Logging.D("Application Activated");
            if (!NetworkInterface.GetIsNetworkAvailable())
                Session.Instance.LogOut();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Logging.D("Application Deactivated");
            if (Logging.LogFile != null)
            {
                Logging.LogFile.Flush();
                //Logging.LogFile.Close();
            }

            Session.Instance.SaveUserFavorites();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Logging.D("Application Closing");
            Session.Instance.SaveUserFavorites();
            if (Logging.LogFile != null)
            {
                Logging.LogFile.Flush();
                //Logging.LogFile.Close();
            }

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Logging.D("ERROR: Navigation Failed! ExceptionType = {0}; Message = {1}; StackTrace = {2}",
                      e.Exception.GetType().FullName, e.Exception.Message, e.Exception.StackTrace);
            if (Logging.LogFile != null)
            {
                Logging.LogFile.Flush();
                //Logging.LogFile.Close();
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {

            Logging.D("ERROR: Crash! ExceptionType = {0}; Message = {1}; StackTrace = {2}",
                      e.ExceptionObject.GetType().FullName, e.ExceptionObject.Message, e.ExceptionObject.StackTrace);
            if (Logging.LogFile != null)
            {
                Logging.LogFile.Flush();
                //Logging.LogFile.Close();
            }


            MessageBox.Show(String.Format("An exception of type {0} was unhandled in the Opix.", e.ExceptionObject.GetType().Name), "Error", MessageBoxButton.OK);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        #region Static Session Data

        /// <summary>
        /// Checks if user is running the app in light theme or dark theme.
        /// </summary>
        /// <returns>True if dark theme; false otherwise.</returns>
        public static Boolean IsInDarkTheme()
        {
            return ((Visibility)Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
        }

        #endregion
    }
}