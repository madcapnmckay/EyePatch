/****** Object:  Table [dbo].[MediaFiles]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MediaFiles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[AltText] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
	[LastModified] [datetime2](7) NULL,
 CONSTRAINT [PK_MediaFiles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Folder]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Folder](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ParentID] [int] NULL,
 CONSTRAINT [PK_dbo.Folder] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Template]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Template](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ViewPath] [nvarchar](max) NOT NULL,
	[Controller] [nvarchar](max) NOT NULL,
	[Action] [nvarchar](max) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[AnalyticsKey] [nvarchar](50) NULL,
	[Language] [int] NULL,
	[Author] [nvarchar](50) NULL,
	[Charset] [nvarchar](13) NULL,
	[Copyright] [nvarchar](50) NULL,
	[Description] [nvarchar](155) NULL,
	[Keywords] [nvarchar](874) NULL,
	[Robots] [nvarchar](20) NULL,
	[OgType] [nvarchar](50) NULL,
	[OgEmail] [nvarchar](50) NULL,
	[OgPhone] [nvarchar](25) NULL,
	[OgImage] [nvarchar](max) NULL,
	[OgLongitude] [float] NULL,
	[OgLatitude] [float] NULL,
	[OgStreetAddress] [nvarchar](max) NULL,
	[OgLocality] [nvarchar](50) NULL,
	[OgRegion] [nvarchar](50) NULL,
	[OgCountry] [nvarchar](50) NULL,
	[OgPostcode] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Template] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Site]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Site](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plugin]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plugin](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Author] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.WidgetGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Widget]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Widget](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
	[GroupID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Widget] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Page]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Page](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Url] [nvarchar](max) NULL,
	[ChangeFrequency] [int] NULL,
	[Priority] [float] NULL,
	[Created] [datetime2](7) NOT NULL,
	[LastModified] [datetime2](7) NULL,
	[TemplateID] [int] NOT NULL,
	[FolderID] [int] NULL,
	[MenuOrder] [int] NOT NULL,
	[IsLive] [bit] NOT NULL,
	[IsInMenu] [bit] NOT NULL,
	[IsHidden] [bit] NOT NULL,
	[IsDynamic] [bit] NOT NULL,
	[Language] [int] NULL,
	[Author] [nvarchar](50) NULL,
	[Charset] [nvarchar](13) NULL,
	[Copyright] [nvarchar](50) NULL,
	[Description] [nvarchar](155) NULL,
	[Keywords] [nvarchar](874) NULL,
	[Robots] [nvarchar](20) NULL,
	[OgType] [nvarchar](50) NULL,
	[OgEmail] [nvarchar](50) NULL,
	[OgPhone] [nvarchar](25) NULL,
	[OgImage] [nvarchar](max) NULL,
	[OgLongitude] [float] NULL,
	[OgLatitude] [float] NULL,
	[OgStreetAddress] [nvarchar](max) NULL,
	[OgLocality] [nvarchar](50) NULL,
	[OgRegion] [nvarchar](50) NULL,
	[OgCountry] [nvarchar](50) NULL,
	[OgPostcode] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Page] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContentArea]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContentArea](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PageID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.ContentArea] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WidgetInstance]    Script Date: 08/08/2011 17:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WidgetInstance](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[WidgetID] [int] NOT NULL,
	[ContentAreaID] [int] NOT NULL,
	[Contents] [text] NULL,
	[Position] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PageWidgets] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Default [DF_Page_ChangeFrequency]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_ChangeFrequency]  DEFAULT ((3)) FOR [ChangeFrequency]
GO
/****** Object:  Default [DF_Page_Priority]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_Priority]  DEFAULT ((0.5)) FOR [Priority]
GO
/****** Object:  Default [DF_Page_Created]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_Page_LastModified]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_LastModified]  DEFAULT (getdate()) FOR [LastModified]
GO
/****** Object:  Default [DF_Page_MenuOrder]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_MenuOrder]  DEFAULT ((0)) FOR [MenuOrder]
GO
/****** Object:  Default [DF_Page_IsInMenu]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_IsInMenu]  DEFAULT ((0)) FOR [IsInMenu]
GO
/****** Object:  Default [DF_Page_IsHidden]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_IsHidden]  DEFAULT ((0)) FOR [IsHidden]
GO
/****** Object:  Default [DF_Page_IsDynamic]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page] ADD  CONSTRAINT [DF_Page_IsDynamic]  DEFAULT ((0)) FOR [IsDynamic]
GO
/****** Object:  ForeignKey [FK_ContentArea_Page]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[ContentArea]  WITH CHECK ADD  CONSTRAINT [FK_ContentArea_Page] FOREIGN KEY([PageID])
REFERENCES [dbo].[Page] ([ID])
GO
ALTER TABLE [dbo].[ContentArea] CHECK CONSTRAINT [FK_ContentArea_Page]
GO
/****** Object:  ForeignKey [Folder_Folder]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Folder]  WITH CHECK ADD  CONSTRAINT [Folder_Folder] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Folder] ([ID])
GO
ALTER TABLE [dbo].[Folder] CHECK CONSTRAINT [Folder_Folder]
GO
/****** Object:  ForeignKey [Folder_Page]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page]  WITH CHECK ADD  CONSTRAINT [Folder_Page] FOREIGN KEY([FolderID])
REFERENCES [dbo].[Folder] ([ID])
GO
ALTER TABLE [dbo].[Page] CHECK CONSTRAINT [Folder_Page]
GO
/****** Object:  ForeignKey [Template_Page]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Page]  WITH CHECK ADD  CONSTRAINT [Template_Page] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[Template] ([ID])
GO
ALTER TABLE [dbo].[Page] CHECK CONSTRAINT [Template_Page]
GO
/****** Object:  ForeignKey [WidgetGroup_Widget]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[Widget]  WITH CHECK ADD  CONSTRAINT [WidgetGroup_Widget] FOREIGN KEY([GroupID])
REFERENCES [dbo].[Plugin] ([ID])
GO
ALTER TABLE [dbo].[Widget] CHECK CONSTRAINT [WidgetGroup_Widget]
GO
/****** Object:  ForeignKey [ContentArea_PageWidget]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[WidgetInstance]  WITH CHECK ADD  CONSTRAINT [ContentArea_PageWidget] FOREIGN KEY([ContentAreaID])
REFERENCES [dbo].[ContentArea] ([ID])
GO
ALTER TABLE [dbo].[WidgetInstance] CHECK CONSTRAINT [ContentArea_PageWidget]
GO
/****** Object:  ForeignKey [Widget_PageWidget]    Script Date: 08/08/2011 17:06:27 ******/
ALTER TABLE [dbo].[WidgetInstance]  WITH CHECK ADD  CONSTRAINT [Widget_PageWidget] FOREIGN KEY([WidgetID])
REFERENCES [dbo].[Widget] ([ID])
GO
ALTER TABLE [dbo].[WidgetInstance] CHECK CONSTRAINT [Widget_PageWidget]
GO
