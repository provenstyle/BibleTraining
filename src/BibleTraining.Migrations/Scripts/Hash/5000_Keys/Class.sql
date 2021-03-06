﻿USE $DbName$;
GO

IF OBJECT_ID(N'FK_Class_Country') IS NULL
BEGIN
	ALTER TABLE dbo.Class WITH CHECK ADD CONSTRAINT FK_Class_Country FOREIGN KEY(CountryId)
	REFERENCES dbo.Country (Id);
END;

IF OBJECT_ID(N'FK_Class_Course') IS NULL
BEGIN
	ALTER TABLE dbo.Class WITH CHECK ADD CONSTRAINT FK_Class_Course FOREIGN KEY(CourseId)
	REFERENCES dbo.Course (Id);
END;
