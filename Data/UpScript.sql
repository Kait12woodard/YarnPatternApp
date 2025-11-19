GO
USE YarnPatternDB; 

CREATE TABLE dbo.Designers (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE dbo.CraftType (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Craft NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE dbo.Difficulty (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Ranking NVARCHAR(150) NOT NULL 
);

CREATE TABLE dbo.ToolSize (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Size DECIMAL(4,2) NOT NULL UNIQUE
);

CREATE TABLE dbo.YarnWeight (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Weight TINYINT NOT NULL UNIQUE
);

CREATE TABLE dbo.YarnBrand (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE dbo.ProjectType (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE dbo.PatternTag (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Tag NVARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE dbo.Patterns (
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name NVARCHAR(150) NOT NUll,
	DesignerID INT Null,
	CraftTypeID INT NOT NULL,
	DifficultyID INT NULL,
	IsFree BIT NOT NULL DEFAULT 0,
	IsFavorite BIT NOT NULL DEFAULT 0,
	PatSource NVARCHAR(250) NULL,
	DateAdded DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
	LastViewed DATETIME2(0) NULL,
	HaveMade BIT NOT NULL DEFAULT 0,
	FilePath NVARCHAR(250) NOT NULL UNIQUE,
	CONSTRAINT Fk_Pattern_Designer
		FOREIGN KEY (DesignerID) REFERENCES dbo.Designers(ID) ON DELETE SET NULL,
	CONSTRAINT Fk_Pattern_CraftType
		FOREIGN KEY (CraftTypeID) REFERENCES dbo.CraftType(ID),
	CONSTRAINT Fk_Pattern_Difficulty
		FOREIGN KEY (DifficultyID) REFERENCES dbo.Difficulty(ID)
);

CREATE TABLE dbo.ToolSizeLookup (
	ToolSizeID INT NOT NULL,
	PatternID INT NOT NULL,
	CONSTRAINT Pk_ToolSizeLookup PRIMARY KEY(ToolSizeID, PatternID),
	CONSTRAINT Fk_ToolSizeLookup_ToolSize FOREIGN KEY(ToolSizeID)
		REFERENCES dbo.ToolSize(ID) ON DELETE CASCADE,
	CONSTRAINT Fk_ToolSizeLookup_Pattern FOREIGN KEY (PatternID)
		REFERENCES dbo.Patterns(ID) ON DELETE CASCADE
);

CREATE TABLE dbo.YarnWeightLookup (
    YarnWeightID INT NOT NULL,
    PatternID    INT NOT NULL,
    CONSTRAINT PK_YarnWeightLookup PRIMARY KEY (YarnWeightID, PatternID),
    CONSTRAINT FK_YarnWeightLookup_YarnWeight FOREIGN KEY (YarnWeightID)
        REFERENCES dbo.YarnWeight(ID) ON DELETE CASCADE,
    CONSTRAINT FK_YarnWeightLookup_Pattern FOREIGN KEY (PatternID)
        REFERENCES dbo.Patterns(ID) ON DELETE CASCADE
);

CREATE TABLE dbo.YarnBrandLookup (
    YarnBrandID INT NOT NULL,
    PatternID   INT NOT NULL,
    CONSTRAINT PK_YarnBrandLookup PRIMARY KEY (YarnBrandID, PatternID),
    CONSTRAINT FK_YarnBrandLookup_YarnBrand FOREIGN KEY (YarnBrandID)
        REFERENCES dbo.YarnBrand(ID) ON DELETE CASCADE,
    CONSTRAINT FK_YarnBrandLookup_Pattern FOREIGN KEY (PatternID)
        REFERENCES dbo.Patterns(ID) ON DELETE CASCADE
);

CREATE TABLE dbo.ProjectTypeLookup (
    ProjectTypeID INT NOT NULL,
    PatternID     INT NOT NULL,
    CONSTRAINT PK_ProjectTypeLookup PRIMARY KEY (ProjectTypeID, PatternID),
    CONSTRAINT FK_ProjectTypeLookup_ProjectType FOREIGN KEY (ProjectTypeID)
        REFERENCES dbo.ProjectType(ID) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectTypeLookup_Pattern FOREIGN KEY (PatternID)
        REFERENCES dbo.Patterns(ID) ON DELETE CASCADE
);

CREATE TABLE dbo.TagLookup (
    TagID     INT NOT NULL,      
    PatternID INT NOT NULL,
    CONSTRAINT PK_TagLookup PRIMARY KEY (TagID, PatternID),
    CONSTRAINT FK_TagLookup_Tag FOREIGN KEY (TagID)
        REFERENCES dbo.PatternTag(ID) ON DELETE CASCADE,
    CONSTRAINT FK_TagLookup_Pattern FOREIGN KEY (PatternID)
        REFERENCES dbo.Patterns(ID) ON DELETE CASCADE
);


INSERT INTO dbo.Difficulty (Ranking) VALUES ('Beginner');
INSERT INTO dbo.Difficulty (Ranking) VALUES ('Intermediate');
INSERT INTO dbo.Difficulty (Ranking) VALUES ('Advanced');
INSERT INTO dbo.Difficulty (Ranking) VALUES ('Expert');
