using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentManagementV1._5.Services;

namespace StudentManagementV1._5.ViewModels
{
    public class MySubjectsViewModel 
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        public MySubjectsViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            // You can add logic to load subjects here
        }

        public MySubjectsViewModel()
        {

        }

    }
}
