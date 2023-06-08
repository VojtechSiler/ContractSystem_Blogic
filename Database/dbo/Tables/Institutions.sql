CREATE TABLE [dbo].[Institutions] (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_Institutions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

