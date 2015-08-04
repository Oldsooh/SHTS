SET IDENTITY_INSERT [dbo].[AdminRole] ON
INSERT INTO [dbo].[AdminRole] ([RoleId], [RoleNmae], [State], [Permission], [LastUpdateTime]) VALUES (1, N'普通', 0, NULL, N'2014-10-10 00:00:00')
INSERT INTO [dbo].[AdminRole] ([RoleId], [RoleNmae], [State], [Permission], [LastUpdateTime]) VALUES (2, N'编辑', 0, NULL, N'2014-10-10 00:00:00')
INSERT INTO [dbo].[AdminRole] ([RoleId], [RoleNmae], [State], [Permission], [LastUpdateTime]) VALUES (3, N'统计员', 0, NULL, N'2014-10-10 00:00:00')
INSERT INTO [dbo].[AdminRole] ([RoleId], [RoleNmae], [State], [Permission], [LastUpdateTime]) VALUES (4, N'系统管理员', 0, NULL, N'2014-10-10 00:00:00')
SET IDENTITY_INSERT [dbo].[AdminRole] OFF
