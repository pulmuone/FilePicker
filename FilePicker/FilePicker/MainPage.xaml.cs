using ExcelDataReader;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FilePicker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void PickFile_Clicked(object sender, EventArgs args)
        {
            await PickAndShowFile(null);
        }

        private async void PickImage_Clicked(object sender, EventArgs args)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "image/png", "image/jpeg" };
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] { "public.image" }; // same as iOS constant UTType.Image
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".jpg", ".png" };
            }

            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] { "JPEG files (*.jpg)|*.jpg", "PNG files (*.png)|*.png" };
            }

            await PickAndShowFile(fileTypes);
        }

        private async Task PickAndShowFile(string[] fileTypes)
        {
            try
            {
                var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);

                if (pickedFile != null)
                {
                    FileNameLabel.Text = pickedFile.FileName;
                    FilePathLabel.Text = pickedFile.FilePath;

                    string path = Path.Combine(pickedFile.FilePath, pickedFile.FileName);
                    ReadExcel(pickedFile.GetStream());


                    if (pickedFile.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                        || pickedFile.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        FileImagePreview.Source = ImageSource.FromStream(() => pickedFile.GetStream());
                        FileImagePreview.IsVisible = true;
                    }
                    else
                    {
                        FileImagePreview.IsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                FileNameLabel.Text = ex.ToString();
                FilePathLabel.Text = string.Empty;
                FileImagePreview.IsVisible = false;
            }
        }

        private void ReadExcel(Stream fileStream)
        {
            //using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            //{
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(fileStream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    //do
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine(reader.GetString(0));
                    //        Console.WriteLine(reader.GetString(1));
                    //        Console.WriteLine(reader.GetString(2));

                    //        // reader.GetDouble(0);
                    //    }
                    //} while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet();

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        Debug.WriteLine(row[0].ToString());
                        Debug.WriteLine(row[1].ToString());
                        Debug.WriteLine(row[2].ToString());
                    }


                    // The result of each spreadsheet is in result.Tables
                }
            //}
        }
    }
}
