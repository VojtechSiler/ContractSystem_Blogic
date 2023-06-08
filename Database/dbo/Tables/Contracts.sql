CREATE TABLE [dbo].[Contracts] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [RegistrationNumber] NVARCHAR (MAX)   NOT NULL,
    [InstitutionId]      UNIQUEIDENTIFIER NOT NULL,
    [ClientId]           UNIQUEIDENTIFIER NOT NULL,
    [ContractManagerId]  UNIQUEIDENTIFIER NOT NULL,
    [ClosureDate]        DATETIME2 (7)    NOT NULL,
    [StartDate]          DATETIME2 (7)    NOT NULL,
    [EndDate]            DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contracts_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Contracts_Consultants_ContractManagerId] FOREIGN KEY ([ContractManagerId]) REFERENCES [dbo].[Consultants] ([Id]),
    CONSTRAINT [FK_Contracts_Institutions_InstitutionId] FOREIGN KEY ([InstitutionId]) REFERENCES [dbo].[Institutions] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Contracts_ClientId]
    ON [dbo].[Contracts]([ClientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Contracts_ContractManagerId]
    ON [dbo].[Contracts]([ContractManagerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Contracts_InstitutionId]
    ON [dbo].[Contracts]([InstitutionId] ASC);

