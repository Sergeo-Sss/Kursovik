using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Kursovik
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string conn)
        {
            _connectionString = conn;
        }

        #region Database Connection
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        private void ExecuteProcedure(string procedure, Action<NpgsqlCommand> parameterSetup)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(procedure, connection))
                    {
                        parameterSetup(command);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Owners Methods
        public List<Owner> GetOwners()
        {
            var owners = new List<Owner>();
            string query = "SELECT * FROM autoinspection.get_owners()";

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
                            BirthDate = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                            DriverLicenseNumber = reader.GetString(5)
                        });
                    }
                }
            }

            return owners;
        }

        public void AddOwner(Owner owner)
        {
            string procedure = "CALL autoinspection.insert_owner(@FullName, @Address, @PhoneNumber, @BirthDate, @DriverLicenseNumber)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@FullName", owner.FullName);
                command.Parameters.AddWithValue("@Address", owner.Address);
                command.Parameters.AddWithValue("@PhoneNumber", owner.PhoneNumber);
                command.Parameters.AddWithValue("@BirthDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(owner.BirthDate).Date);
                command.Parameters.AddWithValue("@DriverLicenseNumber", owner.DriverLicenseNumber);
            });
        }

        public void UpdateOwner(Owner owner)
        {
            string procedure = "CALL autoinspection.update_owner(@OwnerId, @FullName, @Address, @PhoneNumber, @BirthDate, @DriverLicenseNumber)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@OwnerId", owner.OwnerId);
                command.Parameters.AddWithValue("@FullName", owner.FullName);
                command.Parameters.AddWithValue("@Address", owner.Address);
                command.Parameters.AddWithValue("@PhoneNumber", owner.PhoneNumber);
                command.Parameters.AddWithValue("@BirthDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(owner.BirthDate).Date);
                command.Parameters.AddWithValue("@DriverLicenseNumber", owner.DriverLicenseNumber);
            });
        }

        public void DeleteOwner(int ownerId)
        {
            string procedure = "CALL autoinspection.delete_owner(@OwnerId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@OwnerId", ownerId);
            });
        }
        #endregion

        #region Cars Methods
        public List<Car> GetCars()
        {
            var cars = new List<Car>();
            string query = "SELECT * FROM autoinspection.get_cars()";

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
            string procedure = "CALL autoinspection.insert_car(@VIN, @Brand, @Model, @Year, @Color, @LicensePlate, @OwnerID)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@VIN", car.VIN);
                command.Parameters.AddWithValue("@Brand", car.Brand);
                command.Parameters.AddWithValue("@Model", car.Model);
                command.Parameters.AddWithValue("@Year", car.Year);
                command.Parameters.AddWithValue("@Color", car.Color);
                command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                command.Parameters.AddWithValue("@OwnerID", car.OwnerID);
            });
        }

        public void UpdateCar(Car car)
        {
            string procedure = "CALL autoinspection.update_car(@VIN, @Brand, @Model, @Year, @Color, @LicensePlate, @OwnerID)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@VIN", car.VIN);
                command.Parameters.AddWithValue("@Brand", car.Brand);
                command.Parameters.AddWithValue("@Model", car.Model);
                command.Parameters.AddWithValue("@Year", car.Year);
                command.Parameters.AddWithValue("@Color", car.Color);
                command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                command.Parameters.AddWithValue("@OwnerID", car.OwnerID);
            });
        }

        public void DeleteCar(string vin)
        {
            string procedure = "CALL autoinspection.delete_car(@VIN)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@VIN", vin);
            });
        }
        #endregion

        #region PoliceOfficers Methods
        public List<PoliceOfficer> GetPoliceOfficers()
        {
            var officers = new List<PoliceOfficer>();
            string query = "SELECT * FROM autoinspection.get_traffic_officers()";

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
            string procedure = "CALL autoinspection.insert_trafficofficer(@FullName, @Position, @Department, @ContactDetails)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@FullName", officer.FullName);
                command.Parameters.AddWithValue("@Position", officer.Position);
                command.Parameters.AddWithValue("@Department", officer.Department);
                command.Parameters.AddWithValue("@ContactDetails", officer.ContactDetails);
            });
        }

        public void UpdatePoliceOfficer(PoliceOfficer officer)
        {
            string procedure = "CALL autoinspection.update_trafficofficer(@OfficerId, @FullName, @Position, @Department, @ContactDetails)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@OfficerId", officer.OfficerId);
                command.Parameters.AddWithValue("@FullName", officer.FullName);
                command.Parameters.AddWithValue("@Position", officer.Position);
                command.Parameters.AddWithValue("@Department", officer.Department);
                command.Parameters.AddWithValue("@ContactDetails", officer.ContactDetails);
            });
        }

        public void DeletePoliceOfficer(int officerId)
        {
            string procedure = "CALL autoinspection.delete_trafficofficer(@OfficerId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@OfficerId", officerId);
            });
        }
        #endregion

        #region ServiceStaff Methods
        public List<ServiceStaff> GetServiceStaff()
        {
            var staffList = new List<ServiceStaff>();
            string query = "SELECT * FROM autoinspection.get_service_employees()";

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
            string procedure = "CALL autoinspection.insert_serviceemployee(@FullName, @Position, @ContactDetails)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@FullName", staff.FullName);
                command.Parameters.AddWithValue("@Position", staff.Position);
                command.Parameters.AddWithValue("@ContactDetails", staff.ContactDetails);
            });
        }

        public void UpdateServiceStaff(ServiceStaff staff)
        {
            string procedure = "CALL autoinspection.update_serviceemployee(@StaffId, @FullName, @Position, @ContactDetails)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@StaffId", staff.StaffId);
                command.Parameters.AddWithValue("@FullName", staff.FullName);
                command.Parameters.AddWithValue("@Position", staff.Position);
                command.Parameters.AddWithValue("@ContactDetails", staff.ContactDetails);
            });
        }

        public void DeleteServiceStaff(int staffId)
        {
            string procedure = "CALL autoinspection.delete_serviceemployee(@StaffId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@StaffId", staffId);
            });
        }
        #endregion

        #region Inspections Methods
        public List<Inspection> GetInspections()
        {
            var inspections = new List<Inspection>();
            string query = "SELECT * FROM autoinspection.get_inspections()";

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
            string procedure = "CALL autoinspection.insert_inspection(@InspectionDate, @Result, @Notes, @VIN, @ServiceEmployeeId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@InspectionDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(inspection.InspectionDate).Date);
                command.Parameters.AddWithValue("@Result", inspection.Result);
                command.Parameters.AddWithValue("@Notes", inspection.Notes);
                command.Parameters.AddWithValue("@VIN", inspection.VIN);
                command.Parameters.AddWithValue("@ServiceEmployeeId", inspection.ServiceEmployeeId.HasValue ? (object)inspection.ServiceEmployeeId.Value : DBNull.Value);
            });
        }

        public void UpdateInspection(Inspection inspection)
        {
            string procedure = "CALL autoinspection.update_inspection(@InspectionDate, @Result, @Notes, @VIN, @ServiceEmployeeId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@InspectionDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(inspection.InspectionDate).Date);
                command.Parameters.AddWithValue("@Result", inspection.Result);
                command.Parameters.AddWithValue("@Notes", inspection.Notes);
                command.Parameters.AddWithValue("@VIN", inspection.VIN);
                command.Parameters.AddWithValue("@ServiceEmployeeId", inspection.ServiceEmployeeId.HasValue ? (object)inspection.ServiceEmployeeId.Value : DBNull.Value);
            });
        }

        public void DeleteInspection(string inspectionDate, string vin)
        {
            string procedure = "CALL autoinspection.delete_inspection(@InspectionDate, @VIN)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@InspectionDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(inspectionDate).Date);
                command.Parameters.AddWithValue("@VIN", vin);
            });
        }

        #endregion

        #region Violations Methods
        public List<Violation> GetViolations()
        {
            var violations = new List<Violation>();
            string query = "SELECT * FROM autoinspection.get_protocols()";

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
            string procedure = "CALL autoinspection.insert_protocol(@IssueDate, @ViolationType, @FineAmount, @VIN, @OfficerId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@IssueDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(violation.IssueDate).Date);
                command.Parameters.AddWithValue("@ViolationType", violation.ViolationType);
                command.Parameters.AddWithValue("@FineAmount", violation.FineAmount);
                command.Parameters.AddWithValue("@VIN", violation.VIN);
                command.Parameters.AddWithValue("@OfficerId", violation.OfficerId.HasValue ? (object)violation.OfficerId.Value : DBNull.Value);
            });
        }

        public void UpdateViolation(Violation violation)
        {
            string procedure = "CALL autoinspection.update_protocol(@ProtocolNumber, @IssueDate, @ViolationType, @FineAmount, @VIN, @OfficerId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@ProtocolNumber", violation.ProtocolNumber);
                command.Parameters.AddWithValue("@IssueDate", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Parse(violation.IssueDate).Date);
                command.Parameters.AddWithValue("@ViolationType", violation.ViolationType);
                command.Parameters.AddWithValue("@FineAmount", violation.FineAmount);
                command.Parameters.AddWithValue("@VIN", violation.VIN);
                command.Parameters.AddWithValue("@OfficerId", violation.OfficerId.HasValue ? (object)violation.OfficerId.Value : DBNull.Value);
            });
        }

        public void DeleteViolation(int protocolNumber)
        {
            string procedure = "CALL autoinspection.delete_protocol(@ProtocolNumber)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@ProtocolNumber", protocolNumber);
            });
        }
        #endregion

        #region ServiceCenters Methods
        public List<ServiceCenter> GetServiceCenters()
        {
            var serviceCenters = new List<ServiceCenter>();
            string query = "SELECT * FROM autoinspection.get_service_centers()";

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
            string procedure = "CALL autoinspection.insert_servicecenter(@License, @Name, @Address, @ContactPerson)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@License", serviceCenter.License);
                command.Parameters.AddWithValue("@Name", serviceCenter.Name);
                command.Parameters.AddWithValue("@Address", serviceCenter.Address);
                command.Parameters.AddWithValue("@ContactPerson", serviceCenter.ContactPerson);
            });
        }

        public void UpdateServiceCenter(ServiceCenter serviceCenter)
        {
            string procedure = "CALL autoinspection.update_servicecenter(@License, @Name, @Address, @ContactPerson)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@License", serviceCenter.License);
                command.Parameters.AddWithValue("@Name", serviceCenter.Name);
                command.Parameters.AddWithValue("@Address", serviceCenter.Address);
                command.Parameters.AddWithValue("@ContactPerson", serviceCenter.ContactPerson);
            });
        }

        public void DeleteServiceCenter(string license)
        {
            string procedure = "CALL autoinspection.delete_servicecenter(@License)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@License", license);
            });
        }
        #endregion

        #region Employment Methods
        public List<Employment> GetEmployment()
        {
            var employmentList = new List<Employment>();
            string query = "SELECT * FROM autoinspection.get_employment()";

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
            string procedure = "CALL autoinspection.insert_employment(@ServiceCenterLicense, @ServiceEmployeeId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@ServiceCenterLicense", employment.ServiceCenterLicense);
                command.Parameters.AddWithValue("@ServiceEmployeeId", employment.ServiceEmployeeId);
            });
        }

        public void UpdateEmployment(Employment employment)
        {
            string procedure = "CALL autoinspection.update_employment(@TargetServiceCenterLicense, @TargetServiceEmployeeId, @NewServiceCenterLicense, @NewServiceEmployeeId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@TargetServiceCenterLicense", employment.ServiceCenterLicense);
                command.Parameters.AddWithValue("@TargetServiceEmployeeId", employment.ServiceEmployeeId);
                command.Parameters.AddWithValue("@NewServiceCenterLicense", employment.ServiceCenterLicense);
                command.Parameters.AddWithValue("@NewServiceEmployeeId", employment.ServiceEmployeeId);
            });
        }

        public void DeleteEmployment(string serviceCenterLicense, int serviceEmployeeId)
        {
            string procedure = "CALL autoinspection.delete_employment(@ServiceCenterLicense, @ServiceEmployeeId)";
            ExecuteProcedure(procedure, command =>
            {
                command.Parameters.AddWithValue("@ServiceCenterLicense", serviceCenterLicense);
                command.Parameters.AddWithValue("@ServiceEmployeeId", serviceEmployeeId);
            });
        }
        #endregion
    }
}