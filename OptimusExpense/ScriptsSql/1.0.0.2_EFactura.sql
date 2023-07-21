 IF NOT EXISTS( select 1 from [AspNetRoles] where [Name] = 'UtilizatorEFactura')
 BEGIN
	insert into AspNetRoles values ('C7A93FE0-F25C-4120-9B00-8ECB1EC8B70E','UtilizatorEFactura','UtilizatorEFactura',null)
	insert into UserActionXRoles values ('C7A93FE0-F25C-4120-9B00-8ECB1EC8B70E',5) --Administrare
	insert into UserActionXRoles values ('C7A93FE0-F25C-4120-9B00-8ECB1EC8B70E',20) --Schimbare parola
	insert into UserAction values (28,'EFACT','E-Factura',28,'Meniu E-Factura',null,-49,null,'pi pi-fw pi-book',null)  --EFACTURA
	insert into UserAction values (29,'ISFAC','Import facturi',28,'Meniu Import Facturi','/efactura/ListImportInvoice',-49,1,'pi pi-fw pi-book',null)  --Import facturi
	insert into UserActionXRoles values ('C7A93FE0-F25C-4120-9B00-8ECB1EC8B70E',28) --EFACTURA
	insert into UserActionXRoles values ('C7A93FE0-F25C-4120-9B00-8ECB1EC8B70E',29) --Import facturi
 END
 GO



IF NOT EXISTS ( SELECT * FROM sys.columns AS c WHERE c.object_id=OBJECT_ID('EF_Raportare') AND c.name='NumarFactura')
 BEGIN
	ALTER TABLE EF_Raportare 
	add NumarFactura nvarchar(30)
 END
 GO

 IF NOT EXISTS ( SELECT * FROM sys.columns AS c WHERE c.object_id=OBJECT_ID('EF_Raportare') AND c.name='DataFactura')
 BEGIN
	ALTER TABLE EF_Raportare 
	add DataFactura datetime
 END
 GO


 IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND type in (N'U'))
begin


 CREATE TABLE [dbo].[Log](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](36) NOT NULL,
	[Action] [nvarchar](250) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[Value2] [nvarchar](max) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 

ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_Date]  DEFAULT (getdate()) FOR [Date]
end
GO


