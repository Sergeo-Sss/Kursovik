----------------------------FUNCTIONS----------------------------------------------------------------

-- 1. Функция: проверка доступности автомобиля
CREATE OR REPLACE FUNCTION autoinspection.check_car_exists(vin_input VARCHAR)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на пустое значение VIN
    IF vin_input IS NULL OR TRIM(vin_input) = '' THEN
        RAISE EXCEPTION 'VIN автомобиля не может быть пустым.';
    END IF;

    -- Если автомобиль не найден, вернуть FALSE
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = vin_input
    ) THEN
        RETURN FALSE;
    END IF;

    -- Если найден, вернуть TRUE
    RETURN TRUE;
END;
$$;

-- Пример использования
SELECT autoinspection.check_car_exists('1HGCM82633A123456');

-- 2. Функция: расчет общей суммы штрафов владельца
CREATE OR REPLACE FUNCTION autoinspection.get_total_fines_by_owner(owner_id_input INTEGER)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    total_fines INTEGER;
BEGIN
    -- Проверка на пустое значение или отрицательное значение ID владельца
    IF owner_id_input IS NULL OR owner_id_input <= 0 THEN
        RAISE EXCEPTION 'ID владельца должен быть положительным числом.';
    END IF;

    -- Если владелец не найден, вернуть 0
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE owner_id = owner_id_input
    ) THEN
        RAISE NOTICE 'Владелец с ID % не найден.', owner_id_input;
        RETURN 0;
    END IF;

    SELECT SUM(fine_amount)
    INTO total_fines
    FROM autoinspection.protocols p
    JOIN autoinspection.cars c ON p.vin = c.vin
    WHERE c.owner_id = owner_id_input;

    RETURN COALESCE(total_fines, 0);
END;
$$;

-- Пример использования
SELECT autoinspection.get_total_fines_by_owner(3);

-- 3. Функция: расчет количества техосмотров автомобиля
CREATE OR REPLACE FUNCTION autoinspection.get_inspection_count_by_car(vin_input VARCHAR)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    inspection_count INTEGER;
BEGIN
    -- Проверка на пустое значение VIN
    IF vin_input IS NULL OR TRIM(vin_input) = '' THEN
        RAISE EXCEPTION 'VIN автомобиля не может быть пустым.';
    END IF;

    -- Если автомобиль не найден, вернуть 0
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = vin_input
    ) THEN
        RAISE NOTICE 'Автомобиль с VIN % не найден.', vin_input;
        RETURN 0;
    END IF;

    SELECT COUNT(*)
    INTO inspection_count
    FROM autoinspection.inspections
    WHERE vin = vin_input;

    RETURN inspection_count;
END;
$$;

-- Пример использования
SELECT autoinspection.get_inspection_count_by_car('1HGCM82633A123456');

---------------------------------------------------- ГЕТТЕРЫ ЗНАЧЕНИЙ-------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION autoinspection.get_owners()
RETURNS TABLE(
    owner_id INT,
    full_name VARCHAR,
    address VARCHAR,
    phone_number VARCHAR,
    birth_date DATE,
    driver_license_number VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT o.owner_id, o.full_name, o.address, o.phone_number, o.birth_date, o.driver_license_number
    FROM autoinspection.owners o;
END;
$$;

SELECT * FROM autoinspection.get_owners();

CREATE OR REPLACE FUNCTION autoinspection.get_cars()
RETURNS TABLE(
    vin VARCHAR,
    brand VARCHAR,
    model VARCHAR,
    year INT,
    color VARCHAR,
    license_plate VARCHAR,
    owner_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT c.vin, c.brand, c.model, c.year, c.color, c.license_plate, c.owner_id
    FROM autoinspection.cars c;
END;
$$;

SELECT * FROM autoinspection.get_cars();

CREATE OR REPLACE FUNCTION autoinspection.get_traffic_officers()
RETURNS TABLE(
    officer_id INT,
    full_name VARCHAR,
    position_t VARCHAR,
    department VARCHAR,
    contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT t.officer_id, t.full_name, t."position", t.department, t.contact_details
    FROM autoinspection.trafficofficers t;
END;
$$;

SELECT * FROM autoinspection.get_traffic_officers();

CREATE OR REPLACE FUNCTION autoinspection.get_service_employees()
RETURNS TABLE(
    employee_id INT,
    full_name VARCHAR,
    position_t VARCHAR,
    contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT s.employee_id, s.full_name, s."position", s.contact_details
    FROM autoinspection.serviceemployees s;
END;
$$;

SELECT * FROM autoinspection.get_service_employees();

CREATE OR REPLACE FUNCTION autoinspection.get_inspections()
RETURNS TABLE(
    inspection_date DATE,
    result VARCHAR,
    notes VARCHAR,
    vin VARCHAR,
    service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT i.inspection_date, i.result, i.notes, i.vin, i.service_employee_id
    FROM autoinspection.inspections i;
END;
$$;

SELECT * FROM autoinspection.get_inspections();

CREATE OR REPLACE FUNCTION autoinspection.get_service_centers()
RETURNS TABLE(
    license VARCHAR,
    name VARCHAR,
    address VARCHAR,
    contact_person VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT sc.license, sc.name, sc.address, sc.contact_person
    FROM autoinspection.servicecenters sc;
END;
$$;

SELECT * FROM autoinspection.get_service_centers();

CREATE OR REPLACE FUNCTION autoinspection.get_protocols()
RETURNS TABLE(
    protocol_number INT,
    issue_date DATE,
    violation_type VARCHAR,
    fine_amount INT,
    vin VARCHAR,
    officer_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT p.protocol_number, p.issue_date, p.violation_type, p.fine_amount, p.vin, p.officer_id
    FROM autoinspection.protocols p;
END;
$$;

SELECT * FROM autoinspection.get_protocols();

CREATE OR REPLACE FUNCTION autoinspection.get_employment()
RETURNS TABLE(
    service_center_license VARCHAR,
    service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT e.service_center_license, e.service_employee_id
    FROM autoinspection.employment e;
END;
$$;

SELECT * FROM autoinspection.get_employment();

-------------------PROCEDURES--------------------------------------------
-- Таблица owners

-- Вставка в таблицу owners: добавляет нового владельца
CREATE OR REPLACE PROCEDURE autoinspection.insert_owner(
    new_full_name VARCHAR,
    new_address VARCHAR,
    new_phone_number VARCHAR,
    new_birth_date DATE,
    new_driver_license_number VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на дублирование водительского удостоверения
    IF EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE driver_license_number = new_driver_license_number
    ) THEN
        RAISE EXCEPTION 'Владелец с номером водительского удостоверения % уже существует.', new_driver_license_number;
    END IF;

    INSERT INTO autoinspection.owners (full_name, address, phone_number, birth_date, driver_license_number)
    VALUES (new_full_name, new_address, new_phone_number, new_birth_date, new_driver_license_number);
    RAISE NOTICE 'Новый владелец добавлен: %', new_full_name;
END;
$$;

-- Обновление таблицы owners: обновляет данные владельца по ID
CREATE OR REPLACE PROCEDURE autoinspection.update_owner(
    target_owner_id INT,
    new_full_name VARCHAR,
    new_address VARCHAR,
    new_phone_number VARCHAR,
    new_birth_date DATE,
    new_driver_license_number VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование владельца
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE owner_id = target_owner_id
    ) THEN
        RAISE EXCEPTION 'Владелец с ID % не существует.', target_owner_id;
    END IF;

    -- Проверка на дублирование номера водительского удостоверения (для других владельцев)
    IF EXISTS (
        SELECT 1 FROM autoinspection.owners
        WHERE driver_license_number = new_driver_license_number
          AND owner_id != target_owner_id
    ) THEN
        RAISE EXCEPTION 'Номер водительского удостоверения % уже принадлежит другому владельцу.', new_driver_license_number;
    END IF;

    UPDATE autoinspection.owners
    SET full_name = new_full_name,
        address = new_address,
        phone_number = new_phone_number,
        birth_date = new_birth_date,
        driver_license_number = new_driver_license_number
    WHERE owner_id = target_owner_id;
    RAISE NOTICE 'Данные владельца обновлены: ID %', target_owner_id;
END;
$$;

-- Удаление из таблицы owners: удаляет владельца по ID
CREATE OR REPLACE PROCEDURE autoinspection.delete_owner(target_owner_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование владельца
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE owner_id = target_owner_id
    ) THEN
        RAISE EXCEPTION 'Владелец с ID % не существует.', target_owner_id;
    END IF;

    DELETE FROM autoinspection.owners WHERE owner_id = target_owner_id;
    RAISE NOTICE 'Владелец удален: ID %', target_owner_id;
END;
$$;

-- Примеры использования:

-- Вставка нового владельца
CALL autoinspection.insert_owner(
    'Иван Иванов',
    'г. Москва, ул. Ленина, д. 1',
    '89001234567',
    '1985-01-15',
    'A123456784'
);

-- Обновление существующего владельца
CALL autoinspection.update_owner(
    1,
    'Иван Петров',
    'г. Санкт-Петербург, ул. Невский, д. 10',
    '89009876543',
    '1990-05-20',
    'B987654321'
);

-- Удаление владельца
CALL autoinspection.delete_owner(1);

-- Таблица cars

-- Вставка в таблицу cars: добавляет новый автомобиль
CREATE OR REPLACE PROCEDURE autoinspection.insert_car(
    new_vin VARCHAR,
    new_brand VARCHAR,
    new_model VARCHAR,
    new_year INT,
    new_color VARCHAR,
    new_license_plate VARCHAR,
    new_owner_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование владельца
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE owner_id = new_owner_id
    ) THEN
        RAISE EXCEPTION 'Владелец с ID % не существует.', new_owner_id;
    END IF;

    -- Проверка на дублирование VIN
    IF EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = new_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % уже существует.', new_vin;
    END IF;

    INSERT INTO autoinspection.cars (vin, brand, model, year, color, license_plate, owner_id)
    VALUES (new_vin, new_brand, new_model, new_year, new_color, new_license_plate, new_owner_id);
    RAISE NOTICE 'Автомобиль добавлен: VIN %', new_vin;
END;
$$;

-- Обновление таблицы cars: обновляет данные автомобиля по VIN
CREATE OR REPLACE PROCEDURE autoinspection.update_car(
    target_vin VARCHAR,
    new_brand VARCHAR,
    new_model VARCHAR,
    new_year INT,
    new_color VARCHAR,
    new_license_plate VARCHAR,
    new_owner_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = target_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', target_vin;
    END IF;

    -- Проверка на существование владельца
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.owners WHERE owner_id = new_owner_id
    ) THEN
        RAISE EXCEPTION 'Владелец с ID % не существует.', new_owner_id;
    END IF;

    UPDATE autoinspection.cars
    SET brand = new_brand,
        model = new_model,
        year = new_year,
        color = new_color,
        license_plate = new_license_plate,
        owner_id = new_owner_id
    WHERE vin = target_vin;
    RAISE NOTICE 'Данные автомобиля обновлены: VIN %', target_vin;
END;
$$;

-- Удаление из таблицы cars: удаляет автомобиль по VIN
CREATE OR REPLACE PROCEDURE autoinspection.delete_car(target_vin VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = target_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', target_vin;
    END IF;

    DELETE FROM autoinspection.cars WHERE vin = target_vin;
    RAISE NOTICE 'Автомобиль удален: VIN %', target_vin;
END;
$$;

-- Примеры использования:

-- Вставка нового автомобиля
CALL autoinspection.insert_car(
    '1HGCM82633A123450',
    'Toyota',
    'Camry',
    2010,
    'Белый',
    'А123ВС75',
    2
);

-- Обновление существующего автомобиля
CALL autoinspection.update_car(
    '1HGCM82633A123454',
    'Honda',
    'Civic',
    2012,
    'Черный',
    'B123BC77',
    2
);

-- Удаление автомобиля
CALL autoinspection.delete_car('1HGCM82633A123454');

-- Таблица inspections
-- Вставка в таблицу inspections: добавляет новый техосмотр
CREATE OR REPLACE PROCEDURE autoinspection.insert_inspection(
    new_inspection_date DATE,
    new_result VARCHAR,
    new_notes VARCHAR,
    new_vin VARCHAR,
    new_service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = new_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', new_vin;
    END IF;

    -- Проверка на существование сотрудника сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', new_service_employee_id;
    END IF;

    INSERT INTO autoinspection.inspections (inspection_date, result, notes, vin, service_employee_id)
    VALUES (new_inspection_date, new_result, new_notes, new_vin, new_service_employee_id);
    RAISE NOTICE 'Техосмотр добавлен: Дата %', new_inspection_date;
END;
$$;

-- Обновление таблицы inspections: обновляет данные техосмотра по дате
CREATE OR REPLACE PROCEDURE autoinspection.update_inspection(
    target_inspection_date DATE,
    new_result VARCHAR,
    new_notes VARCHAR,
    new_vin VARCHAR,
    new_service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование записи техосмотра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.inspections WHERE inspection_date = target_inspection_date
    ) THEN
        RAISE EXCEPTION 'Техосмотр с датой % не существует.', target_inspection_date;
    END IF;

    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = new_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', new_vin;
    END IF;

    -- Проверка на существование сотрудника сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', new_service_employee_id;
    END IF;

    UPDATE autoinspection.inspections
    SET result = new_result,
        notes = new_notes,
        vin = new_vin,
        service_employee_id = new_service_employee_id
    WHERE inspection_date = target_inspection_date;
    RAISE NOTICE 'Данные техосмотра обновлены: Дата %', target_inspection_date;
END;
$$;

-- Удаление из таблицы inspections: удаляет техосмотр
CREATE OR REPLACE PROCEDURE autoinspection.delete_inspection(
    target_inspection_date DATE,
    target_vin VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование записи техосмотра
    IF NOT EXISTS (
        SELECT 1 
        FROM autoinspection.inspections 
        WHERE inspection_date = target_inspection_date AND vin = target_vin
    ) THEN
        RAISE EXCEPTION 'Техосмотр с датой % и VIN % не существует.', target_inspection_date, target_vin;
    END IF;

    DELETE FROM autoinspection.inspections 
    WHERE inspection_date = target_inspection_date AND vin = target_vin;

    RAISE NOTICE 'Техосмотр удален: Дата %, VIN %', target_inspection_date, target_vin;
END;
$$;

-- Вставка нового техосмотра
CALL autoinspection.insert_inspection(
    '2024-01-01',
    'Успешно',
    'Без замечаний',
    '1HGCM82633A123455',
    2
);

-- Обновление существующего техосмотра
CALL autoinspection.update_inspection(
    '2024-01-01',
    'Неуспешно',
    'Замена тормозных колодок',
    '1HGCM82633A123455',
    2
);

-- Удаление техосмотра
CALL autoinspection.delete_inspection('2024-01-01', '1HGCM82633A123455');

-- Таблица protocols

-- Вставка в таблицу protocols: добавляет новый протокол
CREATE OR REPLACE PROCEDURE autoinspection.insert_protocol(
    new_issue_date DATE,
    new_violation_type VARCHAR,
    new_fine_amount INT,
    new_vin VARCHAR,
    new_officer_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = new_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', new_vin;
    END IF;

    -- Проверка на существование сотрудника ГИБДД
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers WHERE officer_id = new_officer_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник ГИБДД с ID % не существует.', new_officer_id;
    END IF;

    INSERT INTO autoinspection.protocols (issue_date, violation_type, fine_amount, vin, officer_id)
    VALUES (new_issue_date, new_violation_type, new_fine_amount, new_vin, new_officer_id);
    RAISE NOTICE 'Протокол добавлен: Дата %', new_issue_date;
END;
$$;

-- Обновление таблицы protocols: обновляет данные протокола по номеру
CREATE OR REPLACE PROCEDURE autoinspection.update_protocol(
    target_protocol_number INT,
    new_issue_date DATE,
    new_violation_type VARCHAR,
    new_fine_amount INT,
    new_vin VARCHAR,
    new_officer_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование протокола
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.protocols WHERE protocol_number = target_protocol_number
    ) THEN
        RAISE EXCEPTION 'Протокол с номером % не существует.', target_protocol_number;
    END IF;

    -- Проверка на существование автомобиля
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.cars WHERE vin = new_vin
    ) THEN
        RAISE EXCEPTION 'Автомобиль с VIN % не существует.', new_vin;
    END IF;

    -- Проверка на существование сотрудника ГИБДД
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers WHERE officer_id = new_officer_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник ГИБДД с ID % не существует.', new_officer_id;
    END IF;

    UPDATE autoinspection.protocols
    SET issue_date = new_issue_date,
        violation_type = new_violation_type,
        fine_amount = new_fine_amount,
        vin = new_vin,
        officer_id = new_officer_id
    WHERE protocol_number = target_protocol_number;
    RAISE NOTICE 'Данные протокола обновлены: Номер %', target_protocol_number;
END;
$$;

-- Удаление из таблицы protocols: удаляет протокол по номеру
CREATE OR REPLACE PROCEDURE autoinspection.delete_protocol(target_protocol_number INT)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование протокола
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.protocols WHERE protocol_number = target_protocol_number
    ) THEN
        RAISE EXCEPTION 'Протокол с номером % не существует.', target_protocol_number;
    END IF;

    DELETE FROM autoinspection.protocols WHERE protocol_number = target_protocol_number;
    RAISE NOTICE 'Протокол удален: Номер %', target_protocol_number;
END;
$$;

-- Примеры использования:

-- Вставка нового протокола
CALL autoinspection.insert_protocol(
    '2024-03-01',
    'Превышение скорости',
    5000,
    '1HGCM82633A123455',
    2
);

-- Обновление существующего протокола
CALL autoinspection.update_protocol(
    2,
    '2024-03-02',
    'ДТП',
    10000,
    '1HGCM82633A123455',
    2
);

-- Удаление протокола
CALL autoinspection.delete_protocol(2);

-- Таблица trafficofficers

-- Вставка в таблицу trafficofficers: добавляет нового сотрудника ГИБДД
CREATE OR REPLACE PROCEDURE autoinspection.insert_trafficofficer(
    new_full_name VARCHAR,
    new_position VARCHAR,
    new_department VARCHAR,
    new_contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на дублирование сотрудника по ФИО и должности
    IF EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers
        WHERE full_name = new_full_name AND position = new_position
    ) THEN
        RAISE EXCEPTION 'Сотрудник ГИБДД с ФИО % и должностью % уже существует.', new_full_name, new_position;
    END IF;

    INSERT INTO autoinspection.trafficofficers (full_name, position, department, contact_details)
    VALUES (new_full_name, new_position, new_department, new_contact_details);
    RAISE NOTICE 'Сотрудник ГИБДД добавлен: %', new_full_name;
END;
$$;

-- Обновление таблицы trafficofficers: обновляет данные сотрудника по ID
CREATE OR REPLACE PROCEDURE autoinspection.update_trafficofficer(
    target_officer_id INT,
    new_full_name VARCHAR,
    new_position VARCHAR,
    new_department VARCHAR,
    new_contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сотрудника
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers WHERE officer_id = target_officer_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник ГИБДД с ID % не существует.', target_officer_id;
    END IF;

    -- Проверка на дублирование данных сотрудника (для других сотрудников)
    IF EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers
        WHERE full_name = new_full_name AND position = new_position
          AND officer_id != target_officer_id
    ) THEN
        RAISE EXCEPTION 'Другой сотрудник ГИБДД с ФИО % и должностью % уже существует.', new_full_name, new_position;
    END IF;

    UPDATE autoinspection.trafficofficers
    SET full_name = new_full_name,
        position = new_position,
        department = new_department,
        contact_details = new_contact_details
    WHERE officer_id = target_officer_id;
    RAISE NOTICE 'Данные сотрудника ГИБДД обновлены: ID %', target_officer_id;
END;
$$;

-- Удаление из таблицы trafficofficers: удаляет сотрудника по ID
CREATE OR REPLACE PROCEDURE autoinspection.delete_trafficofficer(target_officer_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сотрудника
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.trafficofficers WHERE officer_id = target_officer_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник ГИБДД с ID % не существует.', target_officer_id;
    END IF;

    DELETE FROM autoinspection.trafficofficers WHERE officer_id = target_officer_id;
    RAISE NOTICE 'Сотрудник ГИБДД удален: ID %', target_officer_id;
END;
$$;

-- Примеры использования:

-- Вставка нового сотрудника ГИБДД
CALL autoinspection.insert_trafficofficer(
    'Ковалев Сергей',
    'Инспектор',
    'Отдел 1',
    '88005551234'
);

-- Обновление существующего сотрудника ГИБДД
CALL autoinspection.update_trafficofficer(
    1,
    'Ковалев Сергей Николаевич',
    'Старший инспектор',
    'Отдел 1',
    '88005554321'
);

-- Удаление сотрудника ГИБДД
CALL autoinspection.delete_trafficofficer(1);

-- Таблица servicecenters

-- Вставка в таблицу servicecenters: добавляет новый сервисный центр
CREATE OR REPLACE PROCEDURE autoinspection.insert_servicecenter(
    new_license VARCHAR,
    new_name VARCHAR,
    new_address VARCHAR,
    new_contact_person VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на дублирование лицензии
    IF EXISTS (
        SELECT 1 FROM autoinspection.servicecenters WHERE license = new_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с лицензией % уже существует.', new_license;
    END IF;

    -- Проверка на дублирование имени и адреса
    IF EXISTS (
        SELECT 1 FROM autoinspection.servicecenters
        WHERE name = new_name AND address = new_address
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с именем % и адресом % уже существует.', new_name, new_address;
    END IF;

    INSERT INTO autoinspection.servicecenters (license, name, address, contact_person)
    VALUES (new_license, new_name, new_address, new_contact_person);
    RAISE NOTICE 'Сервисный центр добавлен: %', new_name;
END;
$$;

-- Обновление таблицы servicecenters: обновляет данные сервисного центра по лицензии
CREATE OR REPLACE PROCEDURE autoinspection.update_servicecenter(
    target_license VARCHAR,
    new_name VARCHAR,
    new_address VARCHAR,
    new_contact_person VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.servicecenters WHERE license = target_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с лицензией % не существует.', target_license;
    END IF;

    -- Проверка на дублирование имени и адреса (для других лицензий)
    IF EXISTS (
        SELECT 1 FROM autoinspection.servicecenters
        WHERE name = new_name AND address = new_address
          AND license != target_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с именем % и адресом % уже существует.', new_name, new_address;
    END IF;

    UPDATE autoinspection.servicecenters
    SET name = new_name,
        address = new_address,
        contact_person = new_contact_person
    WHERE license = target_license;
    RAISE NOTICE 'Данные сервисного центра обновлены: Лицензия %', target_license;
END;
$$;

-- Удаление из таблицы servicecenters: удаляет сервисный центр по лицензии
CREATE OR REPLACE PROCEDURE autoinspection.delete_servicecenter(target_license VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.servicecenters WHERE license = target_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с лицензией % не существует.', target_license;
    END IF;

    DELETE FROM autoinspection.servicecenters WHERE license = target_license;
    RAISE NOTICE 'Сервисный центр удален: Лицензия %', target_license;
END;
$$;

-- Примеры использования:

-- Вставка нового сервисного центра
CALL autoinspection.insert_servicecenter(
    'SC12346',
    'АвтоСервис',
    'г. Москва, ул. Гагарина, д. 5',
    'Иванова Анна'
);

-- Обновление существующего сервисного центра
CALL autoinspection.update_servicecenter(
    'SC12346',
    'АвтоСервис Плюс',
    'г. Москва, ул. Гагарина, д. 10',
    'Петрова Мария'
);

-- Удаление сервисного центра
CALL autoinspection.delete_servicecenter('SC12346');

-- Таблица serviceemployees

-- Вставка в таблицу serviceemployees: добавляет нового сотрудника сервисного центра
CREATE OR REPLACE PROCEDURE autoinspection.insert_serviceemployee(
    new_full_name VARCHAR,
    new_position VARCHAR,
    new_contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на дублирование сотрудника по ФИО и должности
    IF EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees
        WHERE full_name = new_full_name AND position = new_position
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ФИО % и должностью % уже существует.', new_full_name, new_position;
    END IF;

    INSERT INTO autoinspection.serviceemployees (full_name, position, contact_details)
    VALUES (new_full_name, new_position, new_contact_details);
    RAISE NOTICE 'Сотрудник сервисного центра добавлен: %', new_full_name;
END;
$$;

-- Обновление таблицы serviceemployees: обновляет данные сотрудника по ID
CREATE OR REPLACE PROCEDURE autoinspection.update_serviceemployee(
    target_employee_id INT,
    new_full_name VARCHAR,
    new_position VARCHAR,
    new_contact_details VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сотрудника
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = target_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', target_employee_id;
    END IF;

    -- Проверка на дублирование данных сотрудника (для других сотрудников)
    IF EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees
        WHERE full_name = new_full_name AND position = new_position
          AND employee_id != target_employee_id
    ) THEN
        RAISE EXCEPTION 'Другой сотрудник сервисного центра с ФИО % и должностью % уже существует.', new_full_name, new_position;
    END IF;

    UPDATE autoinspection.serviceemployees
    SET full_name = new_full_name,
        position = new_position,
        contact_details = new_contact_details
    WHERE employee_id = target_employee_id;
    RAISE NOTICE 'Данные сотрудника сервисного центра обновлены: ID %', target_employee_id;
END;
$$;

-- Удаление из таблицы serviceemployees: удаляет сотрудника по ID
CREATE OR REPLACE PROCEDURE autoinspection.delete_serviceemployee(target_employee_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сотрудника
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = target_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', target_employee_id;
    END IF;

    DELETE FROM autoinspection.serviceemployees WHERE employee_id = target_employee_id;
    RAISE NOTICE 'Сотрудник сервисного центра удален: ID %', target_employee_id;
END;
$$;

-- Примеры использования:

-- Вставка нового сотрудника сервисного центра
CALL autoinspection.insert_serviceemployee(
    'Сидоров Сидр',
    'Механик',
    '88005553535'
);

-- Обновление существующего сотрудника сервисного центра
CALL autoinspection.update_serviceemployee(
    1,
    'Сидоров Алексей',
    'Старший механик',
    '88003332211'
);

-- Удаление сотрудника сервисного центра
CALL autoinspection.delete_serviceemployee(1);

-- Таблица employment

-- Вставка в таблицу employment: добавляет связь между сервисным центром и сотрудником
CREATE OR REPLACE PROCEDURE autoinspection.insert_employment(
    new_service_center_license VARCHAR,
    new_service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.servicecenters WHERE license = new_service_center_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с лицензией % не существует.', new_service_center_license;
    END IF;

    -- Проверка на существование сотрудника сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', new_service_employee_id;
    END IF;

    -- Проверка на дублирование записи занятости
    IF EXISTS (
        SELECT 1 FROM autoinspection.employment
        WHERE service_center_license = new_service_center_license AND service_employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Связь между сервисным центром % и сотрудником % уже существует.', new_service_center_license, new_service_employee_id;
    END IF;

    INSERT INTO autoinspection.employment (service_center_license, service_employee_id)
    VALUES (new_service_center_license, new_service_employee_id);
    RAISE NOTICE 'Связь добавлена: Сервисный центр %, Сотрудник %', new_service_center_license, new_service_employee_id;
END;
$$;

-- Обновление таблицы employment: обновляет связь между сервисным центром и сотрудником
CREATE OR REPLACE PROCEDURE autoinspection.update_employment(
    target_service_center_license VARCHAR,
    target_service_employee_id INT,
    new_service_center_license VARCHAR,
    new_service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование старой записи занятости
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.employment
        WHERE service_center_license = target_service_center_license AND service_employee_id = target_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Связь между сервисным центром % и сотрудником % не существует.', target_service_center_license, target_service_employee_id;
    END IF;

    -- Проверка на существование нового сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.servicecenters WHERE license = new_service_center_license
    ) THEN
        RAISE EXCEPTION 'Сервисный центр с лицензией % не существует.', new_service_center_license;
    END IF;

    -- Проверка на существование нового сотрудника сервисного центра
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.serviceemployees WHERE employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Сотрудник сервисного центра с ID % не существует.', new_service_employee_id;
    END IF;

    -- Проверка на дублирование новой записи занятости
    IF EXISTS (
        SELECT 1 FROM autoinspection.employment
        WHERE service_center_license = new_service_center_license AND service_employee_id = new_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Новая связь между сервисным центром % и сотрудником % уже существует.', new_service_center_license, new_service_employee_id;
    END IF;

    UPDATE autoinspection.employment
    SET service_center_license = new_service_center_license,
        service_employee_id = new_service_employee_id
    WHERE service_center_license = target_service_center_license AND service_employee_id = target_service_employee_id;
    RAISE NOTICE 'Связь обновлена: Сервисный центр %, Сотрудник %', new_service_center_license, new_service_employee_id;
END;
$$;

-- Удаление из таблицы employment: удаляет связь между сервисным центром и сотрудником
CREATE OR REPLACE PROCEDURE autoinspection.delete_employment(
    target_service_center_license VARCHAR,
    target_service_employee_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Проверка на существование записи занятости
    IF NOT EXISTS (
        SELECT 1 FROM autoinspection.employment
        WHERE service_center_license = target_service_center_license AND service_employee_id = target_service_employee_id
    ) THEN
        RAISE EXCEPTION 'Связь между сервисным центром % и сотрудником % не существует.', target_service_center_license, target_service_employee_id;
    END IF;

    DELETE FROM autoinspection.employment
    WHERE service_center_license = target_service_center_license AND service_employee_id = target_service_employee_id;
    RAISE NOTICE 'Связь удалена: Сервисный центр %, Сотрудник %', target_service_center_license, target_service_employee_id;
END;
$$;

-- Примеры использования:

-- Вставка новой связи
CALL autoinspection.insert_employment(
    'SC45678',
    1
);

-- Обновление существующей связи
CALL autoinspection.update_employment(
    'SC12346',
    1,
    'SC67890',
    2
);

-- Удаление связи
CALL autoinspection.delete_employment('SC12346', 1);

----------------------------TRIGGERS----------------------------------------------------------------
-- Создание таблицы logs для записи всех изменений
CREATE TABLE IF NOT EXISTS autoinspection.logs (
    log_id SERIAL PRIMARY KEY,
    table_name VARCHAR NOT NULL,
    action_type VARCHAR NOT NULL,
    action_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Создание функции для логирования изменений
CREATE OR REPLACE FUNCTION autoinspection.log_changes()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- Определяем тип действия (INSERT, UPDATE, DELETE)
    IF TG_OP = 'INSERT' THEN
        INSERT INTO autoinspection.logs (table_name, action_type)
        VALUES (TG_TABLE_NAME, 'INSERT');
    ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO autoinspection.logs (table_name, action_type)
        VALUES (TG_TABLE_NAME, 'UPDATE');
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO autoinspection.logs (table_name, action_type)
        VALUES (TG_TABLE_NAME, 'DELETE');
    END IF;

    -- Возвращаем результат для корректного выполнения действия
    IF TG_OP = 'DELETE' THEN
        RETURN OLD;
    ELSE
        RETURN NEW;
    END IF;
END;
$$;

-- Создание триггеров для таблицы owners
CREATE TRIGGER owners_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.owners
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы cars
CREATE TRIGGER cars_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.cars
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы protocols
CREATE TRIGGER protocols_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.protocols
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы inspections
CREATE TRIGGER inspections_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.inspections
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы trafficofficers
CREATE TRIGGER trafficofficers_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.trafficofficers
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы servicecenters
CREATE TRIGGER servicecenters_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.servicecenters
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы serviceemployees
CREATE TRIGGER serviceemployees_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.serviceemployees
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Создание триггеров для таблицы employment
CREATE TRIGGER employment_log_trigger
AFTER INSERT OR UPDATE OR DELETE ON autoinspection.employment
FOR EACH ROW
EXECUTE FUNCTION autoinspection.log_changes();

-- Функция: получение логов с фильтрацией (возвращает JSON)
CREATE OR REPLACE FUNCTION autoinspection.fetch_logs(
    filter_table_name VARCHAR DEFAULT NULL, -- Фильтр по имени таблицы
    filter_action_type VARCHAR DEFAULT NULL, -- Фильтр по типу действия
    filter_start_time TIMESTAMP DEFAULT NULL, -- Начальная дата/время
    filter_end_time TIMESTAMP DEFAULT NULL -- Конечная дата/время
)
RETURNS SETOF JSONB LANGUAGE plpgsql
AS $$
BEGIN
    -- Возвращаем строки, соответствующие фильтрам, в формате JSONB
    RETURN QUERY
    SELECT to_jsonb(l)
    FROM autoinspection.logs l
    WHERE (filter_table_name IS NULL OR l.table_name = filter_table_name)
      AND (filter_action_type IS NULL OR l.action_type = filter_action_type)
      AND (filter_start_time IS NULL OR l.action_time >= filter_start_time)
      AND (filter_end_time IS NULL OR l.action_time <= filter_end_time);
END;
$$;

INSERT INTO autoinspection.owners (full_name, address, phone_number, birth_date, driver_license_number)
VALUES ('Иван Иванов', 'г. Москва, ул. Ленина, д. 1', '89001234567', '1985-01-15', 'A123456784');

-- Примеры вызова функции

-- 1. Получить все логи
SELECT autoinspection.fetch_logs();

-- 2. Получить логи для таблицы 'owners'
SELECT autoinspection.fetch_logs('owners');

-- 3. Получить логи только с действием 'INSERT'
SELECT autoinspection.fetch_logs(NULL, 'INSERT');

-- 4. Получить логи за последние 7 дней
SELECT autoinspection.fetch_logs(
    NULL, 
    NULL, 
    (NOW() - INTERVAL '7 days')::TIMESTAMP, 
    NOW()::TIMESTAMP
);

-- 5. Получить логи для таблицы 'protocols' с действием 'DELETE' за последние 30 дней
SELECT autoinspection.fetch_logs(
    'protocols', 
    'DELETE', 
    (NOW() - INTERVAL '30 days')::TIMESTAMP, 
    NOW()::TIMESTAMP
);

----------------------------LOGIN----------------------------------------------------------------
-- Установка расширения для хеширования (pgcrypto)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Создание пользователей в PostgreSQL
-- 1. Создание роли для администратора
CREATE ROLE st2093_01_admin WITH LOGIN PASSWORD '';

-- 2. Создание роли для сотрудника ГИБДД
CREATE ROLE st2093_01_traffic_officer WITH LOGIN PASSWORD '';

-- 3. Создание роли для сотрудника автосервиса
CREATE ROLE st2093_01_service_employee WITH LOGIN PASSWORD '';

-- Привилегии для ролей
-- Администратор: Полный доступ ко всем таблицам
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA autoinspection TO st2093_01_admin;

-- Сотрудник ГИБДД: Доступы к таблицам
GRANT USAGE ON SCHEMA autoinspection TO st2093_01_traffic_officer;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.cars TO st2093_01_traffic_officer;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.owners TO st2093_01_traffic_officer;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.protocols TO st2093_01_traffic_officer;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.trafficofficers TO st2093_01_traffic_officer;
GRANT SELECT ON autoinspection.inspections TO st2093_01_traffic_officer;
GRANT SELECT ON autoinspection.employment TO st2093_01_traffic_officer;
GRANT SELECT ON autoinspection.servicecenters TO st2093_01_traffic_officer;
GRANT SELECT ON autoinspection.serviceemployees TO st2093_01_traffic_officer;
GRANT SELECT, INSERT, UPDATE ON autoinspection.logs TO st2093_01_traffic_officer;

-- Дать роль USAGE (использование) на последовательность
GRANT USAGE ON SEQUENCE autoinspection.logs_log_id_seq TO st2093_01_traffic_officer;
GRANT USAGE ON SEQUENCE autoinspection.owners_owner_id_seq TO st2093_01_traffic_officer;
GRANT USAGE ON SEQUENCE autoinspection.trafficofficers_officer_id_seq TO st2093_01_traffic_officer;
GRANT USAGE ON SEQUENCE autoinspection.protocols_protocol_number_seq TO st2093_01_traffic_officer;

-- Доступ к представлениям
GRANT SELECT ON autoinspection.failed_inspection_owners TO st2093_01_traffic_officer
GRANT SELECT ON autoinspection.luxury_car_owners TO st2093_01_traffic_officer
GRANT SELECT ON autoinspection.inspections_by_service_center TO st2093_01_traffic_officer
GRANT SELECT ON autoinspection.protocols_by_officer TO st2093_01_traffic_officer
GRANT SELECT ON autoinspection.accident_participants TO st2093_01_traffic_officer

-- Сотрудник автосервиса: Доступы к таблицам
GRANT USAGE ON SCHEMA autoinspection TO st2093_01_service_employee;
GRANT SELECT ON autoinspection.cars TO st2093_01_service_employee;
GRANT SELECT ON autoinspection.owners TO st2093_01_service_employee;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.employment TO st2093_01_service_employee;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.inspections TO st2093_01_service_employee;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.servicecenters TO st2093_01_service_employee;
GRANT SELECT, INSERT, UPDATE, DELETE ON autoinspection.serviceemployees TO st2093_01_service_employee;
GRANT SELECT ON autoinspection.protocols TO st2093_01_service_employee;
GRANT SELECT ON autoinspection.trafficofficers TO st2093_01_service_employee;
GRANT SELECT, INSERT, UPDATE ON autoinspection.logs TO st2093_01_service_employee;

-- Дать роль USAGE (использование) на последовательность
GRANT USAGE ON SEQUENCE autoinspection.logs_log_id_seq TO st2093_01_service_employee;
GRANT USAGE ON SEQUENCE autoinspection.serviceemployees_employee_id_seq TO st2093_01_service_employee;

-- Доступ к представлениям
GRANT SELECT ON autoinspection.failed_inspection_owners TO st2093_01_service_employee
GRANT SELECT ON autoinspection.inspections_by_service_center TO st2093_01_service_employee

-- Пример проверки прав доступа к таблице
SELECT has_table_privilege('st2093_01_service_employee', 'autoinspection.owners', 'SELECT')