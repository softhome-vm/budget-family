SET timezone = 'Europe/Moscow';
CREATE TABLE IF NOT EXISTS "Члены семьи" (
    "idMember" SERIAL PRIMARY KEY,
    "Фамилия" TEXT NOT NULL,
    "Имя" TEXT NOT NULL,
    "Отчество" TEXT,
    "Номер телефона" TEXT,
    "Эл_почта" TEXT
);

CREATE TABLE IF NOT EXISTS "Категории" (
    "idКатегории" SERIAL PRIMARY KEY,
    "Название_категории" TEXT NOT NULL,
    "Тип" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS "Аккаунты" (
    "idАккаунта" SERIAL PRIMARY KEY,
    "idMember" INTEGER NOT NULL,
    "Название банка" TEXT,
    "Balance" NUMERIC(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT "FK_Аккаунты_ЧленыСемьи" FOREIGN KEY ("idMember")
        REFERENCES "Члены семьи"("idMember") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Транзакции" (
    "idТранзакции" SERIAL PRIMARY KEY,
    "idАккаунта" INTEGER NOT NULL,
    "idКатегории" INTEGER NOT NULL,
    "Сумма" NUMERIC(18,2) NOT NULL DEFAULT 0,
    "Дата" DATE NOT NULL DEFAULT CURRENT_DATE,
    "Комментарий" TEXT,
    CONSTRAINT "FK_Транзакции_Аккаунты" FOREIGN KEY ("idАккаунта")
        REFERENCES "Аккаунты"("idАккаунта") ON DELETE RESTRICT,
    CONSTRAINT "FK_Транзакции_Категории" FOREIGN KEY ("idКатегории")
        REFERENCES "Категории"("idКатегории") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Бюджет" (
    "idБюджета" SERIAL PRIMARY KEY,
    "idКатегории" INTEGER NOT NULL,
    "Дата" DATE NOT NULL,
    "Лимит" NUMERIC(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT "FK_Бюджет_Категории" FOREIGN KEY ("idКатегории")
        REFERENCES "Категории"("idКатегории") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IDX_Транзакции_Дата" ON "Транзакции"("Дата");
CREATE INDEX IF NOT EXISTS "IDX_Транзакции_Аккаунт" ON "Транзакции"("idАккаунта");
CREATE INDEX IF NOT EXISTS "IDX_Бюджет_Дата" ON "Бюджет"("Дата");

INSERT INTO "Члены семьи" ("Фамилия", "Имя", "Отчество", "Номер телефона", "Эл_почта")
SELECT 'Иванов', 'Алексей', 'Сергеевич', '+7 (999) 123-45-67', 'ivanov.a@mail.ru'
WHERE NOT EXISTS (SELECT 1 FROM "Члены семьи" WHERE "Фамилия" = 'Иванов' AND "Имя" = 'Алексей');

INSERT INTO "Члены семьи" ("Фамилия", "Имя", "Отчество", "Номер телефона", "Эл_почта")
SELECT 'Иванова', 'Мария', 'Дмитриевна', '+7 (999) 234-56-78', 'ivanova.m@mail.ru'
WHERE NOT EXISTS (SELECT 1 FROM "Члены семьи" WHERE "Фамилия" = 'Иванова' AND "Имя" = 'Мария');

INSERT INTO "Члены семьи" ("Фамилия", "Имя", "Отчество", "Номер телефона", "Эл_почта")
SELECT 'Петров', 'Иван', 'Александрович', '+7 (999) 345-67-89', 'petrov.i@mail.ru'
WHERE NOT EXISTS (SELECT 1 FROM "Члены семьи" WHERE "Фамилия" = 'Петров' AND "Имя" = 'Иван');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Зарплата', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Зарплата');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Премия', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Премия');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Подработка', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Подработка');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Продукты', FALSE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Продукты');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Коммунальные платежи', FALSE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Коммунальные платежи');

INSERT INTO "Категории" ("Название_категории", "Тип") SELECT 'Транспорт', FALSE
WHERE NOT EXISTS (SELECT 1 FROM "Категории" WHERE "Название_категории" = 'Транспорт');

INSERT INTO "Аккаунты" ("idMember", "Название банка", "Balance")
SELECT m."idMember", 'Сбербанк', 85000.00
FROM "Члены семьи" m WHERE m."Фамилия" = 'Иванов' AND m."Имя" = 'Алексей'
AND NOT EXISTS (SELECT 1 FROM "Аккаунты" WHERE "Название банка" = 'Сбербанк' AND "idMember" = m."idMember");

INSERT INTO "Аккаунты" ("idMember", "Название банка", "Balance")
SELECT m."idMember", 'Тинькофф', 124500.50
FROM "Члены семьи" m WHERE m."Фамилия" = 'Иванова' AND m."Имя" = 'Мария'
AND NOT EXISTS (SELECT 1 FROM "Аккаунты" WHERE "Название банка" = 'Тинькофф' AND "idMember" = m."idMember");

INSERT INTO "Аккаунты" ("idMember", "Название банка", "Balance")
SELECT m."idMember", 'Альфа-Банк', 43000.75
FROM "Члены семьи" m WHERE m."Фамилия" = 'Петров' AND m."Имя" = 'Иван'
AND NOT EXISTS (SELECT 1 FROM "Аккаунты" WHERE "Название банка" = 'Альфа-Банк' AND "idMember" = m."idMember");

INSERT INTO "Транзакции" ("idАккаунта", "idКатегории", "Сумма", "Дата", "Комментарий")
SELECT a."idАккаунта", k."idКатегории", 65000.00, '2025-04-01', 'Зарплата за март'
FROM "Аккаунты" a, "Категории" k
WHERE a."Название банка" = 'Сбербанк' AND k."Название_категории" = 'Зарплата'
AND NOT EXISTS (SELECT 1 FROM "Транзакции" t WHERE t."Комментарий" = 'Зарплата за март' AND t."idАккаунта" = a."idАккаунта");

INSERT INTO "Транзакции" ("idАккаунта", "idКатегории", "Сумма", "Дата", "Комментарий")
SELECT a."idАккаунта", k."idКатегории", 4500.50, '2025-04-03', 'Продукты на неделю'
FROM "Аккаунты" a, "Категории" k
WHERE a."Название банка" = 'Сбербанк' AND k."Название_категории" = 'Продукты'
AND NOT EXISTS (SELECT 1 FROM "Транзакции" t WHERE t."Комментарий" = 'Продукты на неделю' AND t."idАккаунта" = a."idАккаунта");

INSERT INTO "Транзакции" ("idАккаунта", "idКатегории", "Сумма", "Дата", "Комментарий")
SELECT a."idАккаунта", k."idКатегории", 8500.00, '2025-04-05', 'Оплата ЖКХ за апрель'
FROM "Аккаунты" a, "Категории" k
WHERE a."Название банка" = 'Тинькофф' AND k."Название_категории" = 'Коммунальные платежи'
AND NOT EXISTS (SELECT 1 FROM "Транзакции" t WHERE t."Комментарий" = 'Оплата ЖКХ за апрель' AND t."idАккаунта" = a."idАккаунта");

INSERT INTO "Транзакции" ("idАккаунта", "idКатегории", "Сумма", "Дата", "Комментарий")
SELECT a."idАккаунта", k."idКатегории", 15000.00, '2025-04-10', 'Премия за квартал'
FROM "Аккаунты" a, "Категории" k
WHERE a."Название банка" = 'Тинькофф' AND k."Название_категории" = 'Премия'
AND NOT EXISTS (SELECT 1 FROM "Транзакции" t WHERE t."Комментарий" = 'Премия за квартал' AND t."idАккаунта" = a."idАккаунта");

INSERT INTO "Транзакции" ("idАккаунта", "idКатегории", "Сумма", "Дата", "Комментарий")
SELECT a."idАккаунта", k."idКатегории", 2500.00, '2025-04-12', 'Транспортные расходы'
FROM "Аккаунты" a, "Категории" k
WHERE a."Название банка" = 'Альфа-Банк' AND k."Название_категории" = 'Транспорт'
AND NOT EXISTS (SELECT 1 FROM "Транзакции" t WHERE t."Комментарий" = 'Транспортные расходы' AND t."idАккаунта" = a."idАккаунта");

INSERT INTO "Бюджет" ("idКатегории", "Дата", "Лимит")
SELECT k."idКатегории", '2025-04-01', 15000.00
FROM "Категории" k WHERE k."Название_категории" = 'Продукты'
AND NOT EXISTS (SELECT 1 FROM "Бюджет" b WHERE b."idКатегории" = k."idКатегории" AND b."Дата" = '2025-04-01');

INSERT INTO "Бюджет" ("idКатегории", "Дата", "Лимит")
SELECT k."idКатегории", '2025-04-01', 12000.00
FROM "Категории" k WHERE k."Название_категории" = 'Коммунальные платежи'
AND NOT EXISTS (SELECT 1 FROM "Бюджет" b WHERE b."idКатегории" = k."idКатегории" AND b."Дата" = '2025-04-01');

INSERT INTO "Бюджет" ("idКатегории", "Дата", "Лимит")
SELECT k."idКатегории", '2025-04-01', 5000.00
FROM "Категории" k WHERE k."Название_категории" = 'Транспорт'
AND NOT EXISTS (SELECT 1 FROM "Бюджет" b WHERE b."idКатегории" = k."idКатегории" AND b."Дата" = '2025-04-01');

SELECT setval(pg_get_serial_sequence('"Члены семьи"', 'idMember'),
    COALESCE((SELECT MAX("idMember") FROM "Члены семьи"), 0) + 1, false);

SELECT setval(pg_get_serial_sequence('"Категории"', 'idКатегории'),
    COALESCE((SELECT MAX("idКатегории") FROM "Категории"), 0) + 1, false);

SELECT setval(pg_get_serial_sequence('"Аккаунты"', 'idАккаунта'),
    COALESCE((SELECT MAX("idАккаунта") FROM "Аккаунты"), 0) + 1, false);

SELECT setval(pg_get_serial_sequence('"Транзакции"', 'idТранзакции'),
    COALESCE((SELECT MAX("idТранзакции") FROM "Транзакции"), 0) + 1, false);

SELECT setval(pg_get_serial_sequence('"Бюджет"', 'idБюджета'),
    COALESCE((SELECT MAX("idБюджета") FROM "Бюджет"), 0) + 1, false);
