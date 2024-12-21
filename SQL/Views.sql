-- Владельцы, не прошедшие техосмотр
CREATE VIEW autoinspection.failed_inspection_owners AS
SELECT 
    o.full_name AS owner_name,
    o.phone_number AS contact_number,
    c.brand AS car_brand,
    c.model AS car_model,
    i.inspection_date AS failed_date
FROM 
    autoinspection.owners o
JOIN 
    autoinspection.cars c ON o.owner_id = c.owner_id
JOIN 
    autoinspection.inspections i ON c.vin = i.vin
WHERE 
    i.result = 'Не пройден';

select * from autoinspection.failed_inspection_owners;

-- Представление 2: Владельцы дорогих иномарок
CREATE VIEW autoinspection.luxury_car_owners AS
SELECT 
    o.full_name AS owner_name,
    o.phone_number AS contact_number,
    c.brand AS car_brand,
    c.model AS car_model,
    c.year AS car_year,
    c.color AS car_color
FROM 
    autoinspection.owners o
JOIN 
    autoinspection.cars c ON o.owner_id = c.owner_id
WHERE 
    c.brand IN ('Mercedes-Benz', 'BMW', 'Audi', 'Lexus', 'Toyota') AND
    c.year >= 2018;
	
select * from autoinspection.luxury_car_owners;

-- Представление 3: Техосмотры, проведенные в конкретном сервисном
CREATE VIEW autoinspection.inspections_by_service_center AS
SELECT 
    sc.name AS service_center_name,
    i.inspection_date,
    i.result AS inspection_result,
    c.vin AS car_vin,
    c.brand AS car_brand,
    c.model AS car_model,
    se.full_name AS employee_name
FROM 
    autoinspection.inspections i
JOIN 
    autoinspection.cars c ON i.vin = c.vin
LEFT JOIN 
    autoinspection.serviceemployees se ON i.service_employee_id = se.employee_id
JOIN 
    autoinspection.employment e ON se.employee_id = e.service_employee_id
JOIN 
    autoinspection.servicecenters sc ON e.service_center_license = sc.license;

select * from autoinspection.inspections_by_service_center;

-- Представление 4: Протоколы, выписанные конкретным сотрудником ГИБДД
CREATE VIEW autoinspection.protocols_by_officer AS
SELECT 
    t.full_name AS officer_name,
    t.department AS officer_department,
    p.protocol_number,
    p.issue_date,
    p.violation_type,
    p.fine_amount,
    c.vin AS car_vin,
    c.brand AS car_brand,
    c.model AS car_model
FROM 
    autoinspection.protocols p
JOIN 
    autoinspection.cars c ON p.vin = c.vin
JOIN 
    autoinspection.trafficofficers t ON p.officer_id = t.officer_id;

select * from autoinspection.protocols_by_officer;

-- Представление 5: Владельцы автомобилей, участвовавших в ДТП
CREATE VIEW autoinspection.accident_participants AS
SELECT 
    o.full_name AS owner_name,
    o.phone_number AS contact_number,
    c.vin AS car_vin,
    c.brand AS car_brand,
    c.model AS car_model,
    p.issue_date AS accident_date,
    p.violation_type AS accident_details
FROM 
    autoinspection.owners o
JOIN 
    autoinspection.cars c ON o.owner_id = c.owner_id
JOIN 
    autoinspection.protocols p ON c.vin = p.vin
WHERE 
    p.violation_type = 'ДТП';

select * from autoinspection.accident_participants;