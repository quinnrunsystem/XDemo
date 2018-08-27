using System.Collections.Generic;
using System.Threading.Tasks;

namespace XDemo.Core.BusinessServices.Interfaces.Patients
{
    public interface IPatientService
    {
        Task<IList<PatientDto>> Find(string patientCode);
     
        PatientDto Get(string patientCode);
    }
}