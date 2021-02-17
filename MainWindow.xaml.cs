using ImportOpenCNPJ;
using ImportOpenCNPJ.Helpers;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImportOpenCNPJNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private long TotalRegisters { get; set; } = 0;
        private IEnumerable<string> AvailableDatabases = new List<string>();
        private IEnumerable<ImportRegisters> AvailableRegisters = new List<ImportRegisters>()
        {
            // Company
            new ImportRegisters()
            {
                RegistryNumber = 1,
                RegistryName = "Empresa",
                TableName = "empresas"
            },
            // Partners
            new ImportRegisters()
            {
                RegistryNumber = 2,
                RegistryName = "Socio",
                TableName = "socios"
            },
            // CNAE
            new ImportRegisters()
            {
                RegistryNumber = 6,
                RegistryName = "CNAE",
                TableName = "cnaes_secundarias"
            }
        };
        private List<ImportFileLine> FilesToImport = new List<ImportFileLine>();
        private ImportFileLine CurrentFileLineInProcess;

        public MainWindow()
        {
            InitializeComponent();
            dbType.ItemsSource = Enum.GetValues(typeof(Databases));
            importParams.ItemsSource = this.AvailableRegisters;
            importFiles.ItemsSource = this.FilesToImport;
        }

        private void Connectbtn_Click(object sender, RoutedEventArgs e)
        {
            var connString = "Host=" + dbServer.Text + ";Username=" + dbUser.Text + ";Password=" + dbPass.Password + ";Database=postgres";
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    List<string> availableDatabases = new List<string>();
                    using (var cmd = new NpgsqlCommand("SELECT datname FROM pg_database", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            availableDatabases.Add(reader.GetString(0));

                    this.AvailableDatabases = availableDatabases;
                    avalidableDatabases.ItemsSource = this.AvailableDatabases;
                    avalidableDatabases.IsEnabled = true;
                    successIcon.Visibility = Visibility.Visible;
                    importParams.IsEnabled = true;
                    addFileBtn.IsEnabled = true;
                    conn.Close();
                }
            }
            catch (Exception except)
            {
                MessageBox.Show(                   
                    except.Message,
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImportFileLine newSelectedFile = new ImportFileLine()
                {
                    FilePath = openFileDialog.FileName,
                    Progress = 0
                };

                this.FilesToImport.Add(newSelectedFile);
                CollectionViewSource.GetDefaultView(importFiles.ItemsSource).Refresh();
                btnLoadMetadata.IsEnabled = true;
            }
        }

        #region METADATA

        private void BtnLoadMetadata_Click(object sender, RoutedEventArgs e)
        {
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                worker.WorkerReportsProgress = true;
                worker.DoWork += this.LoadMetadata;
                worker.ProgressChanged += this.MetadataProgress;
                worker.RunWorkerCompleted += this.LoadedMetadata;
                worker.RunWorkerAsync(this.FilesToImport);
            }
            addFileBtn.IsEnabled = true;
        }

        private void LoadMetadata(object sender, DoWorkEventArgs e)
        {
            var filesToImport = (List<ImportFileLine>)e.Argument;
            try
            {
                e.Result = filesToImport;
                foreach (var fileLine in filesToImport)
                {
                    var selectedFile = fileLine;
                    int totalCompanies = 0;
                    int totalPartners = 0;
                    int totalCNAEs = 0;

                    selectedFile.LoadingMetaData = Visibility.Visible;
                    CollectionViewSource.GetDefaultView(importFiles.ItemsSource).Refresh();

                    var currentFile = fileLine;

                    if (fileLine == null)
                    {
                        (sender as BackgroundWorker)
                            .ReportProgress(
                                100,
                                new MetadataUpdateState()
                                { UpdateText = "Arquivo Inválido" });
                        return;
                    }

                    (sender as BackgroundWorker)
                        .ReportProgress(
                            0,
                            new MetadataUpdateState()
                            { UpdateText = "Carregando Metadados - " + currentFile.FilePath });

                    using (StreamReader fs = new StreamReader(currentFile.FilePath))
                    {
                        while (!fs.EndOfStream)
                        {
                            var line = fs.ReadLine();
                            var lineType = line.Substring(0, 1);
                            switch (lineType)
                            {
                                case "1":
                                    totalCompanies++;
                                    break;
                                case "2":
                                    totalPartners++;
                                    break;
                                case "6":
                                    totalCNAEs++;
                                    break;
                            }
                        }
                    }

                    fileLine.TotalCompanies = totalCompanies;
                    fileLine.TotalPartners = totalPartners;
                    fileLine.TotalCNAEs = totalCNAEs;
                    fileLine.Progress = 100;
                    fileLine.LoadingMetaData = Visibility.Collapsed;
                    fileLine.LoadedMetaData = Visibility.Visible;

                    (sender as BackgroundWorker)
                        .ReportProgress(
                            100,
                            new MetadataUpdateState()
                            {
                                UpdateText = "Leitura concluída - " + currentFile.FilePath
                            });
                }

                (sender as BackgroundWorker)
                    .ReportProgress(
                        100,
                        new MetadataUpdateState()
                        {
                            UpdateText = "Metadados Carregados com sucesso."
                        });

            }
            catch (Exception excep)
            {
                (sender as BackgroundWorker)
                    .ReportProgress(
                        100,
                        new MetadataUpdateState()
                        {
                            UpdateText = excep.Message
                        });
                return;
            }
        }

        private void MetadataProgress(object sender, ProgressChangedEventArgs e)
        {
            var totalFiles = this.FilesToImport.Count();
            if (e.ProgressPercentage > 0)
            {
                this.totalProgress.Value += (1 / totalFiles) * 100;
            }
            this.importStatusText.Text = (e.UserState as MetadataUpdateState).UpdateText ?? "";
        }

        private void LoadedMetadata(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                this.btnLoadMetadata.IsEnabled = false;
                this.addFileBtn.IsEnabled = false;
                this.startImportBtn.IsEnabled = true;
                CollectionViewSource.GetDefaultView(importFiles.ItemsSource).Refresh();
            }
        }

        private void UpdateMetadataTotals(int totalCompanies, int totalPartners, int totalCNAEs)
        {
            this.CurrentFileLineInProcess.TotalCompanies = totalCompanies;
            this.CurrentFileLineInProcess.TotalPartners = totalPartners;
            this.CurrentFileLineInProcess.TotalCNAEs = totalCNAEs;
            this.TotalRegisters = totalCompanies + totalPartners + totalCNAEs;
        }
        #endregion
        
        #region IMPORT
        private void StartImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var connString = "Host=" + dbServer.Text + ";Username=" + dbUser.Text + ";Password=" + dbPass.Password + ";Database=" + avalidableDatabases.SelectedValue.ToString();
            var startParams = new ImportStartParams()
            {
                ConnString = connString,
                FilesToImport = this.FilesToImport,
                RegisterParams = this.AvailableRegisters
            };

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += this.ImportProcess;
            worker.ProgressChanged += this.ImportProgress;
            worker.RunWorkerCompleted += this.ImportProcessFinished;
            worker.RunWorkerAsync(startParams);
        }

        private void ImportProcess(object sender, DoWorkEventArgs e)
        {
            try
            {
                var startParams = e.Argument as ImportStartParams;
                e.Result = startParams.FilesToImport;

                Dictionary<string, string> tableNames = startParams.RegisterParams.ToDictionary(x => x.RegistryName, y => y.TableName);

                using (var conn = new NpgsqlConnection(startParams.ConnString))
                {
                    conn.Open();
                    foreach (var table in AvailableRegisters)
                    {
                        if (table.ResetData)
                        {
                            TablesHelper.ClearTable(conn, table.TableName);
                        }

                        if (table.CreateTable)
                        {
                            switch (table.RegistryName)
                            {
                                case "Empresa":
                                    TablesHelper.CreateCompaniesTable(conn, table.TableName);
                                    break;
                                case "Socio":
                                    TablesHelper.CreatePartnersTable(conn, table.TableName);
                                    break;
                                case "CNAE":
                                    TablesHelper.CreateCNAEsTable(conn, table.TableName);
                                    break;
                            }
                        }
                    }

                    foreach (var fileLine in startParams.FilesToImport)
                    {

                        ImportFileLine currentFileLine = fileLine;

                        (sender as BackgroundWorker)
                            .ReportProgress(
                                0,
                                new MetadataUserState()
                                { UpdateText = currentFileLine.FilePath + " - Iniciando" });

                        int counter = 0;
                        long imported = 0;
                        long totalLines = fileLine.TotalCompanies + fileLine.TotalPartners + fileLine.TotalCNAEs;

                        using (StreamReader fs = new StreamReader(currentFileLine.FilePath))
                        {
                            while (!fs.EndOfStream)
                            {
                                var line = fs.ReadLine();
                                var lineTyped = LineFactory.ExtractLine(line, tableNames);
                                if (lineTyped != null)
                                {
                                    lineTyped.UploadLine(conn);
                                }
                                imported++;
                                counter++;
                                if (counter >= 1000)
                                {
                                    counter = 0;
                                    currentFileLine.Progress = Convert.ToInt32(Math.Ceiling(((double)imported / totalLines) * 100));
                                    (sender as BackgroundWorker)
                                        .ReportProgress(
                                            Convert.ToInt32(Math.Ceiling(((double)imported / totalLines) * 100)),
                                            new MetadataUserState()
                                            { UpdateText = currentFileLine.FilePath + " - Importando registro " + imported + " de " + totalLines });
                                }
                            }

                            (sender as BackgroundWorker)
                                .ReportProgress(
                                    100,
                                    new MetadataUserState()
                                    { UpdateText = currentFileLine.FilePath + " - Registros Importados com sucesso" });
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception excep)
            {
                (sender as BackgroundWorker)
                    .ReportProgress(
                        100,
                        new MetadataUserState()
                        { UpdateText = excep.Message });
                return;
            }
        }

        private void ImportProgress(object sender, ProgressChangedEventArgs e)
        {
            this.totalProgress.Value = e.ProgressPercentage;
            this.importStatusText.Text = (e.UserState as MetadataUserState).UpdateText ?? "";
        }

        private void ImportProcessFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                this.startImportBtn.IsEnabled = false;
            }
        }
        #endregion
    }

    public class ImportRegisters
    {
        public int RegistryNumber { get; set; } = 0;
        public string RegistryName { get; set; } = "";
        public string TableName { get; set; } = "";
        public bool CreateTable { get; set; } = false;
        public bool ResetData { get; set; } = false;
    }

    public class ImportFileLine
    {
        public string FilePath { get; set; } = "";
        public int TotalCompanies { get; set; } = 0;
        public int TotalPartners { get; set; } = 0;
        public int TotalCNAEs { get; set; } = 0;
        public int Progress { get; set; } = 0;
        public Visibility LoadingMetaData { get; set; } = Visibility.Collapsed;
        public Visibility LoadingData { get; set; } = Visibility.Collapsed;
        public Visibility LoadedMetaData { get; set; } = Visibility.Collapsed;
    }

    class MetadataUserState
    {
        public string UpdateText;
    }
    class MetadataUpdateState
    {
        public ImportFileLine CurrentFileToImport;
        public string UpdateText { get; set; } = "";
        public int TotalCompanies { get; set; } = 0;
        public int TotalPartners { get; set; } = 0;
        public int TotalCNAEs { get; set; } = 0;
    }

    class ImportStartParams
    {
        public IEnumerable<ImportFileLine> FilesToImport;
        public IEnumerable<ImportRegisters> RegisterParams;
        public string ConnString { get; set; } = "";
    }

}
