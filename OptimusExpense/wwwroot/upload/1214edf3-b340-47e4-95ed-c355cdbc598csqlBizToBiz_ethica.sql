IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_SubstringLastInt]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fn_SubstringLastInt]
GO
CREATE function [dbo].[fn_SubstringLastInt](@valoare1 nvarchar(255))
RETURNS int
Begin
declare @result nvarchar(255);
 set  @result=(case when len(@valoare1)-CHARINDEX('_',reverse(@valoare1))>0 then  substring(@valoare1,2+len(@valoare1)-CHARINDEX('_',reverse(@valoare1)), LEN(@valoare1)) else '0' end);
 
 if( ISNUMERIC( @result)=1)
 begin 
 return @result
 end
return 0
end
GO



IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[btb_vwAgent]'))
DROP VIEW [dbo].[btb_vwAgent]
GO
CREATE view [dbo].[btb_vwAgent]
as
select
*
from(
	SELECT DISTINCT
		 P.IdAgentVanzari AS IdAgent
		,ISNULL(PERS.Nume, '') + ' ' + ISNULL(PERS.Prenume, '') AS Nume, PERS.EsteActiva as IsActiv, 21 as IdDistribuitor, cast(null as int) IdSuperior
	FROM Partener P  WITH (NOLOCK) 
		LEFT JOIN Persoana PERS WITH (NOLOCK) ON PERS.IdPersoana = P.IdAgentVanzari
	WHERE P.EsteClient = 1 AND /*P.EsteClientActiv = 1 and*/ P.IdAgentVanzari is not null 
	--and @all=1
	--ORDER BY PERS.Nume, PERS.Prenume ASC
	union
	select
    distinct p.IdPersoana as IdAgent,
	   ISNULL(p.Nume, '') + ' ' + ISNULL(p.Prenume, '') AS Nume, P.EsteActiva as IsActiv,  21 as IdDistribuitor, cast(null as int) IdSuperior
    from wms_BorderouRute q
    join Persoana p on p.IdPersoana=q.IdPersoana
	--where p.EsteActiva=1
	UNION
	SELECT DISTINCT
		 P.IdAgentAchizitii AS IdAgent
		,ISNULL(PERS.Nume, '') + ' ' + ISNULL(PERS.Prenume, '') AS Nume, PERS.EsteActiva as IsActiv,  21 as IdDistribuitor, cast(null as int) IdSuperior
	FROM Partener P  WITH (NOLOCK) 
		LEFT JOIN Persoana PERS WITH (NOLOCK) ON PERS.IdPersoana = P.IdAgentAchizitii
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 and P.IdAgentAchizitii is not null 
	--and @all=1 
	--and PERS.EsteActiva=1
	
	UNION
	SELECT DISTINCT
		 u.IdPersoana AS IdAgent
		,ISNULL(PERS.Nume, '') + ' ' + ISNULL(PERS.Prenume, '') AS Nume, PERS.EsteActiva as IsActiv,  21 as IdDistribuitor, cast(null as int) IdSuperior
	FROM Utilizator u  WITH (NOLOCK) 
		LEFT JOIN Persoana PERS WITH (NOLOCK) ON PERS.IdPersoana = u.IdPersoana
	WHERE /*u.EsteActiv = 1 and */ISNULL(u.EsteBlocat, 0) = 0)as r
	where IdAgent is not null
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetAgenti]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetAgenti]
GO
CREATE PROCEDURE [dbo].[btb_spGetAgenti]
@all bit=0
AS 

SET ANSI_WARNINGS OFF
SET NOCOUNT ON

BEGIN 

select
*
from btb_vwAgent
	
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetAgentiXClienti]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetAgentiXClienti]
GO
create procedure [dbo].[btb_spGetAgentiXClienti]
as
begin
select
IdAgentAchizitii as IdAgent, p.IdPartener as IdClient, pc.IdPunctLucru
from Partener p with(nolock)
join PunctLucru pc with(nolock) on p.IdPartener=pc.IdPartener 
where IdAgentAchizitii is not null and p.EsteClientActiv=1  and pc.EsteActiv=1

union
select
IdAgentVanzari as IdAgent, p.IdPartener as IdClient, pc.IdPunctLucru
from Partener p with(nolock)
join PunctLucru pc with(nolock) on p.IdPartener=pc.IdPartener 
where IdAgentVanzari is not null and p.EsteClientActiv=1 and pc.EsteActiv=1
end
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetDatePunctLucru]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetDatePunctLucru]
GO
CREATE PROCEDURE [dbo].[btb_spGetDatePunctLucru]
(
      @DeviceId varchar(36)
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON
BEGIN 
      
      
         SELECT DISTINCT
            cast(PL.IdPunctLucru as nvarchar(36)) as IdPunctLucru,
            SUM(ISNULL(F.ValoareFTVA,0)) OVER (PARTITION BY PL.IdPunctLucru) as BazaImpozitare
            ,SUM(ISNULL(F.RestDePlata, 0)) OVER (PARTITION BY PL.IdPunctLucru) AS Sold
            ,SUM(CASE WHEN GETDATE()> DATEADD(DAY, F.TermenPlata, F.DataDoc) THEN ISNULL(F.RestDePlata, 0) ELSE 0 END) OVER (PARTITION BY PL.IdPunctLucru) AS SoldDepasit
            ,ISNULL(P.LimitaCredit, 0) AS LimitaCredit,
            P.TermenPlata
      FROM Partener P  WITH (NOLOCK) 
            JOIN PunctLucru PL WITH (NOLOCK) ON PL.IdPartener = P.IdPartener AND PL.EsteActiv = 1
            --JOIN btb_SoldClient BSC (NOLOCK) ON BSC.IdPunctLucru = PL.IdPunctLucru        
			left join(select F.IdPartener,F.ValoareFTVA,F.RestDePlata, F.TermenPlata, F.DataDoc from 
              Factura F WITH (NOLOCK) 
              JOIN Document D WITH (NOLOCK) ON F.IdDocument = D.IdDocument AND D.EsteAnulat=0
			  where F.IdTipFactura = 89) as F on F.IdPartener=P.IdPartener
   
            
      WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1-- AND (@DeviceId in('all','setrio') or #temp.IdClient IS NOT NULL)
      --AND 1=2
    --  ORDER BY PL.IdPunctLucru ASC
                 
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetDistribuitor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetDistribuitor]
GO
CREATE PROCEDURE [dbo].[btb_spGetDistribuitor]
(
	@DeviceId varchar(36) = null
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

BEGIN 
	select
	distinct
	l.IdLocatie as IdDistribuitor, p.Nume NumeDistribuitor
	,(  ISNULL(A.Strada, '') + ' ' + ISNULL(A.Numar, '') +
			CASE WHEN ISNULL(A.Bloc, '') = '' THEN '' ELSE ', bl. ' END +
			ISNULL(A.Bloc, '') + 
			CASE WHEN ISNULL(A.Scara, '') = '' THEN '' ELSE ', sc. ' END +
			ISNULL(A.Scara, '') + 
			CASE WHEN ISNULL(A.Ap, '') = '' THEN '' ELSE ', ap. ' END +
			ISNULL(A.Ap, '') + 
			CASE WHEN ISNULL(O.Nume, '') = '' THEN '' ELSE ', ' END +
			ISNULL(P.Nume, '') + 
			CASE WHEN ISNULL(J.Nume, '') = '' THEN '' ELSE ', judet ' END +
			ISNULL(J.Nume, '')  
		  ) AS Adresa
	from Locatie l WITH (NOLOCK)
	join Partener p WITH (NOLOCK) on p.IdPartener=l.IdPartener	 
	 
	JOIN Adresa A WITH (NOLOCK) ON A.IdAdresa = p.IdAdresa
	JOIN Oras O WITH (NOLOCK) ON O.IdOras = A.IdOras
	JOIN Judet J WITH (NOLOCK) ON J.IdJudet = O.IdJudet  
	where @DeviceId='all'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetCategorieClient]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetCategorieClient]
GO
create procedure [dbo].[btb_spGetCategorieClient]
as
begin
select
dd.IdDictionarDetaliu as IdCategorie,dd.Nume,dd.Cod 
from  DictionarDetaliu dd with(nolock)
join Dictionar d with(nolock) on dd.IdDictionar = d.IdDictionar
where  d.Cod ='CategPart'
order by d.Nume
end
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetCategoriiClienti]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetCategoriiClienti]
GO
CREATE PROCEDURE [dbo].[btb_spGetCategoriiClienti]
(
	@DeviceId varchar(36) = null
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

BEGIN 
	---select IdClient into #temp from dbo.fn_GetPuncteLucruIdDevices(@DeviceID)
	
	SELECT 
		 P.IdPartener AS IdClient
		,isnull(P.IdCategorie,-16068) AS IdCategorie
	FROM Partener P  WITH (NOLOCK) 
--	LEFT JOIN #temp on #temp.IdClient=P.IdPartener AND @DeviceId IS NOT NULL
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 ---AND    (@DeviceId in('all','setrio') or #temp.IdClient IS NOT NULL)
	ORDER BY P.IdPartener ASC

	--DROP TABLE #temp
END
GO




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetDistribuitorXPuncteLucru]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetDistribuitorXPuncteLucru]
GO
create PROCEDURE [dbo].[btb_spGetDistribuitorXPuncteLucru]
(
	@DeviceId varchar(36) = null
)
AS 

SET ANSI_WARNINGS OFF
SET NOCOUNT ON

BEGIN 
	select
	distinct
	0 IdDistribuitor,pc.IdPunctLucru as IdPunctLucruDistribuitor,
	case when dd.Cod like '%Drogherie%' then 9 else 0 end NivelAutorizatie
	
	--join BizToBiz.dbo.AgentXDistribuitori axd WITH (NOLOCK) on l.IdLocatie=axd.IdDistribuitor
	from Partener p WITH (NOLOCK) --on p.IdAgentVanzari=axd.IdAgent	 
	join PunctLucru pc WITH (NOLOCK) on pc.IdPartener=p.IdPartener
	left join DictionarDetaliu dd on dd.IdDictionarDetaliu=pc.IdTipPunctLucru
	where @DeviceId in('all','setrio')
END
GO
 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetFacturi]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetFacturi]
GO
CREATE PROCEDURE [dbo].[btb_spGetFacturi]
(
   @year int,
   @month int,
   @lastId int=0
)
	
 AS
 begin
	SELECT 
	       
		 cast(D.IdDocument as bigint) AS  IdFactura
		,P.IdPartener AS IdClient
		,P.Nume as Client
	    ,PL.PunctLucru as PunctLucru
		,cast(isnull(fe.IdPunctLucru, PL.IdPunctLucru) as nvarchar(36)) AS IdPunctLucru
		,cast(isnull(fe.IdPunctLucru, PL.IdPunctLucru) as nvarchar(36)) AS CodClient
		,F.NumarDoc AS NumarFactura
		,cast(F.DataDoc as date) AS DataFactura
		,ISNULL(F.RestDePlata, 0) AS RestDePlata
		,DATEADD(DAY, F.TermenPlata, F.DataDoc) DataScadenta
		,ISNULL(F.ValoareDoc, 0) AS ValoarePlata
		,ISNULL(F.ValoareFTVA, 0) AS BazaImpozitare
		,F.TermenPlata
		
		
		,ISNULL(F.ValoareTVA, 0) AS TVA
		, @year as An,
			@month as Luna
	FROM Partener P  WITH (NOLOCK) 
	    
		JOIN
		(select PL.IdPunctLucru, PL.IdPartener, row_number() over(partition by PL.IdPartener order by PL.IdPartener) as Nr, PL.Denumire as PunctLucru
		 from PunctLucru PL WITH (NOLOCK)
		 where  PL.EsteActiv = 1) as PL ON PL.IdPartener = P.IdPartener  and PL.Nr=1
		JOIN Factura F WITH (NOLOCK) ON F.IdPartener = P.IdPartener
		LEFT JOIN FacturaIesire fe WITH (NOLOCK) ON fe.IdDocument = F.IdDocument
		JOIN Document D WITH (NOLOCK) ON F.IdDocument = D.IdDocument
		 
		/*LEFT JOIN 
		(
			select	dd.IdDocument, ddd.Nume AS Gestiune
			,row_number() over(partition by  dd.IdDocument order by ddd.Nume) As Poz			
			from DocumentDetaliu dd
			JOIN Articol a ON dd.IdArticol = a.IdArticol
			JOIN DictionarDetaliu ddd ON a.IdCategorie = ddd.IdDictionarDetaliu
		) AS s ON s.Poz = 1 AND s.IdDocument = D.IdDocument*/
		
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1   
	     AND D.EsteAnulat = 0 AND F.IdTipFactura = 89
		 and F.IdDocument>@lastId
	 
	 
 end
 GO


 IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetFacturiComanda]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetFacturiComanda]
GO
CREATE procedure [dbo].[btb_spGetFacturiComanda]
( @IdOrder int=113
)
as
begin
select distinct isnull(fac.IdDocument,0) as InvoiceId, isnull(fac.Serie,'') as SerialNo, isnull(fac.NumarDoc,'') as InvoiceNumber, isnull(fac.TermenPlata,oc.TermenPlata) as Term, isnull(fac.DataDoc,getdate()) as Date, isnull(DATEADD(day,isnull(fac.TermenPlata,oc.TermenPlata), cast(isnull(fac.DataDoc,getdate()) as date)),getdate()) as DueDate, isnull(fac.ValoareFTVA,0) as ValueWithoutVAT,isnull( fac.ValoareTVA,0) as ValueVAT,isnull( fac.ValoareDoc,0) as TotalValue, isnull(fac.RestDePlata,0) as Sold,
 oc.Numar as NumarOrdinCulegere,
 cast(case when d.EsteAnulat=1 Or df.EsteAnulat=1  then 1 else 0 end as bit)  Anulata

from SDOrderHead sh with(nolock)
join SDOrderDetail  sd with(nolock) on sd.IdOrder=sh.IdOrder
join PrecomandaDetaliu pd with(nolock) on pd.IdPrecomandaDetaliu=sd.IdElemComandaBC
join PrecomandaDetaliuXDocumentDetaliu pdd with(nolock) on pdd.IdPrecomandaDetaliu=pd.IdPrecomandaDetaliu
join DocumentDetaliu dd with(nolock) on dd.IdDocumentDetaliu=pdd.IdDocumentDetaliu
join Document d with(nolock) on d.IdDocument=dd.IdDocument
join DictionarDetaliu did with(nolock) on did.IdDictionarDetaliu=d.IdTipDocument
join OrdinCulegereDetaliu ocd with(nolock) on ocd.IdDocumentDetaliu=dd.IdDocumentDetaliu 
join OrdinCulegere oc with(nolock) on oc.IdDocument=ocd.IdDocument
left join OrdinCulegereXDocumentIesire ocdi with(nolock) on ocdi.IdOrdinCulegere=ocd.IdDocument
--join DocumentDetaliu f on f.IdDocument=ocdi.IdDocument and dd.BBD=f.BBD and dd.IdArticol=f.IdArticol and f.Lot=dd.Lot and f.IdTipStoc=dd.IdTipStoc
--join FacturaDetaliu fd on fd.IdDocumentDetaliu=f.IdDocumentDetaliu
left join Factura fac with(nolock) on fac.IdDocument=ocdi.IdDocument
left join Document df with(nolock) on df.IdDocument=fac.IdDocument
where sh.IdOrder=@IdOrder

end
GO

 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetFacturiElem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetFacturiElem]
GO 
CREATE Procedure [dbo].[btb_spGetFacturiElem](
 @year int,
 @month int,
    @lastId int=0
)
AS
BEGIN
		SELECT  D.IdDocument as DocumentId, I.NumarDoc as NumarDoc 
	     into #tmp
		from Factura I WITH (NOLOCK)-- ON F.IdDocument = D.IdDocument
		join Partener p with(nolock) on p.IdPartener=I.IdPartener
		JOIN Document D WITH (NOLOCK) ON D.IdDocument = I.IdDocument AND D.IdLocatie = I.IdLocatie
		--JOIN DocumentDetail DD WITH (NOLOCK) ON DD.DocumentId=D.DocumentId
		--JOIN Item It WITH (NOLOCK) ON It.ItemId=DD.ItemId
		--JOIN InvoiceDetail ID WITH (NOLOCK) ON ID.InvoiceDetailId=DD.DocumentDetailId AND ID.SiteId=DD.SiteId
		--LEFT JOIN ItemProperty IPD WITH (NOLOCK) ON It.ItemId = IPD.ItemId AND IPD.PropertyId = -1108	
		--left join PriceListDetail pld2 with(nolock) on pld2.PriceListId = 35 
			--	and pld2.ItemId = It.ItemId 
			--	and pld2.MeasuringUnitId = It.MeasuringUnitId
				--and pld2.ParentItemId is null 
				--and pld2.ValidTo is null
		  
		WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1   
	     AND D.EsteAnulat = 0 AND I.IdTipFactura = 89
		 and I.IdDocument>@lastId

	
	  SELECT   cast(D.DocumentId as bigint) AS IdFactura, cast(DD.IdArticol as nvarchar) AS IdProdus,
		   cast(  ID.ProcDisc as decimal(12,2)) AS Discount,
			CAST(isnull(DD.PretUnitar,0) AS decimal(12,2)) AS Pret, DD.ProcTVA AS ProcentTVA,-- ISNULL(PL_PretAmMax.Price, 0) AS PretAmanunt,
			It.Nume AS Produs,   
		 
			sum(cast(DD.CantI as int)) as Cantitate, DD.BBD as Expira,DD.Lot as Lot--, pld2.Price as PretAmanunt
	        ,D.NumarDoc as NumarFactura,
			It.Nume as Produs,
			It.CodCAs as CodCAS,
			DD.BBD
			--ID.LotNumber as Lot

		FROM #tmp D
		 
		JOIN DocumentDetaliu  DD WITH (NOLOCK) ON DD.IdDocument=D.DocumentId
		JOIN Articol It WITH (NOLOCK) ON It.IdArticol=DD.IdArticol
		JOIN FacturaDetaliu ID WITH (NOLOCK) ON ID.IdDocumentDetaliu=DD.IdDocumentDetaliu AND ID.IdLocatie=DD.IdLocatie
		--left join ItemProperty ip on ip.PropertyId=-1128 and It.ItemId=ip.ItemId
		group by 
		cast(D.DocumentId as bigint) , cast(DD.IdArticol as nvarchar)  ,
		   cast(  ID.ProcDisc as decimal(12,2))  ,
			CAST(isnull(DD.PretUnitar,0) AS decimal(12,2))  , DD.ProcTVA  ,-- ISNULL(PL_PretAmMax.Price, 0) AS PretAmanunt,
			It.Nume  ,   
		 
			  DD.BBD  ,DD.Lot  --, pld2.Price as PretAmanunt
	        ,D.NumarDoc  ,
			It.Nume  ,
			It.CodCAs ,
			DD.BBD

			   drop table #tmp
		 
END		
GO


 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetFacturiSold]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetFacturiSold]
GO 
CREATE PROCEDURE [dbo].[btb_spGetFacturiSold]
(
	@DeviceId varchar(36) = null
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

	DECLARE @IdDistribuitor INT
	
	SET @IdDistribuitor = ( SELECT CAST (l.IdLocatie AS INT) AS IdDistribuitor
							FROM InitInfo II WITH (NOLOCK) 
							join Locatie l on l.IdLocatie= II.[Value]
							WHERE ii.[Object] = 'HostIdLocatie'  )

BEGIN 
	--select IdClient into #temp from dbo.fn_GetPuncteLucruIdDevices(@DeviceID)
	
/*IF ((SELECT Nume FROM Locatie WHERE IdLocatie = 16) NOT LIKE '%Farmacordis%') 
	SELECT 
	     cast(ROW_NUMBER()  OVER(ORDER BY BSC.DataFactura DESC)as int) as Id,
		 F.IdDocument AS IdFacturaSold
		,P.IdPartener AS IdClient
		,P.Cod AS CodClient
		,PL.IdPunctLucru AS IdPunctLucru
		,BSC.NumarFactura
		,BSC.DataFactura
		,F.TermenPlata
		,BSC.DataScadenta
		,ISNULL(BSC.BazaImpozitare, 0) AS BazaImpozitare
		,BSC.TVA
		,ISNULL(BSC.ValoareDePlata, 0) AS ValoarePlata
		,BSC.RestDePlata, @IdDistribuitor as IdDistribuitor
	FROM Partener P  WITH (NOLOCK) 
		JOIN PunctLucru PL WITH (NOLOCK) ON PL.IdPartener = P.IdPartener AND PL.EsteActiv = 1
		JOIN Factura F WITH (NOLOCK) ON F.IdPartener = P.IdPartener
		JOIN btb_SoldClient BSC WITH (NOLOCK) ON BSC.IdPunctLucru = PL.IdPunctLucru AND F.NumarDoc = BSC.NumarFactura
		LEFT JOIN #temp on #temp.IdClient=P.IdPartener --AND @DeviceId IS NOT NULL
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 AND ((@DeviceId <> 'all' AND #temp.IdClient IS NOT NULL)OR (@DeviceId = 'all'))
	ORDER BY BSC.DataFactura ASC
ELSE*/
	SELECT 
	      cast(ROW_NUMBER() OVER(ORDER BY F.DataDoc DESC)as int) as Id,
		 F.IdDocument AS IdFacturaSold
		,P.IdPartener AS IdClient
		,P.Cod AS CodClient
		,cast(isnull(fe.IdPunctLucru, PL.IdPunctLucru) as nvarchar(36)) AS IdPunctLucru
		,F.NumarDoc AS NumarFactura
		,F.DataDoc AS DataFactura
		,F.TermenPlata
		,DATEADD(DAY, F.TermenPlata, F.DataDoc) DataScadenta
		,ISNULL(F.ValoareFTVA, 0) AS BazaImpozitare
		,ISNULL(F.ValoareTVA, 0) AS TVA
		,ISNULL(F.ValoareDoc, 0) AS ValoarePlata
		,ISNULL(F.RestDePlata, 0) AS RestDePlata,
		@IdDistribuitor as IdDistribuitor,

		case when s.Gestiune in ('TATTOOMED','Ochelari de soare','Ochelari de citit') then '#00ff7f' else   '#ffffff' end as Gestiune
	FROM Partener P  WITH (NOLOCK) 
	    
		JOIN
		(select PL.IdPunctLucru, PL.IdPartener, row_number() over(partition by PL.IdPartener order by PL.IdPartener) as Nr
		 from PunctLucru PL WITH (NOLOCK)
		 where  PL.EsteActiv = 1) as PL ON PL.IdPartener = P.IdPartener  and PL.Nr=1
		JOIN Factura F WITH (NOLOCK) ON F.IdPartener = P.IdPartener
		LEFT JOIN FacturaIesire fe WITH (NOLOCK) ON fe.IdDocument = F.IdDocument
		JOIN Document D WITH (NOLOCK) ON F.IdDocument = D.IdDocument
		--LEFT JOIN #temp on #temp.IdClient=P.IdPartener --AND @DeviceId IS NOT NULL		
		LEFT JOIN 
		(
			select	dd.IdDocument, ddd.Nume AS Gestiune
			,row_number() over(partition by  dd.IdDocument order by ddd.Nume) As Poz			
			from DocumentDetaliu dd
			JOIN Articol a ON dd.IdArticol = a.IdArticol
			JOIN DictionarDetaliu ddd ON a.IdCategorie = ddd.IdDictionarDetaliu
		) AS s ON s.Poz = 1 AND s.IdDocument = D.IdDocument
		
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 --  AND (@DeviceId in('all','setrio') or #temp.IdClient IS NOT NULL)
	    AND F.RestDePlata > 0 AND D.EsteAnulat = 0 AND F.IdTipFactura = 89
		--and f.ValoareDePlata-f.RestDePlata<=5000
	ORDER BY F.DataDoc ASC
	
	--DROP TABLE #temp
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetFirme]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetFirme]
GO 
CREATE PROCEDURE [dbo].[btb_spGetFirme]
(
	@DeviceId varchar(36)
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

BEGIN 
	
	--select IdClient into #temp from dbo.fn_GetPuncteLucruIdDevices(@DeviceID)
	
	SELECT 
		 P.IdPartener AS IdClient
		,P.Nume
		,ISNULL(P.CodFiscal,'') AS CodFiscal
		,(  ISNULL(A.Strada, '') + ' ' + ISNULL(A.Numar, '') +
			CASE WHEN ISNULL(A.Bloc, '') = '' THEN '' ELSE ', bl. ' END +
			ISNULL(A.Bloc, '') + 
			CASE WHEN ISNULL(A.Scara, '') = '' THEN '' ELSE ', sc. ' END +
			ISNULL(A.Scara, '') + 
			CASE WHEN ISNULL(A.Ap, '') = '' THEN '' ELSE ', ap. ' END +
			ISNULL(A.Ap, '') + 
			CASE WHEN ISNULL(O.Nume, '') = '' THEN '' ELSE ', ' END +
			ISNULL(P.Nume, '') + 
			CASE WHEN ISNULL(J.Nume, '') = '' THEN '' ELSE ', judet ' END +
			ISNULL(J.Nume, '')  
		  ) AS Adresa
		,ISNULL(A.TelefonMobil, ISNULL(A.TelefonFix,'')) AS Telefon
		,ISNULL(A.Email,'') AS Email
		-- pt campul blocat nu exista inca implementare
		-- se pune implicit neblocat
		,CAST(0 AS BIT) AS Blocat
		,ISNULL(P.Cod, '') AS CodClient
		,ISNULL(J.Nume,'') AS Judet
		,ISNULL(O.Nume,'') AS Localitate
		,ISNULL(P.NrRegCom, '') AS NrRegComert
		--daca nu sunt asociati operatorii telesales se va afisa
		--numele si telefonul agentului de vanzare asociat clientului respectiv
		,ISNULL(AG.TelefonMobil, ISNULL(AG.TelefonFix, '')) AS TelefonTelesales
		,ISNULL(PERS.Nume, '') + ' ' + ISNULL(PERS.Prenume,'') AS Telesales
	FROM Partener P WITH (NOLOCK)
		LEFT JOIN Adresa A WITH (NOLOCK) ON A.IdAdresa = P.IdAdresa
		LEFT JOIN Oras O WITH (NOLOCK) ON O.IdOras = A.IdOras
		LEFT JOIN Judet J WITH (NOLOCK) ON J.IdJudet = O.IdJudet 
		LEFT JOIN Persoana PERS WITH (NOLOCK) ON PERS.IdPersoana = P.IdAgentVanzari
		LEFT JOIN Adresa AG WITH (NOLOCK) ON AG.IdAdresa = PERS.IdAdresa
		--LEFT JOIN #temp on #temp.IdClient=P.IdPartener --AND @DeviceId <> 'all'
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 --AND (@DeviceId in('all','setrio')-- or #temp.IdClient IS NOT NULL)

	--DROP TABLE #temp

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaSpeciala]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaSpeciala]
GO  
CREATE PROCEDURE [dbo].[btb_spGetOfertaSpeciala]
(
@tip int=0
)
AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale
--======================================================================================
BEGIN
	
	WITH Oferte AS (
	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, so.SpecialOfferTypeId AS TipOferta, dsog.DictionaryItemCode AS TipGrupOferta
		, so.StartDate AS DataInceput, so.EndDate AS DataSfarsit, 
		SUBSTRING(so.SpOfferCode, 0, 21) AS Cod,
		 so.SpOfferName AS Nume,
		so.DaysToDueDate AS TermenPlata,
		 (CASE ISNULL(so.AllowRNToDiscount, 0) WHEN 1 THEN 1 ELSE 0 END) AS AllowRNToDiscount
	FROM SpecialOffer so (NOLOCK)
	JOIN DictionarySpecialOfferGroup dsog WITH (NOLOCK) ON dsog.SpecialOfferGroupId=so.SpecialOfferGroupId
	WHERE CAST(GETDATE() as varchar(12)) BETWEEN so.StartDate AND so.EndDate
		AND so.isValid=1
		AND so.SpecialOfferTypeId IN (1, 2, 1003, 1004, 1005, 1006, 1007, 1008,1011)
		
	)
	SELECT DISTINCT IdOfertaSpeciala, TipOferta, TipGrupOferta, DataInceput, DataSfarsit, Cod,
		(CASE
			WHEN LEFT(Nume, 8)='| Oferta' THEN SUBSTRING(Nume, 3, LEN(Nume))
			WHEN LEFT(Nume, 4)='| C2' THEN SUBSTRING(Nume, 3, LEN(Nume))
			WHEN LEFT(Nume, 12)='D+; | Oferta' THEN SUBSTRING(Nume, 1, 3) + SUBSTRING(Nume, 6, LEN(Nume))
			WHEN LEFT(Nume, 12)='D-; | Oferta' THEN SUBSTRING(Nume, 1, 3) + SUBSTRING(Nume, 6, LEN(Nume))
			WHEN LEFT(Nume, 12)='D+; |Oferta' THEN SUBSTRING(Nume, 1, 3) + ' ' + SUBSTRING(Nume, 6, LEN(Nume))
			WHEN LEFT(Nume, 12)='D-; |Oferta' THEN SUBSTRING(Nume, 1, 3) + ' ' + SUBSTRING(Nume, 6, LEN(Nume))
			ELSE Nume
		END) AS Nume,
		ISNULL(TermenPlata, 180) AS TermenPlata, 'SFA' Aplicatie, AllowRNToDiscount
	FROM Oferte
	ORDER BY IdOfertaSpeciala

END
GO

 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaSpecialaIncludere]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaSpecialaIncludere]
GO  
CREATE PROCEDURE [dbo].[btb_spGetOfertaSpecialaIncludere]

AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale includere
--======================================================================================
BEGIN
	
	CREATE TABLE #tmpSO
		(
		SpecialOfferId INT,
		SpecialOfferTypeId INT
		)

	INSERT INTO #tmpSO (SpecialOfferId, SpecialOfferTypeId)
	SELECT SpecialOfferId, SpecialOfferTypeId
	FROM (
		SELECT so.SpecialOfferId, so.SpecialOfferTypeId
		
		FROM SpecialOffer so WITH (NOLOCK)
		WHERE CAST(GETDATE() as varchar(12)) BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
			
	) AS X

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, soai.xSpecialOfferId AS IdOfertaSpecialaInclusa
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferAutomaticInclusion soai WITH (NOLOCK) ON so.SpecialOfferId = soai.SpecialOfferId	
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId --detaliile de oferta
	WHERE ISNULL(sod.ItemId, 0)<>0

	
	DROP TABLE #tmpSO

END
GO

 
 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaSpecialaInterval]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaSpecialaInterval]
GO  
CREATE PROCEDURE [dbo].[btb_spGetOfertaSpecialaInterval]
AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale interval
--======================================================================================
BEGIN

	CREATE TABLE #tmpSO
		(
		SpecialOfferId INT,
		SpecialOfferTypeId INT
		)
	
	INSERT INTO #tmpSO (SpecialOfferId, SpecialOfferTypeId)
	SELECT SpecialOfferId, SpecialOfferTypeId
	FROM (
		SELECT so.SpecialOfferId, so.SpecialOfferTypeId
		FROM SpecialOffer so WITH (NOLOCK)
		WHERE CAST(GETDATE() as varchar(12)) BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
		
	) AS X

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, s.SOConditionId AS IdOfertaSpecialaInterval, CAST(RANK() OVER (PARTITION BY s.SOConditionId ORDER BY s.MinValue) as varchar(10)) AS Denumire,
		s.MinValue AS IntervalMin, s.MaxValue AS IntervalMax, s.OfferPercent AS ProcDisc,
		s.ProcDiscDistribuitor ,  s.ProcDiscFurnizor  
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SOCondition s WITH (NOLOCK) ON s.SpecialOfferId = so.SpecialOfferId --pragurile valorice
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId --detaliile de oferta
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1) -- OFERTA VALORICA (1) pe produs suma comenzi

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, s.SOConditionId AS IdOfertaSpecialaInterval, CAST(RANK() OVER (PARTITION BY s.SOConditionId ORDER BY s.MinValue) as varchar(10)) AS Denumire,
		s.MinValue AS IntervalMin, s.MaxValue AS IntervalMax, s.OfferPercent AS ProcDisc,
	    s.ProcDiscDistribuitor ,  s.ProcDiscFurnizor 
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SOCondition s WITH (NOLOCK) ON s.SpecialOfferId = so.SpecialOfferId --pragurile valorice
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId --detaliile de oferta
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE  ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1003) -- OFERTA COMBINATIVA PE PRAGURI (1003) pe comenzi suma comenzi

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sc.SODGroupConditionId AS IdOfertaSpecialaInterval, sc.SODGroupName AS Denumire,  
		sc.MinShareQtty AS IntervalMin, sc.MaxShareQtty AS IntervalMax, sc.OfferPercent AS ProcDisc,
			sc.ProcDiscDistribuitor ,  sc.ProcDiscFurnizor 
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId 
	JOIN SODGroupCondition sc WITH (NOLOCK) ON sc.SpecialOfferId = so.SpecialOfferId AND sod.SODGroupConditionId = sc.SODGroupConditionId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1004) -- OFERTA PACHET CANTITATIV (1004) pe pachet

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sc.SODGroupConditionId AS IdOfertaSpecialaInterval, sc.SODGroupName AS Denumire,  
		sc.MinShareValue AS IntervalMin, sc.MaxShareValue AS IntervalMax, sc.OfferPercent AS ProcDisc,
			sc.ProcDiscDistribuitor ,  sc.ProcDiscFurnizor 
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId 
	JOIN SODGroupCondition sc WITH (NOLOCK) ON sc.SpecialOfferId = so.SpecialOfferId AND sod.SODGroupConditionId = sc.SODGroupConditionId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1005) -- OFERTA PACHET VALORICA (1005) pe pachet

	DROP TABLE #tmpSO

END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaSpecialaProdus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaSpecialaProdus]
GO 
USE [BizDepozit]
GO
/****** Object:  StoredProcedure [dbo].[btb_spGetOfertaSpecialaProdus]    Script Date: 16.05.2021 10:31:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATe PROCEDURE [dbo].[btb_spGetOfertaSpecialaProdus]

AS  
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale detaliu
--======================================================================================
BEGIN
	CREATE TABLE #tmpSO
		(
		SpecialOfferId INT,
		SpecialOfferTypeId INT,
		TipGrupOferta VARCHAR(255)
		)
	

	INSERT INTO #tmpSO (SpecialOfferId, SpecialOfferTypeId, TipGrupOferta)
	SELECT SpecialOfferId, SpecialOfferTypeId, TipGrupOferta
	FROM (
		SELECT so.SpecialOfferId, so.SpecialOfferTypeId, dsog.DictionaryItemCode AS TipGrupOferta
		FROM SpecialOffer so WITH (NOLOCK)
		JOIN DictionarySpecialOfferGroup dsog WITH (NOLOCK) ON dsog.SpecialOfferGroupId=so.SpecialOfferGroupId
		WHERE CAST(GETDATE() as varchar(12)) BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
			
	) as X


	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, i.IdArticol  AS IdProdus, 
		ISNULL(sod.MinQtty, 0) AS IntervalMin, ISNULL(sod.MaxQtty, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	--LEFT JOIN FarmexpertERP.dbo.SOCondition s WITH (NOLOCK) ON s.SpecialOfferId = so.SpecialOfferId --pragurile valorice
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId --detaliile de oferta
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE  ISNULL(sod.ItemId, 0)<>0 
		AND so.SpecialOfferTypeId IN (1) -- OFERTA VALORICA (1) pe produs suma comenzi

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus,  i.IdArticol  AS IdProdus, 
		ISNULL(sod.MinQtty, 0) AS IntervalMin, ISNULL(sod.MaxQtty, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE  ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (2,1011) -- OFERTA PRODUSE (2) pe produs

	UNION ALL
	--s.SOConditionId
	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus,  i.IdArticol  AS IdProdus, 
		ISNULL(sod.MinAmount, 0) AS IntervalMin, ISNULL(sod.MaxAmount, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	--JOIN FarmexpertERP.dbo.SOCondition s WITH (NOLOCK) ON s.SpecialOfferId = so.SpecialOfferId --pragurile valorice
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId --detaliile de oferta
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1003) -- OFERTA COMBINATIVA PE PRAGURI (1003) pe produs suma comenzi

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sc.SODGroupConditionId AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus,  i.IdArticol  AS IdProdus,  
		ISNULL(sod.MinQtty, 0) AS IntervalMin, ISNULL(sod.MaxQtty, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId 
	JOIN SODGroupCondition sc WITH (NOLOCK) ON sc.SpecialOfferId = so.SpecialOfferId AND sod.SODGroupConditionId = sc.SODGroupConditionId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1004) -- OFERTA PACHET CANTITATIV (1004) pe pachet

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sc.SODGroupConditionId AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, i.IdArticol  AS IdProdus,  
		ISNULL(sod.MinAmount, 0) AS IntervalMin, ISNULL(sod.MaxAmount, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId 
	JOIN SODGroupCondition sc WITH (NOLOCK) ON sc.SpecialOfferId = so.SpecialOfferId AND sod.SODGroupConditionId = sc.SODGroupConditionId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1005) -- OFERTA PACHET VALORICA (1005) pe pachet

	UNION ALL
	
	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, i.IdArticol  AS IdProdus,  
		ISNULL(sod.MinQtty, 0) AS IntervalMin, ISNULL(sod.MaxQtty, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId            
	JOIN SODConditionValue sv WITH (NOLOCK) ON sv.SpecialOfferDetailId = sod.SpecialOfferDetailId -- produsele oferite ca gratuitate 
	JOIN Articol i2 WITH (NOLOCK) ON i2.IdArticol = sv.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1006) -- OFERTA PRODUSE CU GRATUITATE (1006) pe produs

	UNION ALL

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, i.IdArticol AS IdProdus, 
		ISNULL(sod.MinAmount, 0) AS IntervalMin, ISNULL(sod.MaxAmount, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1007) -- OFERTA VALORICA PE PRODUSE (1007) pe produs

	UNION ALL
	
	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, 0 AS IdOfertaSpecialaInterval, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus,i.IdArticol  AS IdProdus,  
		ISNULL(sod.MinAmount, 0) AS IntervalMin, ISNULL(sod.MaxAmount, 0) AS IntervalMax, ISNULL(sod.OfferPercent, 0)+ISNULL(sod.DiscountPerc, 0) AS ProcDisc,
		sod.ProcDiscDistribuitor, sod.ProcDiscFurnizor
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId            
	JOIN SODConditionValue sv WITH (NOLOCK) ON sv.SpecialOfferDetailId = sod.SpecialOfferDetailId -- produsele oferite ca gratuitate 
	JOIN Articol i2 WITH (NOLOCK) ON i2.IdArticol = sv.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1008) -- OFERTA VALORICA PE PRODUSE CU GRATUITATE (1008) pe produs

	DROP TABLE #tmpSO

END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaSpecialaRabat]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaSpecialaRabat]
GO 
Create PROCEDURE [dbo].[btb_spGetOfertaSpecialaRabat]
AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale rabat
--======================================================================================
BEGIN

	
	CREATE TABLE #tmpSO
		(
		SpecialOfferId INT,
		SpecialOfferTypeId INT
		)
	
	INSERT INTO #tmpSO (SpecialOfferId, SpecialOfferTypeId)
	SELECT SpecialOfferId, SpecialOfferTypeId
	FROM (
		SELECT so.SpecialOfferId, so.SpecialOfferTypeId
		FROM SpecialOffer so WITH (NOLOCK)
		WHERE CAST(GETDATE() as varchar(12)) BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
			
	) AS X

	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, 
		0 AS IdOfertaSpecialaInterval, sv.SODConditionValueId AS IdOfertaSpecialaRabat, 
		i2.IdArticol AS IdProdus, CAST(sv.OfferQtty AS INT) AS Cant, sv.OfferPercent AS ProcDisc,
		 sv.ProcDiscDistribuitor,  sv.ProcDiscFurnizor  
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId            
	JOIN SODConditionValue sv WITH (NOLOCK) ON sv.SpecialOfferDetailId = sod.SpecialOfferDetailId -- produsele oferite ca gratuitate 
	JOIN Articol i2 WITH (NOLOCK) ON i2.IdArticol = sv.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1006) -- OFERTA PRODUSE CU GRATUITATE (1006) pe produs

	UNION ALL
	
	SELECT DISTINCT so.SpecialOfferId AS IdOfertaSpeciala, sod.SpecialOfferDetailId AS IdOfertaSpecialaProdus, 
		0 AS IdOfertaSpecialaInterval, sv.SODConditionValueId AS IdOfertaSpecialaRabat, 
		i2.IdArticol AS IdProdus, CAST(sv.OfferQtty AS INT) AS Cant, sv.OfferPercent AS ProcDisc,
		 sv.ProcDiscDistribuitor ,  sv.ProcDiscFurnizor  
	FROM #tmpSO so WITH (NOLOCK)
	JOIN SpecialOfferDetail sod WITH (NOLOCK) ON sod.SpecialOfferId = so.SpecialOfferId  
	JOIN Articol i WITH (NOLOCK) ON i.IdArticol = sod.ItemId            
	JOIN SODConditionValue sv WITH (NOLOCK) ON sv.SpecialOfferDetailId = sod.SpecialOfferDetailId -- produsele oferite ca gratuitate 
	JOIN Articol i2 WITH (NOLOCK) ON i2.IdArticol = sv.ItemId
	WHERE ISNULL(sod.ItemId, 0)<>0
		AND so.SpecialOfferTypeId IN (1008) -- OFERTA VALORICA PE PRODUSE CU GRATUITATE (1008) pe produs

	DROP TABLE #tmpSO

END
GO
 


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaXCategorieClient]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaXCategorieClient]
GO 
--======================================================================================
CREATE PROCEDURE [dbo].[btb_spGetOfertaXCategorieClient]

AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			19.12.2013			Ioan Gârbacea				Modificare:Returneaza ofertele speciale categorii
--======================================================================================
BEGIN

	SELECT sopc.SpecialOfferId as IdOfertaSpeciala, sopc.CategoryPartnerId as	IdCategorie
	FROM SpecialOffer so WITH (NOLOCK)
	JOIN SpecialOfferPartnerCategory sopc on so.SpecialOfferId=sopc.SpecialOfferId
	WHERE CAST(GETDATE() as varchar(12)) 
	BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetOfertaXClient]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetOfertaXClient]
GO 
CREATE PROCEDURE [dbo].[btb_spGetOfertaXClient]

AS
----------------------------------------------------------------------------------------
--            FOLOSITA IN:
--
--            [BizToBiz]
--                            OfertaSpeciala
----------------------------------------------------------------------------------------
--Versiune          Data                Progr.						Modificare
--======================================================================================
--[1.0.0.0]			23.02.2015			Silviu Pantea				Creare:Returneaza ofertele speciale pe clienti
--======================================================================================
BEGIN

	SELECT sop.SpecialOfferId as IdOfertaSpeciala, sop.PartnerId as	IdClient, sop.IsExcluded AS EsteExclus
	FROM SpecialOffer so WITH (NOLOCK)
	JOIN SpecialOfferXPartner sop on so.SpecialOfferId=sop.SpecialOfferId
	WHERE CAST(GETDATE() as varchar(12)) 
	BETWEEN so.StartDate AND so.EndDate AND so.isValid=1
END
GO
 


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetProduse]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetProduse]
GO 
 
 create procedure [dbo].[btb_spGetProduse]
 (
@tip int=0
)
as
BEGIN   
  
 SELECT   
   A.IdArticol AS IdProdus  
  ,A.Nume AS Denumire  
  --,cast(0 as decimal) AS DiscountCategProdus 
  ,isnull(DD.Nume,'') AS GrupProdus  
  ,PROD.Nume AS Producator  
  -- in nomenclator nu exista IdFurnizor, si aceasta informatie nu se paote aduce  
  ,'' AS Furnizor  
  ,A.ProcTVA AS CotaTVA  
  ,A.PretRidicataMSFaraTVA AS PretVanzare  
  --AndreiP 09.05.2014 -- am modificat incarcarea pretului de amanunt doar pentru RX-uri
  , case when  tp.Tip  like 'Medicamente%'
--  , CASE WHEN ISNULL(A.CodCAS, '') <> ''
		THEN A.PretAmanunt -- pentru RX se va afisa pretul de amanunt
		ELSE cast(0 as money) -- pentru OTC nu se va afisa pretul de amanunt
	END AS PretAmanunt  
  ,P.EsteStupefiant AS IsStupefiant  
  ,DCI.Nume AS DCI
  ,DataActivare   
  -- nu stiu ce anume trebuie sa apara la acest camp  
  -- impachetarea sau alteceva  
  ,'' AS UM  
   ,
   ISNULL(PL.ProcDiscMaximal, 0) Discount,
   p.CantNrBuc AS Impachetare
   , case when  tp.Tip  like 'Medicamente%' then 'RX' 
          when  tp.Tip  like 'OTC%' then 'OTC'
		   when  tp.Tip  like 'Paraf%' then 'PARAF' 
		   else 'ALTELE' end  RX_OTC
   ,A.CodCAS as CodCIM,
   case when  tp.Tip  like 'Medicamente%'  then 5 else null end NivelAutorizatie,
    cast(case when ddt.Nume='Deficitar' then 1 else 0 end  as bit) Deficitar
 FROM Articol A WITH (NOLOCK)
  left JOIN dbo.fnArtTipuriMedicamente(CONVERT(VARCHAR(8), GETDATE(),112)) tp   ON A.IdArticol = tp.IdArticol
  LEFT JOIN DictionarDetaliu DD WITH (NOLOCK) ON DD.IdDictionarDetaliu = A.IdCategorie  
  JOIN Produs P WITH (NOLOCK) ON P.IdArticol = A.IdArticol  
  LEFT JOIN ProdusXDCI PXD WITH (NOLOCK) ON A.IdArticol = PXD.IdArticol
  JOIN ProdusXLocatie PL WITH (NOLOCK) ON PL.IdArticol = A.IdArticol
--  LEFT JOIN btb_DiscountArticol DA WITH (NOLOCK) ON P.IdArticol=DA.IdArticol
  LEFT JOIN DictionarDetaliu DCI WITH (NOLOCK) ON PXD.IdDCI = DCI.IdDictionarDetaliu  
  left join DictionarDetaliu ddt with(nolock) on ddt.IdDictionarDetaliu=A.IdTipArticol 
  JOIN Partener PROD WITH (NOLOCK) ON PROD.IdPartener = P.IdProducator  
  LEFT JOIN vwListaMS vlm WITH (NOLOCK) ON vlm.IdArticol = A.IdArticol
 WHERE A.EsteActiv = 1 -- AND P.EsteVizibilDepozit = 1
 
END

go


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetPuncteLucru]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetPuncteLucru]
GO 
CREATE PROCEDURE [dbo].[btb_spGetPuncteLucru]
(
	@DeviceId varchar(36) 
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

BEGIN 
	--select IdClient into #temp from dbo.fn_GetPuncteLucruIdDevices(@DeviceID)
	
	SELECT 
		 PL.IdPunctLucru
		,P.IdPartener AS IdClient
		,PL.Denumire AS Nume
		,ISNULL(P.CodFiscal,'') AS CodFiscal
		,(  ISNULL(A.Strada, '') + ' ' + ISNULL(A.Numar, '') +
			CASE WHEN ISNULL(A.Bloc, '') = '' THEN '' ELSE ', bl. ' END +
			ISNULL(A.Bloc, '') + 
			CASE WHEN ISNULL(A.Scara, '') = '' THEN '' ELSE ', sc. ' END +
			ISNULL(A.Scara, '') + 
			CASE WHEN ISNULL(A.Ap, '') = '' THEN '' ELSE ', ap. ' END +
			ISNULL(A.Ap, '') + 
			CASE WHEN ISNULL(O.Nume, '') = '' THEN '' ELSE ', ' END +
			ISNULL(PL.Denumire, '') + 
			CASE WHEN ISNULL(J.Nume, '') = '' THEN '' ELSE ', judet ' END +
			ISNULL(J.Nume, '')  
		  ) AS Adresa
		,ISNULL(A.TelefonMobil, ISNULL(A.TelefonFix,'')) AS Telefon
		,ISNULL(A.Email,'') AS Email
		,CAST(0 AS BIT) AS Blocat
		,ISNULL(P.Cod, '') AS CodClient
		,ISNULL(J.Nume,'') AS Judet
		,ISNULL(O.Nume,'') AS Localitate
	FROM PunctLucru PL WITH (NOLOCK)
		JOIN Partener P WITH (NOLOCK) ON P.IdPartener = PL.IdPartener
		LEFT JOIN Adresa A WITH (NOLOCK) ON A.IdAdresa = PL.IdAdresa
		LEFT JOIN Oras O WITH (NOLOCK) ON O.IdOras = A.IdOras
		LEFT JOIN Judet J WITH (NOLOCK) ON J.IdJudet = O.IdJudet 
		--LEFT JOIN #temp on #temp.IdClient=P.IdPartener-- AND @DeviceId IS NOT NULL
	WHERE P.EsteClient = 1 AND P.EsteClientActiv = 1 AND PL.EsteActiv = 1-- AND (@DeviceId in('all','setrio') or #temp.IdClient IS NOT NULL)
	
	--DROP TABLE #temp
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetStocProduse]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetStocProduse]
GO 
CREATE PROCEDURE [dbo].[btb_spGetStocProduse](
@Gestiuni nvarchar(max)=null
)
AS 

SET ANSI_WARNINGS ON
SET ANSI_NULLS ON
SET NOCOUNT ON

BEGIN 
 	
	DECLARE @IdDistribuitor INT
	
	SET @IdDistribuitor = ( SELECT CAST (l.IdLocatie AS INT) AS IdDistribuitor
							FROM InitInfo II WITH (NOLOCK) 
							join Locatie l on l.IdLocatie= II.[Value]
							WHERE ii.[Object] = 'HostIdLocatie'  )
	
	
	select IdTipStoc 
	into #tipStoc
	from dbo.fnTipuriStoc(1)

    where EsteActivIesiriDepozit=1 and EsteActiv=1

	SELECT 
		 A.IdArticol AS IdProdus
		,A.Nume AS Denumire
		/*
			In BizDepozit exista notiunea de Intregi si Fractii
			si am calculat cantitatea din stoc separat pentru fiecare tip de cantitate
			daca e nevoie doar de cantitatea Intregi se ia informatia din coloana
			CantitateTotalaIntregi
			se iau in prima faza la nivel de discriminant de stoc
			ulterior mai trebuie introdusa celula si gestiunea
			inca nu sunt implementate
		*/
		,SUM(isnull(SR.CantIDisp,0)) AS CantitateTotalaIntregi 
		,SUM(isnull(SR.CantFDisp,0)) AS CantitateTotalaFractii      
		,S.BBD AS BBD
		,'' AS Lot
		--,S.Lot
		,@IdDistribuitor AS IdDistribuitor
		--,S.IdTipStoc
		--,S.IdFurnizor
		,A.AreStoc
		,min(d.DataOperareDocument) as DataIntrareStoc
		,--avg(isnull(ddd.DiscountFactura,0)+isnull(ddd.DiscountProdus,0)) as DiscountIntrare
		 min(isnull(cast( ((A.PretRidicataMSFaraTVA-S.PretAchizitie)/ (case when A.PretRidicataMSFaraTVA=0 then 1 else  A.PretRidicataMSFaraTVA end))*100 as decimal(12,2)),0)) as DiscountIntrare
		,max(isnull(ddd.ComAchizitie,'')) as ComAchizitie
		,max(isnull(ddd.ComVanzare,'')) as ComVanzare
		, S.IdStoc
		--, min(S.PretAchizitie) as PretRidicata
	INTO #ProduseStoc	
	FROM Articol  A WITH (NOLOCK)
	    
		left JOIN Stoc S WITH (NOLOCK) ON S.IdArticol = A.IdArticol
		left join #tipStoc ts with(nolock)  on ts.IdTipStoc=S.IdTipStoc
		LEFT JOIN vwStocDisp SR with(nolock)  ON S.IdStoc = SR.IdStoc
		left join DocumentDetaliu dd WITH (NOLOCK) on S.IdDocumentDetaliu= dd.IdDocumentDetaliu
		left join DocumentDetaliuDiscounturi ddd WITH(NOLOCK) on ddd.IdDocumentDetaliu=dd.IdDocumentDetaliu
		left join Document d on d.IdDocument=dd.IdDocument
	WHERE A.EsteActiv = 1 
	  AND( A.AreStoc=0 OR(  ISNULL(S.BBD,'20991231') > GETDATE() --AND A.PretRidicataMSCuTVA>0
		AND ts.IdTipStoc is not null)) -- stoc normal
		 
	GROUP BY A.IdArticol,A.Nume,A.AreStoc, S.BBD, S.IdStoc--,   ddd.ComAchizitie,ddd.ComVanzare
	ORDER BY A.Nume ASC
	
	
	
	
	-- selectul final		
	SELECT 
		 P.IdProdus
		,P.Denumire
		,ISNULL(SUM(P.CantitateTotalaIntregi),0) AS CantitateTotalaIntregi 
		,ISNULL(SUM(P.CantitateTotalaFractii),0) AS CantitateTotalaFractii      
		,case when isnull(P.BBD,'20991231') = '20991231' then   isnull(P.BBD,'20500101') else P.BBD end BBD
		,P.Lot
		,P.IdDistribuitor
		,/*SUM(isnull(PS.CantitateTotalaIntregi,0))*/0  AS StocTotal
		, P.DataIntrareStoc as DataIntrareStoc
		, P.DiscountIntrare 
		, P.ComAchizitie
		, P.ComVanzare 
		,isnull(P.IdStoc,0) IdStoc
		--,P.PretRidicata
	FROM #ProduseStoc P (NOLOCK)
	--LEFT JOIN #ProduseStocTotal PS (NOLOCK) ON P.IdProdus = PS.IdProdus

	GROUP BY 
		 P.IdProdus
		,P.Denumire
	--	,PS.BBD
		,P.BBD
		,P.Lot
		,P.IdDistribuitor
		,P.AreStoc
		,P.DiscountIntrare 
		,P.ComAchizitie
		, P.ComVanzare
		, P.DataIntrareStoc 
		, P.IdStoc
		--, P.PretRidicata
		--,P.DataIntrareStoc
	having ISNULL(SUM(P.CantitateTotalaIntregi),0)>0   or P.AreStoc=0
	ORDER BY P.Denumire ASC
    
	DROP TABLE #tipStoc
	DROP TABLE #ProduseStoc
	--DROP TABLE #ProduseStocTotal
	
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_GetOrderUpdateQtty]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_GetOrderUpdateQtty]
GO 
create PROCEDURE [dbo].[btb_GetOrderUpdateQtty]
(
@IdAgent int=0,
@IdsComenzi nvarchar(max)
)
AS
BEGIN 
  exec('select
  sd.IdOrder as IdComandaDistribuitor,
  cast(sd.ApprovedQty as int) as Aprobat,
  cast(isnull(sd.InvoicedQty,0) as int) as Facturat,
  sd.IdOrderDetail as IdElemComandaDistribuitor
  from SDOrderDetail sd with(nolock)
  where sd.IdOrder in('+@IdsComenzi+')')
end
GO


 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetListaPreturiParteneri]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetListaPreturiParteneri]
GO
create PROCEDURE [dbo].[btb_spGetListaPreturiParteneri]
	@IdPartener VARCHAR(MAX)='',
	@SelectColumns VARCHAR(1000)=null
AS
BEGIN

	DECLARE @HostIdLocatie	INT

	SELECT @HostIdLocatie = CAST(ii.[Value] AS INT) FROM InitInfo ii WITH (NOLOCK) WHERE ii.[Object] = 'HostIdLocatie'

	SET @SelectColumns = ISNULL(@SelectColumns, 'IdPartener, IdArticol, PretFTVA, PretCuTVA, Discount')
	
	-- extrag lista de clienti pentru care se doreste returnarea preturilor conform listei de pret
	IF OBJECT_ID('tempdb..#lp_Clienti') IS NOT NULL DROP TABLE #lp_Clienti
	
	CREATE TABLE #lp_Clienti(IdPartener INT, IdCategorie INT)
	
	EXEC('INSERT INTO #lp_Clienti(IdPartener, IdCategorie)
		  SELECT IdPartener, IdCategorie 
	      FROM Partener WITH (NOLOCK)')	
	
	-- aflu care sunt listele de preturi definite special pentru locatia curenta
	IF OBJECT_ID('tempdb..#lp_LocatieCurenta') IS NOT NULL DROP TABLE #lp_LocatieCurenta

	CREATE TABLE #lp_LocatieCurenta(IdListaPreturi INT)

	INSERT INTO #lp_LocatieCurenta(IdListaPreturi)
	SELECT DISTINCT lp.IdListaPreturi
	FROM ListaPreturi lp WITH (NOLOCK)
		JOIN ListaPreturiXLocatie lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
	WHERE   -- doar listele active
			lp.EsteActiv = 1
			-- doar listele al caror interval cuprind data curenta
			AND GETDATE() BETWEEN lp.DataInceput AND ISNULL(lp.DataSfarsit, CAST('99991231' AS DATETIME))
			-- doar listele pentru locatia curenta
			AND lpx.IdLocatie = @HostIdLocatie

	-- incarc listele de preturi definite pentru toate locatiile
	IF OBJECT_ID('tempdb..#lp_ToateLocatiile') IS NOT NULL DROP TABLE #lp_ToateLocatiile

	CREATE TABLE #lp_ToateLocatiile(IdListaPreturi INT)

	INSERT INTO #lp_ToateLocatiile(IdListaPreturi)
	SELECT DISTINCT lp.IdListaPreturi
	FROM ListaPreturi lp WITH (NOLOCK)
		LEFT JOIN ListaPreturiXLocatie lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		LEFT JOIN #lp_LocatieCurenta lpc ON lpc.IdListaPreturi = lp.IdListaPreturi
	WHERE   -- doar listele active
			lp.EsteActiv = 1
			-- doar listele al caror interval cuprind data curenta
			AND GETDATE() BETWEEN lp.DataInceput AND ISNULL(lp.DataSfarsit, CAST('99991231' AS DATETIME))
			-- doar listele definite pentru toate locatiie
			AND lpx.IdListaPreturi IS NULL
			-- doar listele care nu au fost deja introduse
			AND lpc.IdListaPreturi IS NULL

	-- unesc listele de preturi extrase intr-un singur set de date
	IF OBJECT_ID('tempdb..#ListePreturi') IS NOT NULL DROP TABLE #ListePreturi
			
	CREATE TABLE #ListePreturi(IdListaPreturi INT)

	INSERT INTO #ListePreturi(IdListaPreturi)
	SELECT IdListaPreturi
	FROM #lp_LocatieCurenta
	UNION
	SELECT IdListaPreturi
	FROM #lp_ToateLocatiile

	IF OBJECT_ID('tempdb..#lp_LocatieCurenta') IS NOT NULL DROP TABLE #lp_LocatieCurenta
	IF OBJECT_ID('tempdb..#lp_ToateLocatiile') IS NOT NULL DROP TABLE #lp_ToateLocatiile

	IF OBJECT_ID('tempdb..#ListePreturi_Partener') IS NOT NULL DROP TABLE #ListePreturi_Partener
			
	CREATE TABLE #ListePreturi_Partener(IdListaPreturi INT, IdPartener INT)

	-- listele de preturi definite pentru categoria clientului primit ca parametru
	IF ISNULL((SELECT COUNT(DISTINCT IdCategorie) FROM #lp_Clienti), 0) > 0
		INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
		SELECT DISTINCT lp.IdListaPreturi, lcc.IdPartener
		FROM #ListePreturi lp
			JOIN ListaPreturiXCategoriePartener lpxp WITH (NOLOCK) ON lpxp.IdListaPreturi = lp.IdListaPreturi AND lpxp.EsteActiv = 1
			JOIN #lp_Clienti lcc ON lcc.IdCategorie = lpxp.IdCategorie

	-- listele de preturi definite pentru clientii primit ca parametru
	INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
	SELECT DISTINCT lpx.IdListaPreturi, lpx.IdPartener
	FROM #ListePreturi lp
		JOIN ListaPreturiXPartener lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		JOIN #lp_Clienti lcc ON lcc.IdPartener = lpx.IdPartener
		LEFT JOIN #ListePreturi_Partener lpp WITH (NOLOCK) ON lpx.IdListaPreturi = lpp.IdListaPreturi AND lpx.IdPartener = lpp.IdPartener
	--WHERE lpp.IdListaPreturi IS NOT NULL

	-- listele de preturi definite pentru toti clientii
	INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
	SELECT DISTINCT lp.IdListaPreturi, lc.IdPartener
	FROM #ListePreturi lp
		LEFT JOIN ListaPreturiXCategoriePartener lpxp WITH (NOLOCK) ON lpxp.IdListaPreturi = lp.IdListaPreturi AND lpxp.EsteActiv = 1
		LEFT JOIN ListaPreturiXPartener lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		LEFT JOIN #ListePreturi_Partener lpp ON lp.IdListaPreturi = lpp.IdListaPreturi
		JOIN #lp_Clienti lc ON 1 = 1
	WHERE lpxp.IdListaPreturi IS NULL
			AND lpx.IdListaPreturi IS NULL
			AND lpp.IdListaPreturi IS NULL
			
	IF OBJECT_ID('tempdb..#ListePreturi') IS NOT NULL DROP TABLE #ListePreturi

	-- extragerea produselor si a preturilor acestora
	IF OBJECT_ID('tempdb..#ListePreturi_Articole') IS NOT NULL DROP TABLE #ListePreturi_Articole

	CREATE TABLE #ListePreturi_Articole(IdPartener INT, IdArticol INT, PretFTVA DECIMAL(20,6), PretCuTVA DECIMAL(20,6), ProcAdaos DECIMAL(20, 3), Discount DECIMAL(20,3))

	INSERT INTO #ListePreturi_Articole(IdPartener, IdArticol, PretFTVA, PretCuTVA, ProcAdaos, Discount)
	SELECT T.IdPartener, T.IdArticol, T.PretFTVA, T.PretCuTVA, T.ProcAdaos, T.ProcDiscount
	FROM (
		SELECT lp.IdPartener
				, lpd.IdArticol
				, COALESCE(lpdi.PretFTVA, lpd.PretFTVA, 0) AS PretFTVA
				, COALESCE(lpdi.PretCuTVA, lpd.PretCuTVA, 0) AS PretCuTVA
				, COALESCE(lpd.ProcAdaos, 0) AS ProcAdaos
				, COALESCE(lpd.ProcDiscount, 0) AS ProcDiscount
				, lp.IdListaPreturi
				, lpd.IdListaPreturiDetaliu
				, ISNULL(lpdi.IdInterval, 0) AS IdInterval
				, ROW_NUMBER() OVER(PARTITION BY lp.IdPartener, lpd.IdArticol ORDER BY lp.IdPartener, lpd.IdArticol, lpd.IdListaPreturiDetaliu DESC, ISNULL(lpdi.IdInterval, 0) DESC) AS Ordine
		FROM #ListePreturi_Partener lp
			JOIN ListaPreturiDetaliu lpd WITH (NOLOCK) ON lpd.IdListaPreturi = lp.IdListaPreturi
			LEFT JOIN ListaPreturiDetaliuInterval lpdi WITH (NOLOCK)
					 ON lpdi.IdListaPreturiDetaliu = lpd.IdListaPreturiDetaliu
						AND lpdi.EsteActiv = 1
						AND GETDATE() BETWEEN lpdi.DataInceput AND lpdi.DataSfarsit
		WHERE lpd.EsteActiv = 1 --and lpdi.PretFTVA>0
	) T
	WHERE T.Ordine = 1

	IF OBJECT_ID('tempdb..#ListePreturi_Partener') IS NOT NULL DROP TABLE #ListePreturi_Partener

	EXEC('SELECT ' + @SelectColumns + ' FROM #ListePreturi_Articole')

	IF OBJECT_ID('tempdb..#ListePreturi_Articole') IS NOT NULL DROP TABLE #ListePreturi_Articole
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[btb_spGetListaPreturiParteneri]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[btb_spGetListaPreturiParteneri]
GO
create PROCEDURE [dbo].[btb_spGetListaPreturiParteneri]
	@IdPartener VARCHAR(MAX)='',
	@SelectColumns VARCHAR(1000)=null
AS
BEGIN

	DECLARE @HostIdLocatie	INT

	SELECT @HostIdLocatie = CAST(ii.[Value] AS INT) FROM InitInfo ii WITH (NOLOCK) WHERE ii.[Object] = 'HostIdLocatie'

	SET @SelectColumns = ISNULL(@SelectColumns, 'IdPartener, IdArticol, PretFTVA, PretCuTVA, Discount')
	
	-- extrag lista de clienti pentru care se doreste returnarea preturilor conform listei de pret
	IF OBJECT_ID('tempdb..#lp_Clienti') IS NOT NULL DROP TABLE #lp_Clienti
	
	CREATE TABLE #lp_Clienti(IdPartener INT, IdCategorie INT)
	
	EXEC('INSERT INTO #lp_Clienti(IdPartener, IdCategorie)
		  SELECT IdPartener, IdCategorie 
	      FROM Partener WITH (NOLOCK)')	
	
	-- aflu care sunt listele de preturi definite special pentru locatia curenta
	IF OBJECT_ID('tempdb..#lp_LocatieCurenta') IS NOT NULL DROP TABLE #lp_LocatieCurenta

	CREATE TABLE #lp_LocatieCurenta(IdListaPreturi INT)

	INSERT INTO #lp_LocatieCurenta(IdListaPreturi)
	SELECT DISTINCT lp.IdListaPreturi
	FROM ListaPreturi lp WITH (NOLOCK)
		JOIN ListaPreturiXLocatie lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
	WHERE   -- doar listele active
			lp.EsteActiv = 1
			-- doar listele al caror interval cuprind data curenta
			AND GETDATE() BETWEEN lp.DataInceput AND ISNULL(lp.DataSfarsit, CAST('99991231' AS DATETIME))
			-- doar listele pentru locatia curenta
			AND lpx.IdLocatie = @HostIdLocatie

	-- incarc listele de preturi definite pentru toate locatiile
	IF OBJECT_ID('tempdb..#lp_ToateLocatiile') IS NOT NULL DROP TABLE #lp_ToateLocatiile

	CREATE TABLE #lp_ToateLocatiile(IdListaPreturi INT)

	INSERT INTO #lp_ToateLocatiile(IdListaPreturi)
	SELECT DISTINCT lp.IdListaPreturi
	FROM ListaPreturi lp WITH (NOLOCK)
		LEFT JOIN ListaPreturiXLocatie lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		LEFT JOIN #lp_LocatieCurenta lpc ON lpc.IdListaPreturi = lp.IdListaPreturi
	WHERE   -- doar listele active
			lp.EsteActiv = 1
			-- doar listele al caror interval cuprind data curenta
			AND GETDATE() BETWEEN lp.DataInceput AND ISNULL(lp.DataSfarsit, CAST('99991231' AS DATETIME))
			-- doar listele definite pentru toate locatiie
			AND lpx.IdListaPreturi IS NULL
			-- doar listele care nu au fost deja introduse
			AND lpc.IdListaPreturi IS NULL

	-- unesc listele de preturi extrase intr-un singur set de date
	IF OBJECT_ID('tempdb..#ListePreturi') IS NOT NULL DROP TABLE #ListePreturi
			
	CREATE TABLE #ListePreturi(IdListaPreturi INT)

	INSERT INTO #ListePreturi(IdListaPreturi)
	SELECT IdListaPreturi
	FROM #lp_LocatieCurenta
	UNION
	SELECT IdListaPreturi
	FROM #lp_ToateLocatiile

	IF OBJECT_ID('tempdb..#lp_LocatieCurenta') IS NOT NULL DROP TABLE #lp_LocatieCurenta
	IF OBJECT_ID('tempdb..#lp_ToateLocatiile') IS NOT NULL DROP TABLE #lp_ToateLocatiile

	IF OBJECT_ID('tempdb..#ListePreturi_Partener') IS NOT NULL DROP TABLE #ListePreturi_Partener
			
	CREATE TABLE #ListePreturi_Partener(IdListaPreturi INT, IdPartener INT)

	-- listele de preturi definite pentru categoria clientului primit ca parametru
	IF ISNULL((SELECT COUNT(DISTINCT IdCategorie) FROM #lp_Clienti), 0) > 0
		INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
		SELECT DISTINCT lp.IdListaPreturi, lcc.IdPartener
		FROM #ListePreturi lp
			JOIN ListaPreturiXCategoriePartener lpxp WITH (NOLOCK) ON lpxp.IdListaPreturi = lp.IdListaPreturi AND lpxp.EsteActiv = 1
			JOIN #lp_Clienti lcc ON lcc.IdCategorie = lpxp.IdCategorie

	-- listele de preturi definite pentru clientii primit ca parametru
	INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
	SELECT DISTINCT lpx.IdListaPreturi, lpx.IdPartener
	FROM #ListePreturi lp
		JOIN ListaPreturiXPartener lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		JOIN #lp_Clienti lcc ON lcc.IdPartener = lpx.IdPartener
		LEFT JOIN #ListePreturi_Partener lpp WITH (NOLOCK) ON lpx.IdListaPreturi = lpp.IdListaPreturi AND lpx.IdPartener = lpp.IdPartener
	--WHERE lpp.IdListaPreturi IS NOT NULL

	-- listele de preturi definite pentru toti clientii
	INSERT INTO #ListePreturi_Partener(IdListaPreturi, IdPartener)
	SELECT DISTINCT lp.IdListaPreturi, lc.IdPartener
	FROM #ListePreturi lp
		LEFT JOIN ListaPreturiXCategoriePartener lpxp WITH (NOLOCK) ON lpxp.IdListaPreturi = lp.IdListaPreturi AND lpxp.EsteActiv = 1
		LEFT JOIN ListaPreturiXPartener lpx WITH (NOLOCK) ON lpx.IdListaPreturi = lp.IdListaPreturi AND lpx.EsteActiv = 1
		LEFT JOIN #ListePreturi_Partener lpp ON lp.IdListaPreturi = lpp.IdListaPreturi
		JOIN #lp_Clienti lc ON 1 = 1
	WHERE lpxp.IdListaPreturi IS NULL
			AND lpx.IdListaPreturi IS NULL
			AND lpp.IdListaPreturi IS NULL
			
	IF OBJECT_ID('tempdb..#ListePreturi') IS NOT NULL DROP TABLE #ListePreturi

	-- extragerea produselor si a preturilor acestora
	IF OBJECT_ID('tempdb..#ListePreturi_Articole') IS NOT NULL DROP TABLE #ListePreturi_Articole

	CREATE TABLE #ListePreturi_Articole(IdPartener INT, IdArticol INT, PretFTVA DECIMAL(20,6), PretCuTVA DECIMAL(20,6), ProcAdaos DECIMAL(20, 3), Discount DECIMAL(20,3))

	INSERT INTO #ListePreturi_Articole(IdPartener, IdArticol, PretFTVA, PretCuTVA, ProcAdaos, Discount)
	SELECT T.IdPartener, T.IdArticol, T.PretFTVA, T.PretCuTVA, T.ProcAdaos, T.ProcDiscount
	FROM (
		SELECT lp.IdPartener
				, lpd.IdArticol
				, COALESCE(lpdi.PretFTVA, lpd.PretFTVA, 0) AS PretFTVA
				, COALESCE(lpdi.PretCuTVA, lpd.PretCuTVA, 0) AS PretCuTVA
				, COALESCE(lpd.ProcAdaos, 0) AS ProcAdaos
				, COALESCE(lpd.ProcDiscount, 0) AS ProcDiscount
				, lp.IdListaPreturi
				, lpd.IdListaPreturiDetaliu
				, ISNULL(lpdi.IdInterval, 0) AS IdInterval
				, ROW_NUMBER() OVER(PARTITION BY lp.IdPartener, lpd.IdArticol ORDER BY lp.IdPartener, lpd.IdArticol, lpd.IdListaPreturiDetaliu DESC, ISNULL(lpdi.IdInterval, 0) DESC) AS Ordine
		FROM #ListePreturi_Partener lp
			JOIN ListaPreturiDetaliu lpd WITH (NOLOCK) ON lpd.IdListaPreturi = lp.IdListaPreturi
			LEFT JOIN ListaPreturiDetaliuInterval lpdi WITH (NOLOCK)
					 ON lpdi.IdListaPreturiDetaliu = lpd.IdListaPreturiDetaliu
						AND lpdi.EsteActiv = 1
						AND GETDATE() BETWEEN lpdi.DataInceput AND lpdi.DataSfarsit
		WHERE lpd.EsteActiv = 1 --and lpdi.PretFTVA>0
	) T
	WHERE T.Ordine = 1

	IF OBJECT_ID('tempdb..#ListePreturi_Partener') IS NOT NULL DROP TABLE #ListePreturi_Partener

	EXEC('SELECT ' + @SelectColumns + ' FROM #ListePreturi_Articole')

	IF OBJECT_ID('tempdb..#ListePreturi_Articole') IS NOT NULL DROP TABLE #ListePreturi_Articole
END
GO

