-- SQL-скрипт для создания администратора
-- Креды:
-- Email: 'admin@uni.ru'
-- Пароль: 'Admin123!'
INSERT INTO profiles (id, name, surname, last_name, email, phone, birthday, gender, nationality, password, role, created_at)
VALUES (
    '10000000-0000-0000-0000-000000000001',
    'Админ',
    'Админов',
    'Админович',
    'admin@uni.ru',
    '+79999999999',
    '1990-01-01',
    0,  -- Male
    'Русский',
    'Admin123!',  
    4,  -- Admin (Role.Admin = 4)
    NOW()
);
