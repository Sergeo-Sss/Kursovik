using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace Kursovik
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadOwners();
            LoadCars();
            LoadPoliceOfficers();
            LoadServiceStaff();
            LoadInspections();
            LoadServiceCenters();
            LoadViolations();
            LoadEmployment();
        }

        #region Data Loading Methods
        private void LoadOwners()
        {
            var owners = _dbHelper.GetOwners();
            OwnersDataGrid.ItemsSource = owners;
        }

        private void LoadCars()
        {
            var cars = _dbHelper.GetCars();
            CarsDataGrid.ItemsSource = cars;
        }

        private void LoadPoliceOfficers()
        {
            var officers = _dbHelper.GetPoliceOfficers();
            PoliceOfficersDataGrid.ItemsSource = officers;
        }

        private void LoadServiceStaff()
        {
            var serviceStaff = _dbHelper.GetServiceStaff();
            ServiceStaffDataGrid.ItemsSource = serviceStaff;
        }

        private void LoadInspections()
        {
            var inspections = _dbHelper.GetInspections();
            InspectionsDataGrid.ItemsSource = inspections;
        }

        private void LoadServiceCenters()
        {
            var serviceCenters = _dbHelper.GetServiceCenters();
            ServiceCentersDataGrid.ItemsSource = serviceCenters;
        }

        private void LoadViolations()
        {
            var violations = _dbHelper.GetViolations();
            ViolationsDataGrid.ItemsSource = violations;
        }

        private void LoadEmployment()
        {
            var employmentList = _dbHelper.GetEmployment();
            EmploymentDataGrid.ItemsSource = employmentList;
        }
        #endregion

        #region Login Method
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                LoginForm.Visibility = Visibility.Hidden;
                MainTabControl.Visibility = Visibility.Visible;

                // Отображаем таблицу владельцев при входе по умолчанию
                ShowOwnersButton_Click(sender, e);
            }
            else
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Введите логин и пароль!");
            }
        }
        #endregion

        #region Owners Tab
        private void AddOwnerButton_Click(object sender, RoutedEventArgs e)
        {
            OwnersDataGrid.Visibility = Visibility.Collapsed;
            AddOwnerForm.Visibility = Visibility.Visible;
        }

        private void DeleteOwnerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is int ownerId)
            {
                _dbHelper.DeleteOwner(ownerId); // Удаление записи из базы данных
                MessageBox.Show("Владелец удален!");
                LoadOwners(); // Обновление данных в DataGrid после удаления
            }
        }

        private void UpdateOwnerButton_Click(object sender, RoutedEventArgs e)
        {
            // Сначала зафиксируйте изменения в DataGrid
            OwnersDataGrid.CommitEdit();
            OwnersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is int ownerId)
            {
                var owner = (Owner)OwnersDataGrid.SelectedItem;
                if (owner?.OwnerId != ownerId) return;

                // Проверка, чтобы все обязательные поля были заполнены
                if (string.IsNullOrEmpty(owner.FullName) || string.IsNullOrEmpty(owner.DriverLicenseNumber) ||
                    string.IsNullOrEmpty(owner.Address) || string.IsNullOrEmpty(owner.PhoneNumber))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                // Вызов метода для обновления данных в базе данных
                _dbHelper.UpdateOwner(owner);
                MessageBox.Show("Данные владельца обновлены!");
                LoadOwners(); // Обновляем таблицу
            }
        }

        private void ShowOwnersButton_Click(object sender, RoutedEventArgs e)
        {
            AddOwnerForm.Visibility = Visibility.Collapsed;
            OwnersDataGrid.ItemsSource = _dbHelper.GetOwners();
            OwnersDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveOwnerButton_Click(object sender, RoutedEventArgs e)
        {
            var newOwner = new Owner
            {
                FullName = OwnerFIO.Text,
                Address = OwnerAddress.Text,
                PhoneNumber = OwnerPhone.Text,
                BirthDate = DateTime.Parse(OwnerDOB.Text),
                DriverLicenseNumber = OwnerDriverLicenseNumber.Text
            };

            if (string.IsNullOrEmpty(newOwner.FullName) || string.IsNullOrEmpty(newOwner.DriverLicenseNumber) ||
                string.IsNullOrEmpty(newOwner.Address) || string.IsNullOrEmpty(newOwner.PhoneNumber))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddOwner(newOwner);
            MessageBox.Show("Владелец добавлен!");
            ShowOwnersButton_Click(sender, e);
        }
        #endregion

        #region Cars Tab
        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            CarsDataGrid.Visibility = Visibility.Collapsed;
            AddCarForm.Visibility = Visibility.Visible;
        }

        private void ShowCarsButton_Click(object sender, RoutedEventArgs e)
        {
            AddCarForm.Visibility = Visibility.Collapsed;
            CarsDataGrid.ItemsSource = _dbHelper.GetCars();
            CarsDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveCarButton_Click(object sender, RoutedEventArgs e)
        {
            var newCar = new Car
            {
                VIN = CarVIN.Text,
                Brand = CarBrand.Text,
                Model = CarModel.Text,
                Year = int.Parse(CarYear.Text),
                Color = CarColor.Text,
                LicensePlate = CarNumber.Text,
                OwnerID = int.Parse(OwnerID.Text)
            };

            if (string.IsNullOrEmpty(newCar.VIN) || string.IsNullOrEmpty(newCar.Brand) || string.IsNullOrEmpty(newCar.Model) ||
                newCar.Year == 0 || string.IsNullOrEmpty(newCar.Color) || string.IsNullOrEmpty(newCar.LicensePlate) || newCar.OwnerID == 0)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddCar(newCar);
            MessageBox.Show("Автомобиль добавлен!");
            ShowCarsButton_Click(sender, e);
        }
        #endregion

        #region Police Officers Tab
        private void AddPoliceOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            PoliceOfficersDataGrid.Visibility = Visibility.Collapsed;
            AddPoliceOfficerForm.Visibility = Visibility.Visible;
        }

        private void ShowPoliceOfficersButton_Click(object sender, RoutedEventArgs e)
        {
            AddPoliceOfficerForm.Visibility = Visibility.Collapsed;
            PoliceOfficersDataGrid.ItemsSource = _dbHelper.GetPoliceOfficers();
            PoliceOfficersDataGrid.Visibility = Visibility.Visible;
        }

        private void SavePoliceOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            var newOfficer = new PoliceOfficer
            {
                FullName = PoliceOfficerFIO.Text,
                Position = PoliceOfficerPosition.Text,
                Department = PoliceOfficerDepartment.Text,
                ContactDetails = PoliceOfficerContact.Text
            };

            if (string.IsNullOrEmpty(newOfficer.FullName) || string.IsNullOrEmpty(newOfficer.Position) ||
                string.IsNullOrEmpty(newOfficer.Department) || string.IsNullOrEmpty(newOfficer.ContactDetails))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddPoliceOfficer(newOfficer);
            MessageBox.Show("Сотрудник ГИБДД добавлен!");
            ShowPoliceOfficersButton_Click(sender, e);
        }
        #endregion

        #region Service Staff Tab
        private void AddServiceStaffButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceStaffDataGrid.Visibility = Visibility.Collapsed;
            AddServiceStaffForm.Visibility = Visibility.Visible;
        }

        private void ShowServiceStaffButton_Click(object sender, RoutedEventArgs e)
        {
            AddServiceStaffForm.Visibility = Visibility.Collapsed;
            ServiceStaffDataGrid.ItemsSource = _dbHelper.GetServiceStaff();
            ServiceStaffDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveServiceStaffButton_Click(object sender, RoutedEventArgs e)
        {
            var newStaff = new ServiceStaff
            {
                FullName = ServiceStaffFIO.Text,
                Position = ServiceStaffPosition.Text,
                ContactDetails = ServiceStaffContact.Text
            };

            if (string.IsNullOrEmpty(newStaff.FullName) || string.IsNullOrEmpty(newStaff.Position) || string.IsNullOrEmpty(newStaff.ContactDetails))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddServiceStaff(newStaff);
            MessageBox.Show("Сотрудник сервисного центра добавлен!");
            ShowServiceStaffButton_Click(sender, e);
        }
        #endregion

        #region Inspections Tab
        private void AddInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            InspectionsDataGrid.Visibility = Visibility.Collapsed;
            AddInspectionForm.Visibility = Visibility.Visible;
        }

        private void ShowInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            AddInspectionForm.Visibility = Visibility.Collapsed;
            InspectionsDataGrid.ItemsSource = _dbHelper.GetInspections();
            InspectionsDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            var newInspection = new Inspection
            {
                InspectionDate = InspectionDate.Text,
                Result = InspectionResult.Text,
                Notes = InspectionNotes.Text,
                VIN = InspectionVIN.Text,
                ServiceEmployeeId = int.TryParse(InspectionServiceEmployeeId.Text, out int employeeId) ? (int?)employeeId : null
            };

            if (string.IsNullOrEmpty(newInspection.InspectionDate) || string.IsNullOrEmpty(newInspection.Result) || string.IsNullOrEmpty(newInspection.VIN))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все обязательные поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddInspection(newInspection);
            MessageBox.Show("Техосмотр добавлен!");
            ShowInspectionsButton_Click(sender, e);
        }
        #endregion

        #region Violations Tab
        private void AddViolationButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationsDataGrid.Visibility = Visibility.Collapsed;
            AddViolationForm.Visibility = Visibility.Visible;
        }

        private void ShowViolationsButton_Click(object sender, RoutedEventArgs e)
        {
            AddViolationForm.Visibility = Visibility.Collapsed;
            ViolationsDataGrid.ItemsSource = _dbHelper.GetViolations();
            ViolationsDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveViolationButton_Click(object sender, RoutedEventArgs e)
        {
            var newViolation = new Violation
            {
                IssueDate = ViolationDate.Text,
                ViolationType = ViolationType.Text,
                FineAmount = int.Parse(ViolationFine.Text),
                VIN = ViolationCarVIN.Text,
                OfficerId = int.TryParse(ViolationOfficerId.Text, out int officerId) ? (int?)officerId : null
            };

            if (string.IsNullOrEmpty(newViolation.IssueDate) || string.IsNullOrEmpty(newViolation.ViolationType) ||
                newViolation.FineAmount == 0 || string.IsNullOrEmpty(newViolation.VIN))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все обязательные поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddViolation(newViolation);
            MessageBox.Show("Протокол добавлен!");
            ShowViolationsButton_Click(sender, e);
        }
        #endregion

        #region Service Centers Tab
        private void AddServiceCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceCentersDataGrid.Visibility = Visibility.Collapsed;
            AddServiceCenterForm.Visibility = Visibility.Visible;
        }

        private void ShowServiceCentersButton_Click(object sender, RoutedEventArgs e)
        {
            AddServiceCenterForm.Visibility = Visibility.Collapsed;
            ServiceCentersDataGrid.ItemsSource = _dbHelper.GetServiceCenters();
            ServiceCentersDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveServiceCenterButton_Click(object sender, RoutedEventArgs e)
        {
            var newServiceCenter = new ServiceCenter
            {
                License = ServiceCenterLicense.Text,
                Name = ServiceCenterName.Text,
                Address = ServiceCenterAddress.Text,
                ContactPerson = ServiceCenterContactPerson.Text
            };

            if (string.IsNullOrEmpty(newServiceCenter.License) || string.IsNullOrEmpty(newServiceCenter.Name) ||
                string.IsNullOrEmpty(newServiceCenter.Address) || string.IsNullOrEmpty(newServiceCenter.ContactPerson))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddServiceCenter(newServiceCenter);
            MessageBox.Show("Сервисный центр добавлен!");
            ShowServiceCentersButton_Click(sender, e);
        }
        #endregion

        #region Employment Tab
        private void AddEmploymentButton_Click(object sender, RoutedEventArgs e)
        {
            EmploymentDataGrid.Visibility = Visibility.Collapsed;
            AddEmploymentForm.Visibility = Visibility.Visible;
        }

        private void ShowEmploymentButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmploymentForm.Visibility = Visibility.Collapsed;
            EmploymentDataGrid.ItemsSource = _dbHelper.GetEmployment();
            EmploymentDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveEmploymentButton_Click(object sender, RoutedEventArgs e)
        {
            int? serviceEmployeeId = null;
            if (int.TryParse(EmploymentServiceEmployeeId.Text, out int parsedEmployeeId))
            {
                serviceEmployeeId = parsedEmployeeId;
            }

            var newEmployment = new Employment
            {
                ServiceCenterLicense = EmploymentServiceCenterLicense.Text,
                ServiceEmployeeId = serviceEmployeeId ?? 0
            };

            if (string.IsNullOrEmpty(newEmployment.ServiceCenterLicense) || newEmployment.ServiceEmployeeId == 0)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddEmployment(newEmployment);
            MessageBox.Show("Связь добавлена!");
            ShowEmploymentButton_Click(sender, e);
        }
        #endregion

        #region Request Methods
        private void Request1Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 1
        }

        private void Request2Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 2
        }

        private void Request3Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 3
        }

        private void Request4Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 4
        }

        private void Request5Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 5
        }

        private void Request6Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 6
        }

        private void Request7Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 7
        }

        private void Request8Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 8
        }

        private void Request9Button_Click(object sender, RoutedEventArgs e)
        {
            // Логика для Запроса 9
        }
        #endregion
    }
}