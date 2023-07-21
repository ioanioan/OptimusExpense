--insert into UnitateSanitara
select distinct Spital ,Oras, getdate()
from slv_cluj m

--insert into Medic
select distinct 
rtrim(ltrim(LEFT(m.Medic, CHARINDEX(' ', m.Medic) - 1)))  as Nume
,rtrim(ltrim(REPLACE(SUBSTRING(m.Medic, CHARINDEX(' ', m.Medic), LEN(m.Medic)), '', ''))) as Prenume
,min(m.Sectie) as Sectie
,m.Oras as IdOras
,'' as Adresa
,'' as Program
,0 as IdUnitateSanitara
,min(m.Telefon) as Telefon
,null as NrPac
,null as DataNasterii
,null as Mostre
,min(m.Obs) as Observatii
,'D' as bb
,null as Email
,getdate() as DataServer
,0 as NrVizite
from slv_cluj m
group by 
rtrim(ltrim(LEFT(m.Medic, CHARINDEX(' ', m.Medic) - 1)))
,rtrim(ltrim(REPLACE(SUBSTRING(m.Medic, CHARINDEX(' ', m.Medic), LEN(m.Medic)), '', ''))),m.Oras
order by rtrim(ltrim(LEFT(m.Medic, CHARINDEX(' ', m.Medic) - 1))) 



select distinct m.Nume, m.Prenume,sm.Medic, u.Denumire,sm.Spital,m.idMedic,u.IdUnitateSanitara
--insert into MedicXUnitatiSanitare 
select distinct m.idMedic,u.IdUnitateSanitara
from Medic m
join slv_cluj sm on rtrim(ltrim(LEFT(sm.Medic, CHARINDEX(' ', sm.Medic) - 1))) = m.nume and rtrim(ltrim(REPLACE(SUBSTRING(sm.Medic, CHARINDEX(' ', sm.Medic), LEN(sm.Medic)), '', ''))) = m.Prenume
join UnitateSanitara u on  sm.Spital = u.Denumire and sm.Oras = u.IdOras