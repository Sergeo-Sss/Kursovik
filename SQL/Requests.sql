-- Запрос о владельцах, не прошедших ТО
SELECT 
    o.full_name AS owner_name,
    o.phone_number AS contact_number,
    c.brand AS car_brand,
    c.model AS car_model
FROM 
    autoinspection.owners o
JOIN 
    autoinspection.cars c ON o.owner_id = c.owner_id
JOIN 
    autoinspection.inspections i ON c.vin = i.vin
WHERE 
    i.result = 'Не пройден';

-- Запрос о владельцах дорогих иномарок
SELECT 
    o.full_name AS owner_name,
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
    c.year >= 2018; -- Пример: автомобили, выпущенные после 2018 года

-- Запрос о ТО в определенном сервисе 
SELECT 
    i.inspection_date,
    i.result AS inspection_result,
    i.notes AS inspection_notes,
    c.vin AS car_vin,
    c.brand AS car_brand,
    c.model AS car_model,
    sc.name AS service_center_name
FROM 
    autoinspection.inspections i
JOIN 
    autoinspection.cars c ON i.vin = c.vin
JOIN 
    autoinspection.employment e ON i.service_employee_id = e.service_employee_id
JOIN 
    autoinspection.servicecenters sc ON e.service_center_license = sc.license
WHERE 
    sc.name = 'CarFix'; -- Замените на нужный сервисный центр


-- Протоколы выданные конкретным сотрудником 
SELECT 
    p.protocol_number,
    p.issue_date,
    p.violation_type,
    p.fine_amount,
    c.vin AS car_vin,
    c.brand AS car_brand,
    c.model AS car_model,
    t.full_name AS officer_name
FROM 
    autoinspection.protocols p
JOIN 
    autoinspection.cars c ON p.vin = c.vin
JOIN 
    autoinspection.trafficofficers t ON p.officer_id = t.officer_id
WHERE 
    t.full_name = 'Дмитрий Ильин'; -- Замените на нужное имя инспектора

-- Владельцы автомобилей, участвовавших в ДТП
SELECT 
    o.full_name AS owner_name,
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
    p.violation_type = 'ДТП'; -- Условие для фильтрации только ДТП

-- 1. Простые запросы: Автомобили, выпущенные между 2015 и 2020 годами
SELECT 
    vin, brand, model, year
FROM 
    autoinspection.cars
WHERE 
    year BETWEEN 2015 AND 2020;

-- 2. Простые запросы: Владельцы, чьё имя начинается с буквы "А"
SELECT 
    full_name, phone_number
FROM 
    autoinspection.owners
WHERE 
    full_name LIKE 'А%';

-- 3. Простые запросы: Протоколы с суммой штрафа больше 5000 или за нарушение "Превышение скорости"
SELECT 
    protocol_number, violation_type, fine_amount
FROM 
    autoinspection.protocols
WHERE 
    fine_amount > 5000 OR violation_type = 'Превышение скорости';

-- 4. Скалярные подзапросы: Найти автомобили, принадлежащие самому молодому владельцу
SELECT 
    vin, brand, model
FROM 
    autoinspection.cars
WHERE 
    owner_id = (
        SELECT owner_id
        FROM autoinspection.owners
        ORDER BY birth_date DESC
        LIMIT 1
    );

-- 5. Табличные подзапросы: Владельцы, чьи автомобили зарегистрированы в базе ТО
SELECT 
    o.full_name, o.phone_number
FROM 
    autoinspection.owners o
WHERE 
    EXISTS (
        SELECT 1
        FROM autoinspection.cars c
        JOIN autoinspection.inspections i ON c.vin = i.vin
        WHERE c.owner_id = o.owner_id
    );

-- 6. Кванторы: Автомобили, участвовавшие во всех протоколах
SELECT 
    vin, brand, model
FROM 
    autoinspection.cars c
WHERE 
    NOT EXISTS (
        SELECT 1
        FROM autoinspection.protocols p
        WHERE p.vin = c.vin AND p.violation_type != 'ДТП'
    );

-- 7. Множественные операции: Владельцы, у которых есть автомобили, но нет протоколов
SELECT 
    full_name
FROM 
    autoinspection.owners
WHERE 
    owner_id IN (
        SELECT owner_id
        FROM autoinspection.cars
    )
EXCEPT
SELECT 
    o.full_name
FROM 
    autoinspection.owners o
JOIN 
    autoinspection.cars c ON o.owner_id = c.owner_id
JOIN 
    autoinspection.protocols p ON c.vin = p.vin;

-- 8. С CTE (вынесенные подзапросы): Владельцы и общее количество их автомобилей
WITH OwnerCars AS (
    SELECT 
        owner_id, COUNT(*) AS car_count
    FROM 
        autoinspection.cars
    GROUP BY 
        owner_id
)
SELECT 
    o.full_name, oc.car_count
FROM 
    autoinspection.owners o
JOIN 
    OwnerCars oc ON o.owner_id = oc.owner_id;

-- 9. Агрегатные функции: Средний размер штрафа по каждому типу нарушения
SELECT 
    violation_type, AVG(fine_amount) AS avg_fine
FROM 
    autoinspection.protocols
GROUP BY 
    violation_type
HAVING 
    AVG(fine_amount) > 1000;

-- 10. Многотабличные запросы: ТО автомобилей с указанием имени сотрудника и сервиса
SELECT 
    i.inspection_date, i.result, i.notes, 
    c.brand, c.model, 
    e.full_name AS employee_name, 
    sc.name AS service_center
FROM 
    autoinspection.inspections i
JOIN 
    autoinspection.cars c ON i.vin = c.vin
JOIN 
    autoinspection.serviceemployees e ON i.service_employee_id = e.employee_id
JOIN 
    autoinspection.servicecenters sc ON e.employee_id = i.service_employee_id;

-- 11. Функции работы со строками: Форматирование номеров протоколов (добавить префикс "PR-")
SELECT 
    CONCAT('PR-', protocol_number) AS formatted_protocol, 
    violation_type
FROM 
    autoinspection.protocols;

-- 12. Рекурсивные подзапросы: Пример рекурсивного запроса для поиска всех сервисных центров, связанных по лицензии
WITH RECURSIVE ServiceHierarchy AS (
    SELECT 
        license, name
    FROM 
        autoinspection.servicecenters
    UNION ALL
    SELECT 
        sc.license, sc.name
    FROM 
        autoinspection.servicecenters sc
    JOIN 
        ServiceHierarchy sh ON sc.license = sh.license
)
SELECT 
    * 
FROM 
    ServiceHierarchy;

-- 13. Сводные таблицы: Количество протоколов по видам нарушений и годам
SELECT 
    violation_type,
    COUNT(CASE WHEN EXTRACT(YEAR FROM issue_date) = 2023 THEN 1 END) AS year_2023,
    COUNT(CASE WHEN EXTRACT(YEAR FROM issue_date) = 2022 THEN 1 END) AS year_2022
FROM 
    autoinspection.protocols
GROUP BY 
    violation_type;

-- 14. Оконные функции: Нумерация протоколов по сотрудникам
SELECT 
    protocol_number, 
    officer_id, 
    RANK() OVER (PARTITION BY officer_id ORDER BY issue_date) AS protocol_rank
FROM 
    autoinspection.protocols;