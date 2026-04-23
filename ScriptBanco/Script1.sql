USE [dbAdLume]
GO
/****** Object:  Table [dbo].[tMidia]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tMidia](
	[cMidia] [int] IDENTITY(1,1) NOT NULL,
	[DescMidia] [varchar](255) NOT NULL,
	[NomeMidia] [varchar](255) NOT NULL,
	[HashMidia] [varchar](32) NOT NULL,
	[UrlMidia] [varchar](255) NOT NULL,
	[cAtivo] [int] NOT NULL,
 CONSTRAINT [PK_tMidia] PRIMARY KEY CLUSTERED 
(
	[cMidia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tPlaylist]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tPlaylist](
	[cPlaylist] [int] IDENTITY(1,1) NOT NULL,
	[cEquipamento] [int] NOT NULL,
	[NomePlaylist] [nvarchar](255) NULL,
	[HoraInicio] [nvarchar](5) NULL,
	[cAtivo] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cPlaylist] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tPlaylistItem]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tPlaylistItem](
	[cPlaylistItem] [int] IDENTITY(1,1) NOT NULL,
	[cPlaylist] [int] NOT NULL,
	[Ordem] [int] NOT NULL,
	[cMidia] [int] NOT NULL,
	[cAtivo] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cPlaylistItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tEquipamento]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tEquipamento](
	[cEquipamento] [int] IDENTITY(1,1) NOT NULL,
	[GuidEquipamento] [varchar](50) NULL,
	[NomeEquipamento] [varchar](255) NULL,
	[DescEquipamento] [varchar](500) NULL,
	[DtUltAtu] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tEquipam__1CBE864F539C6D38] PRIMARY KEY CLUSTERED 
(
	[cEquipamento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vTudo]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[vTudo] as
select 
	E.cEquipamento,
	E.GuidEquipamento,
	E.NomeEquipamento,
	E.DescEquipamento,
	E.DtUltAtu,
	PL.cPlaylist,
	--PL.cEquipamento,
	PL.NomePlaylist,
	PL.HoraInicio,
	--PL.cAtivo,
	PLI.cPlaylistItem,
	--PLI.cPlaylist,
	PLI.Ordem,
	PLI.cMidia,
	--PLI.cAtivo,
	--M.cMidia,
	M.DescMidia,
	M.NomeMidia,
	M.HashMidia,
	M.UrlMidia
	--M.cAtivo,
from 
	tEquipamento E
	INNER JOIN tPlaylist PL
		ON PL.cEquipamento = E.cEquipamento
	INNER JOIN tPlaylistItem PLI
		ON PLI.cPlaylist = PL.cPlaylist
	INNER JOIN tMidia M
		ON PLI.cMidia = M.cMidia
--ORDER BY
--	E.cEquipamento,
--	PL.cPlaylist,
--	PL.HoraInicio,
--	PLI.Ordem,
--	PLI.cPlaylistItem

GO
/****** Object:  Table [dbo].[tEquipamento.apagar]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tEquipamento.apagar](
	[cEquipamento] [int] IDENTITY(1,1) NOT NULL,
	[GuidEquipamento] [nvarchar](36) NULL,
	[NomeEquipamento] [nvarchar](255) NULL,
	[DtUltAtu] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cEquipamento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tEquipamento] ON 
GO
INSERT [dbo].[tEquipamento] ([cEquipamento], [GuidEquipamento], [NomeEquipamento], [DescEquipamento], [DtUltAtu]) VALUES (1, N'1D8ABA3D-AB66-424F-B02D-5F0EED335B6C', N'MaquinaDesenv', N'Maquina de Desenvolvimento', CAST(N'2001-01-01T00:00:00.0000000' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[tEquipamento] OFF
GO
SET IDENTITY_INSERT [dbo].[tMidia] ON 
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (1, N'VIDEO MANHA 1', N'video_manha_1.mp4', N'9dfcf7f243a7e14577a0855bc6ad2f27', N'{0}://{1}:{2}/media/video_manha_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (2, N'VIDEO MANHA 2', N'video_manha_2.mp4', N'a08a741101f00280a75d073262032774', N'{0}://{1}:{2}/media/video_manha_2.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (3, N'VIDEO MANHA 3', N'video_manha_3.mp4', N'c376247810e76480d5ae958511839714', N'{0}://{1}:{2}/media/video_manha_3.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (4, N'VIDEO NOITE 1', N'video_noite_1.mp4', N'49c2c4ee470c73a2f85c25ecdf472e54', N'{0}://{1}:{2}/media/video_noite_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (5, N'VIDEO NOITE 2', N'video_noite_2.mp4', N'e0b85d0d2dfe95d23a06c035ea07d20e', N'{0}://{1}:{2}/media/video_noite_2.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (6, N'VIDEO PROMO A', N'video_promo_a.mp4', N'a4bdf48d07443145cd9951dde3864131', N'{0}://{1}:{2}/media/video_promo_a.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (7, N'VIDEO PROMO B', N'video_promo_b.mp4', N'dbe7626d6649f8edec3249234df0093e', N'{0}://{1}:{2}/media/video_promo_b.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (8, N'VIDEO TARDE 1', N'video_tarde_1.mp4', N'3c87ee4ebe9c102b492bf0bf6642e82c', N'{0}://{1}:{2}/media/video_tarde_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (9, N'VIDEO TARDE 2', N'video_tarde_2.mp4', N'5166941b73fad14e8f6c9f2cb5a85808', N'{0}://{1}:{2}/media/video_tarde_2.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (19, N'VIDEO MANHA 1', N'video_manha_1.mp4', N'9dfcf7f243a7e14577a0855bc6ad2f27', N'{0}://{1}:{2}/media/video_manha_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (20, N'VIDEO MANHA 2', N'video_manha_2.mp4', N'a08a741101f00280a75d073262032774', N'{0}://{1}:{2}/media/video_manha_2.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (21, N'VIDEO MANHA 3', N'video_manha_3.mp4', N'c376247810e76480d5ae958511839714', N'{0}://{1}:{2}/media/video_manha_3.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (22, N'VIDEO NOITE 1', N'video_noite_1.mp4', N'49c2c4ee470c73a2f85c25ecdf472e54', N'{0}://{1}:{2}/media/video_noite_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (23, N'VIDEO NOITE 2', N'video_noite_2.mp4', N'e0b85d0d2dfe95d23a06c035ea07d20e', N'{0}://{1}:{2}/media/video_noite_2.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (24, N'VIDEO PROMO A', N'video_promo_a.mp4', N'a4bdf48d07443145cd9951dde3864131', N'{0}://{1}:{2}/media/video_promo_a.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (25, N'VIDEO PROMO B', N'video_promo_b.mp4', N'dbe7626d6649f8edec3249234df0093e', N'{0}://{1}:{2}/media/video_promo_b.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (26, N'VIDEO TARDE 1', N'video_tarde_1.mp4', N'3c87ee4ebe9c102b492bf0bf6642e82c', N'{0}://{1}:{2}/media/video_tarde_1.mp4', 1)
GO
INSERT [dbo].[tMidia] ([cMidia], [DescMidia], [NomeMidia], [HashMidia], [UrlMidia], [cAtivo]) VALUES (27, N'VIDEO TARDE 2', N'video_tarde_2.mp4', N'5166941b73fad14e8f6c9f2cb5a85808', N'{0}://{1}:{2}/media/video_tarde_2.mp4', 1)
GO
SET IDENTITY_INSERT [dbo].[tMidia] OFF
GO
SET IDENTITY_INSERT [dbo].[tPlaylist] ON 
GO
INSERT [dbo].[tPlaylist] ([cPlaylist], [cEquipamento], [NomePlaylist], [HoraInicio], [cAtivo]) VALUES (2, 1, N'Manha', N'00:00', 1)
GO
INSERT [dbo].[tPlaylist] ([cPlaylist], [cEquipamento], [NomePlaylist], [HoraInicio], [cAtivo]) VALUES (3, 1, N'Tarde', N'11:00', 1)
GO
INSERT [dbo].[tPlaylist] ([cPlaylist], [cEquipamento], [NomePlaylist], [HoraInicio], [cAtivo]) VALUES (4, 1, N'Noite', N'16:30', 1)
GO
SET IDENTITY_INSERT [dbo].[tPlaylist] OFF
GO
SET IDENTITY_INSERT [dbo].[tPlaylistItem] ON 
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (1, 2, 1, 1, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (2, 2, 2, 2, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (3, 2, 3, 3, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (4, 2, 4, 6, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (5, 2, 5, 1, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (6, 2, 6, 2, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (7, 2, 7, 3, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (8, 2, 8, 7, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (9, 3, 1, 8, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (10, 3, 2, 9, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (11, 3, 3, 6, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (12, 3, 4, 8, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (13, 3, 5, 9, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (14, 3, 6, 7, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (15, 4, 1, 4, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (16, 4, 2, 5, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (18, 4, 3, 6, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (19, 4, 4, 4, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (20, 4, 5, 5, 1)
GO
INSERT [dbo].[tPlaylistItem] ([cPlaylistItem], [cPlaylist], [Ordem], [cMidia], [cAtivo]) VALUES (21, 4, 6, 7, 1)
GO
SET IDENTITY_INSERT [dbo].[tPlaylistItem] OFF
GO
ALTER TABLE [dbo].[tEquipamento] ADD  CONSTRAINT [DF__tEquipame__DtUlt__4222D4EF]  DEFAULT (getdate()) FOR [DtUltAtu]
GO
/****** Object:  StoredProcedure [dbo].[pEquipamentoPlaylistDto]    Script Date: 22/04/2026 23:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec pEquipamentoPlaylistDto '1D8ABA3D-AB66-424F-B02D-5F0EED335B6C'
CREATE PROCEDURE [dbo].[pEquipamentoPlaylistDto]
	@GuidEquipamento varchar(36)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		E.cEquipamento,
		E.GuidEquipamento,
		E.NomeEquipamento,
		E.DescEquipamento,
		E.DtUltAtu,
		PL.cPlaylist,
		--PL.cEquipamento,
		PL.NomePlaylist,
		PL.HoraInicio,
		--PL.cAtivo,
		PLI.cPlaylistItem,
		--PLI.cPlaylist,
		PLI.Ordem,
		PLI.cMidia,
		--PLI.cAtivo,
		--M.cMidia,
		M.DescMidia,
		M.NomeMidia,
		M.HashMidia,
		M.UrlMidia
		--M.cAtivo,
	from 
		tEquipamento E
		INNER JOIN tPlaylist PL
			ON PL.cEquipamento = E.cEquipamento
		INNER JOIN tPlaylistItem PLI
			ON PLI.cPlaylist = PL.cPlaylist
		INNER JOIN tMidia M
			ON PLI.cMidia = M.cMidia
	WHERE
		E.GuidEquipamento = @GuidEquipamento
	ORDER BY
		E.cEquipamento,
		PL.cPlaylist,
		PL.HoraInicio,
		PLI.Ordem,
		PLI.cPlaylistItem



END
GO
