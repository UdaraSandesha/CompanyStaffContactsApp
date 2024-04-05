using CompanyStaffContactsApp.Enums;

namespace CompanyStaffContactsApp.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public StaffType StaffType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string OfficeExtension { get; set; }
        public string IRDNumber { get; set; }
        public StaffStatus Status { get; set; }
        public int? ManagerId { get; set; }
        public Staff Manager { get; set; }
    }
}