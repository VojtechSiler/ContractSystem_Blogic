CREATE TABLE [dbo].[Users] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Username]     NVARCHAR (20)    NOT NULL,
    [Email]        NVARCHAR (MAX)   NOT NULL,
    [PasswordHash] NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

