using System;
using System.Threading.Tasks;
using System.Windows;
using BackUP_System.Model;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Windows.Controls;
using System.Windows.Media;

namespace BackUP_System
{
    public partial class MainWindow : Window
    {
        public BackupModel BackupModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            BackupModel = new BackupModel();
            DataContext = BackupModel;
        }

        private void BrowseSource_ClickFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                BackupModel.SourceDirectory = dialog.FileName;
            }
            sourcePathDis.Text = BackupModel.SourceDirectory;
            Console.WriteLine($"From Filename: {BackupModel.SourceDirectory}, Value: {sourcePathDis.Text}");

        }

        private void BrowseDestination_ClickFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                BackupModel.DestinationDirectory = dialog.FileName;
            }
            Console.WriteLine($"To Filename: {BackupModel.DestinationDirectory}, Value: {destPathDis.Text}");
            destPathDis.Text = BackupModel.DestinationDirectory;

            
        }
        
        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            dialog.InitialDirectory = "C:\\";

            WinForms.DialogResult result = dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)            {
                BackupModel.SourceDirectory = dialog.SelectedPath;
            }
            sourcePathDis.Text = BackupModel.SourceDirectory;
            Console.WriteLine($"From Filename: {BackupModel.SourceDirectory}, Value: {sourcePathDis.Text}");

        }

        
        private void BrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            dialog.InitialDirectory = "D:\\Backup-AKMH";
            
            WinForms.DialogResult result = dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                BackupModel.DestinationDirectory = dialog.SelectedPath;
            }
            Console.WriteLine($"To Directory: {BackupModel.DestinationDirectory}, Value: {destPathDis.Text}");
            destPathDis.Text = BackupModel.DestinationDirectory;
        }


        
        
        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (TogglePasswordVisibility.IsChecked == true)
            {
                mailPasswordVisible.Visibility = Visibility.Visible;
                mailPasswordVisible.Text = mailPassword.Password;
                mailPassword.Visibility = Visibility.Collapsed;
                // Update the TextBlock within the ToggleButton to show an "eye-slash" or similar icon
                ((TextBlock)TogglePasswordVisibility.Content).Visibility = Visibility.Visible;
                ((TextBlock)TogglePasswordVisibility.Content).Text = "\uE721"; // Unicode for eye-slash icon, replace with actual icon if available
            }
            else
            {
                mailPassword.Visibility = Visibility.Visible;
                mailPassword.Password = mailPasswordVisible.Text;
                mailPasswordVisible.Visibility = Visibility.Collapsed;
                // Update the TextBlock within the ToggleButton to show an "eye" icon
                ((TextBlock)TogglePasswordVisibility.Content).Text = "\uE722"; // Unicode for eye icon, replace with actual icon if available
            }
        }
        private async void StartBackup_Click(object sender, RoutedEventArgs e)
{
    // Disable the start backup button to prevent multiple clicks
    ((Button)sender).IsEnabled = false;

    // Show the progress bar
    progressBar.Visibility = Visibility.Visible;

    // Reset the error message block
    errorMessageBlock.Visibility = Visibility.Collapsed;
    errorMessageBlock.Text = string.Empty;

    // Set up the backup configuration
    bool checkBoxZip = toZip.IsChecked ?? false;
    BackupModel.SecureSave = checkBoxZip;
    BackupModel.Username = mailAddress.Text;
    BackupModel.Password = mailPassword.Password;

    try
    {
        // Run the backup operation on a background thread
        bool success = await Task.Run(() => BackupModel.makeBackup());

        // Once the backup is finished, update the UI accordingly on the UI thread
        Dispatcher.Invoke(() =>
        {
            if (success)
            {
                errorMessageBlock.Text = $"Mail and Files successfully saved in: {BackupModel.DestinationDirectory}";
                errorMessageBlock.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                // Handle the case where backup is not successful
                errorMessageBlock.Text = "Backup failed. Please check the logs for details.";
                errorMessageBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
            errorMessageBlock.Visibility = Visibility.Visible;
        });
    }
    catch (Exception exception)
    {
        // If an exception was thrown, display the error message
        Dispatcher.Invoke(() =>
        {
            errorMessageBlock.Text = exception.Message;
            errorMessageBlock.Foreground = new SolidColorBrush(Colors.Red);
            errorMessageBlock.Visibility = Visibility.Visible;
        });
    }
    finally
    {
        // Hide the progress bar and re-enable the start backup button
        Dispatcher.Invoke(() =>
        {
            progressBar.Visibility = Visibility.Collapsed;
            ((Button)sender).IsEnabled = true;
        });
    }
}

        
        private void StartWork()
        {
            progressBar.Visibility = Visibility.Visible; // Show the progress bar
            Console.WriteLine($" Progress Bar{ progressBar.IsVisible}");
            // Start the work
        }

        private void WorkCompleted()
        {
            // Work is done
            progressBar.Visibility = Visibility.Collapsed; // Hide the progress bar
        }


        
    }
}