use LandmarkIRR
go

if SCHEMA_ID('IRR') is null exec('create schema IRR');
go

if (OBJECT_ID('IRR.LANDMARK_DATASET') is not null) drop table IRR.LANDMARK_DATASET
go

create table IRR.LANDMARK_DATASET(
	LDId			int identity(1,1), 
	LandmarkId		varchar(max) not null, 
	LandmarkURL		varchar(max),
	LandmarkImage		varbinary(max),
	FeatureExtractor	int,
	Date			datetime default GETDATE())
go

create index iLandmarkDataset on IRR.LANDMARK_DATASET(LdId)
go


if (OBJECT_ID('IRR.LANDMARK_INFORMATION') is not null) drop table IRR.LANDMARK_INFORMATION
go

create table IRR.LANDMARK_INFORMATION(
	LDId			int not null,
	Name			nvarchar(max) default 'Lorem ipsum dolor sit amet.',
	Location		nvarchar(max) default 'Lorem ipsum dolor sit amet.',
	Information		nvarchar(max) default 'Lorem ipsum dolor sit amet.',
	Date			datetime default GETDATE())

go

create index iLandmarks on IRR.LANDMARK_INFORMATION(LDId)
go

if (OBJECT_ID('IRR.FEATURE_EXTRACTOR') is not null) drop table IRR.FEATURE_EXTRACTOR
go

create table IRR.FEATURE_EXTRACTOR(
	FEId			int identity(1,1), 
	Code			varchar(50),
	FeatureExtractorName	nvarchar(max),
	IsActive		bit default 0,
	IsComputed		bit default 0,
	Date			datetime default GETDATE())

go


update IRR.FEATURE_EXTRACTOR set FeatureExtractorName = 'Scale-invariant feature transform (SIFT)', IsActive = 1
	where Code = 'SIFT';
if(@@ROWCOUNT = 0)
insert into IRR.FEATURE_EXTRACTOR(Code, FeatureExtractorName, IsActive)
	select 'SIFT', 'Scale-invariant feature transform (SIFT)', 1;

update IRR.FEATURE_EXTRACTOR set FeatureExtractorName = 'Speeded up robust features (SURF)', IsActive = 1
	where Code = 'SURF';
if(@@ROWCOUNT = 0)
insert into IRR.FEATURE_EXTRACTOR(Code, FeatureExtractorName, IsActive)
	select 'SURF', 'Speeded up robust features (SURF)', 0;

















