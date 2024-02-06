CREATE TABLE dbo.Department(
	DepartmentId int PRIMARY KEY IDENTITY(1,1),
	DepartmentCode nvarchar(50),
	DepartmentName nvarchar(500)
)


CREATE TABLE dbo.Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    EmailAddress VARCHAR(100),
    DateOfBirth DATE,
    Age INT,
    Salary DECIMAL(10, 2),
    DepartmentID INT,
    FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID)
);

ALTER TABLE dbo.Employee
ADD ProfilePhotoName NVARCHAR(500);

