CREATE TABLE [dbo].[ContractConsultant] (
    [ConsultantId] UNIQUEIDENTIFIER NOT NULL,
    [ContractId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContractConsultant] PRIMARY KEY CLUSTERED ([ConsultantId] ASC, [ContractId] ASC),
    CONSTRAINT [FK_ContractConsultant_Consultants_ConsultantId] FOREIGN KEY ([ConsultantId]) REFERENCES [dbo].[Consultants] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ContractConsultant_Contracts_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [dbo].[Contracts] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ContractConsultant_ContractId]
    ON [dbo].[ContractConsultant]([ContractId] ASC);

