using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.Core.BusinessServices.Interfaces.Patients;
using XDemo.Core.Extensions;

namespace XDemo.Core.BusinessServices.Implementations.Patients
{
    public class PatientService : IPatientService
    {
        IList<PatientDto> _patients = new List<PatientDto>
            {
                new PatientDto
                {
                    PatientCode="Code1",
                    FullName="Patient 1", 
                    SearchField="Patient1"
                },
                new PatientDto
                {
                    PatientCode="Code2",
                    FullName="Patient 2",
                    SearchField="Patient2"
            }};
        private readonly ISecurityService _securityService;
        public PatientService(ISecurityService securityService)
        {
            //example for services injection together
            _securityService = securityService;
        }

        public Task<IList<PatientDto>> Find(string patientCode)
        {
            return Task.FromResult(patientCode.IsNullOrEmpty() ? _patients : _patients.Where(aa => aa.PatientCode == patientCode).ToList());
        }

        public PatientDto Get(string patientCode)
        {
            return _patients.FirstOrDefault(aa => aa.PatientCode == patientCode);
        }
    }
}