CREATE DATABASE BookstoreUserDb;
GO

USE BookstoreUserDb;
GO

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PhoneNumber BIGINT NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role INT NOT NULL,
    CreatedUtc DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedUtc DEFAULT SYSUTCDATETIME()
);
GO

CREATE UNIQUE INDEX IX_Users_Email
ON Users (Email);
GO
 
select * from Users;



CREATE DATABASE BookstoreAddressDb;
GO

USE BookstoreAddressDb;
GO

CREATE TABLE Addresses
(
    AddressId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    MobileNumber BIGINT,
    UserAddress NVARCHAR(255),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Type INT,
    UserId INT
);

select * from Addresses;