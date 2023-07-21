IF NOT EXISTS ( SELECT * FROM sys.columns AS c WHERE c.object_id=OBJECT_ID('Employee') AND c.name='SectionCode')
 BEGIN
	ALTER TABLE Employee 
	add SectionCode varchar(250)
 END
 GO

 IF NOT EXISTS ( SELECT * FROM sys.columns AS c WHERE c.object_id=OBJECT_ID('Employee') AND c.name='NormaAngajat')
 BEGIN
	ALTER TABLE Employee 
	add NormaAngajat int
 END
 GO

