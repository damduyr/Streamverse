CREATE TABLE USERS(
UserID INT PRIMARY KEY identity,
First_Name VARCHAR(255) NOT NULL,
Last_Name VARCHAR(255) NOT NULL,
Email VARCHAR(255) NOT NULL UNIQUE,
Phone_Number VARCHAR(10) NOT NULL UNIQUE
CHECK (LEN(Phone_Number)=10),
DOB date NOT NULL,
RegionName varchar(255) not null,
[Password] nvarchar(255) NOT NULL,
CreationDate DateTime Default GETDATE(),
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
AccessLevel varchar(255) NOT NULL, ---ContentManager, SystemAdmin, DataAnalyst
Email varchar(255) not null unique,
activeStatus bit default 1 --1 is active 0 is inactive
);

create table GENRE(
GenreID int primary key,
GenreName nvarchar(255) NOT NULL ,
);
alter table GENRE
add constraint unique_GenreName unique(GenreName);
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
OverallRating decimal(3,2) Default 1
Check (OverallRating between 1 and 5),
VideoUrl varchar(MAX) NOT NULL
);
create table WATCHLIST(
ProfileID int foreign key references USER_PROFILE(ProfileID),
ContentID int foreign key references CONTENT(ContentID),
DateAdded Date default GETDATE()
);
alter table WATCHLIST
add WatchlistID int primary key identity;


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
PlanName VARCHAR(255) Not null, --SILVER GOLD BRONZE
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
);

create table VIEWING_HISTORY(
HistoryID int primary key identity,
ProfileID int foreign key references USER_PROFILE(ProfileID),
ContentID int foreign key references CONTENT(ContentID),
ViewedAt datetime not null,
ProgressInSeconds bigint default 0,
);

drop table VIEWING_HISTORY ;

drop table CONTENT_ACTOR ;

drop table ACTOR ;
 
-- @using Streamverse.Models.UserLogin
--@{
--    ViewBag.Title = "Sign In";
--}
--@*@Scripts.Render("~/Scipts/JS/loginRegister.js");
--<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>*@
--@Styles.Render("~/Content/CSS/loginRegister.css");
--@using (Html.BeginForm("","",FormMethod.Post, new { name = "UserLoginForm"}))
--{
--    <body>
--        <div class="bg-image"></div>    
--        <div class='signIn'>
--            <p align=center>Sign In</p>
--            <div class='tabs'>
--                <span style='font-size:25px;'>&#x1F464;</span>
--                <input type="email" id="email-box" placeholder="Email ID">
--                @Html.TextBoxFor(model => model.First_Name, new { htmlAttributes = new { @class = "form-control" } })<br>
--                <span style='font-size:25px;'>&#x1F512;</span>
--                <input type="password" id="password" placeholder="Password"><br>
--                <button type="submit">Sign In</button>

--                <div>New to Streamverse? <a href="" target="_self">Sign up now.</a></div>
                    
--            </div>

--        </div>
--     </body>
--}

delete from CONTENT
where ContentID = 103;
delete from REVIEW
where ContentID = 103;

SELECT * FROM USERS;

