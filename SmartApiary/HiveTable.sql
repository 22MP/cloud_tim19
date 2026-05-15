CREATE TABLE HiveInspections (
    Id NVARCHAR(36) NOT NULL PRIMARY KEY,
    HiveId NVARCHAR(36) NOT NULL,

    InspectionDate DATETIME2 NOT NULL,
    BottomBoardColor NVARCHAR(50) NOT NULL,
    HoneyFrames INT NOT NULL,
    HoneyKg FLOAT NOT NULL,
    BroodFrames INT NOT NULL,
    QueenPresent BIT NOT NULL,
    Notes NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL,

    CONSTRAINT FK_HiveInspections_Hives
        FOREIGN KEY (HiveId)
        REFERENCES Hives(Id)
        ON DELETE CASCADE
);