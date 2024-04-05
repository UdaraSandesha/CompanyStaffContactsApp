using AutoMapper;
using CompanyStaffContactsApp.DataAccess;
using CompanyStaffContactsApp.DataAccess.Services;
using CompanyStaffContactsApp.Dtos;
using CompanyStaffContactsApp.Enums;
using CompanyStaffContactsApp.Models;
using CsvHelper;
using CsvHelper.Configuration;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Globalization;

namespace CompanyStaffContactsApp
{
    public partial class StaffContactForm : Form
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;

        public StaffContactForm(IMapper mapper, IStaffService staffService)
        {
            InitializeComponent();
            _staffService = staffService;
            _mapper = mapper;
        }

        private async void StaffContactForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
            CustomizeDataGridView();
        }

        private async Task LoadDataAsync()
        {
            using (var context = new AppDbContext())
            {
                var staffEntities = await _staffService.GetStaffAsync();
                var staffDtos = _mapper.Map<List<StaffDto>>(staffEntities);

                dataGridView1.DataSource = staffDtos;
            }
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            var staffDtos = dataGridView1.DataSource as List<StaffDto>;
            if (staffDtos == null) return;

            await _staffService.SaveChangesAsync(staffDtos);

            await LoadDataAsync();
        }

        private async void Active_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var staffEntities = new List<Staff>();
                if (checkBox1.Checked)
                {
                    staffEntities = await _staffService.GetStaffAsync(true);
                    var staffDtos = _mapper.Map<List<StaffDto>>(staffEntities);

                    dataGridView1.DataSource = staffDtos;
                }
                else
                {
                    staffEntities = await _staffService.GetStaffAsync();
                    var staffDtos = _mapper.Map<List<StaffDto>>(staffEntities);

                    dataGridView1.DataSource = staffDtos;
                }
            }
        }

        private void CustomizeDataGridView()
        {
            if (dataGridView1.Columns["Status"] != null)
            {
                dataGridView1.Columns.Remove("Status");
            }
            if (dataGridView1.Columns["StaffType"] != null)
            {
                dataGridView1.Columns.Remove("StaffType");
            }

            DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                DataSource = Enum.GetValues(typeof(StaffStatus)),
                ValueType = typeof(StaffStatus),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewComboBoxColumn typeColumn = new DataGridViewComboBoxColumn
            {
                Name = "StaffType",
                HeaderText = "StaffType",
                DataPropertyName = "StaffType",
                DataSource = Enum.GetValues(typeof(StaffType)),
                ValueType = typeof(StaffType),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            dataGridView1.Columns.Add(statusColumn);
            dataGridView1.Columns.Add(typeColumn);
        }

        private async void export_Click(object sender, EventArgs e)
        {
            var staffEntities = await _staffService.GetStaffAsync();
            var staffDtos = _mapper.Map<List<StaffDto>>(staffEntities);

            var groupedAndOrdered = staffDtos
                .GroupBy(staff => staff.StaffType)
                .SelectMany(group => group.OrderBy(staff => staff.FirstName));

            string fileName = $"StaffContact{DateTime.Now:dd-MM-yyyyTHH-mm-ss}.csv";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(groupedAndOrdered);
            }
        }

        private async void Print_Click(object sender, EventArgs e)
        {
            var staffEntities = await _staffService.GetStaffAsync();
            var staffDtos = _mapper.Map<List<StaffDto>>(staffEntities);

            var groupedAndOrderedData = staffDtos
                .GroupBy(staff => staff.StaffType)
                .OrderBy(group => group.Key)
                .Select(group => new
                {
                    StaffType = group.Key,
                    StaffMembers = group.OrderBy(staff => staff.FirstName).ToList()
                })
                .ToList();

            string fileName = $"StaffContact{DateTime.Now:dd-MM-yyyyTHH-mm-ss}.pdf";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            using (var writer = new PdfWriter(filePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    foreach (var group in groupedAndOrderedData)
                    {
                        document.Add(new Paragraph(group.StaffType.ToString())
                            .SetBold()
                            .SetFontSize(14));

                        foreach (var staffMember in group.StaffMembers)
                        {
                            var info = $"{staffMember.FirstName} {staffMember.LastName} - {staffMember.Title}";
                            document.Add(new Paragraph(info));
                        }

                        document.Add(new AreaBreak());
                    }
                }
            }
        }
    }
}