Create table REGION(
RegionID int primary key,
RegionName varchar(255),
CountryName varchar(255)
);

CREATE TABLE USERS(
UserID INT PRIMARY KEY identity,
First_Name VARCHAR(255) NOT NULL,
Last_Name VARCHAR(255) NOT NULL,
Email VARCHAR(255) NOT NULL UNIQUE,
Phone_Number VARCHAR(10) NOT NULL UNIQUE
CHECK (LEN(Phone_Number)=10),
RegionID int foreign key references REGION(RegionID),
[Password] nvarchar(255) NOT NULL,
CreationDate DateTime NOT NULL Default GETDATE(),
[Status] Varchar(10) default 'active' --active,inactive,banned
);

--TO add avatar image if time permits
create table USER_PROFILE(
ProfileID int primary key identity,
UserID int foreign key references USERS(UserID),
ProfileName nvarchar(255) NOT NULL,
Age int NOT NULL,
Avatar varchar(MAX),
IsKidProfile BIT default 0,
);

--currently admin is one and has all access, can add AccessLevel column to increase Field Security
create table [ADMIN](
AdminID int primary key identity,
First_Name VARCHAR(255) NOT NULL,
Last_Name VARCHAR(255) NOT NULL,
Username VARCHAR(255) NOT NULL UNIQUE,
[Password] nvarchar(255) NOT NULL,
AccessLevel varchar(255) NOT NULL ---BillingManager, SupportSpecialist, ContentManager, SuperAdmin, DataAnalyst
);

create table GENRE(
GenreID int primary key,
GenreName nvarchar(255) NOT NULL,
);

create table CONTENT(
ContentID int primary key,
AdminID int foreign key references [ADMIN](AdminID),
Title nvarchar(255) NOT NULL,
[Description] nvarchar(MAX),
ReleaseDate Date NOT NULL,
ContentType Varchar(255) NOT NULL, --SILVER GOLD BRONZE
GenreID int foreign key references GENRE(GenreID),
);

create table MOVIE(
MovieID int foreign key references CONTENT(ContentID),
DurationInMinutes int not null,
TrailerURL varchar(MAX) NOT NULL,
PosterURL varchar(MAX) NOT NULL,
OverallRating int Default 1
Check (OverallRating between 1 and 5),
VideoUrl varchar(MAX) NOT NULL
);


create table ACTOR(
ActorID int primary key,
First_Name varchar(255) NOT NULL,
Last_Name Varchar(255) NOT NULL
);

create table CONTENT_ACTOR(
ContentID int foreign key references CONTENT(ContentID),
ActorID int foreign key references ACTOR(ActorID),
[Role] varchar(255) NOT NULl,
);

create table WATCHLIST(
ProfileID int foreign key references USER_PROFILE(ProfileID),
ContentID int foreign key references CONTENT(ContentID),
DateAdded Date default GETDATE()
);

create table REVIEW(
ReviewID int primary key identity,
ProfileID int foreign key references USER_PROFILE(ProfileID),
ContentID int foreign key references CONTENT(ContentID),
RatingValue int NOT NULL
Check (RatingValue BETWEEN 1 AND 5),
RatingText NVARCHAR(MAX) NOT NULL
);

create table SUBSCRIPTION_PLAN(
PlanID int Primary key,
AdminID int foreign key references [ADMIN](AdminID),
PlanName VARCHAR(255) Not null,
PlanDuration int Not Null,
Price Decimal(10,2) Not null,
MaxScreens int Not null
);

create table SUBSCRIPTION(
SubscriptionID int primary key identity,
UserID int foreign key references USERS(UserID),
PlanID int foreign key references SUBSCRIPTION_PLAN(PlanID),
StartDate date default getDate(),
EndDate datetime,
Status varchar(255) default 'active' --active, expired,cancelled
);

create table VIEWING_HISTORY(
HistoryID int primary key identity,
ProfileID int foreign key references USER_PROFILE(ProfileID),
ContentID int foreign key references CONTENT(ContentID),
ViewedAt datetime not null,
ProgressInSeconds bigint default 0,
);

