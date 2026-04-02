CREATE DATABASE IF NOT EXISTS ai_budget_spending_analyzer;
USE ai_budget_spending_analyzer;

CREATE TABLE IF NOT EXISTS Users (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(200) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    Role INT NOT NULL DEFAULT 1,
    MonthlyIncome DECIMAL(18, 2) NOT NULL DEFAULT 0,
    FinancialGoal VARCHAR(500) NOT NULL DEFAULT '',
    CurrencyCode VARCHAR(3) NOT NULL DEFAULT 'USD',
    PrefersDarkMode TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAtUtc DATETIME(6) NULL
);

CREATE TABLE IF NOT EXISTS Categories (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Icon VARCHAR(50) NOT NULL DEFAULT '',
    ColorHex VARCHAR(20) NOT NULL,
    IsSystemDefined TINYINT(1) NOT NULL DEFAULT 0,
    UserId INT NULL,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAtUtc DATETIME(6) NULL,
    CONSTRAINT FK_Categories_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Transactions (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Amount DECIMAL(18, 2) NOT NULL,
    Type INT NOT NULL,
    TransactionDateUtc DATETIME(6) NOT NULL,
    Notes VARCHAR(500) NOT NULL DEFAULT '',
    IsRecurring TINYINT(1) NOT NULL DEFAULT 0,
    RecurringFrequency INT NULL,
    NextOccurrenceDateUtc DATETIME(6) NULL,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAtUtc DATETIME(6) NULL,
    CONSTRAINT FK_Transactions_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Transactions_Categories_CategoryId FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS Budgets (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    LimitAmount DECIMAL(18, 2) NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,
    AlertThresholdPercentage DECIMAL(5, 2) NOT NULL DEFAULT 90,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAtUtc DATETIME(6) NULL,
    CONSTRAINT UQ_Budgets_User_Category_Period UNIQUE (UserId, CategoryId, Month, Year),
    CONSTRAINT FK_Budgets_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Budgets_Categories_CategoryId FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS Notifications (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Type INT NOT NULL DEFAULT 1,
    Title VARCHAR(150) NOT NULL,
    Message VARCHAR(500) NOT NULL,
    IsRead TINYINT(1) NOT NULL DEFAULT 0,
    ReferenceId VARCHAR(150) NOT NULL DEFAULT '',
    UserId INT NOT NULL,
    CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    UpdatedAtUtc DATETIME(6) NULL,
    CONSTRAINT FK_Notifications_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE
);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Salary', 'payments', '#2563EB', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Salary' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Food', 'restaurant', '#F97316', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Food' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Housing', 'home', '#059669', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Housing' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Transport', 'directions_car', '#8B5CF6', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Transport' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Utilities', 'bolt', '#14B8A6', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Utilities' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Entertainment', 'movie', '#EC4899', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Entertainment' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Healthcare', 'favorite', '#EF4444', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Healthcare' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Savings', 'savings', '#22C55E', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Savings' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Shopping', 'shopping_bag', '#0EA5E9', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Shopping' AND IsSystemDefined = 1);

INSERT INTO Categories (Name, Icon, ColorHex, IsSystemDefined)
SELECT 'Travel', 'flight', '#A855F7', 1
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Travel' AND IsSystemDefined = 1);
