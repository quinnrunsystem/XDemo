using System;

namespace XDemo.Core.BusinessServices.Dtos.Patients
{
    public class PatientDto
    {
        public string PatientCode { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Pid { get; set; }
        public DateTime DoB { get; set; }
        public string SearchField { get; set; }
    }
}