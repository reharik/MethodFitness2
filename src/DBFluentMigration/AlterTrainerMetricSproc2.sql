/****** Object:  StoredProcedure [dbo].[TrainerMetric]    Script Date: 1/31/2013 7:18:34 PM ******/਍ഀ
SET ANSI_NULLS ON਍ഀ
GO਍ഀ
SET QUOTED_IDENTIFIER ON਍ഀ
GO਍ഀ
ALTER PROCEDURE [dbo].[TrainerMetric] ਍ऀ䀀匀琀愀爀琀䐀愀琀攀 䐀愀琀攀吀椀洀攀 Ⰰഀ
	@EndDate DateTime ,਍ऀ䀀吀爀愀椀渀攀爀䤀搀 椀渀琀ഀ
AS਍䈀䔀䜀䤀一ഀ
	SET NOCOUNT ON;਍ഀ
਍ഀ
select ਍ഀ
	t.firstname + ' ' + t.lastName as Trainer,਍ऀ挀⸀昀椀爀猀琀渀愀洀攀 ⬀ ✀ ✀ ⬀ 挀⸀氀愀猀琀一愀洀攀 愀猀 䌀氀椀攀渀琀Ⰰഀ
਍ഀ
    SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) Hour,਍ഀ
    SUM(case when s.appointmentType = 'HalfHour' then 1 else 0 end) HalfHour,਍ഀ
    SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) Pair,਍ഀ
਍ഀ
	cast((SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) ਍ऀऀ⬀ 匀唀䴀⠀挀愀猀攀 眀栀攀渀 猀⸀愀瀀瀀漀椀渀琀洀攀渀琀吀礀瀀攀 㴀 ✀䠀愀氀昀䠀漀甀爀✀ 琀栀攀渀 ㄀ 攀氀猀攀 　 攀渀搀⤀⼀㈀ഀ
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end)) as numeric(10,2)) TotalHours,਍ഀ
਍ഀ
	cast((DATEDIFF(DAY, @StartDate, @EndDate)/7) as numeric(10,2))NumberOfWeeks,਍ഀ
	਍ऀ挀愀猀琀⠀ഀ
	(cast(SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) ਍ऀऀ⬀ 匀唀䴀⠀挀愀猀攀 眀栀攀渀 猀⸀愀瀀瀀漀椀渀琀洀攀渀琀吀礀瀀攀 㴀 ✀䠀愀氀昀䠀漀甀爀✀ 琀栀攀渀 ㄀ 攀氀猀攀 　 攀渀搀⤀⼀㈀ഀ
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) as numeric(10,2)) /਍    挀愀猀琀⠀⠀䐀䄀吀䔀䐀䤀䘀䘀⠀䐀䄀夀Ⰰ 䀀匀琀愀爀琀䐀愀琀攀Ⰰ 䀀䔀渀搀䐀愀琀攀⤀⼀㜀⤀ 愀猀 渀甀洀攀爀椀挀⠀㄀　Ⰰ㈀⤀⤀⤀ 愀猀 渀甀洀攀爀椀挀⠀㄀　Ⰰ㈀⤀⤀ഀ
਍ऀ 愀猀 䠀漀甀爀猀倀攀爀圀攀攀欀ഀ
਍ഀ
਍䘀爀漀洀 䌀氀椀攀渀琀 愀猀 挀 ഀ
਍氀攀昀琀 漀甀琀攀爀 樀漀椀渀 嬀猀攀猀猀椀漀渀崀 愀猀 猀 漀渀 挀⸀䔀渀琀椀琀礀椀搀 㴀 猀⸀䌀氀椀攀渀琀䤀搀ഀ
਍氀攀昀琀 樀漀椀渀 嬀甀猀攀爀崀 愀猀 琀 漀渀 猀⸀琀爀愀椀渀攀爀椀搀 㴀 琀⸀攀渀琀椀琀礀椀搀ഀ
਍氀攀昀琀 樀漀椀渀 愀瀀瀀漀椀渀琀洀攀渀琀 愀猀 愀 漀渀 猀⸀愀瀀瀀漀椀渀琀洀攀渀琀椀搀 㴀 愀⸀攀渀琀椀琀礀䤀搀ഀ
where t.entityid = @TrainerId ਍ऀ䄀一䐀 愀⸀嬀搀愀琀攀崀 㸀㴀  䀀匀琀愀爀琀䐀愀琀攀 愀渀搀 愀⸀嬀搀愀琀攀崀 㰀㴀 䀀䔀渀搀䐀愀琀攀 ഀ
group by c.firstname, c.lastname, t.firstname, t.lastname਍栀愀瘀椀渀最 ⠀匀唀䴀⠀挀愀猀攀 眀栀攀渀 猀⸀愀瀀瀀漀椀渀琀洀攀渀琀吀礀瀀攀 㴀 ✀䠀漀甀爀✀ 琀栀攀渀 ㄀ 攀氀猀攀 　 攀渀搀⤀ ഀ
		+ SUM(case when s.appointmentType = 'HalfHour' then 1 else 0 end)/2਍ऀऀ⬀ 匀唀䴀⠀挀愀猀攀 眀栀攀渀 猀⸀愀瀀瀀漀椀渀琀洀攀渀琀吀礀瀀攀 㴀 ✀倀愀椀爀✀ 琀栀攀渀 ㄀ 攀氀猀攀 　 攀渀搀⤀⤀ 㸀 　ഀ
਍ഀ
order by c.LastName਍ഀ
ENൄ�