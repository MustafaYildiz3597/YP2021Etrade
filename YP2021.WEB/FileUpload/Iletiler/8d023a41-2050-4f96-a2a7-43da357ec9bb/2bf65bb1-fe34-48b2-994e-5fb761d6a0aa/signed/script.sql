USE [ilekacom_kepiktestdb]
GO
/****** Object:  Table [dbo].[Action]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Action](
	[ID] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[PageUrl] [nvarchar](50) NULL,
	[RankNumber] [int] NULL,
 CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiRequestLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiRequestLog](
	[ID] [int] NOT NULL,
	[RequestURI] [nvarchar](1024) NULL,
	[JSONData] [nvarchar](max) NULL,
	[ResultText] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BordroDonem]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BordroDonem](
	[ID] [varchar](6) NOT NULL,
	[Title] [nvarchar](64) NULL,
	[Description] [nvarchar](64) NULL,
	[Year] [int] NULL,
	[Month] [int] NULL,
 CONSTRAINT [PK_BordroDonem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BordroEmployee]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BordroEmployee](
	[ID] [uniqueidentifier] NOT NULL,
	[FirmaDonemID] [uniqueidentifier] NULL,
	[FirmaID] [uniqueidentifier] NULL,
	[DonemID] [varchar](6) NULL,
	[EmployeeID] [uniqueidentifier] NULL,
	[SendToGsmNumber] [varchar](10) NULL,
	[SendToEmailAddress] [varchar](128) NULL,
	[SendingDate] [datetime] NULL,
	[ParsedTCIdentityNo] [nvarchar](50) NULL,
	[PDFFileName] [nvarchar](256) NULL,
	[PDFUploadedBy] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
	[DeleteDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_BordroEmployee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BordroEmployeeArchived]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BordroEmployeeArchived](
	[ID] [uniqueidentifier] NOT NULL,
	[FirmaDonemID] [uniqueidentifier] NULL,
	[FirmaID] [uniqueidentifier] NULL,
	[DonemID] [varchar](6) NULL,
	[EmployeeID] [uniqueidentifier] NULL,
	[SendToGsmNumber] [varchar](10) NULL,
	[SendToEmailAddress] [varchar](128) NULL,
	[SendingDate] [datetime] NULL,
	[ParsedTCIdentityNo] [nvarchar](50) NULL,
	[PDFFileName] [nvarchar](256) NULL,
	[PDFUploadedBy] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
	[DeleteDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[BatchID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_BordroEmployeeArchived] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BordroFirmaDonem]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BordroFirmaDonem](
	[ID] [uniqueidentifier] NOT NULL,
	[FirmID] [uniqueidentifier] NULL,
	[DonemID] [varchar](6) NULL,
	[BulkPDFUploadDT] [datetime] NULL,
	[BulkPDFFileName] [nvarchar](256) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
	[DeletedDT] [datetime] NULL,
 CONSTRAINT [PK_BordroFirmaDonem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_BordroFirmaDonem] UNIQUE NONCLUSTERED 
(
	[FirmID] ASC,
	[DonemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[ID] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[ID] [uniqueidentifier] NOT NULL,
	[FirmID] [uniqueidentifier] NULL,
	[FirstName] [nvarchar](64) NULL,
	[LastName] [nvarchar](64) NULL,
	[TCIdentityNo] [nvarchar](11) NULL,
	[GSMNumber] [varchar](10) NULL,
	[SEPAddress] [nvarchar](128) NULL,
	[KEPAddress] [nvarchar](128) NULL,
	[SentPayrollsCount] [int] NULL,
	[KEPStatus] [bit] NULL,
	[ApprovalStatus] [bit] NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
	[Deleted] [bit] NULL,
	[DeleteDT] [datetime] NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [uniqueidentifier] NULL,
	[FirmID] [uniqueidentifier] NULL,
	[ExcelUploadLogID] [int] NULL,
	[TCIdentityNo] [varchar](16) NULL,
	[RowJsonStr] [nvarchar](max) NULL,
	[ActionType] [varchar](64) NULL,
	[ControllerName] [nvarchar](32) NULL,
	[ActionName] [nvarchar](32) NULL,
	[CreatedAtRole] [varchar](16) NULL,
	[LogText] [nvarchar](128) NULL,
	[HasError] [bit] NULL,
	[ApprovalStatus] [bit] NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
 CONSTRAINT [PK_EmployeeLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ESignerCounterLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ESignerCounterLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MemberID] [nvarchar](128) NULL,
	[ActionCount] [int] NULL,
	[ActionType] [int] NULL,
	[IsCanceled] [bit] NULL,
	[CancelationDT] [datetime] NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ESignerCounterLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExcelUploadLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExcelUploadLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ExcelUploadEntity] [nvarchar](32) NULL,
	[FirmID] [uniqueidentifier] NULL,
	[TransferedRowCount] [int] NULL,
	[TotalRowCount] [int] NULL,
	[HasError] [bit] NULL,
	[ErrorText] [nvarchar](max) NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
 CONSTRAINT [PK_ExcelUploadLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Firm]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Firm](
	[ID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](128) NULL,
	[Address] [nvarchar](256) NULL,
	[City] [nvarchar](32) NULL,
	[ContactName] [nvarchar](64) NULL,
	[ContactGSMNo] [nvarchar](16) NULL,
	[ContactEmail] [nvarchar](64) NULL,
	[KEPEmail] [nvarchar](64) NULL,
	[PhoneNo] [nvarchar](16) NULL,
	[TaxNumber] [nvarchar](16) NULL,
	[TaxOffice] [nvarchar](32) NULL,
	[KEPMemberMaxLimit] [int] NULL,
	[KEPMemberCount] [int] NULL,
	[KEPExpirationDT] [datetime] NULL,
	[KEPStatus] [bit] NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
	[Deleted] [bit] NULL,
	[DeleteDT] [datetime] NULL,
 CONSTRAINT [PK_Firm] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FirmKEPStatusLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FirmKEPStatusLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirmID] [uniqueidentifier] NULL,
	[BeginDate] [datetime] NULL,
	[ExpireDate] [datetime] NULL,
	[PrevStatus] [int] NULL,
	[Status] [int] NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
 CONSTRAINT [PK_FirmKEPStatusLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImzaciFileLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImzaciFileLog](
	[ID] [nvarchar](128) NOT NULL,
	[MemberID] [nvarchar](128) NULL,
	[FileName] [nvarchar](256) NULL,
	[FileFullName] [nvarchar](312) NULL,
	[FileSize] [decimal](18, 4) NULL,
	[SignedCount] [int] NULL,
	[SignersCount] [int] NULL,
	[SignersStatus] [int] NULL,
	[SignersApprovalDT] [datetime] NULL,
	[SignersCompletionDT] [datetime] NULL,
	[SigningType] [int] NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_ImzaciFileLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImzaciFileLogItem]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImzaciFileLogItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileLogID] [nvarchar](128) NULL,
	[Description] [nvarchar](1024) NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_ImzaciFileLogItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImzaciFileSigner]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImzaciFileSigner](
	[ID] [nvarchar](128) NOT NULL,
	[ImzaciFileLogID] [nvarchar](128) NULL,
	[SignerMemberID] [nvarchar](128) NULL,
	[RankNumber] [int] NULL,
	[SignType] [varchar](16) NULL,
	[Status] [int] NULL,
	[LastActionDT] [datetime] NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_ImzaciFileSigner] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImzaciOLD]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImzaciOLD](
	[ID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](64) NULL,
	[LastName] [nvarchar](64) NULL,
	[TCIdentityNo] [varchar](11) NULL,
	[EmailAddress] [nvarchar](128) NULL,
	[GSMNumber] [nvarchar](16) NULL,
	[UsageLimit] [int] NULL,
	[SpentCount] [int] NULL,
	[Status] [bit] NULL,
	[CreateUserID] [nvarchar](128) NULL,
	[CreateDT] [datetime] NULL,
	[Deleted] [bit] NULL,
	[DeleteDT] [datetime] NULL,
 CONSTRAINT [PK_Imzaci] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImzaciUsageLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImzaciUsageLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MemberID] [nvarchar](128) NULL,
	[Quantity] [int] NULL,
	[TranType] [int] NULL,
	[Description] [nvarchar](64) NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_ImzaciUsageLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[ID] [nvarchar](128) NOT NULL,
	[FirmID] [uniqueidentifier] NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[TCIdentityNo] [varchar](11) NULL,
	[PhotoPath] [nvarchar](256) NULL,
	[GSMNumber] [nvarchar](16) NULL,
	[Email] [nvarchar](128) NULL,
	[DepartmentID] [nvarchar](128) NULL,
	[RoleID] [nvarchar](128) NULL,
	[IsESigner] [bit] NULL,
	[ESignerUsageLimit] [int] NULL,
	[ESignerSpentCount] [int] NULL,
	[ESignerExpiringTime] [datetime] NULL,
	[ESignerUsageType] [int] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedDT] [datetime] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[DeletedDT] [datetime] NULL,
	[DeletedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](128) NULL,
	[ActionID] [nvarchar](128) NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TokenLog]    Script Date: 26.10.2020 23:00:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TokenLog](
	[ID] [nvarchar](128) NOT NULL,
	[sid] [nvarchar](128) NULL,
	[userid] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_TokenLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_BordroDonem] FOREIGN KEY([DonemID])
REFERENCES [dbo].[BordroDonem] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_BordroDonem]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_BordroFirmaDonem] FOREIGN KEY([FirmaDonemID])
REFERENCES [dbo].[BordroFirmaDonem] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_BordroFirmaDonem]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_Employee]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_Firm] FOREIGN KEY([FirmaID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_Firm]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_MemberCreatedBy]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_MemberDeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_MemberDeletedBy]
GO
ALTER TABLE [dbo].[BordroEmployee]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployee_MemberPDFUploadBy] FOREIGN KEY([PDFUploadedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployee] CHECK CONSTRAINT [FK_BordroEmployee_MemberPDFUploadBy]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_BordroDonem] FOREIGN KEY([DonemID])
REFERENCES [dbo].[BordroDonem] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_BordroDonem]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_BordroFirmaDonem] FOREIGN KEY([FirmaDonemID])
REFERENCES [dbo].[BordroFirmaDonem] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_BordroFirmaDonem]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_Employee]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_Firm] FOREIGN KEY([FirmaID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_Firm]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_MemberCreatedBy]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_MemberDeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_MemberDeletedBy]
GO
ALTER TABLE [dbo].[BordroEmployeeArchived]  WITH CHECK ADD  CONSTRAINT [FK_BordroEmployeeArchived_MemberPDFUploadBy] FOREIGN KEY([PDFUploadedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[BordroEmployeeArchived] CHECK CONSTRAINT [FK_BordroEmployeeArchived_MemberPDFUploadBy]
GO
ALTER TABLE [dbo].[BordroFirmaDonem]  WITH CHECK ADD  CONSTRAINT [FK_BordroFirmaDonemKodu_BordroDonemKodu] FOREIGN KEY([DonemID])
REFERENCES [dbo].[BordroDonem] ([ID])
GO
ALTER TABLE [dbo].[BordroFirmaDonem] CHECK CONSTRAINT [FK_BordroFirmaDonemKodu_BordroDonemKodu]
GO
ALTER TABLE [dbo].[BordroFirmaDonem]  WITH CHECK ADD  CONSTRAINT [FK_BordroFirmaDonemKodu_Firm] FOREIGN KEY([FirmID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[BordroFirmaDonem] CHECK CONSTRAINT [FK_BordroFirmaDonemKodu_Firm]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Firm] FOREIGN KEY([FirmID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Firm]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Member] FOREIGN KEY([CreateUserID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Member]
GO
ALTER TABLE [dbo].[EmployeeLog]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLog_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[EmployeeLog] CHECK CONSTRAINT [FK_EmployeeLog_Employee]
GO
ALTER TABLE [dbo].[EmployeeLog]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLog_Firm] FOREIGN KEY([FirmID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[EmployeeLog] CHECK CONSTRAINT [FK_EmployeeLog_Firm]
GO
ALTER TABLE [dbo].[EmployeeLog]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLog_Member] FOREIGN KEY([CreateUserID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[EmployeeLog] CHECK CONSTRAINT [FK_EmployeeLog_Member]
GO
ALTER TABLE [dbo].[ESignerCounterLog]  WITH CHECK ADD  CONSTRAINT [FK_ESignerCounterLog_Member] FOREIGN KEY([MemberID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ESignerCounterLog] CHECK CONSTRAINT [FK_ESignerCounterLog_Member]
GO
ALTER TABLE [dbo].[ExcelUploadLog]  WITH CHECK ADD  CONSTRAINT [FK_ExcelUploadLog_Firm] FOREIGN KEY([FirmID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[ExcelUploadLog] CHECK CONSTRAINT [FK_ExcelUploadLog_Firm]
GO
ALTER TABLE [dbo].[ExcelUploadLog]  WITH CHECK ADD  CONSTRAINT [FK_ExcelUploadLog_Member] FOREIGN KEY([CreateUserID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ExcelUploadLog] CHECK CONSTRAINT [FK_ExcelUploadLog_Member]
GO
ALTER TABLE [dbo].[Firm]  WITH CHECK ADD  CONSTRAINT [FK_Firm_Member] FOREIGN KEY([CreateUserID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Firm] CHECK CONSTRAINT [FK_Firm_Member]
GO
ALTER TABLE [dbo].[ImzaciFileLog]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileLog_Member] FOREIGN KEY([MemberID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileLog] CHECK CONSTRAINT [FK_ImzaciFileLog_Member]
GO
ALTER TABLE [dbo].[ImzaciFileLog]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileLog_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileLog] CHECK CONSTRAINT [FK_ImzaciFileLog_MemberCreatedBy]
GO
ALTER TABLE [dbo].[ImzaciFileLogItem]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileLogItem_ImzaciFileLog] FOREIGN KEY([FileLogID])
REFERENCES [dbo].[ImzaciFileLog] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileLogItem] CHECK CONSTRAINT [FK_ImzaciFileLogItem_ImzaciFileLog]
GO
ALTER TABLE [dbo].[ImzaciFileLogItem]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileLogItem_Member] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileLogItem] CHECK CONSTRAINT [FK_ImzaciFileLogItem_Member]
GO
ALTER TABLE [dbo].[ImzaciFileSigner]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileSigner_ImzaciFileLog] FOREIGN KEY([ImzaciFileLogID])
REFERENCES [dbo].[ImzaciFileLog] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileSigner] CHECK CONSTRAINT [FK_ImzaciFileSigner_ImzaciFileLog]
GO
ALTER TABLE [dbo].[ImzaciFileSigner]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileSigner_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileSigner] CHECK CONSTRAINT [FK_ImzaciFileSigner_MemberCreatedBy]
GO
ALTER TABLE [dbo].[ImzaciFileSigner]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciFileSigner_MemberSginer] FOREIGN KEY([SignerMemberID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciFileSigner] CHECK CONSTRAINT [FK_ImzaciFileSigner_MemberSginer]
GO
ALTER TABLE [dbo].[ImzaciUsageLog]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciUsageLog_Member] FOREIGN KEY([MemberID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciUsageLog] CHECK CONSTRAINT [FK_ImzaciUsageLog_Member]
GO
ALTER TABLE [dbo].[ImzaciUsageLog]  WITH CHECK ADD  CONSTRAINT [FK_ImzaciUsageLog_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[ImzaciUsageLog] CHECK CONSTRAINT [FK_ImzaciUsageLog_MemberCreatedBy]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Member_AspNetRoles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[AspNetRoles] ([Id])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Member_AspNetRoles]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Member_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Department] ([ID])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Member_Department]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Member_Firm] FOREIGN KEY([FirmID])
REFERENCES [dbo].[Firm] ([ID])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Member_Firm]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Member_MemberCreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Member_MemberCreatedBy]
GO
ALTER TABLE [dbo].[Member]  WITH CHECK ADD  CONSTRAINT [FK_Member_MemberDeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Member] CHECK CONSTRAINT [FK_Member_MemberDeletedBy]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_Action] FOREIGN KEY([ActionID])
REFERENCES [dbo].[Action] ([ID])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_Action]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_Member] FOREIGN KEY([UserID])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_Member]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1:Bekliyor;2:Onaylandı;3:Tamamlandı;  9: Durduruldu.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImzaciFileLog', @level2type=N'COLUMN',@level2name=N'SignersStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1:Tek; 2:Müşterek' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImzaciFileLog', @level2type=N'COLUMN',@level2name=N'SigningType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:İşlem Yok; 1:imza bekleniyor; 2:imzalandı. 3:imza hatası; 9:imza red' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ImzaciFileSigner', @level2type=N'COLUMN',@level2name=N'Status'
GO
