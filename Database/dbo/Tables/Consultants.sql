CREATE TABLE [dbo].[Consultants] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [FirstName]            NVARCHAR (MAX)   NOT NULL,
    [LastName]             NVARCHAR (MAX)   NOT NULL,
    [Email]                NVARCHAR (MAX)   NOT NULL,
    [PhoneNumber]          NVARCHAR (9)     NOT NULL,
    [IdentificationNumber] NVARCHAR (10)    NOT NULL,
    [ProfilePicture]       NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Consultants] PRIMARY KEY CLUSTERED ([Id] ASC)
);

