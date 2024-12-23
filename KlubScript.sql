USE [Klub]
GO
/****** Object:  Table [dbo].[Basket]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Basket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_User] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Delivery_time] [int] NOT NULL,
	[SumOrder] [decimal](18, 2) NOT NULL,
	[Descount] [decimal](18, 2) NOT NULL,
	[GenericCode] [int] NOT NULL,
	[Id_status] [int] NOT NULL,
 CONSTRAINT [PK_Basket] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Books]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Books](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Image] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Prise] [decimal](18, 2) NOT NULL,
	[Discount] [int] NULL,
	[Id_Supplier] [int] NOT NULL,
	[Id_Status] [int] NOT NULL,
	[Remains] [int] NULL,
 CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_book] [int] NOT NULL,
	[Id_Busket] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[SumOrder] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status_Basket]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status_Basket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Status_Basket] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status_Book]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status_Book](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Status_Book] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supply]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supply](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_Book] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Supply] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 12.12.2024 11:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FIO] [nvarchar](150) NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Id_Role] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Basket] ON 

INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (1, 3, CAST(N'2024-12-10T17:01:09.117' AS DateTime), 6, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 633, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (2, 3, CAST(N'2024-12-10T17:38:03.660' AS DateTime), 6, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 104, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (3, 3, CAST(N'2024-12-12T01:10:35.967' AS DateTime), 6, CAST(600.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), 441, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (4, 2, CAST(N'2024-12-12T02:49:06.890' AS DateTime), 6, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 226, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (5, 2, CAST(N'2024-12-12T03:34:20.080' AS DateTime), 6, CAST(15000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 310, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (6, 3, CAST(N'2024-12-12T04:13:51.707' AS DateTime), 6, CAST(7500.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 575, 2)
INSERT [dbo].[Basket] ([Id], [Id_User], [Date], [Delivery_time], [SumOrder], [Descount], [GenericCode], [Id_status]) VALUES (7, 3, CAST(N'2024-12-12T11:33:42.833' AS DateTime), 6, CAST(8700.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), 368, 2)
SET IDENTITY_INSERT [dbo].[Basket] OFF
GO
SET IDENTITY_INSERT [dbo].[Books] ON 

INSERT [dbo].[Books] ([Id], [Name], [Image], [Description], [Prise], [Discount], [Id_Supplier], [Id_Status], [Remains]) VALUES (1, N'Три поросёнка', N'Tri.png', N'Книга повествует о капиталистическом мире который сжирает нищих', CAST(1200.00 AS Decimal(18, 2)), 50, 1, 2, 14)
INSERT [dbo].[Books] ([Id], [Name], [Image], [Description], [Prise], [Discount], [Id_Supplier], [Id_Status], [Remains]) VALUES (6, N'Незнайка', N'Nez.png', N'Наконец, после стольких лет ожидания, вышла новая книга. При издании пострадало много разработчиков, но это не важно. Литература требует жертв.', CAST(2500.00 AS Decimal(18, 2)), 0, 1, 2, 255)
SET IDENTITY_INSERT [dbo].[Books] OFF
GO
SET IDENTITY_INSERT [dbo].[Order] ON 

INSERT [dbo].[Order] ([Id], [Id_book], [Id_Busket], [Quantity], [SumOrder]) VALUES (1, 1, 3, 1, CAST(600.00 AS Decimal(18, 2)))
INSERT [dbo].[Order] ([Id], [Id_book], [Id_Busket], [Quantity], [SumOrder]) VALUES (3, 6, 5, 1, CAST(2500.00 AS Decimal(18, 2)))
INSERT [dbo].[Order] ([Id], [Id_book], [Id_Busket], [Quantity], [SumOrder]) VALUES (5, 6, 6, 1, CAST(2500.00 AS Decimal(18, 2)))
INSERT [dbo].[Order] ([Id], [Id_book], [Id_Busket], [Quantity], [SumOrder]) VALUES (6, 6, 7, 1, CAST(2500.00 AS Decimal(18, 2)))
INSERT [dbo].[Order] ([Id], [Id_book], [Id_Busket], [Quantity], [SumOrder]) VALUES (7, 1, 7, 1, CAST(600.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Order] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [Name]) VALUES (1, N'Клиент')
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (2, N'Менеджер')
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (3, N'Администратор')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Status_Basket] ON 

INSERT [dbo].[Status_Basket] ([Id], [Name]) VALUES (1, N'Новая')
INSERT [dbo].[Status_Basket] ([Id], [Name]) VALUES (2, N'Отправлен на сборку')
INSERT [dbo].[Status_Basket] ([Id], [Name]) VALUES (3, N'Готов')
INSERT [dbo].[Status_Basket] ([Id], [Name]) VALUES (4, N'Закрыт')
SET IDENTITY_INSERT [dbo].[Status_Basket] OFF
GO
SET IDENTITY_INSERT [dbo].[Status_Book] ON 

INSERT [dbo].[Status_Book] ([Id], [Name]) VALUES (1, N'Удален')
INSERT [dbo].[Status_Book] ([Id], [Name]) VALUES (2, N'На складе')
INSERT [dbo].[Status_Book] ([Id], [Name]) VALUES (3, N'Закончилось')
SET IDENTITY_INSERT [dbo].[Status_Book] OFF
GO
SET IDENTITY_INSERT [dbo].[Supplier] ON 

INSERT [dbo].[Supplier] ([Id], [Name]) VALUES (1, N'Pailo')
SET IDENTITY_INSERT [dbo].[Supplier] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [FIO], [Login], [Password], [Id_Role]) VALUES (1, N'Глухов Руслан Владимирович', N'kartonnka', N'kartonnka', 3)
INSERT [dbo].[Users] ([Id], [FIO], [Login], [Password], [Id_Role]) VALUES (2, N'Артемидова Ксения Максимовна', N'kefi', N'kefi', 2)
INSERT [dbo].[Users] ([Id], [FIO], [Login], [Password], [Id_Role]) VALUES (3, N'Василькова Анна Михайловна', N'anna', N'anna', 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Basket_Status_Basket] FOREIGN KEY([Id_status])
REFERENCES [dbo].[Status_Basket] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Basket_Status_Basket]
GO
ALTER TABLE [dbo].[Basket]  WITH CHECK ADD  CONSTRAINT [FK_Basket_Users] FOREIGN KEY([Id_User])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Basket] CHECK CONSTRAINT [FK_Basket_Users]
GO
ALTER TABLE [dbo].[Books]  WITH CHECK ADD  CONSTRAINT [FK_Books_Status_Book] FOREIGN KEY([Id_Status])
REFERENCES [dbo].[Status_Book] ([Id])
GO
ALTER TABLE [dbo].[Books] CHECK CONSTRAINT [FK_Books_Status_Book]
GO
ALTER TABLE [dbo].[Books]  WITH CHECK ADD  CONSTRAINT [FK_Books_Supplier] FOREIGN KEY([Id_Supplier])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[Books] CHECK CONSTRAINT [FK_Books_Supplier]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Basket] FOREIGN KEY([Id_Busket])
REFERENCES [dbo].[Basket] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Basket]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Books] FOREIGN KEY([Id_book])
REFERENCES [dbo].[Books] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Books]
GO
ALTER TABLE [dbo].[Supply]  WITH CHECK ADD  CONSTRAINT [FK_Supply_Books] FOREIGN KEY([Id_Book])
REFERENCES [dbo].[Books] ([Id])
GO
ALTER TABLE [dbo].[Supply] CHECK CONSTRAINT [FK_Supply_Books]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([Id_Role])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
