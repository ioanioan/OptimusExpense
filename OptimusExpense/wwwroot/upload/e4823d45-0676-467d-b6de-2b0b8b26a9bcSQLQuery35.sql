
GO
/****** Object:  StoredProcedure [dbo].[spTrimiteMail]    Script Date: 5/21/2021 7:39:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  create  procedure [dbo].[spTrimiteMail](
  @Emails nvarchar(500),
  @Continut nvarchar(max),
  @Subiect nvarchar(max)
  )
  as
  begin
 --- 	exec  GOPHARMA.Gopharma.dbo.spTrimiteMail @Emails,@Continut,@Subiect

	select 1 as R
  end

