using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace Kursovik
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PostgreSqlConnection"].ConnectionString;
        }

        #region Database Connection
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
        #endregion

        #region Owners Methods
        public List<Owner> GetOwners()
        {
            var owners = new List<Owner>();
            string query = "SELECT owner_id, full_name, address, phone_number, birth_date, driver_license_number FROM autoinspection.owners";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        owners.Add(new Owner
                        {
                            OwnerId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Address = reader.GetString(2),
                            PhoneNumber = reader.GetString(3),
                            BirthDate = reader.GetDateTime(4),
                            DriverLicenseNumber = reader.GetString(5)
                        });
                    }
                }
            }

            return owners;
        }

        public void AddOwner(Owner owner)
        {
            string query = "INSERT INTO autoinspection.owners (full_name, address, phone_number, birth_date, driver_license_number) VALUES (@FullName, @Address, @PhoneNumber, @BirthDate, @DriverLicenseNumber)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", owner.FullName);
                    command.Parameters.AddWithValue("@Address", owner.Address);
                    command.Parameters.AddWithValue("@PhoneNumber", owner.PhoneNumber);
                    command.Parameters.AddWithValue("@BirthDate", owner.BirthDate);
                    command.Parameters.AddWithValue("@DriverLicenseNumber", owner.DriverLicenseNumber);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            string query = "DELETE FROM autoinspection.owners WHERE owner_id = @OwnerId";

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OwnerId", ownerId);
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            MessageBox.Show("Владелец с указанным ID не найден.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении владельца: " + ex.Message);
            }
        }

        public void UpdateOwner(Owner owner)
        {
            string query = "UPDATE autoinspection.owners SET full_name = @FullName, address = @Address, " +
                           "phone_number = @PhoneNumber, birth_date = @BirthDate, driver_license_number = @DriverLicenseNumber " +
                           "WHERE owner_id = @OwnerId";

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", owner.FullName);
                        command.Parameters.AddWithValue("@Address", owner.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", owner.PhoneNumber);
                        command.Parameters.AddWithValue("@BirthDate", owner.BirthDate);
                        command.Parameters.AddWithValue("@DriverLicenseNumber", owner.DriverLicenseNumber);
                        command.Parameters.AddWithValue("@OwnerId", owner.OwnerId);

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            MessageBox.Show("Не удалось обновить данные владельца. Проверьте корректность ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении владельца: " + ex.Message);
            }
        }

        #endregion

        #region Cars Methods
        public List<Car> GetCars()
        {
            var cars = new List<Car>();
            string query = "SELECT vin, brand, model, year, color, license_plate, owner_id FROM autoinspection.cars";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cars.Add(new Car
                        {
                            VIN = reader.GetString(0),
                            Brand = reader.GetString(1),
                            Model = reader.GetString(2),
                            Year = reader.GetInt32(3),
                            Color = reader.GetString(4),
                            LicensePlate = reader.GetString(5),
                            OwnerID = reader.GetInt32(6)
                        });
                    }
                }
            }

            return cars;
        }

        public void AddCar(Car car)
        {
            string query = "INSERT INTO autoinspection.cars (vin, brand, model, year, color, license_plate, owner_id) VALUES (@VIN, @Brand, @Model, @Year, @Color, @LicensePlate, @OwnerID)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VIN", car.VIN);
                    command.Parameters.AddWithValue("@Brand", car.Brand);
                    command.Parameters.AddWithValue("@Model", car.Model);
                    command.Parameters.AddWithValue("@Year", car.Year);
                    command.Parameters.AddWithValue("@Color", car.Color);
                    command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                    command.Parameters.AddWithValue("@OwnerID", car.OwnerID);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCar(Car car)
        {
            string query = "UPDATE autoinspection.cars SET brand = @Brand, model = @Model, year = @Year, color = @Color, license_plate = @LicensePlate, owner_id = @OwnerID WHERE vin = @VIN";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Brand", car.Brand);
                    command.Parameters.AddWithValue("@Model", car.Model);
                    command.Parameters.AddWithValue("@Year", car.Year);
                    command.Parameters.AddWithValue("@Color", car.Color);
                    command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                    command.Parameters.AddWithValue("@OwnerID", car.OwnerID);
                    command.Parameters.AddWithValue("@VIN", car.VIN);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCar(string vin)
        {
            string query = "DELETE FROM autoinspection.cars WHERE vin = @VIN";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VIN", vin);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region PoliceOfficers Methods
        public List<PoliceOfficer> GetPoliceOfficers()
        {
            var officers = new List<PoliceOfficer>();
            string query = "SELECT officer_id, full_name, position, department, contact_details FROM autoinspection.trafficofficers";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        officers.Add(new PoliceOfficer
                        {
                            OfficerId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Position = reader.GetString(2),
                            Department = reader.GetString(3),
                            ContactDetails = reader.GetString(4)
                        });
                    }
                }
            }

            return officers;
        }

        public void AddPoliceOfficer(PoliceOfficer officer)
        {
            string query = "INSERT INTO autoinspection.trafficofficers (full_name, position, department, contact_details) VALUES (@FullName, @Position, @Department, @ContactDetails)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", officer.FullName);
                    command.Parameters.AddWithValue("@Position", officer.Position);
                    command.Parameters.AddWithValue("@Department", officer.Department);
                    command.Parameters.AddWithValue("@ContactDetails", officer.ContactDetails);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePoliceOfficer(PoliceOfficer officer)
        {
            string query = "UPDATE autoinspection.trafficofficers SET full_name = @FullName, position = @Position, department = @Department, contact_details = @ContactDetails WHERE officer_id = @OfficerId";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", officer.FullName);
                    command.Parameters.AddWithValue("@Position", officer.Position);
                    command.Parameters.AddWithValue("@Department", officer.Department);
                    command.Parameters.AddWithValue("@ContactDetails", officer.ContactDetails);
                    command.Parameters.AddWithValue("@OfficerId", officer.OfficerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePoliceOfficer(int officerId)
        {
            string query = "DELETE FROM autoinspection.trafficofficers WHERE officer_id = @OfficerId";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OfficerId", officerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region ServiceStaff Methods
        public List<ServiceStaff> GetServiceStaff()
        {
            var staffList = new List<ServiceStaff>();
            string query = "SELECT employee_id, full_name, position, contact_details FROM autoinspection.serviceemployees";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        staffList.Add(new ServiceStaff
                        {
                            StaffId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Position = reader.GetString(2),
                            ContactDetails = reader.GetString(3)
                        });
                    }
                }
            }

            return staffList;
        }

        public void AddServiceStaff(ServiceStaff staff)
        {
            string query = "INSERT INTO autoinspection.serviceemployees (full_name, position, contact_details) VALUES (@FullName, @Position, @ContactDetails)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", staff.FullName);
                    command.Parameters.AddWithValue("@Position", staff.Position);
                    command.Parameters.AddWithValue("@ContactDetails", staff.ContactDetails);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateServiceStaff(ServiceStaff staff)
        {
            string query = "UPDATE autoinspection.serviceemployees SET full_name = @FullName, position = @Position, contact_details = @ContactDetails WHERE employee_id = @StaffId";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", staff.FullName);
                    command.Parameters.AddWithValue("@Position", staff.Position);
                    command.Parameters.AddWithValue("@ContactDetails", staff.ContactDetails);
                    command.Parameters.AddWithValue("@StaffId", staff.StaffId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteServiceStaff(int staffId)
        {
            string query = "DELETE FROM autoinspection.serviceemployees WHERE employee_id = @StaffId";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StaffId", staffId);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Inspections Methods
        public List<Inspection> GetInspections()
        {
            var inspections = new List<Inspection>();
            string query = "SELECT inspection_date, result, notes, vin, service_employee_id FROM autoinspection.inspections";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inspections.Add(new Inspection
                        {
                            InspectionDate = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                            Result = reader.GetString(1),
                            Notes = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            VIN = reader.GetString(3),
                            ServiceEmployeeId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                        });
                    }
                }
            }

            return inspections;
        }

        public void AddInspection(Inspection inspection)
        {
            string query = "INSERT INTO autoinspection.inspections (inspection_date, result, notes, vin, service_employee_id) VALUES (@InspectionDate, @Result, @Notes, @VIN, @ServiceEmployeeId)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InspectionDate", DateTime.Parse(inspection.InspectionDate));
                    command.Parameters.AddWithValue("@Result", inspection.Result);
                    command.Parameters.AddWithValue("@Notes", inspection.Notes);
                    command.Parameters.AddWithValue("@VIN", inspection.VIN);
                    command.Parameters.AddWithValue("@ServiceEmployeeId", inspection.ServiceEmployeeId.HasValue ? (object)inspection.ServiceEmployeeId.Value : DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateInspection(Inspection inspection)
        {
            string query = "UPDATE autoinspection.inspections SET inspection_date = @InspectionDate, result = @Result, notes = @Notes, service_employee_id = @ServiceEmployeeId WHERE vin = @VIN";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InspectionDate", DateTime.Parse(inspection.InspectionDate));
                    command.Parameters.AddWithValue("@Result", inspection.Result);
                    command.Parameters.AddWithValue("@Notes", inspection.Notes);
                    command.Parameters.AddWithValue("@ServiceEmployeeId", inspection.ServiceEmployeeId.HasValue ? (object)inspection.ServiceEmployeeId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@VIN", inspection.VIN);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteInspection(string vin)
        {
            string query = "DELETE FROM autoinspection.inspections WHERE vin = @VIN";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VIN", vin);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Violations Methods
        public List<Violation> GetViolations()
        {
            var violations = new List<Violation>();
            string query = "SELECT protocol_number, issue_date, violation_type, fine_amount, vin, officer_id FROM autoinspection.protocols";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        violations.Add(new Violation
                        {
                            ProtocolNumber = reader.GetInt32(0),
                            IssueDate = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                            ViolationType = reader.GetString(2),
                            FineAmount = reader.GetInt32(3),
                            VIN = reader.GetString(4),
                            OfficerId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                        });
                    }
                }
            }

            return violations;
        }

        public void AddViolation(Violation violation)
        {
            string query = "INSERT INTO autoinspection.protocols (issue_date, violation_type, fine_amount, vin, officer_id) VALUES (@IssueDate, @ViolationType, @FineAmount, @VIN, @OfficerId)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IssueDate", DateTime.Parse(violation.IssueDate));
                    command.Parameters.AddWithValue("@ViolationType", violation.ViolationType);
                    command.Parameters.AddWithValue("@FineAmount", violation.FineAmount);
                    command.Parameters.AddWithValue("@VIN", violation.VIN);
                    command.Parameters.AddWithValue("@OfficerId", violation.OfficerId.HasValue ? (object)violation.OfficerId.Value : DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateViolation(Violation violation)
        {
            string query = "UPDATE autoinspection.protocols SET issue_date = @IssueDate, violation_type = @ViolationType, fine_amount = @FineAmount, officer_id = @OfficerId WHERE protocol_number = @ProtocolNumber";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IssueDate", DateTime.Parse(violation.IssueDate));
                    command.Parameters.AddWithValue("@ViolationType", violation.ViolationType);
                    command.Parameters.AddWithValue("@FineAmount", violation.FineAmount);
                    command.Parameters.AddWithValue("@OfficerId", violation.OfficerId.HasValue ? (object)violation.OfficerId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ProtocolNumber", violation.ProtocolNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteViolation(int protocolNumber)
        {
            string query = "DELETE FROM autoinspection.protocols WHERE protocol_number = @ProtocolNumber";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProtocolNumber", protocolNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region ServiceCenters Methods
        public List<ServiceCenter> GetServiceCenters()
        {
            var serviceCenters = new List<ServiceCenter>();
            string query = "SELECT license, name, address, contact_person FROM autoinspection.servicecenters";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serviceCenters.Add(new ServiceCenter
                        {
                            License = reader.GetString(0),
                            Name = reader.GetString(1),
                            Address = reader.GetString(2),
                            ContactPerson = reader.GetString(3)
                        });
                    }
                }
            }

            return serviceCenters;
        }

        public void AddServiceCenter(ServiceCenter serviceCenter)
        {
            string query = "INSERT INTO autoinspection.servicecenters (license, name, address, contact_person) VALUES (@License, @Name, @Address, @ContactPerson)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@License", serviceCenter.License);
                    command.Parameters.AddWithValue("@Name", serviceCenter.Name);
                    command.Parameters.AddWithValue("@Address", serviceCenter.Address);
                    command.Parameters.AddWithValue("@ContactPerson", serviceCenter.ContactPerson);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateServiceCenter(ServiceCenter center)
        {
            string query = "UPDATE autoinspection.servicecenters SET name = @Name, address = @Address, contact_person = @ContactPerson WHERE license = @License";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", center.Name);
                    command.Parameters.AddWithValue("@Address", center.Address);
                    command.Parameters.AddWithValue("@ContactPerson", center.ContactPerson);
                    command.Parameters.AddWithValue("@License", center.License);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteServiceCenter(string license)
        {
            string query = "DELETE FROM autoinspection.servicecenters WHERE license = @License";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@License", license);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Employment Methods
        public List<Employment> GetEmployment()
        {
            var employmentList = new List<Employment>();
            string query = "SELECT service_center_license, service_employee_id FROM autoinspection.employment";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employmentList.Add(new Employment
                        {
                            ServiceCenterLicense = reader.GetString(0),
                            ServiceEmployeeId = reader.GetInt32(1)
                        });
                    }
                }
            }

            return employmentList;
        }

        public void AddEmployment(Employment employment)
        {
            string query = "INSERT INTO autoinspection.employment (service_center_license, service_employee_id) VALUES (@ServiceCenterLicense, @ServiceEmployeeId)";

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceCenterLicense", employment.ServiceCenterLicense);
                    command.Parameters.AddWithValue("@ServiceEmployeeId", employment.ServiceEmployeeId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEmployment(Employment employment)
        {
            string query = "UPDATE autoinspection.employment SET service_employee_id = @ServiceEmployeeId WHERE service_center_license = @ServiceCenterLicense";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceEmployeeId", employment.ServiceEmployeeId);
                    command.Parameters.AddWithValue("@ServiceCenterLicense", employment.ServiceCenterLicense);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEmployment(string serviceCenterLicense)
        {
            string query = "DELETE FROM autoinspection.employment WHERE service_center_license = @ServiceCenterLicense";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceCenterLicense", serviceCenterLicense);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}