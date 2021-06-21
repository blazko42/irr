use LandmarkIRR
go

--roluri aplicatie
create role ADMIN_ROLE
create role PUBLIC_ROLE
create user anonymous_access with password ='Irr1234'
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.GetLandmarks') is not null) drop proc IRR.GetLandmarks
go

create procedure IRR.GetLandmarks
	@LandmarkId	varchar(max) = null
as set nocount on

select ld.LDId, ld.LandmarkId, ISNULL(ld.LandmarkURL, 'N/A') as [LandmarkURL], ld.LandmarkImage, ld.FeatureExtractor, li.Name, li.Location, li.Information, ld.Date
	from IRR.LANDMARK_DATASET ld
	inner join IRR.LANDMARK_INFORMATION li on li.LDId = ld.LDId
	inner join IRR.FEATURE_EXTRACTOR fe on fe.FEId = ld.FeatureExtractor
	where ld.LandmarkId = ISNULL(@LandmarkId, LandmarkId) and fe.IsActive = 1
	order by ld.LandmarkId;

go

grant exec on IRR.GetLandmarks to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------
---face retrieveul de landmark din db

if (OBJECT_ID('IRR.RetrieveLandmark') is not null) drop proc IRR.RetrieveLandmark
go

create procedure IRR.RetrieveLandmark
	@Landmarks	xml
as set nocount on

select ld.LDId, ld.LandmarkId, ISNULL(ld.LandmarkURL, 'N/A') as [LandmarkURL], ld.LandmarkImage, ld.FeatureExtractor, li.Name, li.Location, li.Information, li.Date
	from IRR.LANDMARK_DATASET ld
	inner join IRR.LANDMARK_INFORMATION li on li.LDId = ld.LDId
	inner join @Landmarks.nodes('/Landmarks/Landmark') as L(mark) on L.mark.value('(./@LandmarkId)[1]','varchar(max)') = ld.LandmarkId
	order by L.mark.value('(./@LandmarkRank)[1]','int');

go

grant exec on IRR.RetrieveLandmark to ADMIN_ROLE
go

grant exec on IRR.RetrieveLandmark to PUBLIC_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.DeleteLandmark') is not null) drop proc IRR.DeleteLandmark
go

create procedure IRR.DeleteLandmark
	@LDId	int
as set nocount on
begin try
begin tran
delete from IRR.LANDMARK_DATASET
	where LDId = @LDId;

delete from IRR.LANDMARK_INFORMATION
	where LDId = @LDId;

commit tran
end try
begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE		nvarchar(4000),
		@ERROR_SEVERITY		int,
		@ERROR_STATE		int

	select @ERROR_MESSAGE = convert(nvarchar, ERROR_NUMBER()) + isnull(' - in SP: ' + ERROR_PROCEDURE()+ ' at line: ' + convert(nvarchar, ERROR_LINE()), '') + ':' + ERROR_MESSAGE() ,
		@ERROR_SEVERITY = ERROR_SEVERITY(), @ERROR_STATE = ERROR_STATE()
	raiserror(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
end catch
go

grant exec on IRR.DeleteLandmark to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.SaveLandmark') is not null) drop proc IRR.SaveLandmark
go

create procedure IRR.SaveLandmark
	@LDId			int = null,
	@LandmarkId		varchar(max) = null,
	@LandmarkURL		varchar(max) = null,
	@Name			nvarchar(max) = null,
	@Location		nvarchar(max) = null,
	@Information		nvarchar(max) = null

as set nocount on
begin try
begin tran
update IRR.LANDMARK_DATASET set LandmarkId = ISNULL(@LandmarkId, LandmarkId),
	LandmarkURL = ISNULL(@LandmarkURL, LandmarkURL)
	where LDId = @LDId;
if(@@ROWCOUNT = 0)
begin
	insert into IRR.LANDMARK_DATASET(LandmarkId, LandmarkURL)
		select @LandmarkId, @LandmarkURL;
	insert into IRR.LANDMARK_INFORMATION(LDId, Name, Location, Information)
		select SCOPE_IDENTITY(), @Name, @Location, @Information;
end
else
update IRR.LANDMARK_INFORMATION set Name = ISNULL(@Name, Name),
	Location = ISNULL(@Location, Location),
	Information = ISNULL(@Information, Information)
	where LDId = @LDId;

commit tran
end try
begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE		nvarchar(4000),
		@ERROR_SEVERITY		int,
		@ERROR_STATE		int

	select @ERROR_MESSAGE = convert(nvarchar, ERROR_NUMBER()) + isnull(' - in SP: ' + ERROR_PROCEDURE()+ ' at line: ' + convert(nvarchar, ERROR_LINE()), '') + ':' + ERROR_MESSAGE() ,
		@ERROR_SEVERITY = ERROR_SEVERITY(), @ERROR_STATE = ERROR_STATE()
	raiserror(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
end catch
go

grant exec on IRR.SaveLandmark to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.SaveLandmarkBinaryImage') is not null) drop proc IRR.SaveLandmarkBinaryImage
go

create procedure IRR.SaveLandmarkBinaryImage
	@LandmarkId		varchar(max),
	@LandmarkImage		varbinary(max),
	@FeatureExtractor	varchar(20),
	@Name			nvarchar(max),
	@Location		nvarchar(max),
	@Information		nvarchar(max),
	@DatasetSize		int
as set nocount on

declare @IsActiveComputed bit,
	@ActiveDatasetDBSize int;
begin try
begin tran

select @IsActiveComputed  = IsComputed 
	from IRR.FEATURE_EXTRACTOR 
	where IsActive = 1;

if(@IsActiveComputed = 1)
	raiserror('Delete current active feature extractor dataset first!', 11, 1);

insert into IRR.LANDMARK_DATASET(LandmarkId, LandmarkImage, FeatureExtractor)
	select @LandmarkId, @LandmarkImage, FEId
		from IRR.FEATURE_EXTRACTOR
		where Code = @FeatureExtractor;

insert into IRR.LANDMARK_INFORMATION(LDId, Name, Location, Information)
	select SCOPE_IDENTITY(), @Name, @Location, @Information;


select @ActiveDatasetDBSize = COUNT(ld.LDId)
	from IRR.LANDMARK_DATASET ld
	inner join IRR.FEATURE_EXTRACTOR fe on fe.FEId = ld.FeatureExtractor
	where fe.Code = @FeatureExtractor;

if(@ActiveDatasetDBSize = @DatasetSize)
	update IRR.FEATURE_EXTRACTOR set IsComputed = 1 
		where Code = @FeatureExtractor;

commit tran
end try
begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE		nvarchar(4000),
		@ERROR_SEVERITY		int,
		@ERROR_STATE		int

	select @ERROR_MESSAGE =  ERROR_MESSAGE(), @ERROR_SEVERITY = ERROR_SEVERITY(), @ERROR_STATE = ERROR_STATE()
	raiserror(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
end catch
go

grant exec on IRR.SaveLandmarkBinaryImage to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.GetFeatureExtractors') is not null) drop proc IRR.GetFeatureExtractors
go

create procedure IRR.GetFeatureExtractors
as set nocount on

select FEId, Code, FeatureExtractorName, IsActive, IsComputed
	from IRR.FEATURE_EXTRACTOR
	order by FEId;

go

grant exec on IRR.GetFeatureExtractors to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.GetActiveFeatureExtractor') is not null) drop proc IRR.GetActiveFeatureExtractor
go

create procedure IRR.GetActiveFeatureExtractor
as set nocount on

select FEId, Code, FeatureExtractorName, IsActive, IsComputed
	from IRR.FEATURE_EXTRACTOR
	where IsActive = 1;

go

grant exec on IRR.GetActiveFeatureExtractor to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.SetActiveFeatureExtractor') is not null) drop proc IRR.SetActiveFeatureExtractor
go

create procedure IRR.SetActiveFeatureExtractor
	@FEId	int
as set nocount on

begin try
begin tran

update IRR.FEATURE_EXTRACTOR set IsActive = 0;

update IRR.FEATURE_EXTRACTOR set IsActive = 1
	where FEId = @FEId;

commit tran
end try
begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE		nvarchar(4000),
		@ERROR_SEVERITY		int,
		@ERROR_STATE		int

	select @ERROR_MESSAGE = convert(nvarchar, ERROR_NUMBER()) + isnull(' - in SP: ' + ERROR_PROCEDURE()+ ' at line: ' + convert(nvarchar, ERROR_LINE()), '') + ':' + ERROR_MESSAGE() ,
		@ERROR_SEVERITY = ERROR_SEVERITY(), @ERROR_STATE = ERROR_STATE()
	raiserror(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
end catch

go

grant exec on IRR.SetActiveFeatureExtractor to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------

if (OBJECT_ID('IRR.ClearDatasetForActiveFeatureExtractor') is not null) drop proc IRR.ClearDatasetForActiveFeatureExtractor
go

create procedure IRR.ClearDatasetForActiveFeatureExtractor
as set nocount on

declare @IsActiveNotComputed	bit;
begin try
begin tran

select @IsActiveNotComputed = IsComputed
	from IRR.FEATURE_EXTRACTOR
	where IsActive = 1;

if(@IsActiveNotComputed = 0)
	raiserror('No dataset to clear.', 11, 1);

update IRR.FEATURE_EXTRACTOR set IsComputed = 0 
	where IsActive = 1;


delete li
	from IRR.LANDMARK_INFORMATION li
	inner join IRR.LANDMARK_DATASET ld on ld.LDId = li.LDId
	inner join IRR.FEATURE_EXTRACTOR fe on fe.FEId = ld.FeatureExtractor
	where fe.IsActive = 1;

delete ld
	from IRR.LANDMARK_DATASET ld 
	inner join IRR.FEATURE_EXTRACTOR fe on fe.FEId = ld.FeatureExtractor
	where fe.IsActive = 1;

commit tran
end try
begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE		nvarchar(4000),
		@ERROR_SEVERITY		int,
		@ERROR_STATE		int

	select @ERROR_MESSAGE = ERROR_MESSAGE(), @ERROR_SEVERITY = ERROR_SEVERITY(), @ERROR_STATE = ERROR_STATE()
	raiserror(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE)
end catch

go

grant exec on IRR.ClearDatasetForActiveFeatureExtractor to ADMIN_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------
if OBJECT_ID('IRR.IsAdmin') is not null drop proc IRR.IsAdmin 
go

create proc IRR.IsAdmin
	@IsAdmin		bit = null out
as set nocount on
select @IsAdmin = IS_MEMBER('db_owner') | IS_MEMBER('ADMIN_ROLE');
go

grant exec on IRR.IsAdmin to [anonymous_access]
go

grant exec on IRR.IsAdmin to ADMIN_ROLE
go

grant exec on IRR.IsAdmin to PUBLIC_ROLE
go

--------------------------------------------------------------------------------------------------------------------------------------
if OBJECT_ID('IRR.CreateUser') is not null drop proc IRR.CreateUser 
go

create proc IRR.CreateUser
	@UserName		sysname,
	@Password		sysname
with execute as owner

as set nocount on

begin try
	declare @query		nvarchar(max) = 'create user ' + QUOTENAME(@UserName) + ' with password = ''' + REPLACE(@Password, '''', '''''') + '''';
begin tran
	exec(@query);
	select @query = 'alter role PUBLIC_ROLE add member [' + @UserName + ']';
	exec(@query);
commit tran
end try

begin catch
	if @@TRANCOUNT > 0 rollback tran
	declare @ERROR_MESSAGE	nvarchar(max) = ERROR_MESSAGE();
	raiserror(@ERROR_MESSAGE, 11, 1);
end catch
go

grant exec on IRR.CreateUser to ADMIN_ROLE
go

grant exec on IRR.CreateUser to PUBLIC_ROLE
go

grant exec on IRR.CreateUser to [anonymous_access]
go

--------------------------------------------------------------------------------------------------------------------------------------
