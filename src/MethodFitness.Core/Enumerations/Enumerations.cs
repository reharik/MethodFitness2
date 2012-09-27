using System;
using CC.Core.Localization;

namespace MethodFitness.Core.Enumerations
{
    public class Enumerations{}

    [Serializable]
    public class Source : Enumeration
    {
        public static readonly Source Empty = new Source { IsActive = false, Key = "" };
        public static readonly Source Referral = new Source { IsActive = true, Key = "Referral" };
        public static readonly Source DriveBy = new Source { IsActive = true, Key = "Drive/Walk By" };
        public static readonly Source WebSearch = new Source { IsActive = true, Key = "Web Search" };
        public static readonly Source PrintAd = new Source { IsActive = true, Key = "Print Ad" };
        public static readonly Source Network = new Source { IsActive = true, Key = "Network" };
        public static readonly Source Other = new Source { IsActive = true, Key = "Other" };
    }
   
    [Serializable]
    public class DocumentFileType : Enumeration
    {
        public static readonly DocumentFileType Empty = new DocumentFileType { IsActive = false, Key = "" };
        public static readonly DocumentFileType Document = new DocumentFileType { IsActive = true, Key = "Document" };
        public static readonly DocumentFileType Image = new DocumentFileType { IsActive = true, Key = "Image"};
        public static readonly DocumentFileType Headshot = new DocumentFileType { IsActive = true, Key = "Headshot"};
    }

    [Serializable]
    public class AreaName : CC.Core.Enumerations.AreaName
    {
        public static readonly AreaName Empty = new AreaName { IsActive = false, Key = "" };
        public static readonly AreaName Schedule = new AreaName { IsActive = true, Key = "Schedule" };
        public static readonly AreaName Reports = new AreaName { IsActive = true, Key = "Reports" };
        public static readonly AreaName Billing = new AreaName { IsActive = true, Key = "Billing" };
    }

    [Serializable]
    public class SecurityUserGroups : Enumeration
    {
        public static readonly SecurityUserGroups Empty = new SecurityUserGroups { IsActive = false, Key = "" };
        public static readonly SecurityUserGroups Administrator = new SecurityUserGroups { IsActive = true, Key = "Administrator" };
        public static readonly SecurityUserGroups Trainer = new SecurityUserGroups { IsActive = true, Key = "Trainer" };
    }

    [Serializable]
    public class AppointmentLength : Enumeration
    {
        public static readonly AppointmentLength Empty = new AppointmentLength { IsActive = false, Key = "" };
        public static readonly AppointmentLength HalfHour = new AppointmentLength { IsActive = true, Key = "Half Hour"};
        public static readonly AppointmentLength Hour = new AppointmentLength { IsActive = true, Key = "Hour",IsDefault = true};
        public static readonly AppointmentLength Pair = new AppointmentLength { IsActive = true, Key = "Pair" };
    }

    [Serializable]
    public class AppointmentType : Enumeration
    {
        public static readonly AppointmentType Empty = new AppointmentType { IsActive = false, Key = "" };
        public static readonly AppointmentType HalfHour = new AppointmentType { IsActive = true, Key = "Half Hour", Value = "30", OnlyUseKeyForSelector=true };
        public static readonly AppointmentType Hour = new AppointmentType { IsActive = true, Key = "Hour", Value = "60", IsDefault = true, OnlyUseKeyForSelector = true };
        public static readonly AppointmentType Pair = new AppointmentType { IsActive = true, Value = "60", Key = "Pair", OnlyUseKeyForSelector = true };
    }
}