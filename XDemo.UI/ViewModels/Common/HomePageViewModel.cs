using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Dtos.Patients;
using XDemo.Core.BusinessServices.Interfaces.Patients;
using XDemo.Core.Extensions;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels.Common
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly IPatientService _patientService;
        private IList<PatientDto> _allPatients;
        public List<string> DataTest { get; set; }


        public HomePageViewModel(IPatientService patientService, INavigationService navigationService) : base(navigationService)
        {
            _patientService = patientService;
            this.Title = "Home";

            DataTest = new List<string>
            {
                "1","2","3","4","5","6"
            };
        }

        #region Overrides

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await GetAllPatients();
            SelectedPatient = Patients.FirstOrDefault();
            base.OnNavigatedTo(parameters);
        }

        private async Task GetAllPatients()
        {
            _allPatients = await _patientService.Find(null);
            Patients = _allPatients.ToList();
        }
        #endregion

        #region Properties
        public bool SelectMode { get; set; }

        string _searchPatientCode;

        public string SearchPatientCode
        {
            get => _searchPatientCode;

            set
            {
                _searchPatientCode = value;
                if (value.IsNullOrEmpty())
                    SearchCommand.Execute(null);
            }
        }

        public List<PatientDto> Patients { get; set; }

        public PatientDto SelectedPatient { get; set; }


        #endregion

        #region Commands
        #region CancelCommand

        private ICommand _CancelCommand;

        public ICommand CancelCommand => _CancelCommand ?? (_CancelCommand = new Command(async () => { await CancelCommandExecute(); }, CancelCommandCanExecute));

        private bool CancelCommandCanExecute()
        {
            return SelectMode;
        }

        private async Task CancelCommandExecute()
        {
            await PopAsync();
        }

        #endregion

        #region SearchCommand

        private ICommand _searchCommand;

        /// <summary>
        /// Gets the SearchCommand command.
        /// </summary>
        public ICommand SearchCommand => _searchCommand ??
                                        (_searchCommand = new Command(OnSearchCommandExecute,
                                            CanExecuteSearchCommand));

        private bool CanExecuteSearchCommand()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the SearchCommand command is executed.
        /// </summary>
        private void OnSearchCommandExecute()
        {
            try
            {
                IsBusy = true;
                Patients = _allPatients.Where(aa => aa.SearchField != null && aa.SearchField.Contains(SearchPatientCode ?? string.Empty)).ToList();
                SelectedPatient = Patients.FirstOrDefault();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #endregion
    }
}