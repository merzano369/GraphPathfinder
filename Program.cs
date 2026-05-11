using System;
using System.Windows.Forms;

namespace GraphPathfinder
{
    /// <summary>
    /// Application entry point. Configures global exception handling and launches the main form.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// Initializes Windows Forms, configures exception handling, and runs the main form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (sender, args) =>
            {
                MessageBox.Show($"Interface error occurred: {args.Exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Critical error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}