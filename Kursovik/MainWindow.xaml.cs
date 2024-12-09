using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursovik
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private Button _activeButton;

        public MainWindow()
        {
            InitializeComponent();
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
            string userLogin = LoginTextBox.Text;
            string userPassword = PasswordBox.Password;

            if (!string.IsNullOrEmpty(userLogin) && !string.IsNullOrEmpty(userPassword))
            {
                try
                {
                    // Формируем строку подключения
                    string connectionString = $"Host=172.20.7.54;Port=5432;Username={userLogin};Password={userPassword};Database=db2093_01";

                    using (var connection = new Npgsql.NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        LoginForm.Visibility = Visibility.Hidden;
                        MainTabControl.Visibility = Visibility.Visible;

                        // Инициализируем DatabaseHelper
                        _dbHelper = new DatabaseHelper(connectionString);

                        // Проверяем права доступа
                        ConfigureButtonAccess();

                        // Загружаем данные только для доступных таблиц
                        if (isTabItemEnabled("Владельцы автомобилей")) LoadOwners();
                        if (isTabItemEnabled("Автомобили")) LoadCars();
                        if (isTabItemEnabled("Сотрудники ГАИ")) LoadPoliceOfficers();
                        if (isTabItemEnabled("Сотрудники сервисных центров")) LoadServiceStaff();
                        if (isTabItemEnabled("Техосмотры")) LoadInspections();
                        if (isTabItemEnabled("Сервисные центры")) LoadServiceCenters();
                        if (isTabItemEnabled("Протоколы нарушений")) LoadViolations();
                        if (isTabItemEnabled("Работа")) LoadEmployment();

                        // Отображаем таблицу владельцев, если доступна
                        if (isTabItemEnabled("Владельцы автомобилей"))
                        {
                            ShowOwnersButton_Click(sender, e);
                        }
                        else
                        {
                            ShowServiceStaffButton_Click(sender, e);
                        }

                        //Проверяем доступ к запросам
                        RequestsCheckAuthority();
                    }
                }
                catch (Exception ex)
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                }
            }
            else
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Введите логин и пароль!");
            }
        }
        private bool isTabItemEnabled(string header)
        {
            foreach (TabItem tabItem in MainTabControl.Items)
            {
                if (tabItem.Header.ToString() == header)
                {
                    return tabItem.IsVisible;
                }
            }
            return false;
        }

        private void ConfigureButtonAccess()
        {
            try
            {
                SetTabItemByHeader("Владельцы автомобилей", _dbHelper.HasSelectPermission("owners"));
                SetTabItemByHeader("Автомобили", _dbHelper.HasSelectPermission("cars"));
                SetTabItemByHeader("Сотрудники ГАИ", _dbHelper.HasSelectPermission("trafficofficers"));
                SetTabItemByHeader("Сотрудники сервисных центров", _dbHelper.HasSelectPermission("serviceemployees"));
                SetTabItemByHeader("Техосмотры", _dbHelper.HasSelectPermission("inspections"));
                SetTabItemByHeader("Сервисные центры", _dbHelper.HasSelectPermission("servicecenters"));
                SetTabItemByHeader("Протоколы нарушений", _dbHelper.HasSelectPermission("protocols"));
                SetTabItemByHeader("Работа", _dbHelper.HasSelectPermission("employment"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки прав доступа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TabItem SetTabItemByHeader(string header, bool hasAccess)
        {
            foreach (TabItem tabItem in MainTabControl.Items)
            {
                if (tabItem.Header.ToString() == header)
                {
                    tabItem.Visibility = (hasAccess) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    return tabItem;
                }
            }
            return null;
        }

        private Button SetButtonByName(string buttonContent, bool hasAccess)
        {
            // Фиксированный заголовок вкладки
            string tabHeader = "Запросы";

            foreach (TabItem tabItem in MainTabControl.Items)
            {
                // Ищем TabItem с заголовком "Запросы"
                if (tabItem.Header != null && tabItem.Header.ToString() == tabHeader)
                {
                    // Проверяем, является ли содержимое TabItem контейнером, например Grid
                    if (tabItem.Content is Panel panel)
                    {
                        // Рекурсивный поиск кнопки внутри контейнера
                        foreach (var child in panel.Children)
                        {
                            if (child is Button button && button.Content?.ToString() == buttonContent)
                            {
                                // Меняем видимость кнопки
                                button.Visibility = hasAccess ? Visibility.Visible : Visibility.Collapsed;
                                return button;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void RequestsCheckAuthority()
        {
            try
            {
                SetButtonByName("Не прошедшие техосмотр", _dbHelper.HasSelectPermission("failed_inspection_owners", "SELECT"));
                SetButtonByName("Дорогие иномарки", _dbHelper.HasSelectPermission("luxury_car_owners", "SELECT"));
                SetButtonByName("Техосмотры в СЦ", _dbHelper.HasSelectPermission("inspections_by_service_center", "SELECT"));
                SetButtonByName("Протоколы сотрудника", _dbHelper.HasSelectPermission("protocols_by_officer", "SELECT"));
                SetButtonByName("Участники ДТП", _dbHelper.HasSelectPermission("accident_participants", "SELECT"));
                SetButtonByName("Получить логи", _dbHelper.HasSelectPermission("logs", "DELETE"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки прав доступа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrEmpty(OwnerFIO.Text) || string.IsNullOrEmpty(OwnerAddress.Text) ||
                string.IsNullOrEmpty(OwnerPhone.Text) || string.IsNullOrEmpty(OwnerDOB.Text) || string.IsNullOrEmpty(OwnerDriverLicenseNumber.Text))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            var newOwner = new Owner
            {
                FullName = OwnerFIO.Text,
                Address = OwnerAddress.Text,
                PhoneNumber = OwnerPhone.Text,
                BirthDate = OwnerDOB.Text,
                DriverLicenseNumber = OwnerDriverLicenseNumber.Text
            };

            _dbHelper.AddOwner(newOwner);
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
            if (string.IsNullOrEmpty(CarVIN.Text) || string.IsNullOrEmpty(CarBrand.Text) || string.IsNullOrEmpty(CarModel.Text) ||
                string.IsNullOrEmpty(CarYear.Text) || string.IsNullOrEmpty(CarColor.Text) || string.IsNullOrEmpty(CarNumber.Text) || string.IsNullOrEmpty(OwnerID.Text))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            var newCar = new Car
            {
                VIN = CarVIN.Text,
                Brand = CarBrand.Text,
                Model = CarModel.Text,
                Year = int.TryParse(CarYear.Text, out int year) ? (int?)year : null,
                Color = CarColor.Text,
                LicensePlate = CarNumber.Text,
                OwnerID = int.TryParse(OwnerID.Text, out int ownerId) ? (int?)ownerId : null
            };

            _dbHelper.AddCar(newCar);
            ShowCarsButton_Click(sender, e);
        }

        private void UpdateCarButton_Click(object sender, RoutedEventArgs e)
        {
            CarsDataGrid.CommitEdit();
            CarsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is string vin)
            {
                var car = (Car)CarsDataGrid.SelectedItem;
                if (car?.VIN != vin) return;

                if (string.IsNullOrEmpty(car.Brand) || string.IsNullOrEmpty(car.Model) || car.Year == 0 ||
                    string.IsNullOrEmpty(car.Color) || string.IsNullOrEmpty(car.LicensePlate) || car.OwnerID == 0)
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateCar(car);
                LoadCars();
            }
        }

        private void DeleteCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is string vin)
            {
                _dbHelper.DeleteCar(vin);
                LoadCars();
            }
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
            ShowPoliceOfficersButton_Click(sender, e);
        }

        private void UpdatePoliceOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            PoliceOfficersDataGrid.CommitEdit();
            PoliceOfficersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is int officerId)
            {
                var officer = (PoliceOfficer)PoliceOfficersDataGrid.SelectedItem;
                if (officer?.OfficerId != officerId) return;

                if (string.IsNullOrEmpty(officer.FullName) || string.IsNullOrEmpty(officer.Position) ||
                    string.IsNullOrEmpty(officer.Department) || string.IsNullOrEmpty(officer.ContactDetails))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdatePoliceOfficer(officer);
                LoadPoliceOfficers();
            }
        }

        private void DeletePoliceOfficerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is int officerId)
            {
                _dbHelper.DeletePoliceOfficer(officerId);
                LoadPoliceOfficers();
            }
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
            ShowServiceStaffButton_Click(sender, e);
        }

        private void UpdateServiceStaffButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceStaffDataGrid.CommitEdit();
            ServiceStaffDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is int staffId)
            {
                var staff = (ServiceStaff)ServiceStaffDataGrid.SelectedItem;
                if (staff?.StaffId != staffId) return;

                if (string.IsNullOrEmpty(staff.FullName) || string.IsNullOrEmpty(staff.Position) ||
                    string.IsNullOrEmpty(staff.ContactDetails))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateServiceStaff(staff);
                LoadServiceStaff();
            }
        }

        private void DeleteServiceStaffButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is int staffId)
            {
                _dbHelper.DeleteServiceStaff(staffId);
                LoadServiceStaff();
            }
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
            ShowInspectionsButton_Click(sender, e);
        }

        private void UpdateInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            InspectionsDataGrid.CommitEdit();
            InspectionsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is string vin)
            {
                var inspection = (Inspection)InspectionsDataGrid.SelectedItem;
                if (inspection?.VIN != vin) return;

                if (string.IsNullOrEmpty(inspection.InspectionDate) || string.IsNullOrEmpty(inspection.Result) || string.IsNullOrEmpty(inspection.VIN))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все обязательные поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateInspection(inspection);
                LoadInspections();
            }
        }

        private void DeleteInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton
                && deleteButton.Tag is string inspectionDate
                && deleteButton.CommandParameter is string vin)
            {
                _dbHelper.DeleteInspection(inspectionDate, vin);
                LoadInspections();
            }
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
            if (string.IsNullOrEmpty(ViolationDate.Text) || string.IsNullOrEmpty(ViolationType.Text) ||
                string.IsNullOrEmpty(ViolationFine.Text) || string.IsNullOrEmpty(ViolationCarVIN.Text) || string.IsNullOrEmpty(ViolationOfficerId.Text))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все обязательные поля должны быть заполнены!");
                return;
            }

            var newViolation = new Violation
            {
                IssueDate = ViolationDate.Text,
                ViolationType = ViolationType.Text,
                FineAmount = int.TryParse(ViolationFine.Text, out int fine) ? (int?)fine : null,
                VIN = ViolationCarVIN.Text,
                OfficerId = int.TryParse(ViolationOfficerId.Text, out int officerId) ? (int?)officerId : null
            };

            _dbHelper.AddViolation(newViolation);
            ShowViolationsButton_Click(sender, e);
        }

        private void UpdateViolationButton_Click(object sender, RoutedEventArgs e)
        {
            ViolationsDataGrid.CommitEdit();
            ViolationsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is int protocolNumber)
            {
                var violation = (Violation)ViolationsDataGrid.SelectedItem;
                if (violation?.ProtocolNumber != protocolNumber) return;

                if (string.IsNullOrEmpty(violation.IssueDate) || string.IsNullOrEmpty(violation.ViolationType) ||
                    violation.FineAmount == 0 || string.IsNullOrEmpty(violation.VIN))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все обязательные поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateViolation(violation);
                LoadViolations();
            }
        }

        private void DeleteViolationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is int protocolNumber)
            {
                _dbHelper.DeleteViolation(protocolNumber);
                LoadViolations();
            }
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
            ShowServiceCentersButton_Click(sender, e);
        }

        private void UpdateServiceCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceCentersDataGrid.CommitEdit();
            ServiceCentersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is string license)
            {
                var center = (ServiceCenter)ServiceCentersDataGrid.SelectedItem;
                if (center?.License != license) return;

                if (string.IsNullOrEmpty(center.Name) || string.IsNullOrEmpty(center.Address) || string.IsNullOrEmpty(center.ContactPerson))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateServiceCenter(center);
                LoadServiceCenters();
            }
        }

        private void DeleteServiceCenterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.CommandParameter is string license)
            {
                _dbHelper.DeleteServiceCenter(license);
                LoadServiceCenters();
            }
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
                ServiceEmployeeId = serviceEmployeeId ?? -1
            };

            if (string.IsNullOrEmpty(newEmployment.ServiceCenterLicense) || newEmployment.ServiceEmployeeId == -1)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            _dbHelper.AddEmployment(newEmployment);
            ShowEmploymentButton_Click(sender, e);
        }

        private void UpdateEmploymentButton_Click(object sender, RoutedEventArgs e)
        {
            EmploymentDataGrid.CommitEdit();
            EmploymentDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (sender is Button updateButton && updateButton.Tag is string license)
            {
                var employment = (Employment)EmploymentDataGrid.SelectedItem;
                if (employment?.ServiceCenterLicense != license) return;

                if (string.IsNullOrEmpty(employment.ServiceCenterLicense) || employment.ServiceEmployeeId == 0)
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                    return;
                }

                _dbHelper.UpdateEmployment(employment);
                LoadEmployment();
            }
        }

        private void DeleteEmploymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton)
            {
                string license = deleteButton.CommandParameter as string;
                if (int.TryParse(deleteButton.Tag?.ToString(), out int employeeId))
                {
                    _dbHelper.DeleteEmployment(license, employeeId); // Передача двух параметров
                    LoadEmployment();
                }
                else
                {
                    MessageBox.Show("Ошибка: не удалось определить ID сотрудника.");
                }
            }
        }

        #endregion

        #region Request Methods

        private void RequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                // Сбрасываем цвета предыдущей активной кнопки
                if (_activeButton != null)
                {
                    _activeButton.Background = new SolidColorBrush(Colors.DarkSlateBlue);
                    _activeButton.Foreground = new SolidColorBrush(Colors.White);
                }

                // Устанавливаем новую активную кнопку
                _activeButton = clickedButton;

                // Меняем цвета активной кнопки
                _activeButton.Background = new SolidColorBrush(Colors.White);
                _activeButton.Foreground = new SolidColorBrush(Colors.DarkSlateBlue);

                // Выполняем соответствующий запрос
                ExecuteRequest(_activeButton.Tag.ToString());
            }
        }

        private void ExecuteRequest(string requestId)
        {
            switch (requestId)
            {
                case "1":
                    Request1Button_Click(null, null);
                    break;
                case "2":
                    Request2Button_Click(null, null);
                    break;
                case "3":
                    Request3Button_Click(null, null);
                    break;
                case "4":
                    Request4Button_Click(null, null);
                    break;
                case "5":
                    Request5Button_Click(null, null);
                    break;
                case "6":
                    Request6Button_Click(null, null);
                    break;
                default:
                    MessageBox.Show("Неизвестный запрос.");
                    break;
            }
        }

        private void Request1Button_Click(object sender, RoutedEventArgs e)
        {
            var failedInspectionOwners = _dbHelper.GetFailedInspectionOwners();
            RequestsDataGrid.ItemsSource = failedInspectionOwners;
        }

        private void Request2Button_Click(object sender, RoutedEventArgs e)
        {
            var luxuryCarOwners = _dbHelper.GetLuxuryCarOwners();
            RequestsDataGrid.ItemsSource = luxuryCarOwners;
        }

        private void Request3Button_Click(object sender, RoutedEventArgs e)
        {
            var serviceCenterInspections = _dbHelper.GetServiceCenterInspections();
            RequestsDataGrid.ItemsSource = serviceCenterInspections;
        }

        private void Request4Button_Click(object sender, RoutedEventArgs e)
        {
            var officerProtocols = _dbHelper.GetOfficerProtocols();
            RequestsDataGrid.ItemsSource = officerProtocols;
        }

        private void Request5Button_Click(object sender, RoutedEventArgs e)
        {
            var accidentParticipants = _dbHelper.GetAccidentParticipants();
            RequestsDataGrid.ItemsSource = accidentParticipants;
        }

        private void Request6Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var logs = _dbHelper.GetLogs(null, null, null, null);
                RequestsDataGrid.ItemsSource = logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки логов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #endregion
    }
}