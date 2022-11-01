using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace neolink.Services
{
    public static class FileManager
    {
        //public static string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp.txt");

        public static string LoadEmbeddedFile(string filename)
        {
            string readData = "";
            try
            {
                string readFileName = filename;

                System.Reflection.Assembly Assembly = IntrospectionExtensions.GetTypeInfo(typeof(DataReader)).Assembly;

                Stream stream = Assembly.GetManifestResourceStream($"neolink.{readFileName}");

                using (var reader = new System.IO.StreamReader(stream))
                {
                    readData = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
            }

            return readData;
        }

        public static string LoadLocalFile(string filename)
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
                string data = File.ReadAllText(fileName);
                return data;
            }
            catch (Exception e)
            {
            }

            return "";
        }


        public static bool SaveEmbeddedFile(string filename, string text)
        {

            string readData = "";
            try
            {
                string readFileName = filename;

                System.Reflection.Assembly Assembly = IntrospectionExtensions.GetTypeInfo(typeof(DataReader)).Assembly;

                Stream stream = Assembly.GetManifestResourceStream($"neolink.{readFileName}");

                using (var reader = new System.IO.StreamWriter(stream))
                {
                    reader.Write(text);
                }

                return true;
            }
            catch (Exception e)
            {
            }
            return false;

            /*
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
                File.WriteAllText(fileName, text);
                return true;
            }
            catch (Exception e)
            { }
            return false;*/

        }

        public static bool SaveFile(string filename, string text)
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
                File.WriteAllText(fileName, text);
                return true;
            }
            catch (Exception e)
            { }
            return false;

        }

        public static List<string> GetLocalNeoLinkFiles()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var list = assembly.GetManifestResourceNames();

            List<string> fileList = new List<string>();

            foreach (var item in list)
            {
                //if (item.Contains("MyFolder"))
                //{
                    fileList.Add(item);
                //}
            }
            return fileList;
        }

        public static async Task<FileResult> Pick(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();
                        var Image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        /*public static async Task<string> PickResource()
        {

            var fileManager = ((FileManager)Modules.ModuleManager.GetModule(Modules.Modules.Document));
            FileResult File = await fileManager.Pick(Xamarin.Essentials.PickOptions.Default);
            if (File != null)
            {
                return File.FullPath;
            }

            return null;
        }*/

        /*
        Button loadButton, saveButton;

        public FileManager()
        {
            var input = new Entry { Text = "" };
            var output = new Label { Text = "" };
            saveButton = new Button { Text = "Save" };

            saveButton.Clicked += (sender, e) =>
            {
                loadButton.IsEnabled = saveButton.IsEnabled = false;
                File.WriteAllText(fileName, input.Text);
                loadButton.IsEnabled = saveButton.IsEnabled = true;
            };

            loadButton = new Button { Text = "Load" };
            loadButton.Clicked += (sender, e) =>
            {
                loadButton.IsEnabled = saveButton.IsEnabled = false;
                output.Text = File.ReadAllText(fileName);
                loadButton.IsEnabled = saveButton.IsEnabled = true;
            };
            loadButton.IsEnabled = File.Exists(fileName);

            var buttonLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = { saveButton, loadButton }
            };

            Content = new StackLayout
            {
                Margin = new Thickness(20),
                Children =
                {
                    new Label
                    {
                        Text = "Save and Load Text",
                        FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
                        FontAttributes = FontAttributes.Bold
                    },
                    new Label { Text = "Type below and press Save, then Load" },
                    input,
                    buttonLayout,
                    output
                }
            };
        }*/
    }
}


