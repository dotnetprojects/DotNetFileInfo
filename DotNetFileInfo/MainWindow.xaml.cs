using dnlib.DotNet;
using Microsoft.Win32;
using System.Collections;
using System.Windows;

namespace DotNetFileInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog() { Filter = "*.exe;*.dll|*.exe;*.dll" };
            if (dlg.ShowDialog() == true)
            {
                LoadFile(dlg.FileName);
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileList != null && fileList.Length > 0)
            {
                LoadFile(fileList[0]);
            }
        }

        private void LoadFile(string file)
        {
            try
            {
                grdAssembly.ItemsSource = null;
                grdModule.ItemsSource = null;
                this.Title = "DotNetFileInfo - " + file;
                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                using (ModuleDefMD module = ModuleDefMD.Load(file, modCtx))
                {
                    grdAssembly.ItemsSource = GetPropertiesAsList(module.Assembly);
                    grdModule.ItemsSource = GetPropertiesAsList(module.Assembly.Modules[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private IEnumerable GetPropertiesAsList(Object obj)
        {
            return obj.GetType().GetProperties().Select(x => new PropInfo { Name = x.Name, Value = x.GetValue(obj)?.ToString() ?? "null" });
        }

        public class PropInfo
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}