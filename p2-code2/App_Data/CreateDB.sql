BEGIN TRANSACTION; -- CREATE TABLES
  CREATE TABLE dbo.Movie (
    [ID]          INT           NOT NULL IDENTITY(0,1),
    [Name]        VARCHAR (255) NOT NULL,
    [Released_at] VARCHAR (4)   NOT NULL,
    [Genres]      VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
  );
  CREATE NONCLUSTERED INDEX [IX_Movie_ReleasedAt] ON dbo.Movie([Released_at] ASC);

  CREATE TABLE dbo.Concept (
    [ID]        INT PRIMARY KEY CLUSTERED IDENTITY(0,1),
    [Weight]    FLOAT
  );

  CREATE TABLE dbo.Rating (
    [ID]        INT        IDENTITY (0,1) NOT NULL,
    [UserID]    INT        NOT NULL,
    [MovieID]   INT        NOT NULL,
    [Score]     FLOAT (53) NOT NULL,
    [CreatedAt] DATETIME   NOT NULL,
    PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT [FK_Rating_OnMovie] FOREIGN KEY (MovieID) REFERENCES dbo.Movie(ID)
  );
  CREATE NONCLUSTERED INDEX [IX_Rating_UserID] ON dbo.Rating([UserID] ASC);
  CREATE NONCLUSTERED INDEX [IX_Rating_UserID_MovieID] ON dbo.Rating([UserID] asc, [MovieID] asc);
  
  CREATE TABLE dbo.TrainingData (
    [ID]        INT        IDENTITY (0,1) NOT NULL,
    [UserID]    INT        NOT NULL,
    [MovieID]   INT        NOT NULL,
    [Score]     FLOAT (53) NOT NULL,
    [CreatedAt] DATETIME   NOT NULL,
    PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT [FK_TrainingData_OnMovie] FOREIGN KEY (MovieID) REFERENCES dbo.Movie(ID)
  );
  CREATE NONCLUSTERED INDEX [IX_TrainingData_UserID] ON dbo.TrainingData([UserID] ASC);
  CREATE NONCLUSTERED INDEX [IX_TrainingData_UserID_MovieID] ON dbo.TrainingData([UserID] asc, [MovieID] asc);

  CREATE TABLE dbo.UserConcept (
    [ID]        INT PRIMARY KEY CLUSTERED IDENTITY(0,1),
    [ConceptID] INT REFERENCES dbo.Concept(ID),
    [UserID]    INT,
    [Weight]    FLOAT
  );
  CREATE UNIQUE NONCLUSTERED INDEX ConceptUserIndex ON dbo.UserConcept(ConceptID,UserID);
  CREATE NONCLUSTERED INDEX [IX_UserConcept_UserID] ON dbo.UserConcept(UserID);

  CREATE TABLE dbo.MovieConcept (
    [ID]        INT PRIMARY KEY CLUSTERED IDENTITY(0,1),
    [ConceptID] INT REFERENCES dbo.concept(ID),
    [MovieID]   INT REFERENCES dbo.Movie(ID),
    [Weight]    FLOAT
  );
  CREATE UNIQUE NONCLUSTERED INDEX ConceptMovieIndex ON dbo.MovieConcept(ConceptID,MovieID);
  CREATE NONCLUSTERED INDEX [IX_UserConcept_UserID] ON dbo.MovieConcept(MovieID);
  
  CREATE TABLE dbo.[User] (
    [ID]      INT PRIMARY KEY CLUSTERED IDENTITY(0,1),
    [Name]    VARCHAR(128)
    );
COMMIT;
GO

BEGIN TRANSACTION; -- INSERT DATA FROM FILES
	BULK INSERT dbo.Movie
	FROM 'H:\Documents\Visual Studio 2017\Projects\p2-code\App_Data\Movie.CSV'
	WITH
	(
		FIELDTERMINATOR = '^'
	);
	GO
	BULK INSERT dbo.[User]
	FROM 'H:\Documents\Visual Studio 2017\Projects\p2-code\App_Data\User.CSV'
	WITH
	(
		FIELDTERMINATOR = ','
	);
	GO
	BULK INSERT dbo.Rating
	FROM 'H:\Documents\Visual Studio 2017\Projects\p2-code\App_Data\Rating.CSV'
	WITH
	(
		FIELDTERMINATOR = ','
	);
	GO
	BULK INSERT dbo.TrainingData
	FROM 'H:\Documents\Visual Studio 2017\Projects\p2-code\App_Data\TrainingData.CSV'
	WITH
	(
		FIELDTERMINATOR = ','
	);
	GO
COMMIT;
GO