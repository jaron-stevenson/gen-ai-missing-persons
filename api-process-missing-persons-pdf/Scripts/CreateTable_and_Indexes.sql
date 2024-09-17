-- Create the MissingPersons table
CREATE TABLE MissingPersons (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
    Race NVARCHAR(50),
    Age INT,
    Sex NVARCHAR(10),
    Height NVARCHAR(20),
    Weight NVARCHAR(20),
    EyeColor NVARCHAR(20),
    Hair NVARCHAR(50),
    Alias NVARCHAR(100),
    Tattoos NVARCHAR(MAX),
    LastSeen DATE,
    DateReported DATE,
    MissingFrom NVARCHAR(100),
    ConditionsOfDisappearance NVARCHAR(MAX),
    OfficerInfo NVARCHAR(100),
    PhoneNumber1 NVARCHAR(20),
    PhoneNumber2 NVARCHAR(20),
    CurrentStatus NVARCHAR(7) CHECK (CurrentStatus IN ('Missing', 'Found'))
);

-- Create indexes
CREATE NONCLUSTERED INDEX IX_MissingPersons_Name_Status ON MissingPersons (Name, CurrentStatus);
CREATE NONCLUSTERED INDEX IX_MissingPersons_LastSeen_Status ON MissingPersons (LastSeen, CurrentStatus);
CREATE NONCLUSTERED INDEX IX_MissingPersons_DateReported_Status ON MissingPersons (DateReported, CurrentStatus);
CREATE NONCLUSTERED INDEX IX_MissingPersons_MissingFrom_Status ON MissingPersons (MissingFrom, CurrentStatus);
CREATE NONCLUSTERED INDEX IX_MissingPersons_CurrentStatus ON MissingPersons (CurrentStatus);
