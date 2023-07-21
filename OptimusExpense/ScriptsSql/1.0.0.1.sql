 IF NOT EXISTS ( SELECT * FROM sys.columns AS c WHERE c.object_id=OBJECT_ID('Expense') AND c.name='CurrencyRate')
 BEGIN
	ALTER TABLE Expense 
	add CurrencyRate decimal(18,5)
 END
 GO
 
 IF NOT EXISTS( select 1 from UserAction where Name = 'Lista cheltuieli')
 BEGIN
	insert into UserAction values (27,'CHEL','Lista cheltuieli',23,'Meniu Lista Cheltuieli','/expense/ListExpense',-49,4,'pi pi-fw pi-credit-card',null)
	insert into UserActionXRoles values ('444041F9-E152-4BF9-8D3D-91FEF786683F',27)
	insert into UserActionXRoles values ('D0F09EF4-D007-4A69-A098-49BF8AF49E00',27)	
 END
 GO

 IF NOT EXISTS( select 1 from UserActionXRoles where RoleId = 'EF5A8559-E00B-4F7B-988A-4A361BD3884F' and UserActionId = 23)
 BEGIN
	insert into UserActionXRoles values ('EF5A8559-E00B-4F7B-988A-4A361BD3884F',23)
	insert into UserActionXRoles values ('EF5A8559-E00B-4F7B-988A-4A361BD3884F',25)
	insert into UserActionXRoles values ('EF5A8559-E00B-4F7B-988A-4A361BD3884F',26)
	insert into UserActionXRoles values ('EF5A8559-E00B-4F7B-988A-4A361BD3884F',27)
 END
 GO